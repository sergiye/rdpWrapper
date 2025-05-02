using System.Drawing;
using System.Windows.Forms;

namespace rdpWrapper {
  partial class MainForm {
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent() {
      this.btnClose = new System.Windows.Forms.Button();
      this.btnApply = new System.Windows.Forms.Button();
      this.cbxSingleSessionPerUser = new System.Windows.Forms.CheckBox();
      this.cbxAllowTSConnections = new System.Windows.Forms.CheckBox();
      this.cbDontDisplayLastUser = new System.Windows.Forms.CheckBox();
      this.rgNLAOptions = new System.Windows.Forms.ComboBox();
      this.rgShadowOptions = new System.Windows.Forms.ComboBox();
      this.seRDPPort = new System.Windows.Forms.NumericUpDown();
      this.lRDPPort = new System.Windows.Forms.Label();
      this.cbxHonorLegacy = new System.Windows.Forms.CheckBox();
      this.panActions = new System.Windows.Forms.Panel();
      this.btnTest = new System.Windows.Forms.Button();
      this.btnRestartService = new System.Windows.Forms.Button();
      this.gbxGeneralSettings = new System.Windows.Forms.GroupBox();
      this.lblShadowMode = new System.Windows.Forms.Label();
      this.lblAuthMode = new System.Windows.Forms.Label();
      this.gbxStatus = new System.Windows.Forms.GroupBox();
      this.btnGenerate = new System.Windows.Forms.Button();
      this.txtServiceVersion = new System.Windows.Forms.TextBox();
      this.lblSupported = new System.Windows.Forms.Label();
      this.lblListenerStateValue = new System.Windows.Forms.Label();
      this.lblServiceStateValue = new System.Windows.Forms.Label();
      this.lblWrapperVersion = new System.Windows.Forms.Label();
      this.lblWrapperStateValue = new System.Windows.Forms.Label();
      this.lblListenerState = new System.Windows.Forms.Label();
      this.lblServiceState = new System.Windows.Forms.Label();
      this.lblWrapperState = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.seRDPPort)).BeginInit();
      this.panActions.SuspendLayout();
      this.gbxGeneralSettings.SuspendLayout();
      this.gbxStatus.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnClose.Location = new System.Drawing.Point(448, 12);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(100, 35);
      this.btnClose.TabIndex = 2;
      this.btnClose.Text = "Close";
      this.btnClose.Click += new System.EventHandler(this.BtnCloseClick);
      // 
      // btnApply
      // 
      this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnApply.Location = new System.Drawing.Point(332, 12);
      this.btnApply.Name = "btnApply";
      this.btnApply.Size = new System.Drawing.Size(100, 35);
      this.btnApply.TabIndex = 1;
      this.btnApply.Text = "Apply";
      this.btnApply.Click += new System.EventHandler(this.BtnApplyClick);
      // 
      // cbxSingleSessionPerUser
      // 
      this.cbxSingleSessionPerUser.AutoSize = true;
      this.cbxSingleSessionPerUser.Location = new System.Drawing.Point(6, 87);
      this.cbxSingleSessionPerUser.Name = "cbxSingleSessionPerUser";
      this.cbxSingleSessionPerUser.Size = new System.Drawing.Size(199, 24);
      this.cbxSingleSessionPerUser.TabIndex = 3;
      this.cbxSingleSessionPerUser.Text = "Single session per user";
      this.cbxSingleSessionPerUser.CheckedChanged += new System.EventHandler(this.OnChanged);
      // 
      // cbxAllowTSConnections
      // 
      this.cbxAllowTSConnections.AutoSize = true;
      this.cbxAllowTSConnections.Location = new System.Drawing.Point(6, 34);
      this.cbxAllowTSConnections.Name = "cbxAllowTSConnections";
      this.cbxAllowTSConnections.Size = new System.Drawing.Size(210, 24);
      this.cbxAllowTSConnections.TabIndex = 0;
      this.cbxAllowTSConnections.Text = "Enable Remote Desktop";
      this.cbxAllowTSConnections.CheckedChanged += new System.EventHandler(this.OnChanged);
      // 
      // cbDontDisplayLastUser
      // 
      this.cbDontDisplayLastUser.AutoSize = true;
      this.cbDontDisplayLastUser.Location = new System.Drawing.Point(6, 117);
      this.cbDontDisplayLastUser.Name = "cbDontDisplayLastUser";
      this.cbDontDisplayLastUser.Size = new System.Drawing.Size(239, 24);
      this.cbDontDisplayLastUser.TabIndex = 4;
      this.cbDontDisplayLastUser.Text = "Do not display last username";
      this.cbDontDisplayLastUser.CheckedChanged += new System.EventHandler(this.OnChanged);
      // 
      // rgNLAOptions
      // 
      this.rgNLAOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.rgNLAOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.rgNLAOptions.Location = new System.Drawing.Point(274, 85);
      this.rgNLAOptions.Name = "rgNLAOptions";
      this.rgNLAOptions.Size = new System.Drawing.Size(274, 28);
      this.rgNLAOptions.TabIndex = 7;
      this.rgNLAOptions.SelectedIndexChanged += new System.EventHandler(this.OnChanged);
      // 
      // rgShadowOptions
      // 
      this.rgShadowOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.rgShadowOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.rgShadowOptions.Location = new System.Drawing.Point(274, 146);
      this.rgShadowOptions.Name = "rgShadowOptions";
      this.rgShadowOptions.Size = new System.Drawing.Size(274, 28);
      this.rgShadowOptions.TabIndex = 9;
      this.rgShadowOptions.SelectedIndexChanged += new System.EventHandler(this.OnChanged);
      // 
      // seRDPPort
      // 
      this.seRDPPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.seRDPPort.Location = new System.Drawing.Point(428, 32);
      this.seRDPPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
      this.seRDPPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.seRDPPort.Name = "seRDPPort";
      this.seRDPPort.Size = new System.Drawing.Size(120, 26);
      this.seRDPPort.TabIndex = 2;
      this.seRDPPort.Value = new decimal(new int[] {
            3389,
            0,
            0,
            0});
      this.seRDPPort.ValueChanged += new System.EventHandler(this.OnChanged);
      // 
      // lRDPPort
      // 
      this.lRDPPort.Location = new System.Drawing.Point(274, 32);
      this.lRDPPort.Name = "lRDPPort";
      this.lRDPPort.Size = new System.Drawing.Size(100, 23);
      this.lRDPPort.TabIndex = 1;
      this.lRDPPort.Text = "RDP Port:";
      // 
      // cbxHonorLegacy
      // 
      this.cbxHonorLegacy.AutoSize = true;
      this.cbxHonorLegacy.Location = new System.Drawing.Point(6, 149);
      this.cbxHonorLegacy.Name = "cbxHonorLegacy";
      this.cbxHonorLegacy.Size = new System.Drawing.Size(253, 24);
      this.cbxHonorLegacy.TabIndex = 5;
      this.cbxHonorLegacy.Text = "Allow to start custom programs";
      // 
      // panActions
      // 
      this.panActions.Controls.Add(this.btnTest);
      this.panActions.Controls.Add(this.btnRestartService);
      this.panActions.Controls.Add(this.btnClose);
      this.panActions.Controls.Add(this.btnApply);
      this.panActions.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panActions.Location = new System.Drawing.Point(0, 319);
      this.panActions.Name = "panActions";
      this.panActions.Size = new System.Drawing.Size(560, 59);
      this.panActions.TabIndex = 2;
      // 
      // btnTest
      // 
      this.btnTest.Location = new System.Drawing.Point(116, 12);
      this.btnTest.Name = "btnTest";
      this.btnTest.Size = new System.Drawing.Size(100, 35);
      this.btnTest.TabIndex = 1;
      this.btnTest.Text = "Test";
      this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
      // 
      // btnRestartService
      // 
      this.btnRestartService.Location = new System.Drawing.Point(10, 12);
      this.btnRestartService.Name = "btnRestartService";
      this.btnRestartService.Size = new System.Drawing.Size(100, 35);
      this.btnRestartService.TabIndex = 0;
      this.btnRestartService.Text = "Restart";
      this.btnRestartService.Click += new System.EventHandler(this.btnRestartService_Click);
      // 
      // gbxGeneralSettings
      // 
      this.gbxGeneralSettings.Controls.Add(this.rgShadowOptions);
      this.gbxGeneralSettings.Controls.Add(this.lblShadowMode);
      this.gbxGeneralSettings.Controls.Add(this.rgNLAOptions);
      this.gbxGeneralSettings.Controls.Add(this.lblAuthMode);
      this.gbxGeneralSettings.Controls.Add(this.cbDontDisplayLastUser);
      this.gbxGeneralSettings.Controls.Add(this.lRDPPort);
      this.gbxGeneralSettings.Controls.Add(this.cbxHonorLegacy);
      this.gbxGeneralSettings.Controls.Add(this.seRDPPort);
      this.gbxGeneralSettings.Controls.Add(this.cbxAllowTSConnections);
      this.gbxGeneralSettings.Controls.Add(this.cbxSingleSessionPerUser);
      this.gbxGeneralSettings.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.gbxGeneralSettings.Location = new System.Drawing.Point(0, 130);
      this.gbxGeneralSettings.Name = "gbxGeneralSettings";
      this.gbxGeneralSettings.Size = new System.Drawing.Size(560, 189);
      this.gbxGeneralSettings.TabIndex = 1;
      this.gbxGeneralSettings.TabStop = false;
      this.gbxGeneralSettings.Text = "General settings";
      // 
      // lblShadowMode
      // 
      this.lblShadowMode.AutoSize = true;
      this.lblShadowMode.Location = new System.Drawing.Point(274, 123);
      this.lblShadowMode.Name = "lblShadowMode";
      this.lblShadowMode.Size = new System.Drawing.Size(193, 20);
      this.lblShadowMode.TabIndex = 8;
      this.lblShadowMode.Text = "Session Shadowing Mode";
      // 
      // lblAuthMode
      // 
      this.lblAuthMode.AutoSize = true;
      this.lblAuthMode.Location = new System.Drawing.Point(274, 62);
      this.lblAuthMode.Name = "lblAuthMode";
      this.lblAuthMode.Size = new System.Drawing.Size(156, 20);
      this.lblAuthMode.TabIndex = 6;
      this.lblAuthMode.Text = "Authentication Mode";
      // 
      // gbxStatus
      // 
      this.gbxStatus.Controls.Add(this.btnGenerate);
      this.gbxStatus.Controls.Add(this.txtServiceVersion);
      this.gbxStatus.Controls.Add(this.lblSupported);
      this.gbxStatus.Controls.Add(this.lblListenerStateValue);
      this.gbxStatus.Controls.Add(this.lblServiceStateValue);
      this.gbxStatus.Controls.Add(this.lblWrapperVersion);
      this.gbxStatus.Controls.Add(this.lblWrapperStateValue);
      this.gbxStatus.Controls.Add(this.lblListenerState);
      this.gbxStatus.Controls.Add(this.lblServiceState);
      this.gbxStatus.Controls.Add(this.lblWrapperState);
      this.gbxStatus.Dock = System.Windows.Forms.DockStyle.Fill;
      this.gbxStatus.Location = new System.Drawing.Point(0, 0);
      this.gbxStatus.Name = "gbxStatus";
      this.gbxStatus.Size = new System.Drawing.Size(560, 130);
      this.gbxStatus.TabIndex = 0;
      this.gbxStatus.TabStop = false;
      this.gbxStatus.Text = "Diagnostics";
      // 
      // btnGenerate
      // 
      this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnGenerate.Location = new System.Drawing.Point(448, 92);
      this.btnGenerate.Name = "btnGenerate";
      this.btnGenerate.Size = new System.Drawing.Size(100, 29);
      this.btnGenerate.TabIndex = 9;
      this.btnGenerate.Text = "Generate";
      this.btnGenerate.Visible = false;
      this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
      // 
      // txtServiceVersion
      // 
      this.txtServiceVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtServiceVersion.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.txtServiceVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.txtServiceVersion.Location = new System.Drawing.Point(274, 31);
      this.txtServiceVersion.Name = "txtServiceVersion";
      this.txtServiceVersion.ReadOnly = true;
      this.txtServiceVersion.Size = new System.Drawing.Size(274, 19);
      this.txtServiceVersion.TabIndex = 2;
      // 
      // lblSupported
      // 
      this.lblSupported.AutoSize = true;
      this.lblSupported.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblSupported.ForeColor = System.Drawing.Color.Red;
      this.lblSupported.Location = new System.Drawing.Point(270, 96);
      this.lblSupported.Name = "lblSupported";
      this.lblSupported.Size = new System.Drawing.Size(131, 20);
      this.lblSupported.TabIndex = 8;
      this.lblSupported.Text = "[not supported]";
      // 
      // lblListenerStateValue
      // 
      this.lblListenerStateValue.AutoSize = true;
      this.lblListenerStateValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblListenerStateValue.ForeColor = System.Drawing.Color.Red;
      this.lblListenerStateValue.Location = new System.Drawing.Point(150, 96);
      this.lblListenerStateValue.Name = "lblListenerStateValue";
      this.lblListenerStateValue.Size = new System.Drawing.Size(83, 20);
      this.lblListenerStateValue.TabIndex = 7;
      this.lblListenerStateValue.Text = "Unknown";
      // 
      // lblServiceStateValue
      // 
      this.lblServiceStateValue.AutoSize = true;
      this.lblServiceStateValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblServiceStateValue.ForeColor = System.Drawing.Color.Red;
      this.lblServiceStateValue.Location = new System.Drawing.Point(150, 31);
      this.lblServiceStateValue.Name = "lblServiceStateValue";
      this.lblServiceStateValue.Size = new System.Drawing.Size(83, 20);
      this.lblServiceStateValue.TabIndex = 1;
      this.lblServiceStateValue.Text = "Unknown";
      // 
      // lblWrapperVersion
      // 
      this.lblWrapperVersion.AutoSize = true;
      this.lblWrapperVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblWrapperVersion.Location = new System.Drawing.Point(270, 63);
      this.lblWrapperVersion.Name = "lblWrapperVersion";
      this.lblWrapperVersion.Size = new System.Drawing.Size(64, 20);
      this.lblWrapperVersion.TabIndex = 5;
      this.lblWrapperVersion.Text = "1.0.0.0";
      // 
      // lblWrapperStateValue
      // 
      this.lblWrapperStateValue.AutoSize = true;
      this.lblWrapperStateValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblWrapperStateValue.ForeColor = System.Drawing.Color.Red;
      this.lblWrapperStateValue.Location = new System.Drawing.Point(150, 63);
      this.lblWrapperStateValue.Name = "lblWrapperStateValue";
      this.lblWrapperStateValue.Size = new System.Drawing.Size(83, 20);
      this.lblWrapperStateValue.TabIndex = 4;
      this.lblWrapperStateValue.Text = "Unknown";
      // 
      // lblListenerState
      // 
      this.lblListenerState.AutoSize = true;
      this.lblListenerState.Location = new System.Drawing.Point(12, 96);
      this.lblListenerState.Name = "lblListenerState";
      this.lblListenerState.Size = new System.Drawing.Size(110, 20);
      this.lblListenerState.TabIndex = 6;
      this.lblListenerState.Text = "Listener state:";
      // 
      // lblServiceState
      // 
      this.lblServiceState.AutoSize = true;
      this.lblServiceState.Location = new System.Drawing.Point(12, 31);
      this.lblServiceState.Name = "lblServiceState";
      this.lblServiceState.Size = new System.Drawing.Size(105, 20);
      this.lblServiceState.TabIndex = 0;
      this.lblServiceState.Text = "Service state:";
      // 
      // lblWrapperState
      // 
      this.lblWrapperState.AutoSize = true;
      this.lblWrapperState.Location = new System.Drawing.Point(12, 63);
      this.lblWrapperState.Name = "lblWrapperState";
      this.lblWrapperState.Size = new System.Drawing.Size(114, 20);
      this.lblWrapperState.TabIndex = 3;
      this.lblWrapperState.Text = "Wrapper state:";
      // 
      // MainForm
      // 
      this.AcceptButton = this.btnApply;
      this.CancelButton = this.btnClose;
      this.ClientSize = new System.Drawing.Size(560, 378);
      this.Controls.Add(this.gbxStatus);
      this.Controls.Add(this.gbxGeneralSettings);
      this.Controls.Add(this.panActions);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "MainForm";
      this.Text = "RDP Wrapper";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
      ((System.ComponentModel.ISupportInitialize)(this.seRDPPort)).EndInit();
      this.panActions.ResumeLayout(false);
      this.gbxGeneralSettings.ResumeLayout(false);
      this.gbxGeneralSettings.PerformLayout();
      this.gbxStatus.ResumeLayout(false);
      this.gbxStatus.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private Button btnClose;
    private Button btnApply;
    private CheckBox cbxSingleSessionPerUser;
    private CheckBox cbxAllowTSConnections;
    private CheckBox cbDontDisplayLastUser;
    private ComboBox rgNLAOptions;
    private ComboBox rgShadowOptions;
    private NumericUpDown seRDPPort;
    private Label lRDPPort;
    private CheckBox cbxHonorLegacy;
    private Panel panActions;
    private GroupBox gbxGeneralSettings;
    private Label lblAuthMode;
    private Label lblShadowMode;
    private Button btnRestartService;
    private GroupBox gbxStatus;
    private Label lblSupported;
    private Label lblListenerStateValue;
    private Label lblServiceStateValue;
    private Label lblWrapperVersion;
    private Label lblWrapperStateValue;
    private Label lblListenerState;
    private Label lblServiceState;
    private Label lblWrapperState;
    private TextBox txtServiceVersion;
    private Button btnTest;
    private Button btnGenerate;
  }
}