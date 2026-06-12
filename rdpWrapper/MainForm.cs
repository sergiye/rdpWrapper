using sergiye.Common;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace rdpWrapper {

  internal partial class MainForm : Form {

    private int oldPort;
    private string wrapperIniLastPath;
    private DateTime wrapperIniLastChecked = DateTime.MinValue;
    private bool wrapperIniLastSupported;
    private readonly Timer refreshTimer;
    private readonly Logger logger;
    private readonly Wrapper wrapper;
    private readonly PersistentSettings settings;
    private SupportedWrappers preferredWrapper;
    private readonly UserOption showAntivirusWarn;
    private readonly UserOption addDefenderExclusion;
    private readonly UserOption setFirewallRule;

    public MainForm() {

      InitializeComponent();

      Icon = Icon.ExtractAssociatedIcon(typeof(MainForm).Assembly.Location);
      var appArch = Environment.Is64BitProcess ? "x64" : "x86";
      var sysArch = Environment.Is64BitOperatingSystem ? "x64" : "x86";
      var title = $"{Updater.ApplicationTitle} v{Updater.CurrentVersion} {appArch}";
      if (appArch != sysArch)
        title += "/" + sysArch;
      Text = title;

      settings = new PersistentSettings();
      settings.Load();
      InitializeTheme();

      var sizeSpan = Height - ClientSize.Height;
      MinimumSize = new Size { Width = Width, Height = sizeSpan + gbxGeneralSettings.Height + gbxStatus.Height + mainMenu.Height };

      var showLog = new UserOption("showLog", true, showLogToolStripMenuItem, settings);
      showLog.Changed += delegate {
        SetLogVisible(showLogToolStripMenuItem.Checked);
      };
      SetLogVisible(showLog.Value);
      var portable = new UserOption("portable", false, storeSeiingsInFileToolStripMenuItem, settings);
      portable.Changed += delegate {
        settings.IsPortable = portable.Value;
      };
      showAntivirusWarn = new UserOption("showAntivirusWarn", true, showAntivirusWarnMenuItem, settings);
      addDefenderExclusion = new UserOption("addDefenderExclusion", true, addDefenderExclusionMenuItem, settings);
      setFirewallRule = new UserOption("setFirewallRule", true, addFirewallRuleMenuItem, settings);

      if (!Enum.TryParse(settings.GetValue("preferredWrapper", "TermWrap"), out preferredWrapper)){
        preferredWrapper = SupportedWrappers.TermWrap;
      }
      foreach (SupportedWrappers wrap in Enum.GetValues(typeof(SupportedWrappers))) {
        var menuItem = new ToolStripRadioButtonMenuItem(wrap.ToString(), null, (sender, _) => {
          if (sender is not ToolStripRadioButtonMenuItem menuItem || !Enum.TryParse(menuItem.Text, out preferredWrapper))
            return;
          settings.SetValue("preferredWrapper", preferredWrapper.ToString());
          menuItem.Checked = true;
        });
        wrapperToInstallMenuItem.DropDownItems.Add(menuItem);
        if (wrap == preferredWrapper)
          menuItem.Checked = true;
      }

      logger = new Logger();
      logger.OnNewLogEvent += AddToLog;
      logger.Log($"Application started: {title}", Logger.StateKind.Info, false);

      wrapper = new Wrapper(logger);

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

      refreshTimer = new Timer();
      refreshTimer.Tick += TimerTick;
      refreshTimer.Interval = 1000;

      Load += (_, _) => {
        RestorePosition();
        RefreshSystemSettings();
        TimerTick(null, EventArgs.Empty);
        refreshTimer.Enabled = true;

        cbxSingleSessionPerUser.CheckedChanged += (_, _) => { wrapper.SingleSessionPerUser = cbxSingleSessionPerUser.Checked; };
        cbxAllowTSConnections.CheckedChanged += (_, _) => { wrapper.AllowTsConnections = cbxAllowTSConnections.Checked; };
        cbDontDisplayLastUser.CheckedChanged += (_, _) => { wrapper.DontDisplayLastUser = cbDontDisplayLastUser.Checked; };
        cbxDisableSecurityWarning.CheckedChanged += (_, _) => { wrapper.DisableSecurityWarning = cbxDisableSecurityWarning.Checked; };
        cbxRestrictClientUsbRedirection.CheckedChanged += (_, _) => { wrapper.RestrictUsbRedirection = cbxRestrictClientUsbRedirection.Checked; };
        rgShadowOptions.SelectedIndexChanged += (_, _) => { wrapper.ShadowOptions = rgShadowOptions.SelectedIndex; };
        cbxHonorLegacy.CheckedChanged += (_, _) => { wrapper.HonorLegacy = cbxHonorLegacy.Checked; };
        cbxAllowPlaybackRedirect.CheckedChanged += (_, _) => { wrapper.AllowHostPlaybackRedirect = cbxAllowPlaybackRedirect.Checked; };
        cbxAllowAudioCapture.CheckedChanged += (_, _) => { wrapper.AllowClientAudioCapture = cbxAllowAudioCapture.Checked; };
        cbxAllowVideoCapture.CheckedChanged += (_, _) => { wrapper.AllowClientVideoCapture = cbxAllowVideoCapture.Checked; };
        cbxAllowPnp.CheckedChanged += (_, _) => { wrapper.AllowPnpRedirect = cbxAllowPnp.Checked; };

        numRDPPort.ValueChanged += (_, _) => {
          var newPort = (int)numRDPPort.Value;
          if (oldPort != newPort) {
            if (setFirewallRule.Value) {
              try {
                dynamic fwPolicy = Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2")); //INetFwPolicy2
                dynamic inboundRule = null; //INetFwRule
                try {
                  inboundRule = fwPolicy.Rules.Item(Updater.ApplicationName);
                }
                catch (FileNotFoundException) {
                  //ignore
                }
                var createNewRule = inboundRule != null;
                if (inboundRule == null) {
                  inboundRule = Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                  inboundRule.Name = Updater.ApplicationName;
                }

                inboundRule.Enabled = true;
                inboundRule.Action = 1;// NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                inboundRule.Protocol = 6; // TCP
                inboundRule.LocalPorts = newPort.ToString();
                inboundRule.Profiles = fwPolicy.CurrentProfileTypes;

                if (!createNewRule) {
                  fwPolicy.Rules.Add(inboundRule);
                  logger.Log($"Firewall rule added for port {newPort}", Logger.StateKind.Info);
                }
                else {
                  logger.Log($"Firewall rule updated to use port {newPort}", Logger.StateKind.Info);
                }
              }
              catch (Exception ex) {
                logger.Log($"Error configuring firewall: " + ex.Message, Logger.StateKind.Error);
              }
            }
            oldPort = wrapper.RdpPort = newPort;
          }
        };
        numMaxConnections.ValueChanged += (_, _) => {
          wrapper.MaximumConnectionsAllowed = (int)numMaxConnections.Value;
        };

        rgNLAOptions.SelectedIndexChanged += (_, _) => {
          switch (rgNLAOptions.SelectedIndex) {
            case 0:
              wrapper.UserAuthentication = 0;
              wrapper.SecurityLayer = 0;
              break;
            case 1:
              wrapper.UserAuthentication = 0;
              wrapper.SecurityLayer = 1;
              break;
            case 2:
              wrapper.UserAuthentication = 1;
              wrapper.SecurityLayer = 2;
              break;
          }
        };
      };

      FormClosed += (_, _) => {
        SavePosition();
      };

      Updater.Subscribe(
        (message, isError) => {
          if (InvokeRequired)
            Invoke(new Action(() => MessageBox.Show(message, Updater.ApplicationName, MessageBoxButtons.OK, isError ? MessageBoxIcon.Warning : MessageBoxIcon.Information)));
          else
            MessageBox.Show(message, Updater.ApplicationName, MessageBoxButtons.OK, isError ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
        },
        message => InvokeRequired
          ? (bool)Invoke(new Func<bool>(() => MessageBox.Show(this, message, Updater.ApplicationName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK))
          : MessageBox.Show(this, message, Updater.ApplicationName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK,
        () => { btnClose_Click(null, EventArgs.Empty); }
      );
    }

    protected override void WndProc(ref Message m) {

      base.WndProc(ref m);
      if (m.Msg == WinApiHelper.WM_SHOWME) {
        if (WindowState == FormWindowState.Minimized)
          WindowState = FormWindowState.Normal;
        BringToFront();
        Activate();
        bool top = TopMost;
        TopMost = true;
        TopMost = top;
      }
    }

    private void SetLogVisible(bool visible) {
      var sizeSpan = Height - ClientSize.Height;
      if (visible) {
        FormBorderStyle = FormBorderStyle.Sizable;
        txtLog.Visible = true;
        Height = sizeSpan + gbxGeneralSettings.Height + gbxStatus.Height + 100 + mainMenu.Height;
      }
      else {
        FormBorderStyle = FormBorderStyle.FixedDialog;
        Height = sizeSpan + gbxGeneralSettings.Height + gbxStatus.Height + mainMenu.Height;
        txtLog.Visible = false;
      }
    }

    private void checkFoNewVersionToolStripMenuItem_Click(object sender, EventArgs e) {
      Updater.CheckForUpdates(Updater.CheckUpdatesMode.AllMessages);
    }

    private void siteToolStripMenuItem_Click(object sender, EventArgs e) {
      Updater.VisitAppSite();
    }

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
      Updater.ShowAbout();
    }

    private void RefreshSystemSettings() {
      try {
        logger.Log("Retrieving system configuration...");

        cbxSingleSessionPerUser.Checked = wrapper.SingleSessionPerUser;
        cbxAllowTSConnections.Checked = wrapper.AllowTsConnections;
        cbxHonorLegacy.Checked = wrapper.HonorLegacy;
        numRDPPort.Value = oldPort = wrapper.RdpPort;
        numMaxConnections.Enabled = OSHelper.IsWindowsServer;
        numMaxConnections.Value = wrapper.MaximumConnectionsAllowed;

        rgNLAOptions.SelectedIndex = wrapper.SecurityLayer switch {
          0 when wrapper.UserAuthentication == 0 => 0,
          1 when wrapper.UserAuthentication == 0 => 1,
          2 when wrapper.UserAuthentication == 1 => 2,
          _ => rgNLAOptions.SelectedIndex
        };

        rgShadowOptions.SelectedIndex = wrapper.ShadowOptions;
        cbDontDisplayLastUser.Checked = wrapper.DontDisplayLastUser;
        cbxDisableSecurityWarning.Checked = wrapper.DisableSecurityWarning;
        cbxRestrictClientUsbRedirection.Checked = wrapper.RestrictUsbRedirection ?? false;
        cbxAllowPlaybackRedirect.Checked = wrapper.AllowHostPlaybackRedirect;
        cbxAllowAudioCapture.Checked = wrapper.AllowClientAudioCapture;
        cbxAllowVideoCapture.Checked = wrapper.AllowClientVideoCapture;
        cbxAllowPnp.Checked = wrapper.AllowPnpRedirect;

        logger.Log(" Done", Logger.StateKind.Info, false);
      }
      catch (Exception ex) {
        var message = "Error loading settings: " + ex.Message;
        logger.Log(message, Logger.StateKind.Error);
        //MessageBox.Show(message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void btnClose_Click(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(btnClose_Click), sender, e);
        return;
      }
      Close();
    }

    private void InitializeTheme() {

      ToolStripRadioButtonMenuItem.DisplayAsCheckboxes = true;
      mainMenu.Renderer = new ThemedToolStripRenderer();

      var currentItem = CustomTheme.FillThemesMenu((title, theme, onClick) => {
        if (theme == null && onClick == null) {
          themeMenuItem.DropDownItems.Add(title);
          return null;
        }
        var item = new ToolStripRadioButtonMenuItem(title, null, onClick);
        themeMenuItem.DropDownItems.Add(item);
        return item;
      }, () => {
        settings.SetValue("theme", Theme.IsAutoThemeEnabled ? "auto" : Theme.Current.Id);
      }, settings.GetValue("theme", "auto"), "rdpWrapper.themes");
      currentItem?.PerformClick();
      Theme.Current.Apply(this);
    }

    private void btnRestartService_Click(object sender, EventArgs e) {
      try {
        SetControlsState(false);
        //todo: async
        wrapper.StopService(TimeSpan.FromSeconds(10));
        wrapper.StartService(TimeSpan.FromSeconds(10));
      }
      catch (Exception ex) {
        var message = "Error restarting service: " + ex.Message;
        logger.Log(message, Logger.StateKind.Error);
        MessageBox.Show(message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally {
        SetControlsState(true);
      }
    }

    private void TimerTick(object sender, EventArgs e) {

      bool? checkSupported = false;
      var wrapperInstalled = wrapper.CheckWrapperInstalled();
      switch (wrapperInstalled) {
        case WrapperInstalledState.Unknown:
          lblWrapperStateValue.Text = "Unknown";
          lblWrapperStateValue.ForeColor = Theme.Current.InfoColor;
          uninstallMenuItem.Enabled = installMenuItem.Enabled = btnInstall.Enabled = btnInstall.Visible = false;
          break;
        case WrapperInstalledState.NotInstalled:
          lblWrapperStateValue.Text = "Not installed";
          lblWrapperStateValue.ForeColor = Theme.Current.InfoColor;
          installMenuItem.Enabled = btnInstall.Enabled = btnInstall.Visible = true;
          uninstallMenuItem.Enabled = false;
          btnInstall.Text = "Install";
          break;
        case WrapperInstalledState.RdpWrap:
          lblWrapperStateValue.Text = "RdpWrap";
          lblWrapperStateValue.ForeColor = Theme.Current.MessageColor;
          string wrapperIniPath = null;
          if (!wrapper.WrapperPath.IsNullOrEmpty()) {
            var wrappedDir = Path.GetDirectoryName(wrapper.WrapperPath);
            if (wrappedDir != null) {
              wrapperIniPath = Path.Combine(wrappedDir, Wrapper.RdpWrapIniName);
              checkSupported = File.Exists(wrapperIniPath);
            }
          }
          if (wrapperIniPath != wrapperIniLastPath) {
            wrapperIniLastPath = wrapperIniPath;
            wrapperIniLastChecked = DateTime.MinValue;
          }
          installMenuItem.Enabled = false;
          uninstallMenuItem.Enabled = btnInstall.Enabled = btnInstall.Visible = true;
          btnInstall.Text = "Uninstall";
          break;
        case WrapperInstalledState.ThirdParty:
          lblWrapperStateValue.Text = "3rd-party";
          lblWrapperStateValue.ForeColor = Theme.Current.WarnColor;
          uninstallMenuItem.Enabled = installMenuItem.Enabled = btnInstall.Enabled = btnInstall.Visible = false;
          break;
        case WrapperInstalledState.TermWrap:
          lblWrapperStateValue.Text = "TermWrap";
          lblWrapperStateValue.ForeColor = Theme.Current.MessageColor;
          checkSupported = null;
          wrapperIniLastChecked = DateTime.MinValue;
          wrapperIniLastPath = null;
          installMenuItem.Enabled = false;
          uninstallMenuItem.Enabled = btnInstall.Enabled = btnInstall.Visible = true;
          btnInstall.Text = "Uninstall";
          break;
      }

      switch (wrapper.GetServiceState()) {
        case ServiceControllerStatus.Stopped:
          lblServiceStateValue.Text = "Stopped";
          lblServiceStateValue.ForeColor = Theme.Current.WarnColor;
          break;
        case ServiceControllerStatus.StartPending:
          lblServiceStateValue.Text = "Starting..";
          lblServiceStateValue.ForeColor = Theme.Current.ForegroundColor;
          break;
        case ServiceControllerStatus.StopPending:
          lblServiceStateValue.Text = "Stopping...";
          lblServiceStateValue.ForeColor = Theme.Current.ForegroundColor;
          break;
        case ServiceControllerStatus.Running:
          lblServiceStateValue.Text = "Running";
          lblServiceStateValue.ForeColor = Theme.Current.MessageColor;
          break;
        case ServiceControllerStatus.ContinuePending:
          lblServiceStateValue.Text = "Resuming...";
          lblServiceStateValue.ForeColor = Theme.Current.ForegroundColor;
          break;
        case ServiceControllerStatus.PausePending:
          lblServiceStateValue.Text = "Suspending...";
          lblServiceStateValue.ForeColor = Theme.Current.ForegroundColor;
          break;
        case ServiceControllerStatus.Paused:
          lblServiceStateValue.Text = "Suspended";
          lblServiceStateValue.ForeColor = Theme.Current.ForegroundColor;
          break;
        default:
          lblServiceStateValue.Text = "Unknown";
          lblServiceStateValue.ForeColor = Theme.Current.ForegroundColor;
          break;
      }

      if (WinStationHelper.IsListenerWorking()){
        lblListenerStateValue.Text = "Listening";
        lblListenerStateValue.ForeColor = Theme.Current.MessageColor;
      }
      else {
        lblListenerStateValue.Text = "Not listening";
        lblListenerStateValue.ForeColor = Theme.Current.WarnColor;
      }

      if (wrapper.WrapperPath.IsNullOrEmpty() || !File.Exists(wrapper.WrapperPath)) {
        lblWrapperVersion.Text = "N/A";
        lblWrapperVersion.ForeColor = Theme.Current.WarnColor;
      }
      else {
        var versionInfo = FileVersionInfo.GetVersionInfo(wrapper.WrapperPath);
        lblWrapperVersion.Text = Wrapper.GetVersionString(versionInfo);
        lblWrapperVersion.ForeColor = Theme.Current.ForegroundColor;
      }

      if (!File.Exists(wrapper.TermSrvFile)) {
        txtServiceVersion.Text = "N/A";
        txtServiceVersion.ForeColor = Theme.Current.WarnColor;
      }
      else {
        var versionInfo = FileVersionInfo.GetVersionInfo(wrapper.TermSrvFile);
        txtServiceVersion.Text = Wrapper.GetVersionString(versionInfo);
        txtServiceVersion.ForeColor = Theme.Current.ForegroundColor;

#if LITEVERSION
        btnGenerate.Visible = false;
#else
        editWrapIniMenuItem.Enabled = generateMenuItem.Enabled = btnGenerate.Enabled = btnGenerate.Visible = wrapperInstalled == WrapperInstalledState.RdpWrap;
#endif
        lblSupported.Visible = checkSupported is true;
        if (checkSupported is true) {
          if (versionInfo.FileMajorPart == 6 && versionInfo.FileMinorPart == 0 ||
              versionInfo.FileMajorPart == 6 && versionInfo.FileMinorPart == 1) {
            lblSupported.Text = "[supported partially]";
            lblSupported.ForeColor = Theme.Current.InfoColor;
          }
          else {
            var lastModified = File.GetLastWriteTime(wrapperIniLastPath);
            if (lastModified > wrapperIniLastChecked) {
              var iniContent = File.ReadAllText(wrapperIniLastPath);
              wrapperIniLastSupported = iniContent.Contains("[" + Wrapper.GetVersionString(versionInfo) + "]");
              wrapperIniLastChecked = lastModified;
            }
            if (wrapperIniLastSupported) {
              lblSupported.Text = "[fully supported]";
              lblSupported.ForeColor = Theme.Current.MessageColor;
              return;
            }
          }
          lblSupported.Text = "[not supported]";
          lblSupported.ForeColor = Theme.Current.WarnColor;
        }
        else if (!checkSupported.HasValue) {
          lblSupported.Text = "[supported]";
          lblSupported.ForeColor = Theme.Current.MessageColor;
        }
      }
    }

    private void addUserToolStripMenuItem_Click(object sender, EventArgs e) {
      try {
        SetControlsState(false);
        addUserToolStripMenuItem.Enabled = false;
        if (InputForm.GetValue(Updater.ApplicationName, "Please enter the 'User name':", out var userName) == DialogResult.OK) {
          using (var usersManager = new LocalUsersManager(logger)) {
            if (usersManager.GetRemoteDesktopUsers().Any(u => u.Equals(userName, StringComparison.OrdinalIgnoreCase))) {
              MessageBox.Show($"User '{userName}' is already a member of 'Remote Desktop Users' group.", Updater.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else {
              var user = usersManager.CreateUserIfNotExist(userName);
              //if (user.GetAuthorizationGroups().Any(g => g.Sid.Value == LocalUsersManager.RemoteDesktopUsersGroupSid)) {
              //  MessageBox.Show($"User '{userName}' is already a member of 'Remote Desktop Users' group.", Updater.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
              //}
              //else {
                if (InputForm.GetValue(Updater.ApplicationName, "Please enter the 'Password':", out var password) == DialogResult.OK) {
                  usersManager.SetUserPassword(user, password);
                  usersManager.EnsureUserInRemoteDesktopUsers(user);
                  MessageBox.Show($"User '{user.Name}' is a member of 'Remote Desktop Users' group.", Updater.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
              //}
            }
          }
        }
      }
      catch (Exception ex) {
        logger.Log(ex.Message, Logger.StateKind.Error);
        MessageBox.Show(ex.Message, Updater.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
      finally {
        SetControlsState(true);
        addUserToolStripMenuItem.Enabled = true;
      }
    }

    private void fixMSUserMenuItem_Click(object sender, EventArgs e) {
      try {
        SetControlsState(false);
        fixMSUserMenuItem.Enabled = false;
        if (InputForm.GetValue(Updater.ApplicationName, "Enter MS account 'User name':", out var userName) == DialogResult.OK) {
          var process = Process.Start("runAs", $"/u:{userName} \"{Updater.CurrentFileLocation}\"");
          if (process != null) {
            process.WaitForExit(10000);
            if (process.ExitCode == 0) {
              MessageBox.Show("Action completed successfully!", Updater.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
              return;
            }
          }
          MessageBox.Show("Oops, something went wrong. Please try again later.", Updater.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
      }
      catch (Exception ex) {
        logger.Log(ex.Message, Logger.StateKind.Error);
        MessageBox.Show(ex.Message, Updater.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
      finally {
        SetControlsState(true);
        fixMSUserMenuItem.Enabled = true;
      }
    }

    private void manageUsersToolStripMenuItem_Click(object sender, EventArgs e) {
      Process.Start("lusrmgr.msc");
    }

    private void manageUsersoldToolStripMenuItem_Click(object sender, EventArgs e) {
      Process.Start("netplwiz");
      //Process.Start("control", "userpasswords2");
    }

    private void btnTest_Click(object sender, EventArgs e) {

      var width = Screen.PrimaryScreen.Bounds.Width * 2 / 3;
      var height = Screen.PrimaryScreen.Bounds.Height * 2 / 3;
      var arguments = $"/v:127.0.0.2:{oldPort} /w:{width} /h:{height}";
      if (MessageBox.Show("Test as public connection? (Passwords and bitmaps aren't cached)", Updater.ApplicationTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
        arguments += " /public";
      }
      //https://learn.microsoft.com/en-us/windows-server/administration/windows-commands/mstsc
      Process.Start("mstsc.exe", arguments);
    }

    private void btnGenerate_Click(object sender, EventArgs e) {
      try {
        SetControlsState(false);
#if LITEVERSION
        MessageBox.Show("No need for Ini file with TermWrap.", Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
#else
        wrapper.GenerateIniFile(wrapperIniLastPath, true, (message) =>
          MessageBox.Show(message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        );
#endif
      }
      finally {
        SetControlsState(true);
      }
    }

    private void patchTermsrvMenuItem_Click(object sender, EventArgs e) {
      if (MessageBox.Show(
            "This will patch C:\\Windows\\System32\\termsrv.dll for the 10.0.26100.8521 byte pattern and restart TermService. Continue?",
            Updater.ApplicationTitle,
            MessageBoxButtons.OKCancel,
            MessageBoxIcon.Warning) != DialogResult.OK) {
        return;
      }

      try {
        SetControlsState(false);
        var result = wrapper.ApplyTermsrvPatch26100_8521();
        logger.Log(result, Logger.StateKind.Info);
        MessageBox.Show(result, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      catch (Exception ex) {
        var message = "Failed to patch termsrv.dll: " + ex.Message;
        logger.Log(message, Logger.StateKind.Error);
        MessageBox.Show(message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally {
        SetControlsState(true);
      }
    }

    private void restoreTermsrvMenuItem_Click(object sender, EventArgs e) {
      if (MessageBox.Show(
            "This will restore the latest backed up termsrv.dll and restart TermService. Continue?",
            Updater.ApplicationTitle,
            MessageBoxButtons.OKCancel,
            MessageBoxIcon.Warning) != DialogResult.OK) {
        return;
      }

      try {
        SetControlsState(false);
        var result = wrapper.RestoreTermsrvPatch26100_8521();
        logger.Log(result, Logger.StateKind.Info);
        MessageBox.Show(result, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      catch (Exception ex) {
        var message = "Failed to restore termsrv.dll: " + ex.Message;
        logger.Log(message, Logger.StateKind.Error);
        MessageBox.Show(message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally {
        SetControlsState(true);
      }
    }

    private void btnEditWrapIni_Click(object sender, EventArgs e) {
      try {
        SetControlsState(false);
#if LITEVERSION
        MessageBox.Show("No Ini file available with TermWrap.", Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
#else
        if (!File.Exists(wrapperIniLastPath)) {
          MessageBox.Show($"File '{wrapperIniLastPath}' not found.", Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        Process.Start(new ProcessStartInfo {
          FileName = wrapperIniLastPath,
          UseShellExecute = true
        });
#endif
      }
      finally {
        SetControlsState(true);
      }
    }

    private void AddToLog(string message, Logger.StateKind state, bool newLine) {
      if (InvokeRequired) {
        Invoke(new Action<string, Logger.StateKind, bool>(AddToLog), message, state);
        return;
      }

      if (newLine)
        txtLog.AppendLine($"{DateTime.Now:T} - ", Theme.Current.ForegroundColor);
      switch (state) {
        case Logger.StateKind.Error:
          txtLog.AppendLine(message, Theme.Current.WarnColor, false);
          break;
        case Logger.StateKind.Info:
          txtLog.AppendLine(message, Theme.Current.MessageColor, false);
          break;
        default:
          txtLog.AppendLine(message, Theme.Current.ForegroundColor, false);
          break;
      }

      // if (!string.IsNullOrEmpty(_logFileName))
      //   File.AppendAllText(_logFileName, $"{DateTime.Now:T} - {message}\n");
      txtLog.ScrollToCaret();
      Application.DoEvents();
    }

    private void btnInstall_Click(object sender, EventArgs e) {

      var operation = btnInstall.Text;
      SetControlsState(false);
      try {
        if (operation == "Install") {
          wrapper.Install(preferredWrapper, addDefenderExclusion.Value);
          if (showAntivirusWarn.Value) {
            MessageBox.Show($"Wrapper was installed in folder: '{wrapper.WrapperFolderPath}'\nYour antivirus software might block or interfere with wrapper folder.\nPlease verify that it is included in the exclusion list!", Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
          }
        }
        else {
          wrapper.Uninstall();
        }
      }
      catch (Exception ex) {
        var message = $"Failed to {operation}: {ex.Message}";
        logger.Log(message, Logger.StateKind.Error);
        MessageBox.Show(message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally {
        SetControlsState(true);
      }
    }

    private void SetControlsState(bool enabled) {
      refreshTimer.Enabled = enabled;
      if (!enabled) {
        btnInstall.Enabled = installMenuItem.Enabled = uninstallMenuItem.Enabled = false;
        editWrapIniMenuItem.Enabled = btnGenerate.Enabled = generateMenuItem.Enabled = false;
      }
      patchTermsrvMenuItem.Enabled = restoreTermsrvMenuItem.Enabled = enabled;
      btnRestartService.Enabled = restartServiceMenuItem.Enabled = enabled;
    }

    private void SavePosition() {
      var bounds = WindowState == FormWindowState.Normal
          ? Bounds
          : RestoreBounds;
      settings.SetValue("Window_X", bounds.X.ToString());
      settings.SetValue("Window_Y", bounds.Y.ToString());
      //settings.SetValue("Window_Width", bounds.Width.ToString());
      settings.SetValue("Window_Height", bounds.Height.ToString());

      //var state = WindowState == FormWindowState.Minimized
      //    ? FormWindowState.Normal
      //    : WindowState;
      //settings.SetValue("Window_State", ((int)state).ToString());
    }

    private void RestorePosition() {
      var x = settings.GetValue("Window_X", -1);
      var y = settings.GetValue("Window_Y", -1);
      var w = Width;// settings.GetValue("Window_Width", -1);
      var h = settings.GetValue("Window_Height", -1);
      bool hasAll = x >= 0 && y >= 0 && w >= 0 && h >= 0;

      Rectangle bounds = new(x, y, w, h);
      bool isVisible = Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(bounds));

      if (hasAll && isVisible) {
        StartPosition = FormStartPosition.Manual;
        Bounds = bounds;
        //var stateValue = settings.GetValue("Window_State", -1);
        //if (stateValue > -1) {
        //  var state = (FormWindowState)stateValue;
        //  if (state != FormWindowState.Minimized) {
        //    WindowState = state;
        //  }
        //}
      }
      else {
        StartPosition = FormStartPosition.CenterScreen;
      }
    }
  }
}
