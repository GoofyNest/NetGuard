using System.Text;
using Module.Helpers.ClientData;
using Module.Services;
using System.Globalization;
using System.Reflection;

namespace Module.Helpers
{
    public static class Reader
    {
        private static readonly (CharField Field, PropertyInfo Property)[] CharColumns =
            Array.ConvertAll(Enum.GetValues<CharField>(),
                field => (field, typeof(CharData).GetProperty(field.ToString())!));

        // Serializes Init() and the watcher's reload. Without this, two rapid
        // saves could race: one clears the dictionaries while the other is
        // half-way through repopulating them.
        private static readonly object _reloadLock = new();

        private static FileSystemWatcher? _watcher;
        private static int _disposed = 0;

        // Debounce: FSW fires multiple events for one save (Created + Changed +
        // Renamed). We wait this long after the last event before reloading.
        private const int DebounceMs = 500;
        private static readonly object _debounceLock = new();
        private static Timer? _debounceTimer;

        public static void Init()
        {
            var settings = Main.Settings.Data;

            lock (_reloadLock)
            {
                if (!Directory.Exists(settings.Path))
                {
                    Custom.WriteLine($"Creating folder {settings.Path}", ConsoleColor.Yellow);
                    Directory.CreateDirectory(settings.Path);
                }

                ReloadAll(settings.Path);

                AttachWatcher(settings.Path);
            }
        }

        /// <summary>
        /// Parses replacements privately, then publishes a complete generation.
        /// Called by Init() and by the file watcher.
        /// </summary>
        private static void ReloadAll(string path)
        {
            var next = new ClientDataSnapshot();

            var folderFiles = Directory.GetFiles(path);
            var fileCount = folderFiles.Length;

            for (var i = 0; i < fileCount; i++)
            {
                LoadFile(folderFiles[i], next);
            }

            Module.PacketHandler.Filtering.FilterRegistry.ExecutionGate.EnterWriteLock();
            try { Main.PublishData(next); }
            finally { Module.PacketHandler.Filtering.FilterRegistry.ExecutionGate.ExitWriteLock(); }

            Custom.WriteLine(
                $"Items loaded successfully {Main.Items.Count}",
                ConsoleColor.Green);
            Custom.WriteLine(
                $"Skills loaded successfully {Main.Skills.Count}",
                ConsoleColor.Green);
            Custom.WriteLine(
                $"Characters loaded successfully {Main.Chars.Count}",
                ConsoleColor.Green);
        }

        private static void LoadFile(string file, ClientDataSnapshot next)
        {
            var fileName = Path.GetFileName(file);
            bool isItemData = false, isSkillData = false, isCharData = false;

            if (fileName.Contains("itemdata", StringComparison.OrdinalIgnoreCase))
                isItemData = true;
            else if (fileName.Equals("skilldata.txt", StringComparison.OrdinalIgnoreCase))
                isSkillData = true;
            else if ((fileName.StartsWith("chardata", StringComparison.OrdinalIgnoreCase)
                   || fileName.StartsWith("characterdata", StringComparison.OrdinalIgnoreCase))
                  && fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                isCharData = true;
            else
                return;

            List<string> data;
            try
            {
                data = File.ReadAllLines(file, Encoding.Unicode).ToList();
            }
            catch (IOException ex)
            {
                Custom.WriteLine(
                    $"[Reader] Skipping {Path.GetFileName(file)} (still being written?): {ex.Message}",
                    ConsoleColor.DarkYellow);
                throw;
            }
            catch (Exception ex)
            {
                Custom.WriteLine(
                    $"[Reader] Failed to read {Path.GetFileName(file)}: {ex}",
                    ConsoleColor.Red);
                throw;
            }

            var linesCount = data.Count;
            for (var d = 0; d < linesCount; d++)
            {
                var line = data[d];
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("//"))
                    continue;
                var searchBetweenTabs = line.Split('\t');
                if (searchBetweenTabs[0].Trim().Equals("Service", StringComparison.OrdinalIgnoreCase))
                    continue;

                try
                {
                    if (isItemData)
                    {
                        ProcessItemData(searchBetweenTabs, next);
                    }
                    else if (isSkillData)
                    {
                        ProcessSkillData(searchBetweenTabs, next);
                    }
                    else if (isCharData)
                    {
                        ProcessCharData(searchBetweenTabs, next);
                    }
                }
                catch (Exception ex)
                {
                    throw new FormatException($"{fileName}, line {d + 1}: {ex.Message}", ex);
                }
            }
        }

