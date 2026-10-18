using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Module.Networking;
using Module.Services;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Loader;

namespace Module.PacketHandler.Filtering
{
    public sealed class FilterLoader : IDisposable
    {
        private readonly string _rootDir;
        private readonly string _parentDir;
        private readonly string _rootName;

        // key = (context, opcode)
        private readonly ConcurrentDictionary<(FilterContext, ushort), IFilter> _filters = new();

        private readonly System.Timers.Timer _debounce;
        private readonly object _reloadLock = new();
        private FileSystemWatcher? _rootWatcher;    // watches inside filters/
        private FileSystemWatcher? _parentWatcher;  // watches filters/ itself (create/delete/rename)
        private AssemblyLoadContext? _alc;
        private bool _disposed;

        private ConstructorInfo? _hostCtor;
        private Action<SessionData>? _attach;
        private Action<SessionData>? _sync;
        private Action<object?, object?>? _migrate;

        public FilterLoader(string rootDir)
        {
            _rootDir = Path.IsPathRooted(rootDir)
                ? Path.GetFullPath(rootDir)
                : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, rootDir));

            _parentDir = Path.GetDirectoryName(_rootDir) ?? AppContext.BaseDirectory;
            _rootName = Path.GetFileName(_rootDir);

            _debounce = new System.Timers.Timer(750) { AutoReset = false };
            _debounce.Elapsed += (_, _) =>
            {
                try { Reload(); }
                catch (Exception ex)
                {
                    Custom.WriteLine($"[filters] reload threw: {ex}", ConsoleColor.Red);
                }
            };
        }

        public void Start()
        {
            EnsureDirectories();
            Reload();
            StartWatchers();
            Custom.WriteLine($"[filters] watching {_rootDir}", ConsoleColor.Cyan);
        }

        private void EnsureDirectories()
        {
            Directory.CreateDirectory(_rootDir);
            foreach (FilterContext ctx in Enum.GetValues<FilterContext>())
                Directory.CreateDirectory(Path.Combine(_rootDir, ctx.ToFolderPath()));
        }

        private void StartWatchers()
        {
            // --- Root watcher: files inside filters/ (recursive) ---
            if (Directory.Exists(_rootDir))
            {
                try
                {
                    _rootWatcher = new FileSystemWatcher(_rootDir, "*.cs")
                    {
                        IncludeSubdirectories = true,
                        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
                                     | NotifyFilters.Size | NotifyFilters.DirectoryName,
                        EnableRaisingEvents = true
                    };
                    _rootWatcher.Changed += OnFileChanged;
                    _rootWatcher.Created += OnFileChanged;
                    _rootWatcher.Deleted += OnFileChanged;
                    _rootWatcher.Renamed += OnFileChanged;
                    _rootWatcher.Error += OnRootWatcherError;
                }
                catch (Exception ex)
                {
                    Custom.WriteLine($"[filters] could not start root watcher: {ex.Message}", ConsoleColor.Red);
                    _rootWatcher = null;
                }
            }

            // --- Parent watcher: the filters/ folder itself (create/delete/rename) ---
            try
            {
                _parentWatcher = new FileSystemWatcher(_parentDir)
                {
                    IncludeSubdirectories = false,
                    NotifyFilter = NotifyFilters.DirectoryName,
                    Filter = _rootName,
                    EnableRaisingEvents = true
                };
                _parentWatcher.Created += OnRootDirChanged;
                _parentWatcher.Deleted += OnRootDirChanged;
                _parentWatcher.Renamed += OnRootDirChanged;
                _parentWatcher.Error += OnParentWatcherError;
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"[filters] could not start parent watcher: {ex.Message}", ConsoleColor.Red);
                _parentWatcher = null;
            }
        }

        private void StopWatchers()
        {
            try { _rootWatcher?.Dispose(); } catch { }
            try { _parentWatcher?.Dispose(); } catch { }
            _rootWatcher = null;
            _parentWatcher = null;
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            _debounce.Stop();
            _debounce.Start();
        }

        private void OnRootDirChanged(object sender, FileSystemEventArgs e)
        {
            // filters/ folder was created, deleted, or renamed. A full reload will
            // recreate the folder (if deleted) and restart the root watcher.
            _debounce.Stop();
            _debounce.Start();
        }

        private void OnRootWatcherError(object sender, ErrorEventArgs e)
        {
            Custom.WriteLine($"[filters] root watcher error: {e.GetException().Message} — restarting", ConsoleColor.Yellow);
            _debounce.Stop();
            _debounce.Start();
        }

        private void OnParentWatcherError(object sender, ErrorEventArgs e)
        {
            Custom.WriteLine($"[filters] parent watcher error: {e.GetException().Message} — restarting", ConsoleColor.Yellow);
            _debounce.Stop();
            _debounce.Start();
        }

        public IFilter? Resolve(FilterContext ctx, ushort opcode)
            => _filters.TryGetValue((ctx, opcode), out var f) ? f : null;

        private static FilterContext? ContextFromNamespace(string? ns)
        {
            if (string.IsNullOrEmpty(ns))
                return null;

            if (ns.EndsWith("Agent.Client", StringComparison.OrdinalIgnoreCase))
                return FilterContext.AgentClient;

            if (ns.EndsWith("Agent.Server", StringComparison.OrdinalIgnoreCase))
                return FilterContext.AgentServer;

            if (ns.EndsWith("Gateway.Client", StringComparison.OrdinalIgnoreCase))
                return FilterContext.GatewayClient;

            if (ns.EndsWith("Gateway.Server", StringComparison.OrdinalIgnoreCase))
                return FilterContext.GatewayServer;

            return null;
        }

        /// <summary>
        /// Clears all loaded state: filters, bus plugins, bridge delegates, ALC.
        /// Safe to call when root or all .cs files have been deleted.
        /// </summary>
        private void ClearAll(string reason)
        {
            Custom.WriteLine($"[filters] {reason} — clearing all filters", ConsoleColor.Yellow);

            _hostCtor = null;
            _attach = null;
            _sync = null;
            _migrate = null;

            try { BusRegistry.Dispatcher.Clear(); }
            catch (Exception ex) { Custom.WriteLine($"[filters] bus clear: {ex.Message}", ConsoleColor.Red); }

            FilterRegistry.ClearLandings();

            _filters.Clear();

            var old = _alc;
            _alc = null;
            if (old != null)
            {
                try { old.Unload(); } catch { }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private void Reload()
        {
            lock (_reloadLock)
            {
                if (_disposed) return;

                Custom.WriteLine("[filters] reloading...", ConsoleColor.Cyan);

                // 0. Root folder missing? Clear everything, recreate dirs, restart watchers.
                if (!Directory.Exists(_rootDir))
                {
                    ClearAll("root folder missing");

                    try
                    {
                        EnsureDirectories();
                    }
                    catch (Exception ex)
                    {
                        Custom.WriteLine($"[filters] could not recreate root: {ex.Message}", ConsoleColor.Red);
                    }

                    // Restart the root watcher (it died when the dir was deleted).
                    try { _rootWatcher?.Dispose(); } catch { }
                    _rootWatcher = null;
                    if (Directory.Exists(_rootDir))
                    {
                        try
                        {
                            _rootWatcher = new FileSystemWatcher(_rootDir, "*.cs")
                            {
                                IncludeSubdirectories = true,
                                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
                                             | NotifyFilters.Size | NotifyFilters.DirectoryName,
                                EnableRaisingEvents = true
                            };
                            _rootWatcher.Changed += OnFileChanged;
                            _rootWatcher.Created += OnFileChanged;
                            _rootWatcher.Deleted += OnFileChanged;
                            _rootWatcher.Renamed += OnFileChanged;
                            _rootWatcher.Error += OnRootWatcherError;
                        }
                        catch (Exception ex)
                        {
                            Custom.WriteLine($"[filters] could not restart root watcher: {ex.Message}", ConsoleColor.Red);
                        }
                    }
                    return;
                }

                // 1. Discover .cs files per context folder
                var filesByContext = new Dictionary<FilterContext, List<string>>();
                foreach (FilterContext ctx in Enum.GetValues<FilterContext>())
                {
                    var dir = Path.Combine(_rootDir, ctx.ToFolderPath());
                    List<string> files;
                    try
                    {
                        files = Directory.Exists(dir)
                            ? Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories).ToList()
                            : new List<string>();
                    }
                    catch (Exception ex)
                    {
                        Custom.WriteLine($"[filters] enumerate {dir}: {ex.Message}", ConsoleColor.Red);
                        files = new List<string>();
                    }
                    filesByContext[ctx] = files;
                }

                // 1b. Root-level files (anything not inside a context folder)
                var contextDirs = Enum.GetValues<FilterContext>()
                    .Select(ctx => Path.GetFullPath(Path.Combine(_rootDir, ctx.ToFolderPath())))
                    .ToArray();

                var rootFiles = new List<string>();
                try
                {
                    rootFiles = Directory.GetFiles(_rootDir, "*.cs", SearchOption.AllDirectories)
                        .Where(f =>
                        {
                            var full = Path.GetFullPath(f);
                            return !contextDirs.Any(dir =>
                                full.StartsWith(dir + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase));
                        })
                        .ToList();
                }
                catch (Exception ex)
                {
                    Custom.WriteLine($"[filters] enumerate root: {ex.Message}", ConsoleColor.Red);
                }

                int totalContextFiles = filesByContext.Values.Sum(v => v.Count);
                int totalFiles = totalContextFiles + rootFiles.Count;

                // 2. Nothing to compile -> full clear (unload ALC, drop bus, reset bridge).
                if (totalFiles == 0)
                {
                    ClearAll("no .cs files found");
                    return;
                }

                // 3. Compile everything in one shot (so filters can share helpers)
                var syntaxTrees = new List<SyntaxTree>();

                // 3a. Root-level files first
                foreach (var file in rootFiles)
                {
                    try
                    {
                        var text = File.ReadAllText(file);
                        syntaxTrees.Add(CSharpSyntaxTree.ParseText(text, path: file));
                    }
                    catch (Exception ex)
                    {
                        Custom.WriteLine($"[filters] read {file}: {ex.Message}", ConsoleColor.Red);
                    }
                }

                // 3b. Context files
                foreach (var kvp in filesByContext)
                {
                    foreach (var file in kvp.Value)
                    {
                        try
                        {
                            var text = File.ReadAllText(file);
                            syntaxTrees.Add(CSharpSyntaxTree.ParseText(text, path: file));
                        }
                        catch (Exception ex)
                        {
                            Custom.WriteLine($"[filters] read {file}: {ex.Message}", ConsoleColor.Red);
                        }
                    }
                }

                // 3c. Reference every assembly the host has loaded
                var refs = new List<MetadataReference>();
                var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var appAsm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (appAsm.IsDynamic) continue;
                    if (string.IsNullOrEmpty(appAsm.Location)) continue;
                    if (!seen.Add(appAsm.Location)) continue;
                    try { refs.Add(MetadataReference.CreateFromFile(appAsm.Location)); }
                    catch { }
                }

                var probeDirs = new[]
                {
                    AppContext.BaseDirectory,
                    Path.GetDirectoryName(typeof(FilterLoader).Assembly.Location) ?? AppContext.BaseDirectory,
                }.Distinct().ToArray();

                foreach (var dir in probeDirs)
                {
                    if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir)) continue;

                    foreach (var dll in Directory.GetFiles(dir, "*.dll"))
                    {
                        if (!seen.Add(dll)) continue;
                        try { refs.Add(MetadataReference.CreateFromFile(dll)); }
                        catch { }
                    }
                }

                var tpa = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;
                if (!string.IsNullOrEmpty(tpa))
                {
                    foreach (var path in tpa.Split(Path.PathSeparator))
                    {
                        if (string.IsNullOrEmpty(path)) continue;
                        if (!path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)) continue;
                        if (!seen.Add(path)) continue;
                        try { refs.Add(MetadataReference.CreateFromFile(path)); }
                        catch { }
                    }
                }

                // Generate SessionHost from any ISessionState implementors
                var hostTree = HostGeneration.Generate(syntaxTrees);
                if (hostTree != null)
                {
                    syntaxTrees.Add(hostTree);
                    Custom.WriteLine($"[filters] generated SessionHost.g.cs", ConsoleColor.Cyan);
                }

                var options = new CSharpCompilationOptions(
                    OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Release,
                    allowUnsafe: true);

                var compilation = CSharpCompilation.Create(
                    assemblyName: $"NetGuard.Filters.{Guid.NewGuid():N}",
                    syntaxTrees: syntaxTrees,
                    references: refs,
                    options: options);

                using var ms = new MemoryStream();
                var emit = compilation.Emit(ms);

                if (!emit.Success)
                {
                    foreach (var d in emit.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error))
                        Custom.WriteLine($"[filters] {d.Id}: {d.GetMessage()} @ {d.Location}", ConsoleColor.Red);
                    Custom.WriteLine("[filters] compile failed — keeping previous filters", ConsoleColor.Yellow);
                    return;
                }

                ms.Seek(0, SeekOrigin.Begin);

                // 4. Load into a collectible ALC
                var newAlc = new AssemblyLoadContext("NetGuard.Filters", isCollectible: true);

                newAlc.Resolving += (context, name) =>
                {
                    var loaded = AppDomain.CurrentDomain.GetAssemblies()
                        .FirstOrDefault(a => a.GetName().Name == name.Name);
                    if (loaded != null)
                        return loaded;

                    var fileName = name.Name + ".dll";
                    foreach (var dir in probeDirs)
                    {
                        if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir)) continue;

                        var candidate = Path.Combine(dir, fileName);
                        if (File.Exists(candidate))
                        {
                            try { return context.LoadFromAssemblyPath(candidate); }
                            catch { }
                        }
                    }

                    return null;
                };

                Assembly asm;
                try
                {
                    asm = newAlc.LoadFromStream(ms);
                }
                catch (Exception ex)
                {
                    Custom.WriteLine($"[filters] load failed: {ex.Message}", ConsoleColor.Red);
                    try { newAlc.Unload(); } catch { }
                    return;
                }

                // Wire up bridge delegates
                Type? hostType;
                Type? bridgeType;
                ConstructorInfo? hostCtor;
                Action<SessionData>? attach;
                Action<SessionData>? sync;
                Action<object?, object?>? migrate;

                try
                {
                    hostType = asm.GetType("NetGuard.Hot.SessionHost");
                    bridgeType = asm.GetType("NetGuard.Hot.SessionBridge");
                    hostCtor = hostType?.GetConstructor(Type.EmptyTypes);

                    attach = bridgeType?.GetMethod("Attach", new[] { typeof(SessionData) })
                        ?.CreateDelegate<Action<SessionData>>();
                    sync = bridgeType?.GetMethod("Sync", new[] { typeof(SessionData) })
                        ?.CreateDelegate<Action<SessionData>>();
                    migrate = bridgeType?.GetMethod("Migrate", new[] { typeof(object), typeof(object) })
                        ?.CreateDelegate<Action<object?, object?>>();
                }
                catch (Exception ex)
                {
                    Custom.WriteLine($"[filters] bridge wiring failed: {ex.Message}", ConsoleColor.Red);
                    try { newAlc.Unload(); } catch { }
                    return;
                }

                if (hostType == null)
                {
                    Custom.WriteLine("[filters] WARNING: no SessionHost generated — session state will be null", ConsoleColor.Yellow);
                }
                if (bridgeType == null)
                {
                    Custom.WriteLine("[filters] WARNING: no SessionBridge found — state will not initialise", ConsoleColor.Yellow);
                }

                // 5. Map each type to its context (via namespace)
                var newFilters = new Dictionary<(FilterContext, ushort), IFilter>();
                var perContext = new Dictionary<FilterContext, int>();
                var sourceMap = new Dictionary<(FilterContext, ushort), string>();

                // 5c. Discover the (single) Landing per context
                var newLandings = new Dictionary<FilterContext, ILanding>();
                foreach (var type in asm.GetTypes())
                {
                    if (!typeof(ILanding).IsAssignableFrom(type) || type.IsAbstract || type.IsInterface)
                        continue;

                    var ctx = ContextFromNamespace(type.Namespace);
                    if (ctx == null)
                    {
                        Custom.WriteLine(
                            $"[landing] {type.FullName}: namespace doesn't map to a context — skipped",
                            ConsoleColor.Yellow);
                        continue;
                    }

                    if (newLandings.ContainsKey(ctx.Value))
                    {
                        Custom.WriteLine(
                            $"[landing] DUPLICATE landing for {ctx.Value} — {type.FullName} ignored",
                            ConsoleColor.Red);
                        continue;
                    }

                    try
                    {
                        newLandings[ctx.Value] = (ILanding)Activator.CreateInstance(type)!;
                    }
                    catch (Exception ex)
                    {
                        Custom.WriteLine($"[landing] instantiate {type.Name}: {ex.Message}", ConsoleColor.Red);
                    }
                }

                foreach (var type in asm.GetTypes())
                {
                    if (!typeof(IFilter).IsAssignableFrom(type) || type.IsAbstract || type.IsInterface)
                        continue;

                    var ctx = ContextFromNamespace(type.Namespace);
                    if (ctx == null)
                    {
                        Custom.WriteLine($"[filters] {type.FullName}: namespace doesn't map to a context — skipped", ConsoleColor.Yellow);
                        continue;
                    }

                    var attr = type.GetCustomAttribute<FilterAttribute>();
                    if (attr == null)
                    {
                        Custom.WriteLine($"[filters] {type.Name}: missing [Filter(opcode)] attribute — skipped", ConsoleColor.Yellow);
                        continue;
                    }

                    var key = (ctx.Value, attr.Opcode);

                    if (newFilters.ContainsKey(key))
                    {
                        var existing = sourceMap.TryGetValue(key, out var s) ? s : "<unknown>";
                        Custom.WriteLine($"[filters] DUPLICATE: opcode 0x{attr.Opcode:X4} in {ctx}", ConsoleColor.Red);
                        Custom.WriteLine($"[filters]   first  -> {existing}", ConsoleColor.Red);
                        Custom.WriteLine($"[filters]   second -> {type.FullName}", ConsoleColor.Red);
                        continue;
                    }

                    try
                    {
                        var instance = (IFilter)Activator.CreateInstance(type)!;
                        newFilters[key] = instance;
                        sourceMap[key] = type.FullName ?? type.Name;
                        perContext[ctx.Value] = perContext.GetValueOrDefault(ctx.Value) + 1;
                    }
                    catch (Exception ex)
                    {
                        Custom.WriteLine($"[filters] instantiate {type.Name}: {ex.Message}", ConsoleColor.Red);
                    }
                }

                // 5b. Register bus plugins
                BusRegistry.Dispatcher.Clear();
                var busPluginCount = 0;

                foreach (var type in asm.GetTypes())
                {
                    if (!typeof(IBusPlugin).IsAssignableFrom(type) || type.IsAbstract || type.IsInterface)
                        continue;

                    try
                    {
                        var plugin = (IBusPlugin)Activator.CreateInstance(type)!;
                        plugin.Register(BusRegistry.Dispatcher);
                        busPluginCount++;
                    }
                    catch (Exception ex)
                    {
                        Custom.WriteLine($"[filters] bus plugin {type.Name}: {ex.Message}", ConsoleColor.Red);
                    }
                }

                if (busPluginCount > 0)
                    Custom.WriteLine($"[filters] registered {busPluginCount} bus plugin(s)", ConsoleColor.Cyan);

                // 6. Swap
                _hostCtor = hostCtor;
                _attach = attach;
                _sync = sync;
                _migrate = migrate;

                FilterRegistry.ClearLandings();
                foreach (var kvp in newLandings)
                    FilterRegistry.SetLanding(kvp.Key, kvp.Value);

                var oldAlc = _alc;
                _filters.Clear();
                foreach (var kvp in newFilters) _filters[kvp.Key] = kvp.Value;
                _alc = newAlc;

                if (newLandings.Count > 0)
                    foreach (var kvp in newLandings)
                        Custom.WriteLine($"[landing] {kvp.Key}: {kvp.Value.GetType().Name}", ConsoleColor.Cyan);

                // Re-attach live sessions to the new assembly
                foreach (var s in FilterRegistry.LiveSessions.Values)
                {
                    try
                    {
                        var oldHost = s.Host;
                        _attach?.Invoke(s);              // creates new SessionHost
                        _migrate?.Invoke(oldHost, s.Host); // copies preserved fields
                        _sync?.Invoke(s);                // shell <-> host
                    }
                    catch (Exception ex)
                    {
                        Custom.WriteLine($"[filters] reattach session failed: {ex.Message}", ConsoleColor.Red);
                    }
                }

                if (oldAlc != null)
                {
                    try { oldAlc.Unload(); } catch { }
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }

                foreach (var kvp in perContext)
                    Custom.WriteLine($"[filters]   {kvp.Key}: {kvp.Value} filter(s)", ConsoleColor.Cyan);

                Custom.WriteLine($"[filters] loaded {newFilters.Count} filter(s) from {totalFiles} file(s)", ConsoleColor.Cyan);
            }
        }

        internal void AttachSession(SessionData s)
        {
            _attach?.Invoke(s);
            _sync?.Invoke(s);
        }

        internal void SyncSession(SessionData s)
        {
            _sync?.Invoke(s);
        }

        public void Dispose()
        {
            _disposed = true;
            StopWatchers();
            try { _debounce.Stop(); } catch { }
            _debounce.Dispose();
            try { _alc?.Unload(); } catch { }
            _alc = null;
        }
    }
}