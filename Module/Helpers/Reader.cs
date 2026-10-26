using System.Text;
using Module.Helpers.ClientData;
using Module.Services;

namespace Module.Helpers
{
    public static class Reader
    {
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
        /// Clears all loaded data and re-parses every .txt file in the folder.
        /// Called by Init() and by the file watcher.
        /// </summary>
        private static void ReloadAll(string path)
        {
            Main.Items.Clear();
            Main.Skills.Clear();

            var folderFiles = Directory.GetFiles(path);
            var fileCount = folderFiles.Length;

            for (var i = 0; i < fileCount; i++)
            {
                LoadFile(folderFiles[i]);
            }

            Custom.WriteLine(
                $"Items loaded successfully {Main.Items.Count}",
                ConsoleColor.Green);
            Custom.WriteLine(
                $"Skills loaded successfully {Main.Skills.Count}",
                ConsoleColor.Green);
        }

        private static void LoadFile(string file)
        {
            file = file.ToLower();

            bool isItemData = false, isSkillData = false;

            if (file.Contains("itemdata"))
                isItemData = true;
            else if (file.Contains("skilldata.txt"))
                isSkillData = true;
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
                return;
            }
            catch (Exception ex)
            {
                Custom.WriteLine(
                    $"[Reader] Failed to read {Path.GetFileName(file)}: {ex}",
                    ConsoleColor.Red);
                return;
            }

            var linesCount = data.Count;
            for (var d = 0; d < linesCount; d++)
            {
                var line = data[d];
                var searchBetweenTabs = line.Split('\t');

                if (isItemData)
                {
                    ProcessItemData(searchBetweenTabs);
                }
                else if (isSkillData)
                {
                    ProcessSkillData(searchBetweenTabs);
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

        private static void ProcessSkillData(string[] searchBetweenTabs)
        {
            var _temp = new SkillData();

            var enabled = searchBetweenTabs[(int)SkillField.Service].Trim();
            if (enabled == "0")
                return;

            try
            {
                var properties = typeof(SkillData).GetProperties();

                foreach (var property in properties)
                {
                    if (Enum.IsDefined(typeof(SkillField), property.Name))
                    {
                        int index = (int)Enum.Parse(typeof(SkillField), property.Name);
                        if (index >= searchBetweenTabs.Length) continue;

                        string value = searchBetweenTabs[index].Trim();

                        if (property.PropertyType == typeof(int) && int.TryParse(value, out int intValue))
                        {
                            property.SetValue(_temp, intValue);
                        }
                        else if (property.PropertyType == typeof(float) && float.TryParse(value, out float floatValue))
                        {
                            property.SetValue(_temp, floatValue);
                        }
                        else if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(_temp, value);
                        }
                    }
                }
                if (_temp.ID > 0 && !Main.Skills.ContainsKey(_temp.ID))
                {
                    Main.Skills[_temp.ID] = _temp;
                }
            }
            catch (Exception ex)
            {
                Custom.WriteLine(ex.ToString());
            }
        }

        private static void ProcessItemData(string[] searchBetweenTabs)
        {
            var _temp = new ItemData();

            var enabled = searchBetweenTabs[(int)ItemField.Service].Trim();
            if (enabled == "0")
                return;

            try
            {
                var properties = typeof(ItemData).GetProperties();

                foreach (var property in properties)
                {
                    if (Enum.IsDefined(typeof(ItemField), property.Name))
                    {
                        int index = (int)Enum.Parse(typeof(ItemField), property.Name);
                        if (index >= searchBetweenTabs.Length) continue;

                        string value = searchBetweenTabs[index].Trim();

                        if (property.PropertyType == typeof(int) && int.TryParse(value, out int intValue))
                        {
                            property.SetValue(_temp, intValue);
                        }
                        else if (property.PropertyType == typeof(float) && float.TryParse(value, out float floatValue))
                        {
                            property.SetValue(_temp, floatValue);
                        }
                        else if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(_temp, value);
                        }
                    }
                }
                if (_temp.ID > 0 && !Main.Items.ContainsKey(_temp.ID))
                {
                    Main.Items[_temp.ID] = _temp;
                }
            }
            catch (Exception ex)
            {
                Custom.WriteLine(ex.ToString());
            }
        }
    }
}