        private static void AttachWatcher(string path)
        {
            if (_watcher != null)
                return;

            _watcher = new FileSystemWatcher(path, "*.txt")
            {
                NotifyFilter = NotifyFilters.FileName
                             | NotifyFilters.LastWrite
                             | NotifyFilters.Size,
                IncludeSubdirectories = false,
                EnableRaisingEvents = true,
            };

            _watcher.Created += OnFileEvent;
            _watcher.Changed += OnFileEvent;
            _watcher.Deleted += OnFileEvent;
            _watcher.Renamed += OnFileEvent;

            _watcher.Error += (_, e) =>
            {
                Custom.WriteLine(
                    $"[Reader] FileSystemWatcher error: {e.GetException()?.Message}",
                    ConsoleColor.Red);
            };

            Custom.WriteLine($"[Reader] Watching {path} for .txt changes", ConsoleColor.DarkCyan);
        }

        private static void OnFileEvent(object sender, FileSystemEventArgs e)
        {
            if (Volatile.Read(ref _disposed) == 1)
                return;

            // Ignore anything that isn't a .txt file. FSW filter already does
            // this for Created/Changed/Deleted, but Renamed uses the *old*
            // extension so a "foo.bin -> foo.txt" rename would slip through.
            if (!e.FullPath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                return;

            ScheduleDebouncedReload();
        }

        /// <summary>
        /// One save produces 2–4 FSW events. Instead of reloading per event, we
        /// reset a timer on each event and only fire once the burst settles.
        /// </summary>
        private static void ScheduleDebouncedReload()
        {
            lock (_debounceLock)
            {
                _debounceTimer?.Dispose();
                _debounceTimer = new Timer(_ => ReloadAfterDebounce(), null, DebounceMs, Timeout.Infinite);
            }
        }

        private static void ReloadAfterDebounce()
        {
            if (Volatile.Read(ref _disposed) == 1)
                return;

            // Give the writer a moment to release its handle. FSW fires on
            // Created the instant the file exists; ReadAllLines would then
            // throw IOException if the writer hasn't closed it yet.
            var path = Main.Settings.Data.Path;

            try
            {
                // WaitForFolderFilesIdle ensures every .txt is stable before
                // we nuke the dictionaries — otherwise we might reload while a
                // different file is still being written.
                if (!WaitForFolderFilesIdle(path, timeoutMs: 2000))
                {
                    Custom.WriteLine(
                        "[Reader] Reload aborted: folder did not settle",
                        ConsoleColor.DarkYellow);
                    return;
                }

                lock (_reloadLock)
                {
                    if (Volatile.Read(ref _disposed) == 1)
                        return;

                    Custom.WriteLine("[Reader] .txt change detected — reloading", ConsoleColor.Cyan);
                    ReloadAll(path);
                }
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"[Reader] Reload failed: {ex}", ConsoleColor.Red);
            }
        }

        /// <summary>
        /// Polls every .txt file in <paramref name="folder"/> until each has a
        /// stable length for two consecutive samples, or the timeout elapses.
        /// </summary>
        private static bool WaitForFolderFilesIdle(string folder, int timeoutMs)
        {
            var deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);
            var lastLengths = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);

