using Microsoft.Win32;
using sergiye.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace rdpWrapper {

  public partial class MainForm : Form {

    private const int MF_SEPARATOR = 0x00000800;
    private const int MF_BY_POSITION = 0x400;
    private const int MF_WM_SYS_COMMAND = 0x112;
    private const int MF_SYS_MENU_CHECK_UPDATES = 1000;
    private const int MF_SYS_MENU_ABOUT_ID = 1001;

    private const string REG_KEY = @"SYSTEM\CurrentControlSet\Control\Terminal Server";
    private const string REG_RDP_KEY = @"SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp";
    private const string REG_WINLOGON_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System";
    private const string REG_TS_KEY = @"SOFTWARE\Policies\Microsoft\Windows NT\Terminal Services";

    private const string VALUE_SINGLE_SESSION = "fSingleSessionPerUser";
    private const string VALUE_DENY_TS_CONNECTIONS = "fDenyTSConnections";
    private const string VALUE_PORT = "PortNumber";
    private const string VALUE_DONTDISPLAYLASTUSERNAME = "DontDisplayLastUsername";
    private const string VALUE_HONORLEGACY = "HonorLegacySettings";
    private const string VALUE_NLA = "UserAuthentication";
    private const string VALUE_SECURITY = "SecurityLayer";
    private const string VALUE_SHADOW = "Shadow";

    private int oldPort;
    private readonly Timer refreshTimer;

    public MainForm() {

      InitializeComponent();

      Icon = System.Drawing.Icon.ExtractAssociatedIcon(typeof(MainForm).Assembly.Location);
      Text = $"{Updater.ApplicationTitle} v{Updater.CurrentVersion} {(Environment.Is64BitProcess ? "x64" : "x86")}";
      StartPosition = FormStartPosition.CenterScreen;

      rgNLAOptions.Items.AddRange([
        "GUI Authentication Only", 
        "Default RDP Authentication", 
        "Network Level Authentication" 
      ]);
      rgShadowOptions.Items.AddRange([
        "Disable Shadowing",
        "Full access with user's permission",
        "Full access without permission",
        "View only with user's permission",
        "View only without permission"
      ]);

      var menuHandle = GetSystemMenu(Handle, false); // Note: to restore default set true
      InsertMenu(menuHandle, 5, MF_BY_POSITION | MF_SEPARATOR, 0, string.Empty); // <-- Add a menu separator
      InsertMenu(menuHandle, 6, MF_BY_POSITION, MF_SYS_MENU_CHECK_UPDATES, "Check for new version");
      InsertMenu(menuHandle, 7, MF_BY_POSITION, MF_SYS_MENU_ABOUT_ID, "&About…");

      Load += mainFormLoad;

      refreshTimer = new Timer();
      refreshTimer.Tick += timerTick;
      refreshTimer.Interval = 1000;

      var timer = new Timer();
      timer.Tick += (_, _) => {
        timer.Enabled = false;
        timer.Enabled = !Updater.CheckForUpdates(true);
      };
      timer.Interval = 1000;
      timer.Enabled = true;
    }

    [DllImport("user32.dll")]
    private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
    [DllImport("user32.dll")]
    private static extern bool InsertMenu(IntPtr hMenu, int wPosition, int wFlags, int wIdNewItem, string lpNewItem);
    
    protected override void WndProc(ref Message m) {

      base.WndProc(ref m);
      
      if (m.Msg != MF_WM_SYS_COMMAND)
        return;
      
      switch ((int)m.WParam) {
        case MF_SYS_MENU_ABOUT_ID:
          var asm = GetType().Assembly;
          MessageBox.Show($"{Updater.ApplicationTitle} {asm.GetName().Version.ToString(3)} {(Environment.Is64BitProcess ? "x64" : "x32")}\nWritten by Sergiy Egoshyn (egoshin.sergey@gmail.com)", Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
          break;
        case MF_SYS_MENU_CHECK_UPDATES:
          Updater.CheckForUpdates(false);
          break;
      }
    }

    private void mainFormLoad(object sender, EventArgs e) {
      try {
        //RDP.DisconnectedText := 'Disconnected.';
        //RDP.ConnectingText := 'Connecting...';
        //RDP.ConnectedStatusText := 'Connected.';
        //RDP.UserName := '';
        //RDP.Server := '127.0.0.2';

        using (var key = Registry.LocalMachine.OpenSubKey(REG_KEY)) {
          cbxSingleSessionPerUser.Checked = Convert.ToInt32(key.GetValue(VALUE_SINGLE_SESSION, 0)) != 0;
          cbxAllowTSConnections.Checked = Convert.ToInt32(key.GetValue(VALUE_DENY_TS_CONNECTIONS, 0)) == 0;
          cbxHonorLegacy.Checked = Convert.ToInt32(key.GetValue(VALUE_HONORLEGACY, 0)) != 0;
        }

        using (var key = Registry.LocalMachine.OpenSubKey(REG_RDP_KEY)) {
          oldPort = Convert.ToInt32(key.GetValue(VALUE_PORT, 3389));
          seRDPPort.Value = oldPort;
          int userAuthentication = Convert.ToInt32(key.GetValue(VALUE_NLA, 0));
          int securityLayer = Convert.ToInt32(key.GetValue(VALUE_SECURITY, 0));

          if (securityLayer == 0 && userAuthentication == 0)
            rgNLAOptions.SelectedIndex = 0;
          else if (securityLayer == 1 && userAuthentication == 0)
            rgNLAOptions.SelectedIndex = 1;
          else if (securityLayer == 2 && userAuthentication == 1) 
            rgNLAOptions.SelectedIndex = 2;

          rgShadowOptions.SelectedIndex = Convert.ToInt32(key.GetValue(VALUE_SHADOW, 0));
        }

        using (var key = Registry.LocalMachine.OpenSubKey(REG_WINLOGON_KEY)) {
          cbDontDisplayLastUser.Checked = Convert.ToInt32(key.GetValue(VALUE_DONTDISPLAYLASTUSERNAME, 0)) != 0;
        }
      }
      catch (Exception ex) {
        MessageBox.Show("Error loading settings: " + ex.Message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      btnApply.Enabled = false;
    }

    private void btnCloseClick(object sender, EventArgs e) {
      Close();
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
      if (btnApply.Enabled && MessageBox.Show("Settings are not saved. Do you want to exit?", Updater.ApplicationTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) {
        e.Cancel = true;
      }
    }

    private void btnApplyClick(object sender, EventArgs e) {
      try {
        using (var key = Registry.LocalMachine.OpenSubKey(REG_KEY, writable: true)) {
          key.SetValue(VALUE_SINGLE_SESSION, cbxSingleSessionPerUser.Checked ? 1 : 0, RegistryValueKind.DWord);
          key.SetValue(VALUE_DENY_TS_CONNECTIONS, cbxAllowTSConnections.Checked ? 0 : 1, RegistryValueKind.DWord);
          key.SetValue(VALUE_HONORLEGACY, cbxHonorLegacy.Checked ? 0 : 1, RegistryValueKind.DWord);
        }

        var newPort = (int)seRDPPort.Value;
        if (oldPort != newPort) {
          var p = StartProcess("netsh", $"advfirewall firewall set rule name=\"Remote Desktop\" new localport={newPort}");
          p.WaitForExit();
        }

        using (var key = Registry.LocalMachine.OpenSubKey(REG_RDP_KEY, true)) {
          key.SetValue(VALUE_PORT, newPort, RegistryValueKind.DWord);
          oldPort = newPort;

          switch (rgNLAOptions.SelectedIndex) {
            case 0:
              key.SetValue(VALUE_NLA, 0, RegistryValueKind.DWord);
              key.SetValue(VALUE_SECURITY, 0, RegistryValueKind.DWord);
              break;
            case 1:
              key.SetValue(VALUE_NLA, 0, RegistryValueKind.DWord);
              key.SetValue(VALUE_SECURITY, 1, RegistryValueKind.DWord);
              break;
            case 2:
              key.SetValue(VALUE_NLA, 1, RegistryValueKind.DWord);
              key.SetValue(VALUE_SECURITY, 2, RegistryValueKind.DWord);
              break;
          }

          key.SetValue(VALUE_SHADOW, rgShadowOptions.SelectedIndex, RegistryValueKind.DWord);
        }

        using (var key = Registry.LocalMachine.CreateSubKey(REG_TS_KEY)) {
          key.SetValue(VALUE_SHADOW, rgShadowOptions.SelectedIndex, RegistryValueKind.DWord);
        }

        using (var key = Registry.LocalMachine.CreateSubKey(REG_WINLOGON_KEY)) {
          key.SetValue(VALUE_DONTDISPLAYLASTUSERNAME, cbDontDisplayLastUser.Checked ? 1 : 0, RegistryValueKind.DWord);
        }
        btnApply.Enabled = false;
      }
      catch (Exception ex) {
        MessageBox.Show("Failed to apply settings: " + ex.Message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void onChanged(object sender, EventArgs e) {
      btnApply.Enabled = true;
    }

    public static Process StartProcess(string app, string arg, string workingDir = null) {

      var p = new Process();
      p.StartInfo.UseShellExecute = true;
      p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
      p.StartInfo.FileName = app;
      p.StartInfo.WorkingDirectory = workingDir ?? Path.GetDirectoryName(app);
      p.StartInfo.Arguments = arg;
      p.Start();
      return p;
    }

    private void timerTick(object sender, EventArgs e) {
      // Placeholder for timer functionality
    }
  }
}
