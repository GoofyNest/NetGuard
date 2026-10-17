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

        // key = (context, opcode)
        private readonly ConcurrentDictionary<(FilterContext, ushort), IFilter> _filters = new();

        private readonly System.Timers.Timer _debounce;
        private readonly object _reloadLock = new();
        private FileSystemWatcher? _watcher;
        private AssemblyLoadContext? _alc;

        private ConstructorInfo? _hostCtor;
        private Action<SessionData>? _attach;
        private Action<SessionData>? _sync;
        private Action<object?, object?>? _migrate;

        public FilterLoader(string rootDir)
        {
            _rootDir = Path.IsPathRooted(rootDir)
                ? Path.GetFullPath(rootDir)
                : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, rootDir));

            _debounce = new System.Timers.Timer(750) { AutoReset = false };
            _debounce.Elapsed += (_, _) => Reload();
        }

        public void Start()
        {
            Directory.CreateDirectory(_rootDir);

            foreach (FilterContext ctx in Enum.GetValues<FilterContext>())
                Directory.CreateDirectory(Path.Combine(_rootDir, ctx.ToFolderPath()));

            Reload();

            _watcher = new FileSystemWatcher(_rootDir, "*.cs")
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size,
                EnableRaisingEvents = true
            };
            _watcher.Changed += OnFileChanged;
            _watcher.Created += OnFileChanged;
            _watcher.Deleted += OnFileChanged;
            _watcher.Renamed += OnFileChanged;

            Custom.WriteLine($"[filters] watching {_rootDir}", ConsoleColor.Cyan);
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
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

        private void Reload()
        {
            lock (_reloadLock)
            {
                Custom.WriteLine("[filters] reloading...", ConsoleColor.Cyan);

                // 1. Discover .cs files per context folder
                var filesByContext = new Dictionary<FilterContext, List<string>>();
                foreach (FilterContext ctx in Enum.GetValues<FilterContext>())
                {
                    var dir = Path.Combine(_rootDir, ctx.ToFolderPath());
                    filesByContext[ctx] = Directory.Exists(dir)
                        ? Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories).ToList()
                        : new List<string>();
                }

                // 1b. All shared files under filters/ — root-level AND any subfolder that isn't
                //     a context folder (Agent/Client, Agent/Server, Gateway/Client, Gateway/Server).
                //     This picks up filters/Root/**/*.cs automatically.
                var contextDirs = Enum.GetValues<FilterContext>()
                    .Select(ctx => Path.GetFullPath(Path.Combine(_rootDir, ctx.ToFolderPath())))
                    .ToArray();

                var rootFiles = Directory.Exists(_rootDir)
                    ? Directory.GetFiles(_rootDir, "*.cs", SearchOption.AllDirectories)
                        .Where(f => !contextDirs.Any(dir => Path.GetFullPath(f).StartsWith(dir + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)))
                        .ToList()
                    : new List<string>();

                int totalContextFiles = filesByContext.Values.Sum(v => v.Count);
                int totalFiles = totalContextFiles + rootFiles.Count;

                if (totalFiles == 0)
                {
                    Custom.WriteLine("[filters] no .cs files found — clearing all filters", ConsoleColor.Yellow);
                    _filters.Clear();
                    return;
                }

                // 2. Compile everything in one shot (so filters can share helpers)
                var syntaxTrees = new List<SyntaxTree>();

                // 2a. Root-level files first
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

                // 2b. Context files
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

                // 3. Reference every assembly the host has loaded
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

                // 3b. Also add every DLL sitting next to Module.dll (covers lazy-loaded deps)
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
                        catch { /* skip native / unloadable */ }
                    }
                }

                // 3c. Add the entire .NET shared framework. This is what lets filter
                //     code use Regex, HttpClient, JsonSerializer, System.Text.*, etc.
                //     Those assemblies aren't in AppDomain at compile time, nor next to
                //     Module.dll — they're in the runtime folder, listed in TPA.
                var tpa = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;
                if (!string.IsNullOrEmpty(tpa))
                {
                    foreach (var path in tpa.Split(Path.PathSeparator))
                    {
                        if (string.IsNullOrEmpty(path)) continue;
                        if (!path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)) continue;
                        if (!seen.Add(path)) continue;
                        try { refs.Add(MetadataReference.CreateFromFile(path)); }
                        catch { /* skip native / unloadable */ }
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

                // Resolve dependencies of the filter assembly. Filters reference the host
                // assembly ('Module') for IFilter / SessionData / PacketHandlingResult, and
                // SilkroadSecurityAPI for Packet. Both are already loaded by the host, so
                // reuse the loaded instances (identity must match, or IsAssignableFrom fails).
                newAlc.Resolving += (context, name) =>
                {
                    // 1. Already loaded? Reuse.
                    var loaded = AppDomain.CurrentDomain.GetAssemblies()
                        .FirstOrDefault(a => a.GetName().Name == name.Name);
                    if (loaded != null)
                        return loaded;

                    // 2. On disk next to Module.dll or the loader exe.
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

                    // 3. In the shared framework? Let the default resolver handle it.
                    return null;
                };

                var asm = newAlc.LoadFromStream(ms);

                // Wire up bridge delegates
                var hostType = asm.GetType("NetGuard.Hot.SessionHost");
                var bridgeType = asm.GetType("NetGuard.Hot.SessionBridge");
                var hostCtor = hostType?.GetConstructor(Type.EmptyTypes);

                // Use MethodInfo.CreateDelegate -> Action<SessionData>
                _attach = bridgeType?.GetMethod("Attach", new[] { typeof(SessionData) })
                    ?.CreateDelegate<Action<SessionData>>();
                _sync = bridgeType?.GetMethod("Sync", new[] { typeof(SessionData) })
                    ?.CreateDelegate<Action<SessionData>>();
                _migrate = bridgeType?.GetMethod("Migrate", new[] { typeof(object), typeof(object) })
                    ?.CreateDelegate<Action<object?, object?>>();

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

                // Track which type declared which (ctx, opcode) so duplicates can be reported precisely.
                var sourceMap = new Dictionary<(FilterContext, ushort), string>();

                foreach (var type in asm.GetTypes())
                {
                    if (!typeof(IFilter).IsAssignableFrom(type) || type.IsAbstract || type.IsInterface)
                        continue;

                    // Context comes from the namespace, not the filename.
                    // This lets two filters with the same class name (e.g. Handshake) exist in
                    // different contexts without colliding.
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

                // 5b. Register bus plugins (clears first, so reloads drop stale handlers)
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
                var oldAlc = _alc;
                _filters.Clear();
                foreach (var kvp in newFilters) _filters[kvp.Key] = kvp.Value;
                _alc = newAlc;

                // Re-attach live sessions to the new assembly
                foreach (var s in FilterRegistry.LiveSessions.Values)
                {
                    var oldHost = s.Host;
                    _attach?.Invoke(s);            // creates new SessionHost
                    _migrate?.Invoke(oldHost, s.Host); // copies preserved fields
                    _sync?.Invoke(s);              // shell <-> host
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
            _watcher?.Dispose();
            _debounce.Dispose();
            _alc?.Unload();
        }
    }
}