            while (DateTime.UtcNow < deadline)
            {
                bool allStable = true;

                try
                {
                    foreach (var file in Directory.EnumerateFiles(folder, "*.txt"))
                    {
                        long len;
                        try { len = new FileInfo(file).Length; }
                        catch { allStable = false; break; }

                        if (!lastLengths.TryGetValue(file, out var prev) || prev != len)
                        {
                            lastLengths[file] = len;
                            allStable = false;
                        }
                    }
                }
                catch
                {
                    return false;
                }

                if (allStable)
                    return true;

                Thread.Sleep(100);
            }

            return false;
        }

        public static void Shutdown()
        {
            Interlocked.Exchange(ref _disposed, 1);

            lock (_debounceLock)
            {
                _debounceTimer?.Dispose();
                _debounceTimer = null;
            }

            _watcher?.Dispose();
            _watcher = null;
        }


        private static void ProcessSkillData(string[] values, ClientDataSnapshot next)
        {
            if (values[0].Trim() != "1") return;
            var record = ParseRecord<SkillData, SkillField>(values);
            if (record.ID <= 0) throw new FormatException("Skill ID must be positive.");
            next.Skills.TryAdd(record.ID, record);
        }

        private static void ProcessItemData(string[] values, ClientDataSnapshot next)
        {
            if (values[0].Trim() != "1") return;
            var record = ParseRecord<ItemData, ItemField>(values);
            if (record.ID <= 0) throw new FormatException("Item ID must be positive.");
            next.Items.TryAdd(record.ID, record);
        }

        private static T ParseRecord<T, TField>(string[] values)
            where T : new() where TField : struct, Enum
        {
            var record = new T();
            foreach (var property in typeof(T).GetProperties())
            {
                if (!Enum.TryParse<TField>(property.Name, out var field)) continue;
                int index = Convert.ToInt32(field);
                if (index >= values.Length)
                    continue; // Older item/skill exports omit optional trailing fields.
                SetValue(record!, property, values[index]);
            }
            return record;
        }

        private static void SetValue(object record, PropertyInfo property, string value)
        {
            try
            {
                object parsed = Convert.ChangeType(value.Trim(), property.PropertyType, CultureInfo.InvariantCulture);
                if (parsed is float f && !float.IsFinite(f) || parsed is double d && !double.IsFinite(d))
                    throw new FormatException("Expected a finite number.");
                property.SetValue(record, parsed);
            }
            catch (Exception ex) when (ex is FormatException || ex is OverflowException)
            {
                throw new FormatException($"Invalid {property.Name}: '{value}'.", ex);
            }
        }

        private static void ProcessCharData(string[] input, ClientDataSnapshot next)
        {
            if (input[0].Trim() != "1") return;
            var values = input.ToList();
            while (values.Count > 0 && string.IsNullOrWhiteSpace(values[^1]))
                values.RemoveAt(values.Count - 1);

            // Some horse rows omit the last common file column. Recognize only
            // that specific extended layout, preserving every other empty cell.
            if (values.Count == 119 && int.TryParse(values[56], out _)
                && values[55] == "xxx")
                values.Insert(56, "");

            bool extended = values.Count == 120;
            if (!extended && values.Count != 104)
                throw new FormatException($"Unsupported character text layout: {values.Count} columns (expected 104 or 120).");

            var record = new CharData();
            foreach (var (field, property) in CharColumns)
            {
                int index = (int)field;
                // The SQL export excludes the six resistance columns present
                // in the client's extended text layout, immediately after MaxMP.
                if (!extended && field >= CharField.Resistance1 && field <= CharField.Resistance6)
                    continue;
                if (!extended && field >= CharField.InventorySize) index -= 6;
                SetValue(record, property, values[index]);
            }
            record.ExtraColumns = extended ? values.Skip(110).ToArray() : Array.Empty<string>();
            if (record.ID <= 0) throw new FormatException("Character ID must be positive.");
            next.Chars.TryAdd(record.ID, record);
        }
    }
}
