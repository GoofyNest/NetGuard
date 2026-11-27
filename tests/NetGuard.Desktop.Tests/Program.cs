using System.Reflection;
using System.Text.Json;
using NetGuard.Desktop;
using NetGuard.DesktopProtocol;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        string work = Path.Combine(Path.GetTempPath(), "NetGuardUiChecks-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(work, "config"));
        File.WriteAllText(Path.Combine(work, "config", "NetworkConfig.json"), JsonSerializer.Serialize(new
        {
            ModuleBinding = new[]
            {
                new { Name = "Gateway", ModuleType = 0, GuardIP = "127.0.0.1", GuardPort = 15779 },
                new { Name = "Agent", ModuleType = 2, GuardIP = "127.0.0.1", GuardPort = 15884 }
            }
        }));
        // The form normally resolves config next to the executable. Use an existing
        // staged config, or report the layout without launching any real workers.
        using var form = new MainForm(Path.Combine(work, "config", "NetworkConfig.json"));
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var add = typeof(MainForm).GetMethod("AddPacket", flags)!;
        add.Invoke(form, [new DesktopMessage { Module = 1, ClientId = 7, Ip = "127.0.0.1", FromClient = true,
            Opcode = 0x7074, Payload = [1, 1, 1, 1, 0x2A, 0x71, 0, 0], Length = 8 }]);
        form.Show(); Application.DoEvents();
        using (var bitmap = new Bitmap(form.Width, form.Height))
        {
            form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, form.Size));
            bitmap.Save(Path.Combine(work, "packet-view.png"));
        }
        var target = (ComboBox)typeof(MainForm).GetField("_target", flags)!.GetValue(form)!;
        if (target.SelectedIndex != -1) throw new Exception("Injection must require explicit target selection.");
        var tabs = Descendants(form).OfType<TabControl>().Single();
        tabs.SelectedIndex = 1; Application.DoEvents();
        using (var bitmap = new Bitmap(form.Width, form.Height))
        {
            form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, form.Size));
            bitmap.Save(Path.Combine(work, "injection-view.png"));
        }
        if (PacketFormatter.Format(new DesktopMessage { Payload = [0x41, 0], Length = 4 }).Contains("truncated") == false)
            throw new Exception("Truncation must be visible.");
        form.Close(); Application.DoEvents();
        Console.WriteLine("PASS: desktop layout, explicit injection target, and truncation presentation.");
        Console.WriteLine(work);
    }

    private static IEnumerable<Control> Descendants(Control control)
    {
        foreach (Control child in control.Controls)
        { yield return child; foreach (var nested in Descendants(child)) yield return nested; }
    }
}
