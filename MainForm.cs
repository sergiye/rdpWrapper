using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace rdpWrapper {

  public partial class MainForm : Form {

    private const string REG_KEY = @"SYSTEM\CurrentControlSet\Control\Terminal Server";
    private const string REG_RDP_KEY = @"SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp";
    private const string REG_WINLOGON_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System";
    private const string REG_TS_KEY = @"SOFTWARE\Policies\Microsoft\Windows NT\Terminal Services";

    private const string VALUE_SINGLE_SESSION = "fSingleSessionPerUser";
    private const string VALUE_TS_CONNECTIONS = "fDenyTSConnections";
    private const string VALUE_PORT = "PortNumber";
    private const string VALUE_DONTDISPLAYLASTUSERNAME = "DontDisplayLastUsername";
    private const string VALUE_NLA = "UserAuthentication";
    private const string VALUE_SECURITY = "SecurityLayer";
    private const string VALUE_SHADOW = "Shadow";

    private readonly Timer timer;

    public MainForm() {

      InitializeComponent();
      
      rgNLAOptions.Items.AddRange(new object[] { 
        "GUI Authentication Only", 
        "Default RDP Authentication", 
        "Network Level Authentication" 
      });
      rgShadowOptions.Items.AddRange(new object[]{
        "Disable Shadowing",
        "Full access with user's permission",
        "Full access without permission",
        "View only with user's permission",
        "View only without permission"
      });

      this.StartPosition = FormStartPosition.CenterScreen;
      Load += mainFormLoad;

      timer = new Timer();
      timer.Tick += timerTick;
      timer.Interval = 1000;
    }

    private void mainFormLoad(object sender, EventArgs e) {
      try {
        using (var key = Registry.LocalMachine.OpenSubKey(REG_KEY)) {
          cbxSingleSessionPerUser.Checked = Convert.ToInt32(key.GetValue(VALUE_SINGLE_SESSION, 0)) != 0;
          cbxAllowTSConnections.Checked = Convert.ToInt32(key.GetValue(VALUE_TS_CONNECTIONS, 1)) == 0;
        }

        using (var key = Registry.LocalMachine.OpenSubKey(REG_RDP_KEY)) {
          seRDPPort.Value = Convert.ToDecimal(key.GetValue(VALUE_PORT, 3389));
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
        MessageBox.Show("Error loading settings: " + ex.Message);
      }
      btnApply.Enabled = false;
    }

    private void btnCloseClick(object sender, EventArgs e) {
      DialogResult = DialogResult.Cancel;
      Close();
    }

    private void timerTick(object sender, EventArgs e) {
      // Placeholder for timer functionality
    }

    private void btnApplyClick(object sender, EventArgs e) {
      try {
        using (var key = Registry.LocalMachine.OpenSubKey(REG_KEY, writable: true)) {
          key.SetValue(VALUE_SINGLE_SESSION, cbxSingleSessionPerUser.Checked ? 1 : 0, RegistryValueKind.DWord);
          key.SetValue(VALUE_TS_CONNECTIONS, cbxAllowTSConnections.Checked ? 0 : 1, RegistryValueKind.DWord);
        }

        using (var key = Registry.LocalMachine.OpenSubKey(REG_RDP_KEY, true)) {
          key.SetValue(VALUE_PORT, (int)seRDPPort.Value, RegistryValueKind.DWord);

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

        MessageBox.Show("Settings applied successfully.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        btnApply.Enabled = false;
      }
      catch (Exception ex) {
        MessageBox.Show("Failed to apply settings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void onChanged(object sender, EventArgs e) {
      btnApply.Enabled = true;
    }
  }
}
