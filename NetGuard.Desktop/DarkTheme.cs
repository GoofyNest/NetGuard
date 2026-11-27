using System.Runtime.InteropServices;

namespace NetGuard.Desktop;

internal static class DarkTheme
{
    internal static readonly Color Background = Color.FromArgb(17, 20, 26);
    internal static readonly Color Panel = Color.FromArgb(25, 30, 38);
    internal static readonly Color Input = Color.FromArgb(32, 38, 48);
    internal static readonly Color Text = Color.FromArgb(226, 232, 240);
    internal static readonly Color Muted = Color.FromArgb(145, 158, 179);
    internal static readonly Color Accent = Color.FromArgb(89, 177, 240);

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr handle, int attribute, ref int value, int size);
    internal static void TitleBar(Form form)
    {
        int enabled = 1;
        DwmSetWindowAttribute(form.Handle, 20, ref enabled, sizeof(int));
    }

    internal static Button Button(string text, EventHandler click, int width = 112)
    {
        var button = new Button { Text = text, AutoSize = false, Width = width, Height = 32, FlatStyle = FlatStyle.Flat,
            BackColor = Input, ForeColor = Text, Margin = new Padding(4), Cursor = Cursors.Hand };
        button.FlatAppearance.BorderColor = Color.FromArgb(55, 66, 83);
        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(43, 57, 74);
        button.Click += click;
        return button;
    }

    internal static void Grid(DataGridView grid)
    {
        grid.BackgroundColor = Panel; grid.BorderStyle = BorderStyle.None; grid.GridColor = Input;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Input, ForeColor = Muted, SelectionBackColor = Input };
        grid.DefaultCellStyle = new DataGridViewCellStyle { BackColor = Panel, ForeColor = Text, SelectionBackColor = Color.FromArgb(39, 67, 92), SelectionForeColor = Text };
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(22, 27, 35);
        grid.RowHeadersVisible = false; grid.AllowUserToAddRows = false; grid.AllowUserToDeleteRows = false;
        grid.ReadOnly = true; grid.MultiSelect = false; grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.RowTemplate.Height = 26; grid.ColumnHeadersHeight = 30;
    }

    internal static RichTextBox Viewer() => new() { Dock = DockStyle.Fill, ReadOnly = true, WordWrap = false,
        BackColor = Background, ForeColor = Text, BorderStyle = BorderStyle.None, Font = new Font("Consolas", 10), DetectUrls = false };

    internal static TextBox TextBox(bool multiline = false) => new() { BackColor = Input, ForeColor = Text,
        BorderStyle = BorderStyle.FixedSingle, Multiline = multiline, Font = new Font("Consolas", 10) };

    internal static ComboBox Combo() => new DarkComboBox { BackColor = Input, ForeColor = Text, FlatStyle = FlatStyle.Flat,
        DropDownStyle = ComboBoxStyle.DropDownList, IntegralHeight = false, DropDownHeight = 220 };
}

internal sealed class DarkComboBox : ComboBox
{
    internal DarkComboBox() { DrawMode = DrawMode.OwnerDrawFixed; ItemHeight = 23; }
    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        using var fill = new SolidBrush((e.State & DrawItemState.Selected) != 0 ? Color.FromArgb(39, 67, 92) : DarkTheme.Input);
        e.Graphics.FillRectangle(fill, e.Bounds);
        string text = (e.Index >= 0 ? GetItemText(Items[e.Index]) : Text) ?? "";
        TextRenderer.DrawText(e.Graphics, text, Font, e.Bounds, DarkTheme.Text, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
    }
    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
        if (m.Msg != 0xF) return;
        using var graphics = Graphics.FromHwnd(Handle);
        using var fill = new SolidBrush(DarkTheme.Input);
        var arrow = new Rectangle(Width - 20, 1, 19, Height - 2);
        graphics.FillRectangle(fill, arrow);
        using var pen = new Pen(DarkTheme.Muted);
        int x = Width - 11, y = Height / 2;
        graphics.DrawLines(pen, [new Point(x - 4, y - 2), new Point(x, y + 2), new Point(x + 4, y - 2)]);
        using var border = new Pen(DarkTheme.Input);
        graphics.DrawRectangle(border, 0, 0, Width - 1, Height - 1);
    }
}

internal sealed class DarkTabControl : TabControl
{
    internal DarkTabControl() => SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.Clear(DarkTheme.Background);
        for (int i = 0; i < TabCount; i++)
        {
            Rectangle rectangle = GetTabRect(i);
            using var brush = new SolidBrush(i == SelectedIndex ? DarkTheme.Input : DarkTheme.Panel);
            e.Graphics.FillRectangle(brush, rectangle);
            TextRenderer.DrawText(e.Graphics, TabPages[i].Text, Font, rectangle, DarkTheme.Text,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            if (i == SelectedIndex)
            {
                using var accent = new Pen(DarkTheme.Accent, 2);
                e.Graphics.DrawLine(accent, rectangle.Left + 8, rectangle.Bottom - 2, rectangle.Right - 8, rectangle.Bottom - 2);
            }
        }
    }
    protected override void OnSelectedIndexChanged(EventArgs e) { base.OnSelectedIndexChanged(e); Invalidate(); }
}
