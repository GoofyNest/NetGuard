namespace NetGuard.Desktop;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        ApplicationConfiguration.Initialize();
        using var singleton = new Mutex(true, "Local\\NetGuard.Desktop." + Convert.ToHexString(
            System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(AppContext.BaseDirectory)))[..16], out bool first);
        if (!first) { MessageBox.Show("NetGuard is already open.", "NetGuard"); return; }
        Application.Run(new MainForm());
    }
}
