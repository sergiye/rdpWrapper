using Microsoft.Win32;
using sergiye.Common;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace rdpWrapper {

  internal partial class MainForm : Form {

    private const int MF_SEPARATOR = 0x00000800;
    private const int MF_BY_POSITION = 0x400;
    private const int MF_WM_SYS_COMMAND = 0x112;
    private const int MF_SYS_MENU_CHECK_UPDATES = 1000;
    private const int MF_SYS_MENU_ABOUT_ID = 1001;

    private const string REG_KEY = @"SYSTEM\CurrentControlSet\Control\Terminal Server";
    private const string REG_TERMSERVICE_KEY = @"SYSTEM\CurrentControlSet\Services\TermService";
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
    
    private const string RDP_WRAP_INI_NAME = "rdpwrap.ini";

    private int oldPort;
    private string wrapperIniLastPath = null;
    private DateTime wrapperIniLastChecked = DateTime.MinValue;
    private bool wrapperIniLastSupported = false;

    private readonly string termSrvFile;
    private readonly Timer refreshTimer;

    public MainForm() {

      InitializeComponent();

      Icon = Icon.ExtractAssociatedIcon(typeof(MainForm).Assembly.Location);
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

      termSrvFile = Path.Combine(Environment.SystemDirectory, "termsrv.dll");

      refreshTimer = new Timer();
      refreshTimer.Tick += timerTick;
      refreshTimer.Interval = 1000;

#if !DEBUG
      var timer = new Timer();
      timer.Tick += (_, _) => {
        timer.Enabled = false;
        timer.Enabled = !Updater.CheckForUpdates(true);
      };
      timer.Interval = 1000;
      timer.Enabled = true;
#endif
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
          if (MessageBox.Show($"{Updater.ApplicationTitle} {asm.GetName().Version.ToString(3)} {(Environment.Is64BitProcess ? "x64" : "x32")}\nWritten by Sergiy Egoshyn (egoshin.sergey@gmail.com)\nDo you want to know more?", Updater.ApplicationTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
            Process.Start("https://github.com/sergiye/" + Updater.ApplicationName);
          }
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

        timerTick(null, EventArgs.Empty);
        refreshTimer.Enabled = true;
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

    private static Process StartProcess(string app, string arg, string workingDir = null) {

      var p = new Process();
      p.StartInfo.UseShellExecute = true;
      p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
      p.StartInfo.FileName = app;
      p.StartInfo.WorkingDirectory = workingDir ?? Path.GetDirectoryName(app);
      p.StartInfo.Arguments = arg;
      p.StartInfo.RedirectStandardOutput = false;
      p.StartInfo.RedirectStandardInput = false;
      p.StartInfo.RedirectStandardError = false;
      p.Start();
      return p;
    }

    private void btnRestartService_Click(object sender, EventArgs e) {
      try {
        //todo: async

        //if (ServiceHelper.RestartServiceNative()) {
        //  MessageBox.Show("Service restarted.", Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        //}
        //else {
        //  MessageBox.Show("Failed to restart TermService.", Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //}

        ServiceHelper.RestartService("TermService", 10000);
        MessageBox.Show("TermService restarted successfully", Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      catch (Exception ex) {
        MessageBox.Show("Error restarting service: " + ex.Message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private static short IsWrapperInstalled(out string wrapperPath) {
      wrapperPath = string.Empty;
      try {
        using (var serviceKey = Registry.LocalMachine.OpenSubKey(REG_TERMSERVICE_KEY)) {
          if (serviceKey == null)
            return -1;
          var termServiceHost = serviceKey.GetValue("ImagePath") as string;
          if (string.IsNullOrWhiteSpace(termServiceHost) || !termServiceHost.ToLower().Contains("svchost.exe"))
            return 2;
        }

        string termServicePath;
        using (var paramKey = Registry.LocalMachine.OpenSubKey(REG_TERMSERVICE_KEY + "\\Parameters")) {
          if (paramKey == null)
            return -1;

          termServicePath = paramKey.GetValue("ServiceDll") as string;
          if (string.IsNullOrWhiteSpace(termServicePath))
            return -1;
        }

        string lowerPath = termServicePath.ToLower();
        if (!lowerPath.Contains("termsrv.dll") && !lowerPath.Contains("rdpwrap.dll"))
          return 2;

        if (lowerPath.Contains("rdpwrap.dll")) {
          wrapperPath = termServicePath;
          return 1;
        }

        return 0;
      }
      catch {
        return -1;
      }
    }

    private string GetVersionString(FileVersionInfo versionInfo) {
      return versionInfo.FileMajorPart + "." + versionInfo.FileMinorPart + "." + versionInfo.FileBuildPart + "." + versionInfo.FilePrivatePart;
    }

    private void timerTick(object sender, EventArgs e) {

      var checkSupported = false;
      var wrapperInstalled = IsWrapperInstalled(out var wrapperPath);
      switch (wrapperInstalled) {
        case -1:
          lblWrapperStateValue.Text = "Unknown";
          lblWrapperStateValue.ForeColor = Color.Gray;
          break;
        case 0:
          lblWrapperStateValue.Text = "Not installed";
          lblWrapperStateValue.ForeColor = Color.Gray;
          break;
        case 1:
          lblWrapperStateValue.Text = "Installed";
          lblWrapperStateValue.ForeColor = Color.Green;
          var wrapperIniPath = Path.Combine(Path.GetDirectoryName(wrapperPath), RDP_WRAP_INI_NAME);
          checkSupported = File.Exists(wrapperIniPath);
          if (wrapperIniPath != wrapperIniLastPath) {
            wrapperIniLastPath = wrapperIniPath;
            wrapperIniLastChecked = DateTime.MinValue;
          }
          break;
        case 2:
          lblWrapperStateValue.Text = "3rd-party";
          lblWrapperStateValue.ForeColor = Color.Red;
          break;
      }

      var termServiceState = ServiceHelper.GetServiceState();
      switch (termServiceState) {
        case -1:
        case 0:
          lblServiceStateValue.Text = "Unknown";
          lblServiceStateValue.ForeColor = ForeColor;
          break;
        case ServiceHelper.SERVICE_STOPPED:
          lblServiceStateValue.Text = "Stopped";
          lblServiceStateValue.ForeColor = Color.Red;
          break;
        case ServiceHelper.SERVICE_START_PENDING:
          lblServiceStateValue.Text = "Starting..";
          lblServiceStateValue.ForeColor = ForeColor;
          break;
        case ServiceHelper.SERVICE_STOP_PENDING:
          lblServiceStateValue.Text = "Stopping...";
          lblServiceStateValue.ForeColor = ForeColor;
          break;
        case ServiceHelper.SERVICE_RUNNING:
          lblServiceStateValue.Text = "Running";
          lblServiceStateValue.ForeColor = Color.Green;
          break;
        case ServiceHelper.SERVICE_CONTINUE_PENDING:
          lblServiceStateValue.Text = "Resuming...";
          lblServiceStateValue.ForeColor = ForeColor;
          break;
        case ServiceHelper.SERVICE_PAUSE_PENDING:
          lblServiceStateValue.Text = "Suspending...";
          lblServiceStateValue.ForeColor = ForeColor;
          break;
        case ServiceHelper.SERVICE_PAUSED:
          lblServiceStateValue.Text = "Suspended";
          lblServiceStateValue.ForeColor = ForeColor;
          break;
      }

      if (WinStationHelper.IsListenerWorking()){
        lblListenerStateValue.Text = "Listening";
        lblListenerStateValue.ForeColor = Color.Green;
      }
      else {
        lblListenerStateValue.Text = "Not listening";
        lblListenerStateValue.ForeColor = Color.Red;
      }

      if (string.IsNullOrEmpty(wrapperPath) || !File.Exists(wrapperPath)) {
        lblWrapperVersion.Text = "N/A";
        lblWrapperVersion.ForeColor = Color.Red;
      }
      else {
        var versionInfo = FileVersionInfo.GetVersionInfo(wrapperPath);
        lblWrapperVersion.Text = "ver. " + GetVersionString(versionInfo);
        lblWrapperVersion.ForeColor = ForeColor;
      }

      if (!File.Exists(termSrvFile)) {
        txtServiceVersion.Text = "N/A";
        txtServiceVersion.ForeColor = Color.Red;
      }
      else {
        var versionInfo = FileVersionInfo.GetVersionInfo(termSrvFile);
        txtServiceVersion.Text = "ver. " + GetVersionString(versionInfo);
        txtServiceVersion.ForeColor = ForeColor;

        lblSupported.Visible = checkSupported;
        if (checkSupported) {
          UpdateSupportedState(versionInfo);
        }
      }
    }

    private void UpdateSupportedState(FileVersionInfo versionInfo) {

      if (versionInfo.FileMajorPart == 6 && versionInfo.FileMinorPart == 0 ||
          versionInfo.FileMajorPart == 6 && versionInfo.FileMinorPart == 1) {
        lblSupported.Text = "[supported partially]";
        lblSupported.ForeColor = Color.Olive;
      }
      else {
        var lastModified = File.GetLastWriteTime(wrapperIniLastPath);
        if (lastModified > wrapperIniLastChecked) {
          var iniContent = File.ReadAllText(wrapperIniLastPath);
          wrapperIniLastSupported = iniContent.Contains("[" + GetVersionString(versionInfo) + "]");
          wrapperIniLastChecked = lastModified;
        }
        if (wrapperIniLastSupported) {
          lblSupported.Text = "[fully supported]";
          lblSupported.ForeColor = Color.Green;
          return;
        }
      }
      lblSupported.Text = "[not supported]";
      lblSupported.ForeColor = Color.Red;
    }

    private string GenerateIniFile(bool executeCleanup = false) {

      string iniFile = null;
      string offsetFinder = null;
      string zydis = null;
      try {
        var workingDir = Path.GetTempPath();
        iniFile = ExtractResourceFile("rdpwrap.ini", workingDir, true);
        offsetFinder = ExtractResourceFile("RDPWrapOffsetFinder.exe", workingDir);
        zydis = ExtractResourceFile("Zydis.dll", workingDir);
        var p = StartProcess("cmd", $"/c \"{offsetFinder}\" >> rdpwrap.ini & exit", workingDir);
        p.WaitForExit();
      }
      catch (Exception ex) {
        MessageBox.Show("Failed to generate config: " + ex.Message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally {
        if (executeCleanup) {
          SafeDeleteFile(offsetFinder);
          SafeDeleteFile(zydis);
        }
      }
      if (File.Exists(iniFile))
        return iniFile;
      return null;
    }

    private string ExtractResourceFile(string resourceName, string path, bool deleteExisting = false) {
      var fileName = resourceName;//.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
      var filePath = Path.Combine(path, fileName);
      if (File.Exists(filePath)) {
        if (!deleteExisting) {
          return filePath; //todo: delete
        }
        SafeDeleteFile(filePath);
      }
      try {
        var type = this.GetType();
        var scriptsPath = $"{type.Namespace}.externals.{resourceName}";
        using (var stream = type.Assembly.GetManifestResourceStream(scriptsPath))
        using (var fileStream = File.Create(filePath)) {
          stream.Seek(0, SeekOrigin.Begin);
          stream.CopyTo(fileStream);
        }
        return filePath;
      }
      catch (Exception) {
        return null;
      }
    }

    private void SafeDeleteFile(string filePath) {
      try {
        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath)) File.Delete(filePath);
      }
      catch (Exception) {
        //ignore
      }
    }

    private void btnTest_Click(object sender, EventArgs e) {
      Process.Start("mstsc.exe", $"/v:127.0.0.2:{oldPort}");
    }
  }
}
