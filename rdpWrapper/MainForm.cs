using Microsoft.Win32;
using sergiye.Common;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.ServiceProcess;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace rdpWrapper {

  internal partial class MainForm : Form {

    private const int MfSeparator = 0x00000800;
    private const int MfByPosition = 0x400;
    private const int MfWmSysCommand = 0x112;
    private const int MfSysMenuCheckUpdates = 1000;
    private const int MfSysMenuAboutId = 1001;

    private const string RegKey = @"SYSTEM\CurrentControlSet\Control\Terminal Server";
    private const string RegTermServiceKey = @"SYSTEM\CurrentControlSet\Services\TermService";
    private const string RegRdpKey = @"SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp";
    private const string RegWinLogonKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System";
    private const string RegTsKey = @"SOFTWARE\Policies\Microsoft\Windows NT\Terminal Services";

    private const string ValueSingleSession = "fSingleSessionPerUser";
    private const string ValueDenyTsConnections = "fDenyTSConnections";
    private const string ValuePort = "PortNumber";
    private const string ValueDontDisplayLastUserName = "DontDisplayLastUsername";
    private const string ValueHonorLegacy = "HonorLegacySettings";
    private const string ValueNla = "UserAuthentication";
    private const string ValueSecurity = "SecurityLayer";
    private const string ValueShadow = "Shadow";
    
    private const string RdpWrapDllName = "rdpwrap.dll";
    private const string RdpWrapIniName = "rdpwrap.ini";
    private const string TermSrvName = "termsrv.dll";
    private const string RdpServiceName = "TermService";

    private int oldPort;
    private string wrapperIniLastPath;
    private DateTime wrapperIniLastChecked = DateTime.MinValue;
    private bool wrapperIniLastSupported;

    private readonly string termSrvFile;
    private readonly string wrapperFolderPath;
    private readonly Timer refreshTimer;
    private readonly Logger logger;
    private readonly ServiceHelper serviceHelper;

    public MainForm() {

      InitializeComponent();

      Icon = Icon.ExtractAssociatedIcon(typeof(MainForm).Assembly.Location);
      Text = $"{Updater.ApplicationTitle} v{Updater.CurrentVersion} {(Environment.Is64BitProcess ? "x64" : "x86")}";

      logger = new Logger();
      logger.OnNewLogEvent += AddToLog;
      logger.Log($"Application started: {Text}", Logger.StateKind.Info, false);

      serviceHelper = new ServiceHelper(logger);
      
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
      InsertMenu(menuHandle, 5, MfByPosition | MfSeparator, 0, string.Empty); // <-- Add a menu separator
      InsertMenu(menuHandle, 6, MfByPosition, MfSysMenuCheckUpdates, "Check for new version");
      InsertMenu(menuHandle, 7, MfByPosition, MfSysMenuAboutId, "&About…");

      Load += MainFormLoad;

      termSrvFile = Path.Combine(Environment.SystemDirectory, TermSrvName);
      //string programFilesX86 = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%");
      wrapperFolderPath = Path.Combine(Environment.ExpandEnvironmentVariables("%ProgramW6432%"), "RDP Wrapper");

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
      
      if (m.Msg != MfWmSysCommand)
        return;
      
      switch ((int)m.WParam) {
        case MfSysMenuAboutId:
          var asm = GetType().Assembly;
          if (MessageBox.Show($"{Updater.ApplicationTitle} {asm.GetName().Version.ToString(3)} {(Environment.Is64BitProcess ? "x64" : "x32")}\nWritten by Sergiy Egoshyn (egoshin.sergey@gmail.com)\nDo you want to know more?", Updater.ApplicationTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
            Process.Start("https://github.com/sergiye/" + Updater.ApplicationName);
          }
          break;
        case MfSysMenuCheckUpdates:
          Updater.CheckForUpdates(false);
          break;
      }
    }

    private void MainFormLoad(object sender, EventArgs e) {
      
      try {
        logger.Log("Retrieving system configuration...");

        using (var key = Registry.LocalMachine.OpenSubKey(RegKey)) {
          if (key != null) {
            cbxSingleSessionPerUser.Checked = Convert.ToInt32(key.GetValue(ValueSingleSession, 0)) != 0;
            cbxAllowTSConnections.Checked = Convert.ToInt32(key.GetValue(ValueDenyTsConnections, 0)) == 0;
            cbxHonorLegacy.Checked = Convert.ToInt32(key.GetValue(ValueHonorLegacy, 0)) != 0;
          }
        }

        using (var key = Registry.LocalMachine.OpenSubKey(RegRdpKey)) {
          if (key != null) {
            oldPort = Convert.ToInt32(key.GetValue(ValuePort, 3389));
            seRDPPort.Value = oldPort;
            var userAuthentication = Convert.ToInt32(key.GetValue(ValueNla, 0));
            var securityLayer = Convert.ToInt32(key.GetValue(ValueSecurity, 0));

            rgNLAOptions.SelectedIndex = securityLayer switch {
              0 when userAuthentication == 0 => 0,
              1 when userAuthentication == 0 => 1,
              2 when userAuthentication == 1 => 2,
              _ => rgNLAOptions.SelectedIndex
            };

            rgShadowOptions.SelectedIndex = Convert.ToInt32(key.GetValue(ValueShadow, 0));
          }
        }

        using (var key = Registry.LocalMachine.OpenSubKey(RegWinLogonKey)) {
          if (key != null)
            cbDontDisplayLastUser.Checked = Convert.ToInt32(key.GetValue(ValueDontDisplayLastUserName, 0)) != 0;
        }

        logger.Log(" Completed", Logger.StateKind.Info, false);
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

    private void btnApply_Click(object sender, EventArgs e) {
      try {
        using (var key = Registry.LocalMachine.OpenSubKey(RegKey, writable: true)) {
          if (key != null) {
            key.SetValue(ValueSingleSession, cbxSingleSessionPerUser.Checked ? 1 : 0, RegistryValueKind.DWord);
            key.SetValue(ValueDenyTsConnections, cbxAllowTSConnections.Checked ? 0 : 1, RegistryValueKind.DWord);
            key.SetValue(ValueHonorLegacy, cbxHonorLegacy.Checked ? 1 : 0, RegistryValueKind.DWord);
          }
        }

        var newPort = (int)seRDPPort.Value;
        if (oldPort != newPort) {
          var p = StartProcess("netsh", $"advfirewall firewall set rule name=\"Remote Desktop\" new localport={newPort}");
          p.WaitForExit();
          logger.Log($"Firewall rule added for port {newPort}", Logger.StateKind.Info);
        }

        using (var key = Registry.LocalMachine.OpenSubKey(RegRdpKey, true)) {
          if (key != null) {
            key.SetValue(ValuePort, newPort, RegistryValueKind.DWord);
            oldPort = newPort;

            switch (rgNLAOptions.SelectedIndex) {
              case 0:
                key.SetValue(ValueNla, 0, RegistryValueKind.DWord);
                key.SetValue(ValueSecurity, 0, RegistryValueKind.DWord);
                break;
              case 1:
                key.SetValue(ValueNla, 0, RegistryValueKind.DWord);
                key.SetValue(ValueSecurity, 1, RegistryValueKind.DWord);
                break;
              case 2:
                key.SetValue(ValueNla, 1, RegistryValueKind.DWord);
                key.SetValue(ValueSecurity, 2, RegistryValueKind.DWord);
                break;
            }

            key.SetValue(ValueShadow, rgShadowOptions.SelectedIndex, RegistryValueKind.DWord);
          }
        }

        using (var key = Registry.LocalMachine.CreateSubKey(RegTsKey)) {
          key?.SetValue(ValueShadow, rgShadowOptions.SelectedIndex, RegistryValueKind.DWord);
        }

        using (var key = Registry.LocalMachine.CreateSubKey(RegWinLogonKey)) {
          key?.SetValue(ValueDontDisplayLastUserName, cbDontDisplayLastUser.Checked ? 1 : 0, RegistryValueKind.DWord);
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
        btnRestartService.Enabled = false;
        //todo: async
        serviceHelper.StopService(RdpServiceName, TimeSpan.FromSeconds(10));
        serviceHelper.StartService(RdpServiceName, TimeSpan.FromSeconds(10));
      }
      catch (Exception ex) {
        var message = "Error restarting service: " + ex.Message;
        logger.Log(message, Logger.StateKind.Error);
        MessageBox.Show(message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally {
        btnRestartService.Enabled = true;
      }
    }

    private static short IsWrapperInstalled(out string wrapperPath) {
      wrapperPath = string.Empty;
      try {
        using (var serviceKey = Registry.LocalMachine.OpenSubKey(RegTermServiceKey)) {
          if (serviceKey == null)
            return -1;
          var termServiceHost = serviceKey.GetValue("ImagePath") as string;
          if (string.IsNullOrWhiteSpace(termServiceHost) || !termServiceHost.ToLower().Contains("svchost.exe"))
            return 2;
        }

        string termServicePath;
        using (var paramKey = Registry.LocalMachine.OpenSubKey(RegTermServiceKey + "\\Parameters")) {
          if (paramKey == null)
            return -1;

          termServicePath = paramKey.GetValue("ServiceDll") as string;
          if (string.IsNullOrWhiteSpace(termServicePath))
            return -1;
        }

        var lowerPath = termServicePath.ToLower();
        if (!lowerPath.Contains(TermSrvName) && !lowerPath.Contains(RdpWrapDllName))
          return 2;

        if (!lowerPath.Contains(RdpWrapDllName)) return 0;
        wrapperPath = termServicePath;
        return 1;
      }
      catch {
        return -1;
      }
    }

    private static string GetVersionString(FileVersionInfo versionInfo) {
      return versionInfo.ProductMajorPart + "." + versionInfo.ProductMinorPart + "." + versionInfo.ProductBuildPart + "." + versionInfo.ProductPrivatePart;
    }

    private void TimerTick(object sender, EventArgs e) {

      var checkSupported = false;
      var wrapperInstalled = IsWrapperInstalled(out var wrapperPath);
      switch (wrapperInstalled) {
        case -1:
          lblWrapperStateValue.Text = "Unknown";
          lblWrapperStateValue.ForeColor = Color.Gray;
          btnInstall.Visible = false;
          btnUninstall.Visible = false;
          break;
        case 0:
          lblWrapperStateValue.Text = "Not installed";
          lblWrapperStateValue.ForeColor = Color.Gray;
          btnInstall.Visible = true;
          btnUninstall.Visible = false;
          break;
        case 1:
          lblWrapperStateValue.Text = "Installed";
          lblWrapperStateValue.ForeColor = Color.DarkGreen;
          var wrapperIniPath = Path.Combine(Path.GetDirectoryName(wrapperPath), RdpWrapIniName);
          checkSupported = File.Exists(wrapperIniPath);
          if (wrapperIniPath != wrapperIniLastPath) {
            wrapperIniLastPath = wrapperIniPath;
            wrapperIniLastChecked = DateTime.MinValue;
          }
          btnInstall.Visible = false;
          btnUninstall.Visible = true;
          break;
        case 2:
          lblWrapperStateValue.Text = "3rd-party";
          lblWrapperStateValue.ForeColor = Color.Red;
          btnInstall.Visible = false;
          btnUninstall.Visible = false;
          break;
      }

      switch (serviceHelper.GetServiceState(RdpServiceName)) {
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
        lblWrapperVersion.Text = GetVersionString(versionInfo);
        lblWrapperVersion.ForeColor = ForeColor;
      }

      if (!File.Exists(termSrvFile)) {
        txtServiceVersion.Text = "N/A";
        txtServiceVersion.ForeColor = Color.Red;
      }
      else {
        var versionInfo = FileVersionInfo.GetVersionInfo(termSrvFile);
        txtServiceVersion.Text = GetVersionString(versionInfo);
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

    private void GenerateIniFile(string destFilePath, bool executeCleanup = true) {

      string iniFile = null;
      string offsetFinder = null;
      string zydis = null;
      try {
        logger.Log("Generating config...");
        var workingDir = Path.GetTempPath();
        iniFile = ExtractResourceFile(RdpWrapIniName, workingDir, true);
        offsetFinder = ExtractResourceFile("RDPWrapOffsetFinder.exe", workingDir);
        zydis = ExtractResourceFile("Zydis.dll", workingDir);
        var p = StartProcess("cmd", $"/c \"{offsetFinder}\" >> {RdpWrapIniName} & exit", workingDir);
        p.WaitForExit();
        logger.Log(" Done", Logger.StateKind.Info, false);
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

      if (string.IsNullOrEmpty(iniFile) || !File.Exists(iniFile)) return;
      
      try {
        serviceHelper.StopService(RdpServiceName, TimeSpan.FromSeconds(10));

        SafeDeleteFile(destFilePath);
        File.Move(iniFile, destFilePath);

        serviceHelper.StartService(RdpServiceName, TimeSpan.FromSeconds(10));
      }
      catch (Exception ex) {
        var message = "Failed to update config: " + ex.Message;
        logger.Log(message, Logger.StateKind.Error);
        MessageBox.Show(message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
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
        logger.Log(ex.Message, Logger.StateKind.Error);
        return null;
      }
    }

    private void SafeDeleteFile(string filePath) {
      try {
        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath)) File.Delete(filePath);
      }
      catch (Exception ex) {
        logger.Log(ex.Message, Logger.StateKind.Error);
      }
    }

    private void btnTest_Click(object sender, EventArgs e) {
      Process.Start("mstsc.exe", $"/v:127.0.0.2:{oldPort}");
    }

    private void btnGenerate_Click(object sender, EventArgs e) {
      try {
        btnGenerate.Enabled = false;
        GenerateIniFile(wrapperIniLastPath);
      }
      finally {
        btnGenerate.Enabled = true;
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
        case Logger.StateKind.Info:
          txtLog.AppendLine(message, Color.Blue, false);
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

    private void btnInstall_Click(object sender, EventArgs e) {

      try {
        btnInstall.Enabled = false;
        Directory.CreateDirectory(wrapperFolderPath);
        logger.Log("Folder created: " + wrapperFolderPath);
        AclHelper.GrantSidFullAccess(wrapperFolderPath, "S-1-5-18", logger); // Local System account
        AclHelper.GrantSidFullAccess(wrapperFolderPath, "S-1-5-6", logger); // Service group
        AclHelper.GrantSidFullAccess(wrapperFolderPath, "S-1-5-32-545", logger); // SID for "Users"

        var rdpWrap = ExtractResourceFile(RdpWrapDllName, wrapperFolderPath);
        logger.Log("Extracted rdpw64 -> " + rdpWrap);

        logger.Log("Configuring service library...");
        using var reg = Registry.LocalMachine.OpenSubKey(RegTermServiceKey + "\\Parameters", writable: true);
        if (reg == null) {
          logger.Log($"OpenKey error (code {Marshal.GetLastWin32Error()}).", Logger.StateKind.Error);
          return;
        }
        reg.SetValue("ServiceDll", rdpWrap, RegistryValueKind.ExpandString);
        logger.Log(" Done", Logger.StateKind.Info, false);

        GenerateIniFile(Path.Combine(wrapperFolderPath, RdpWrapIniName));

        //todo: Thread.Sleep(1000); 
        cbxAllowTSConnections.Checked = true;
        btnApply.PerformClick();
      }
      catch (Exception ex) {
        var message = "Failed to Install: " + ex.Message;
        logger.Log(message, Logger.StateKind.Error);
        MessageBox.Show(message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally {
        btnInstall.Enabled = true;
      }
    }

    private void btnUninstall_Click(object sender, EventArgs e) {
      try {
        btnUninstall.Enabled = false;
        logger.Log("Resetting service library...");
        using var reg = Registry.LocalMachine.OpenSubKey(RegTermServiceKey + "\\Parameters", writable: true);
        if (reg == null) {
          var code = Marshal.GetLastWin32Error();
          logger.Log($"OpenKey error (code {Marshal.GetLastWin32Error()}).", Logger.StateKind.Error);
          return;
        }
        reg.SetValue("ServiceDll", @"%SystemRoot%\System32\termsrv.dll", RegistryValueKind.ExpandString);
        logger.Log(" Done", Logger.StateKind.Info, false);

        var serviceState = serviceHelper.GetServiceState(RdpServiceName); 
        if (serviceState.HasValue && serviceState == ServiceControllerStatus.Running)
          serviceHelper.StopService(RdpServiceName, TimeSpan.FromSeconds(10));

        logger.Log("Removed folder: " + wrapperFolderPath);
        Directory.Delete(wrapperFolderPath, true);

        if (serviceState.HasValue)
          serviceHelper.StartService(RdpServiceName, TimeSpan.FromSeconds(10));

        cbxAllowTSConnections.Checked = false;
        btnApply.PerformClick();
      }
      catch (Exception ex) {
        var message = "Failed to Install: " + ex.Message;
        logger.Log(message, Logger.StateKind.Error);
        MessageBox.Show(message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally {
        btnUninstall.Enabled = true;
      }
    }
  }
}
