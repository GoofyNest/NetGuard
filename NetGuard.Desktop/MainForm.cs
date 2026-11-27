using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Threading.Channels;
using NetGuard.DesktopProtocol;

namespace NetGuard.Desktop;

internal sealed class MainForm : Form
{
    private readonly DataGridView _modules = new() { Dock = DockStyle.Fill };
    private readonly DataGridView _packets = new() { Dock = DockStyle.Fill };
    private readonly RichTextBox _hex = DarkTheme.Viewer(), _logs = DarkTheme.Viewer();
    private readonly ComboBox _ip = DarkTheme.Combo(), _moduleFilter = DarkTheme.Combo(), _target = DarkTheme.Combo(), _destination = DarkTheme.Combo();
    private readonly ListBox _ignore = OpcodeList(), _only = OpcodeList();
    private readonly CheckBox _client = Check("Client → server", true), _server = Check("Server → client", true),
        _fileLogging = Check("Save captured packets to files", false), _follow = Check("Follow latest packet", true),
        _encrypted = Check("Encrypted", true), _massive = Check("Massive", false);
    private readonly TextBox _opcode = DarkTheme.TextBox(), _payload = DarkTheme.TextBox(true);
    private readonly Label _captureState = Label("Capture is off", DarkTheme.Muted), _footer = Label("Ready", DarkTheme.Muted),
        _injectResult = Label("Select a live connection, then choose the destination.", DarkTheme.Muted);
    private readonly List<ModuleController> _controllers = [];
    private readonly Dictionary<int, PeerInfo[]> _peers = [];
    private readonly Dictionary<int, long> _sourceDrops = [];
    private readonly Channel<UiEvent> _events;
    private readonly PacketFileLog _fileLog = new();
    private readonly SemaphoreSlim _captureUpdate = new(1);
    private readonly System.Windows.Forms.Timer _refresh = new() { Interval = 100 }, _poll = new() { Interval = 1000 };
    private readonly HashSet<string> _seenIps = new(StringComparer.OrdinalIgnoreCase);
    private Button _captureButton = null!;
    private bool _capturing, _polling, _closing, _allowClose;
    private volatile bool _writeFiles;
    private long _displayDrops, _retainedBytes;
    private long _captured;

    private sealed record Choice(int Index, string Text) { public override string ToString() => Text; }
    private sealed record Target(int Module, int Client, string Ip)
    { public override string ToString() => $"Module {Module} · client {Client} · {Ip}"; }

    private readonly string _configurationPath;

    internal MainForm(string? configurationPath = null)
    {
        _configurationPath = configurationPath ?? Path.Combine(AppContext.BaseDirectory, "config", "NetworkConfig.json");
        Text = "NetGuard · Control Center"; Size = new Size(1380, 940); MinimumSize = new Size(1120, 780);
        StartPosition = FormStartPosition.CenterScreen; BackColor = DarkTheme.Background; ForeColor = DarkTheme.Text;
        Font = new Font("Segoe UI", 9.5f); AutoScaleMode = AutoScaleMode.Dpi;
        _events = Channel.CreateBounded<UiEvent>(new BoundedChannelOptions(256)
            { SingleReader = true, FullMode = BoundedChannelFullMode.DropOldest, AllowSynchronousContinuations = false },
            _ => Interlocked.Increment(ref _displayDrops));
        BuildLayout();
        LoadModules();
        _refresh.Tick += (_, _) => DrainEvents();
        _poll.Tick += async (_, _) => await PollAsync();
        _refresh.Start(); _poll.Start();
        Shown += (_, _) => DarkTheme.TitleBar(this);
        FormClosing += CloseAsync;
        _packets.SelectionChanged += (_, _) =>
        {
            if (_packets.SelectedRows.Count != 0 && _packets.SelectedRows[0].Tag is DesktopMessage packet)
                _hex.Text = PacketFormatter.Format(packet);
        };
    }

