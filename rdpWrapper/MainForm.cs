using Microsoft.Win32;
using sergiye.Common;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;
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
    private string wrapperIniLastPath;
    private DateTime wrapperIniLastChecked = DateTime.MinValue;
    private bool wrapperIniLastSupported;

    private readonly string termSrvFile;
    private readonly Timer refreshTimer;
    private readonly Logger logger;

    public MainForm() {

      InitializeComponent();

      Icon = Icon.ExtractAssociatedIcon(typeof(MainForm).Assembly.Location);
      Text = $"{Updater.ApplicationTitle} v{Updater.CurrentVersion} {(Environment.Is64BitProcess ? "x64" : "x86")}";

      logger = new Logger();
      logger.OnNewLogEvent += AddToLog;
      logger.Log(Text + " - Application started", Logger.StateKind.Info, false);

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

      Load += MainFormLoad;

      termSrvFile = Path.Combine(Environment.SystemDirectory, "termsrv.dll");

      refreshTimer = new Timer();
      refreshTimer.Tick += TimerTick;
      refreshTimer.Interval = 1000;

#if !DEBUG
      var timer = new Timer();
      timer.Tick += (_, _) => {
        timer.Enabled = false;
        timer.Enabled = !Updater.CheckForUpdates(true);
      };
      timer.Interval = 3000;
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

    private void MainFormLoad(object sender, EventArgs e) {
      
      try {
        logger.Log("Retrieving system configuration...");

        using (var key = Registry.LocalMachine.OpenSubKey(REG_KEY)) {
          if (key != null) {
            cbxSingleSessionPerUser.Checked = Convert.ToInt32(key.GetValue(VALUE_SINGLE_SESSION, 0)) != 0;
            cbxAllowTSConnections.Checked = Convert.ToInt32(key.GetValue(VALUE_DENY_TS_CONNECTIONS, 0)) == 0;
            cbxHonorLegacy.Checked = Convert.ToInt32(key.GetValue(VALUE_HONORLEGACY, 0)) != 0;
          }
        }

        using (var key = Registry.LocalMachine.OpenSubKey(REG_RDP_KEY)) {
          if (key != null) {
            oldPort = Convert.ToInt32(key.GetValue(VALUE_PORT, 3389));
            seRDPPort.Value = oldPort;
            var userAuthentication = Convert.ToInt32(key.GetValue(VALUE_NLA, 0));
            var securityLayer = Convert.ToInt32(key.GetValue(VALUE_SECURITY, 0));

            rgNLAOptions.SelectedIndex = securityLayer switch {
              0 when userAuthentication == 0 => 0,
              1 when userAuthentication == 0 => 1,
              2 when userAuthentication == 1 => 2,
              _ => rgNLAOptions.SelectedIndex
            };

            rgShadowOptions.SelectedIndex = Convert.ToInt32(key.GetValue(VALUE_SHADOW, 0));
          }
        }

        using (var key = Registry.LocalMachine.OpenSubKey(REG_WINLOGON_KEY)) {
          if (key != null)
            cbDontDisplayLastUser.Checked = Convert.ToInt32(key.GetValue(VALUE_DONTDISPLAYLASTUSERNAME, 0)) != 0;
        }

        logger.Log("Retrieving system configuration completed", Logger.StateKind.Info);
        TimerTick(null, EventArgs.Empty);
        refreshTimer.Enabled = true;
      }
      catch (Exception ex) {
        var message = "Error loading settings: " + ex.Message;
        logger.Log(message, Logger.StateKind.Error);
        MessageBox.Show(message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      btnApply.Enabled = false;
    }

    private void BtnCloseClick(object sender, EventArgs e) {
      Close();
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
      if (btnApply.Enabled && MessageBox.Show("Settings are not saved. Do you want to exit?", Updater.ApplicationTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) {
        e.Cancel = true;
      }
    }

    private void BtnApplyClick(object sender, EventArgs e) {
      try {
        using (var key = Registry.LocalMachine.OpenSubKey(REG_KEY, writable: true)) {
          if (key != null) {
            key.SetValue(VALUE_SINGLE_SESSION, cbxSingleSessionPerUser.Checked ? 1 : 0, RegistryValueKind.DWord);
            key.SetValue(VALUE_DENY_TS_CONNECTIONS, cbxAllowTSConnections.Checked ? 0 : 1, RegistryValueKind.DWord);
            key.SetValue(VALUE_HONORLEGACY, cbxHonorLegacy.Checked ? 0 : 1, RegistryValueKind.DWord);
          }
        }

        var newPort = (int)seRDPPort.Value;
        if (oldPort != newPort) {
          var p = StartProcess("netsh", $"advfirewall firewall set rule name=\"Remote Desktop\" new localport={newPort}");
          p.WaitForExit();
          logger.Log($"Firewall rule added for port {newPort}", Logger.StateKind.Warning);
        }

        using (var key = Registry.LocalMachine.OpenSubKey(REG_RDP_KEY, true)) {
          if (key != null) {
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
        }

        using (var key = Registry.LocalMachine.CreateSubKey(REG_TS_KEY)) {
          key?.SetValue(VALUE_SHADOW, rgShadowOptions.SelectedIndex, RegistryValueKind.DWord);
        }

        using (var key = Registry.LocalMachine.CreateSubKey(REG_WINLOGON_KEY)) {
          key?.SetValue(VALUE_DONTDISPLAYLASTUSERNAME, cbDontDisplayLastUser.Checked ? 1 : 0, RegistryValueKind.DWord);
        }
        btnApply.Enabled = false;
        logger.Log("Settings applied", Logger.StateKind.Info);
      }
      catch (Exception ex) {
        var message = "Failed to apply settings: " + ex.Message;
        logger.Log(message, Logger.StateKind.Error);
        MessageBox.Show(message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void OnChanged(object sender, EventArgs e) {
      btnApply.Enabled = true;
    }

    private static Process StartProcess(string app, string arg, string workingDir = null) {

      var p = new Process();
      p.StartInfo.UseShellExecute = true;
      p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
      p.StartInfo.FileName = app;
      p.StartInfo.WorkingDirectory = (workingDir ?? Path.GetDirectoryName(app)) ?? string.Empty;
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
        ServiceHelper.RestartService("TermService", 10000);
        logger.Log("TermService restarted successfully", Logger.StateKind.Info);
      }
      catch (Exception ex) {
        var message = "Error restarting service: " + ex.Message;
        logger.Log(message, Logger.StateKind.Error);
        MessageBox.Show(message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        var lowerPath = termServicePath.ToLower();
        if (!lowerPath.Contains("termsrv.dll") && !lowerPath.Contains("rdpwrap.dll"))
          return 2;

        if (!lowerPath.Contains("rdpwrap.dll")) return 0;
        wrapperPath = termServicePath;
        return 1;
      }
      catch {
        return -1;
      }
    }

    private static string GetVersionString(FileVersionInfo versionInfo) {
      return versionInfo.FileMajorPart + "." + versionInfo.FileMinorPart + "." + versionInfo.FileBuildPart + "." + versionInfo.FilePrivatePart;
    }

    private void TimerTick(object sender, EventArgs e) {

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
          lblWrapperStateValue.ForeColor = Color.DarkGreen;
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

      switch (ServiceHelper.GetServiceState()) {
        case ServiceControllerStatus.Stopped:
          lblServiceStateValue.Text = "Stopped";
          lblServiceStateValue.ForeColor = Color.Red;
          break;
        case ServiceControllerStatus.StartPending:
          lblServiceStateValue.Text = "Starting..";
          lblServiceStateValue.ForeColor = ForeColor;
          break;
        case ServiceControllerStatus.StopPending:
          lblServiceStateValue.Text = "Stopping...";
          lblServiceStateValue.ForeColor = ForeColor;
          break;
        case ServiceControllerStatus.Running:
          lblServiceStateValue.Text = "Running";
          lblServiceStateValue.ForeColor = Color.DarkGreen;
          break;
        case ServiceControllerStatus.ContinuePending:
          lblServiceStateValue.Text = "Resuming...";
          lblServiceStateValue.ForeColor = ForeColor;
          break;
        case ServiceControllerStatus.PausePending:
          lblServiceStateValue.Text = "Suspending...";
          lblServiceStateValue.ForeColor = ForeColor;
          break;
        case ServiceControllerStatus.Paused:
          lblServiceStateValue.Text = "Suspended";
          lblServiceStateValue.ForeColor = ForeColor;
          break;
        default:
          lblServiceStateValue.Text = "Unknown";
          lblServiceStateValue.ForeColor = ForeColor;
          break;
      }

      if (WinStationHelper.IsListenerWorking()){
        lblListenerStateValue.Text = "Listening";
        lblListenerStateValue.ForeColor = Color.DarkGreen;
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
        btnGenerate.Visible = false;
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
          lblSupported.ForeColor = Color.DarkGreen;
          btnGenerate.Visible = false;
          return;
        }
      }
      lblSupported.Text = "[not supported]";
      lblSupported.ForeColor = Color.Red;
      btnGenerate.Visible = true;
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
        logger.Log("Config regenerated", Logger.StateKind.Info);
      }
      catch (Exception ex) {
        var message = "Failed to generate config: " + ex.Message;
        logger.Log(message, Logger.StateKind.Error);
        MessageBox.Show(message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally {
        if (executeCleanup) {
          SafeDeleteFile(offsetFinder);
          SafeDeleteFile(zydis);
        }
      }
      return File.Exists(iniFile) ? iniFile : null;
    }

    private string ExtractResourceFile(string resourceName, string path, bool deleteExisting = false) {
      var filePath = Path.Combine(path, resourceName);
      if (File.Exists(filePath)) {
        if (!deleteExisting) {
          return filePath; //todo: delete
        }
        SafeDeleteFile(filePath);
      }
      try {
        var type = GetType();
        var scriptsPath = $"{type.Namespace}.externals.{resourceName}";
        using var stream = type.Assembly.GetManifestResourceStream(scriptsPath);
        using var fileStream = File.Create(filePath);
        stream?.Seek(0, SeekOrigin.Begin);
        stream?.CopyTo(fileStream);
        return filePath;
      }
      catch (Exception ex) {
        logger.Log(ex.Message, Logger.StateKind.Warning);
        return null;
      }
    }

    private void SafeDeleteFile(string filePath) {
      try {
        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath)) File.Delete(filePath);
      }
      catch (Exception ex) {
        logger.Log(ex.Message, Logger.StateKind.Warning);
      }
    }

    private void btnTest_Click(object sender, EventArgs e) {
      Process.Start("mstsc.exe", $"/v:127.0.0.2:{oldPort}");
    }

    private void btnGenerate_Click(object sender, EventArgs e) {
      var newIniFile = GenerateIniFile(true);
      if (newIniFile == null) return;
      try {
        ServiceHelper.StopService("TermService", TimeSpan.FromSeconds(10));
        logger.Log("TermService stopped", Logger.StateKind.Info);
        SafeDeleteFile(wrapperIniLastPath);
        File.Move(newIniFile, wrapperIniLastPath);
        ServiceHelper.StartService("TermService", TimeSpan.FromSeconds(10));
        logger.Log("TermService started", Logger.StateKind.Info);
      }
      catch (Exception ex) {
        var message = "Failed to update config: " + ex.Message;
        logger.Log(message, Logger.StateKind.Error);
        MessageBox.Show(message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void AddToLog(string message, Logger.StateKind state, bool newLine) {
      if (InvokeRequired) {
        Invoke(new Action<string, Logger.StateKind, bool>(AddToLog), message, state);
        return;
      }

      if (newLine)
        txtLog.AppendLine($"{DateTime.Now:T} - ", txtLog.ForeColor);
      switch (state) {
        case Logger.StateKind.Error:
          txtLog.AppendLine(message, Color.Red, false);
          break;
        case Logger.StateKind.Warning:
          txtLog.AppendLine(message, Color.Blue, false);
          break;
        case Logger.StateKind.Info:
          txtLog.AppendLine(message, Color.DarkGreen, false);
          break;
        default:
          txtLog.AppendLine(message, txtLog.ForeColor, false);
          break;
      }

      // if (!string.IsNullOrEmpty(_logFileName))
      //   File.AppendAllText(_logFileName, $"{DateTime.Now:T} - {message}\n");
      txtLog.ScrollToCaret();
      Application.DoEvents();
    }
  }
}
