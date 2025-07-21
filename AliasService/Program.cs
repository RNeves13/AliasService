using System.Runtime.InteropServices;
using AliasService.Runner;

namespace AliasService;

static class Program
{
    
    // Constants for hotkey registration
    private const int HOTKEY_ID = 9000;
    private const int WM_HOTKEY = 0x0312;

    // Modifier keys
    private const uint MOD_CONTROL = 0x0002;
    private const uint MOD_SHIFT = 0x0004;
    private const uint MOD_NOREPEAT = 0x4000;

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private static AliasRunner runner;
    

    [STAThread]
    static void Main()
    {
        
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        
        var trayIcon = new NotifyIcon();
        trayIcon.Icon = SystemIcons.Application;
        trayIcon.Text = "AliasService";
        trayIcon.Visible = true;
        
        var contextMenu = new ContextMenuStrip();
        var runItem = new ToolStripMenuItem("Run alias");
        var exitItem = new ToolStripMenuItem("Exit");

        try
        {
            runner = new AliasRunner();
        }
        catch (Exception e)
        {
            MessageBox.Show("Invalid aliases file", "Error");
            Application.Exit();
        }
        
        runItem.Click += (s, e) =>
        {
           MainAction();
        };
        
        exitItem.Click += (s, e) =>
        {
            trayIcon.Visible = false;
            Application.Exit();
        };
        
        contextMenu.Items.Add(runItem);
        contextMenu.Items.Add(exitItem);
        trayIcon.ContextMenuStrip = contextMenu;
        
        trayIcon.DoubleClick += (s, e) =>
        {
            MainAction();
        };

        // Create a hidden form to handle hotkey messages
        using (var messageWindow = new HotkeyWindow(MainAction))
        {
            RegisterHotKey(messageWindow.Handle, HOTKEY_ID, MOD_CONTROL | MOD_SHIFT | MOD_NOREPEAT, (uint)Keys.R);

            Application.Run();
            
            UnregisterHotKey(messageWindow.Handle, HOTKEY_ID);
        }
    }


    private static void MainAction()
    {
        var result = ShowInputDialog("What's your name?");
        if (string.IsNullOrWhiteSpace(result)) MessageBox.Show("Please enter a alias", "Error");
        if (!runner.RunAlias(result)) MessageBox.Show("Alias not found", "Error");
    }
    
    private static string ShowInputDialog(string prompt)
    {
        Form inputForm = new Form()
        {
            Width = 300,
            Height = 150,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            Text = "Input Required",
            StartPosition = FormStartPosition.CenterScreen
        };

        Label textLabel = new Label() { Left = 10, Top = 20, Text = prompt, AutoSize = true };
        TextBox inputBox = new TextBox() { Left = 10, Top = 45, Width = 260 };
        Button confirmation = new Button() { Text = "OK", Left = 190, Width = 80, Top = 75, DialogResult = DialogResult.OK };

        inputForm.Controls.Add(textLabel);
        inputForm.Controls.Add(inputBox);
        inputForm.Controls.Add(confirmation);
        inputForm.AcceptButton = confirmation;

        return inputForm.ShowDialog() == DialogResult.OK ? inputBox.Text : null;
    }
    
    
    private class HotkeyWindow : Form
    {
        private readonly Action _onHotkey;

        public HotkeyWindow(Action onHotkey)
        {
            _onHotkey = onHotkey;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Minimized;
            Load += (s, e) => Hide(); // Stay hidden
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
            {
                _onHotkey?.Invoke();
            }
            base.WndProc(ref m);
        }
    }
}