    private static Label Label(string text, Color? color = null) => new() { Text = text, ForeColor = color ?? DarkTheme.Text,
        BackColor = Color.Transparent, AutoSize = false, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
    private static CheckBox Check(string text, bool value) => new() { Text = text, Checked = value, AutoSize = true,
        ForeColor = DarkTheme.Text, Margin = new Padding(0, 5, 0, 5) };
    private static ListBox OpcodeList() => new() { BackColor = DarkTheme.Input, ForeColor = DarkTheme.Text,
        BorderStyle = BorderStyle.None, Font = new Font("Consolas", 10), Dock = DockStyle.Fill };

    private void BuildLayout()
    {
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(18), ColumnCount = 1, RowCount = 4 };
        root.RowStyles.Add(new(SizeType.Absolute, 58)); root.RowStyles.Add(new(SizeType.Absolute, 142));
        root.RowStyles.Add(new(SizeType.Percent, 100)); root.RowStyles.Add(new(SizeType.Absolute, 28));
        Controls.Add(root);
        var header = new Panel { Dock = DockStyle.Fill };
        var brand = Label("NETGUARD", DarkTheme.Accent); brand.Font = new Font("Segoe UI", 22, FontStyle.Bold); brand.Dock = DockStyle.Left; brand.Width = 235;
        header.Controls.Add(brand);
        var actions = new FlowLayoutPanel { Dock = DockStyle.Right, Width = 354, WrapContents = false, FlowDirection = FlowDirection.LeftToRight };
        actions.Controls.Add(DarkTheme.Button("Start all", async (_, _) => await StartAsync(_controllers), 100));
        actions.Controls.Add(DarkTheme.Button("Stop all", async (_, _) => await StopAsync(_controllers), 100));
        actions.Controls.Add(DarkTheme.Button("Output folder", (_, _) => OpenOutput(), 122));
        header.Controls.Add(actions); root.Controls.Add(header, 0, 0);
        var modulePanel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(0, 0, 0, 12) };
        modulePanel.ColumnStyles.Add(new(SizeType.Percent, 100)); modulePanel.ColumnStyles.Add(new(SizeType.Absolute, 118));
        DarkTheme.Grid(_modules);
        foreach (string name in new[] { "#", "Module", "Listen", "State", "Clients", "Live performance" }) _modules.Columns.Add(name, name);
        _modules.Columns[0].FillWeight = 20; _modules.Columns[1].FillWeight = 100; _modules.Columns[2].FillWeight = 100;
        _modules.Columns[3].FillWeight = 70; _modules.Columns[4].FillWeight = 35; _modules.Columns[5].FillWeight = 280;
        modulePanel.Controls.Add(_modules, 0, 0);
        var moduleActions = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown };
        moduleActions.Controls.Add(DarkTheme.Button("Start selected", async (_, _) => await StartAsync(SelectedModules()), 110));
        moduleActions.Controls.Add(DarkTheme.Button("Stop selected", async (_, _) => await StopAsync(SelectedModules()), 110));
        modulePanel.Controls.Add(moduleActions, 1, 0); root.Controls.Add(modulePanel, 0, 1);

        var body = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
        body.ColumnStyles.Add(new(SizeType.Percent, 100)); body.ColumnStyles.Add(new(SizeType.Absolute, 290));
        var tabs = new DarkTabControl { Dock = DockStyle.Fill, DrawMode = TabDrawMode.OwnerDrawFixed, ItemSize = new Size(135, 32), SizeMode = TabSizeMode.Fixed };
        var capture = new TabPage("Packet viewer") { BackColor = DarkTheme.Panel, Padding = new Padding(6) };
        var injection = new TabPage("Inject packet") { BackColor = DarkTheme.Panel, Padding = new Padding(18) };
        var logs = new TabPage("Module logs") { BackColor = DarkTheme.Panel, Padding = new Padding(6) };
        tabs.TabPages.AddRange([capture, injection, logs]); logs.Controls.Add(_logs);
        var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, BackColor = DarkTheme.Input,
            SplitterWidth = 6, Size = new Size(850, 600), SplitterDistance = 270, Panel1MinSize = 120, Panel2MinSize = 100 };
        DarkTheme.Grid(_packets);
        foreach (string name in new[] { "Time", "Module", "Client IP", "ID", "Direction", "Opcode", "Bytes" }) _packets.Columns.Add(name, name);
        _packets.Columns[0].FillWeight = 95; _packets.Columns[1].FillWeight = 55; _packets.Columns[2].FillWeight = 120;
        _packets.Columns[3].FillWeight = 40; _packets.Columns[4].FillWeight = 75; _packets.Columns[5].FillWeight = 65; _packets.Columns[6].FillWeight = 55;
        split.Panel1.Controls.Add(_packets); split.Panel2.Controls.Add(_hex); capture.Controls.Add(split);
        BuildInjection(injection);
        body.Controls.Add(tabs, 0, 0); body.Controls.Add(BuildSidebar(), 1, 0); root.Controls.Add(body, 0, 2);
        root.Controls.Add(_footer, 0, 3);
    }

    private Control BuildSidebar()
    {
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, BackColor = DarkTheme.Panel, Padding = new Padding(14, 8, 10, 8), ColumnCount = 1, RowCount = 11 };
        foreach (int height in new[] { 25, 33, 25, 33, 43, 25 }) panel.RowStyles.Add(new(SizeType.Absolute, height));
        panel.RowStyles.Add(new(SizeType.Percent, 50)); panel.RowStyles.Add(new(SizeType.Percent, 50));
        panel.RowStyles.Add(new(SizeType.Absolute, 95)); panel.RowStyles.Add(new(SizeType.Absolute, 40)); panel.RowStyles.Add(new(SizeType.Absolute, 38));
        panel.Controls.Add(Label("MODULE", DarkTheme.Muted), 0, 0); _moduleFilter.Dock = DockStyle.Fill; panel.Controls.Add(_moduleFilter, 0, 1);
        panel.Controls.Add(Label("CLIENT IP", DarkTheme.Muted), 0, 2);
        _ip.DropDownStyle = ComboBoxStyle.DropDown; _ip.Items.Add("All IPs"); _ip.SelectedIndex = 0; _ip.Dock = DockStyle.Fill; panel.Controls.Add(_ip, 0, 3);
        _captureButton = DarkTheme.Button("Start capture", async (_, _) =>
        {
            try { _capturing = !_capturing; await ApplyCaptureAsync(); }
            catch (Exception ex) { _capturing = false; ShowError(ex); }
            UpdateCaptureLabel();
        }, 240);
        _captureButton.Dock = DockStyle.Fill; panel.Controls.Add(_captureButton, 0, 4); panel.Controls.Add(_captureState, 0, 5);
        panel.Controls.Add(BuildOpcodePanel("IGNORE OPCODES", _ignore), 0, 6);
        panel.Controls.Add(BuildOpcodePanel("SHOW ONLY OPCODES", _only), 0, 7);
        var options = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false };
        var directions = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Margin = Padding.Empty };
        directions.Controls.Add(_client); directions.Controls.Add(_server); options.Controls.Add(directions);
        options.Controls.Add(_fileLogging); options.Controls.Add(_follow);
        _fileLogging.CheckedChanged += (_, _) => _writeFiles = _fileLogging.Checked;
        panel.Controls.Add(options, 0, 8);
        panel.Controls.Add(DarkTheme.Button("Apply filters", async (_, _) => { try { await ApplyCaptureAsync(); } catch (Exception ex) { ShowError(ex); } }, 240), 0, 9);
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        buttons.Controls.Add(DarkTheme.Button("Clear view", (_, _) => ClearPackets(), 112));
        buttons.Controls.Add(DarkTheme.Button("Save view", async (_, _) => await SaveViewAsync(), 112));
        panel.Controls.Add(buttons, 0, 10);
        return panel;
    }

    private Control BuildOpcodePanel(string title, ListBox list)
    {
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1, Padding = new Padding(0, 5, 0, 5) };
        panel.RowStyles.Add(new(SizeType.Absolute, 23)); panel.RowStyles.Add(new(SizeType.Percent, 100)); panel.RowStyles.Add(new(SizeType.Absolute, 37));
        panel.Controls.Add(Label(title, DarkTheme.Muted), 0, 0); panel.Controls.Add(list, 0, 1);
        var controls = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, Margin = Padding.Empty };
        var value = DarkTheme.TextBox(); value.Width = 62; value.Margin = new Padding(0, 7, 3, 0); value.PlaceholderText = "0x7074";
        controls.Controls.Add(value);
        controls.Controls.Add(DarkTheme.Button("+", (_, _) =>
        {
            if (!TryOpcode(value.Text, out ushort opcode)) { ShowError(new FormatException("Enter a hexadecimal opcode, for example 7074 or 0x7074.")); return; }
            string item = opcode.ToString("X4"); if (!list.Items.Contains(item)) list.Items.Add(item); value.Clear();
        }, 36));
        controls.Controls.Add(DarkTheme.Button("−", (_, _) => { if (list.SelectedIndex >= 0) list.Items.RemoveAt(list.SelectedIndex); }, 36));
        controls.Controls.Add(DarkTheme.Button("Clear", (_, _) => list.Items.Clear(), 65)); panel.Controls.Add(controls, 0, 2);
        return panel;
    }

    private void BuildInjection(Control page)
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 8 };
        layout.ColumnStyles.Add(new(SizeType.Absolute, 145)); layout.ColumnStyles.Add(new(SizeType.Percent, 100));
        foreach (int height in new[] { 40, 40, 40, 40, 30 }) layout.RowStyles.Add(new(SizeType.Absolute, height));
        layout.RowStyles.Add(new(SizeType.Percent, 100)); layout.RowStyles.Add(new(SizeType.Absolute, 46)); layout.RowStyles.Add(new(SizeType.Absolute, 70));
        layout.Controls.Add(Label("Live connection"), 0, 0); _target.Dock = DockStyle.Fill; layout.Controls.Add(_target, 1, 0);
        layout.Controls.Add(Label("Destination"), 0, 1); _destination.Items.AddRange(["Remote → server", "Local → client"]); _destination.SelectedIndex = 0;
        _destination.Dock = DockStyle.Fill; layout.Controls.Add(_destination, 1, 1);
        layout.Controls.Add(Label("Opcode (hex)"), 0, 2); _opcode.Text = ""; _opcode.PlaceholderText = "e.g. 0x7074"; _opcode.Dock = DockStyle.Fill; layout.Controls.Add(_opcode, 1, 2);
        layout.Controls.Add(Label("Packet flags"), 0, 3);
        var flags = new FlowLayoutPanel { Dock = DockStyle.Fill }; flags.Controls.Add(_encrypted); flags.Controls.Add(_massive); layout.Controls.Add(flags, 1, 3);
        _encrypted.CheckedChanged += (_, _) => { if (_encrypted.Checked) _massive.Checked = false; };
        _massive.CheckedChanged += (_, _) => { if (_massive.Checked) _encrypted.Checked = false; };
        var description = Label("HEX PAYLOAD  ·  bytes only, without the packet header", DarkTheme.Muted); layout.Controls.Add(description, 0, 4); layout.SetColumnSpan(description, 2);
        _payload.Dock = DockStyle.Fill; _payload.ScrollBars = ScrollBars.Both; _payload.AcceptsReturn = true; _payload.WordWrap = false;
        _payload.PlaceholderText = "01 00 2A 71 00 00"; layout.Controls.Add(_payload, 0, 5); layout.SetColumnSpan(_payload, 2);
        var send = DarkTheme.Button("Inject packet", async (_, _) => await InjectAsync(), 160); layout.Controls.Add(send, 1, 6);
        _injectResult.Text = "Injection uses the ordered socket writer. 0x5000, 0x9000 and 0x600D are reserved.\nMaximum injection payload: 32,767 bytes.";
        layout.Controls.Add(_injectResult, 0, 7); layout.SetColumnSpan(_injectResult, 2); page.Controls.Add(layout);
    }

    private void LoadModules()
    {
        _moduleFilter.Items.Add(new Choice(-1, "All modules")); _moduleFilter.SelectedIndex = 0;
        try
        {
            string path = _configurationPath;
            using var json = JsonDocument.Parse(File.ReadAllText(path));
            int index = 0;
            foreach (var binding in json.RootElement.GetProperty("ModuleBinding").EnumerateArray())
            {
                string name = binding.GetProperty("Name").GetString() ?? $"Module {index}";
                var type = binding.GetProperty("ModuleType");
                string kind = type.ValueKind == JsonValueKind.Number ? type.GetInt32() switch { 0 => "Gateway", 2 => "Agent", _ => "Unsupported" } : type.GetString()!.Replace("Module", "");
                var definition = new ModuleDefinition(index, name, kind, $"{binding.GetProperty("GuardIP").GetString()}:{binding.GetProperty("GuardPort").GetInt32()}");
                _controllers.Add(new(definition, Receive));
                _modules.Rows.Add(index, $"{kind} · {name}", definition.Listen, "Stopped", "—", "Start module to view live performance");
                _moduleFilter.Items.Add(new Choice(index, $"{index} · {kind} · {name}")); index++;
            }
            AppendLog("Open NetGuard.exe to manage modules. Existing console instances must be stopped before starting the same ports here.");
            AppendLog("Capture shows received packets before filters, including packets later blocked. Payloads over 64 KiB are explicitly truncated.");
        }
        catch (Exception ex) { AppendLog("Configuration error: " + ex.Message); _footer.Text = "Cannot read config/NetworkConfig.json"; }
    }

    private IEnumerable<ModuleController> SelectedModules() => _modules.SelectedRows.Count == 0 ? [] : [_controllers[_modules.SelectedRows[0].Index]];
    private async Task StartAsync(IEnumerable<ModuleController> controllers)
    {
        foreach (var controller in controllers.ToArray())
        {
            try
            {
                if (controller.Definition.Kind is not ("Gateway" or "Agent")) throw new InvalidOperationException("This module type is not supported.");
                controller.Start(); await controller.SendAsync(new("capture", CaptureFor(controller.Definition.Index)));
            }
            catch (Exception ex) { AppendLog($"Module {controller.Definition.Index}: {ex.Message}"); }
        }
    }

    private async Task StopAsync(IEnumerable<ModuleController> controllers)
    {
        foreach (var controller in controllers.ToArray())
            try { await controller.StopAsync(); } catch (Exception ex) { AppendLog($"Module {controller.Definition.Index}: {ex.Message}"); }
    }

    private CaptureOptions CaptureFor(int module)
    {
        if (!_capturing) return new CaptureOptions();
        string? ip = _ip.Text.Trim(); if (ip is "" or "All IPs") ip = null;
        if (ip != null && !IPAddress.TryParse(ip, out _)) throw new FormatException("Select an IP or enter a valid IP address.");
        int selected = (_moduleFilter.SelectedItem as Choice)?.Index ?? -1;
        return new CaptureOptions { Enabled = _capturing && (selected == -1 || selected == module), Ip = ip,
            ClientToServer = _client.Checked, ServerToClient = _server.Checked,
            Ignore = _ignore.Items.Cast<string>().Select(s => ushort.Parse(s, NumberStyles.HexNumber)).ToArray(),
            Only = _only.Items.Cast<string>().Select(s => ushort.Parse(s, NumberStyles.HexNumber)).ToArray() };
    }

    private async Task ApplyCaptureAsync()
    {
        await _captureUpdate.WaitAsync();
        try
        {
            var settings = _controllers.Select(c => (Controller: c, Options: CaptureFor(c.Definition.Index))).ToArray();
            foreach (var (controller, options) in settings) await controller.SendAsync(new("capture", options));
            UpdateCaptureLabel();
        }
        finally { _captureUpdate.Release(); }
    }

    private void UpdateCaptureLabel()
    {
        _captureButton.Text = _capturing ? "Stop capture" : "Start capture";
        _captureState.Text = _capturing ? "● Capturing · filters applied" : "Capture is off";
        _captureState.ForeColor = _capturing ? DarkTheme.Accent : DarkTheme.Muted;
    }

    private async Task PollAsync()
    {
        if (_polling || _closing) return;
        _polling = true;
        try
        {
            foreach (var controller in _controllers.Where(c => c.Running))
                try { await controller.SendAsync(new("status")); } catch (Exception ex) { AppendLog(ex.Message); }
        }
        finally { _polling = false; }
    }

    private void Receive(UiEvent entry)
    {
        if (entry.Message is { Type: "packet" } packet && _writeFiles) _fileLog.Enqueue(packet);
        _events.Writer.TryWrite(entry);
    }

    private void DrainEvents()
    {
        for (int i = 0; i < 100 && _events.Reader.TryRead(out var entry); i++)
        {
            if (entry.Log != null) AppendLog($"[{entry.Module}] {entry.Log}");
            if (entry.State != null)
            {
                _modules.Rows[entry.Module].Cells[3].Value = entry.State;
                if (entry.State.StartsWith("Stopped") || entry.State.StartsWith("Exited"))
                { _peers.Remove(entry.Module); _modules.Rows[entry.Module].Cells[4].Value = "0"; UpdateTargets(); }
            }
            if (entry.Message is not { } message) continue;
            if (message.Type == "status")
            {
                _modules.Rows[entry.Module].Cells[3].Value = "Running";
                _modules.Rows[entry.Module].Cells[4].Value = message.Peers?.Length ?? 0;
                _modules.Rows[entry.Module].Cells[5].Value = message.Text;
                _modules.Rows[entry.Module].Cells[5].ToolTipText = message.Text;
                _peers[entry.Module] = message.Peers ?? []; _sourceDrops[entry.Module] = message.Dropped;
                foreach (var peer in message.Peers ?? []) if (_seenIps.Add(peer.Ip)) _ip.Items.Add(peer.Ip);
                UpdateTargets();
            }
            else if (message.Type == "packet") AddPacket(message);
            else if (message.Type == "log") AppendLog($"[{entry.Module}] [{message.Time:HH:mm:ss}] {message.Text}");
            else if (message.Type == "injection") { _injectResult.Text = message.Text; AppendLog(message.Text ?? ""); }
        }
        _footer.Text = $"Captured {_captured:N0} · Showing {_packets.Rows.Count:N0} · Dropped: source {_sourceDrops.Values.Sum():N0}, viewer {Interlocked.Read(ref _displayDrops):N0}, file {_fileLog.Dropped:N0}"
            + (_fileLog.Error != null ? " · File logging error: " + _fileLog.Error : "");
    }

    private void UpdateTargets()
    {
        var targets = _peers.OrderBy(p => p.Key).SelectMany(p => p.Value.Select(peer => new Target(p.Key, peer.ClientId, peer.Ip))).ToArray();
        if (_target.Items.Cast<Target>().SequenceEqual(targets)) return;
        var selected = _target.SelectedItem as Target;
        _target.Items.Clear(); _target.Items.AddRange(targets);
        if (selected != null && _target.Items.Contains(selected)) _target.SelectedItem = selected;
        else _target.SelectedIndex = -1;
    }

    private void AddPacket(DesktopMessage packet)
    {
        _captured++; _retainedBytes += packet.Payload?.Length ?? 0;
        int row = _packets.Rows.Add(packet.Time.ToString("HH:mm:ss.fff"), packet.Module, packet.Ip ?? "", packet.ClientId,
            packet.FromClient ? "C → S" : "S → C", $"0x{packet.Opcode:X4}", packet.Length);
        _packets.Rows[row].Tag = packet;
        _packets.Rows[row].Cells[4].Style.ForeColor = packet.FromClient ? DarkTheme.Accent : Color.FromArgb(137, 208, 168);
        while (_packets.Rows.Count > 1000 || _retainedBytes > 16 * 1024 * 1024)
        {
            _retainedBytes -= ((_packets.Rows[0].Tag as DesktopMessage)?.Payload?.Length ?? 0); _packets.Rows.RemoveAt(0);
        }
        if (_follow.Checked && _packets.Rows.Count != 0)
        {
            _packets.ClearSelection(); _packets.Rows[^1].Selected = true;
            _packets.FirstDisplayedScrollingRowIndex = _packets.Rows.Count - 1;
        }
    }

    private void ClearPackets() { _packets.Rows.Clear(); _hex.Clear(); _retainedBytes = 0; }
    private void AppendLog(string text)
    {
        if (_logs.TextLength > 250000) { _logs.Select(0, 100000); _logs.SelectedText = ""; }
        _logs.AppendText(text + Environment.NewLine); _logs.SelectionStart = _logs.TextLength; _logs.ScrollToCaret();
    }
    private void ShowError(Exception ex) => MessageBox.Show(this, ex.Message, "NetGuard", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    private void OpenOutput()
    {
        try { Directory.CreateDirectory(_fileLog.DirectoryPath); Process.Start(new ProcessStartInfo(_fileLog.DirectoryPath) { UseShellExecute = true }); }
        catch (Exception ex) { ShowError(ex); }
    }
    private async Task SaveViewAsync()
    {
        using var dialog = new SaveFileDialog { Filter = "Text files|*.txt", FileName = "packets.txt", InitialDirectory = AppContext.BaseDirectory };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        var packets = _packets.Rows.Cast<DataGridViewRow>().Select(r => (DesktopMessage)r.Tag!).ToArray();
        try { await Task.Run(() => File.WriteAllText(dialog.FileName, string.Join(Environment.NewLine, packets.Select(PacketFormatter.Format)))); }
        catch (Exception ex) { ShowError(ex); }
    }
    private static bool TryOpcode(string text, out ushort opcode) => ushort.TryParse(text.Trim().Replace("0x", "", StringComparison.OrdinalIgnoreCase), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out opcode);

    private async Task InjectAsync()
    {
        try
        {
            if (_target.SelectedItem is not Target target) throw new InvalidOperationException("Select a specific live connection first.");
            if (!TryOpcode(_opcode.Text, out ushort opcode)) throw new FormatException("Enter a hexadecimal opcode.");
            if (opcode is 0x5000 or 0x9000 or 0x600D) throw new InvalidOperationException("Transport-control opcodes are reserved.");
            string hex = string.Concat(_payload.Text.Where(c => !char.IsWhiteSpace(c)));
            if (hex.Length > 65534) throw new FormatException("Maximum payload is 32,767 bytes.");
            byte[] bytes = Convert.FromHexString(hex);
            if (_massive.Checked && bytes.Length == 0) throw new FormatException("A massive packet needs a payload.");
            if (!_controllers[target.Module].Running) throw new InvalidOperationException("The module is not running.");
            await _controllers[target.Module].SendAsync(new("inject", Injection: new(target.Client, _destination.SelectedIndex == 0, opcode, bytes, _encrypted.Checked, _massive.Checked)));
            _injectResult.Text = "Injection queued; waiting for socket-write confirmation…";
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private async void CloseAsync(object? sender, FormClosingEventArgs e)
    {
        if (_allowClose) return;
        e.Cancel = true;
        if (_closing) return;
        if (_controllers.Any(c => c.Running) && MessageBox.Show(this, "Stop the running modules and close NetGuard? Connected players will disconnect.",
            "Close NetGuard", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        _closing = true; Enabled = false; _poll.Stop();
        await StopAsync(_controllers);
        if (_controllers.Any(c => c.Running))
        {
            _closing = false; Enabled = true; _poll.Start(); ShowError(new IOException("A module has not stopped yet. Check Module logs and retry.")); return;
        }
        _refresh.Stop();
        try { await _fileLog.DisposeAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(5)); } catch { }
        _allowClose = true; Close();
    }
}
