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

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
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
      this.btnUninstall = new System.Windows.Forms.Button();
      this.btnInstall = new System.Windows.Forms.Button();
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
      this.txtLog = new rdpWrapper.SimplTextBox();
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
      this.btnClose.Location = new System.Drawing.Point(299, 4);
      this.btnClose.Margin = new System.Windows.Forms.Padding(2);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(67, 23);
      this.btnClose.TabIndex = 2;
      this.btnClose.Text = "Close";
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnApply
      // 
      this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnApply.Location = new System.Drawing.Point(221, 4);
      this.btnApply.Margin = new System.Windows.Forms.Padding(2);
      this.btnApply.Name = "btnApply";
      this.btnApply.Size = new System.Drawing.Size(67, 23);
      this.btnApply.TabIndex = 1;
      this.btnApply.Text = "Apply";
      this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
      // 
      // cbxSingleSessionPerUser
      // 
      this.cbxSingleSessionPerUser.AutoSize = true;
      this.cbxSingleSessionPerUser.Location = new System.Drawing.Point(4, 58);
      this.cbxSingleSessionPerUser.Margin = new System.Windows.Forms.Padding(2);
      this.cbxSingleSessionPerUser.Name = "cbxSingleSessionPerUser";
      this.cbxSingleSessionPerUser.Size = new System.Drawing.Size(134, 17);
      this.cbxSingleSessionPerUser.TabIndex = 1;
      this.cbxSingleSessionPerUser.Text = "Single session per user";
      this.cbxSingleSessionPerUser.CheckedChanged += new System.EventHandler(this.OnChanged);
      // 
      // cbxAllowTSConnections
      // 
      this.cbxAllowTSConnections.AutoSize = true;
      this.cbxAllowTSConnections.Location = new System.Drawing.Point(4, 23);
      this.cbxAllowTSConnections.Margin = new System.Windows.Forms.Padding(2);
      this.cbxAllowTSConnections.Name = "cbxAllowTSConnections";
      this.cbxAllowTSConnections.Size = new System.Drawing.Size(142, 17);
      this.cbxAllowTSConnections.TabIndex = 0;
      this.cbxAllowTSConnections.Text = "Enable Remote Desktop";
      this.cbxAllowTSConnections.CheckedChanged += new System.EventHandler(this.OnChanged);
      // 
      // cbDontDisplayLastUser
      // 
      this.cbDontDisplayLastUser.AutoSize = true;
      this.cbDontDisplayLastUser.Location = new System.Drawing.Point(4, 78);
      this.cbDontDisplayLastUser.Margin = new System.Windows.Forms.Padding(2);
      this.cbDontDisplayLastUser.Name = "cbDontDisplayLastUser";
      this.cbDontDisplayLastUser.Size = new System.Drawing.Size(161, 17);
      this.cbDontDisplayLastUser.TabIndex = 2;
      this.cbDontDisplayLastUser.Text = "Do not display last username";
      this.cbDontDisplayLastUser.CheckedChanged += new System.EventHandler(this.OnChanged);
      // 
      // rgNLAOptions
      // 
      this.rgNLAOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.rgNLAOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.rgNLAOptions.Location = new System.Drawing.Point(183, 57);
      this.rgNLAOptions.Margin = new System.Windows.Forms.Padding(2);
      this.rgNLAOptions.Name = "rgNLAOptions";
      this.rgNLAOptions.Size = new System.Drawing.Size(184, 21);
      this.rgNLAOptions.TabIndex = 7;
      this.rgNLAOptions.SelectedIndexChanged += new System.EventHandler(this.OnChanged);
      // 
      // rgShadowOptions
      // 
      this.rgShadowOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.rgShadowOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.rgShadowOptions.Location = new System.Drawing.Point(183, 97);
      this.rgShadowOptions.Margin = new System.Windows.Forms.Padding(2);
      this.rgShadowOptions.Name = "rgShadowOptions";
      this.rgShadowOptions.Size = new System.Drawing.Size(184, 21);
      this.rgShadowOptions.TabIndex = 9;
      this.rgShadowOptions.SelectedIndexChanged += new System.EventHandler(this.OnChanged);
      // 
      // seRDPPort
      // 
      this.seRDPPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.seRDPPort.Location = new System.Drawing.Point(285, 21);
      this.seRDPPort.Margin = new System.Windows.Forms.Padding(2);
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
      this.seRDPPort.Size = new System.Drawing.Size(80, 20);
      this.seRDPPort.TabIndex = 5;
      this.seRDPPort.Value = new decimal(new int[] {
            3389,
            0,
            0,
            0});
      this.seRDPPort.ValueChanged += new System.EventHandler(this.OnChanged);
      // 
      // lRDPPort
      // 
      this.lRDPPort.Location = new System.Drawing.Point(183, 21);
      this.lRDPPort.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.lRDPPort.Name = "lRDPPort";
      this.lRDPPort.Size = new System.Drawing.Size(67, 15);
      this.lRDPPort.TabIndex = 4;
      this.lRDPPort.Text = "RDP Port:";
      // 
      // cbxHonorLegacy
      // 
      this.cbxHonorLegacy.AutoSize = true;
      this.cbxHonorLegacy.Location = new System.Drawing.Point(4, 99);
      this.cbxHonorLegacy.Margin = new System.Windows.Forms.Padding(2);
      this.cbxHonorLegacy.Name = "cbxHonorLegacy";
      this.cbxHonorLegacy.Size = new System.Drawing.Size(169, 17);
      this.cbxHonorLegacy.TabIndex = 3;
      this.cbxHonorLegacy.Text = "Allow to start custom programs";
      this.cbxHonorLegacy.CheckedChanged += new System.EventHandler(this.OnChanged);
      // 
      // panActions
      // 
      this.panActions.Controls.Add(this.btnTest);
      this.panActions.Controls.Add(this.btnClose);
      this.panActions.Controls.Add(this.btnApply);
      this.panActions.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panActions.Location = new System.Drawing.Point(0, 267);
      this.panActions.Margin = new System.Windows.Forms.Padding(2);
      this.panActions.Name = "panActions";
      this.panActions.Size = new System.Drawing.Size(373, 30);
      this.panActions.TabIndex = 3;
      // 
      // btnTest
      // 
      this.btnTest.Location = new System.Drawing.Point(6, 4);
      this.btnTest.Margin = new System.Windows.Forms.Padding(2);
      this.btnTest.Name = "btnTest";
      this.btnTest.Size = new System.Drawing.Size(67, 23);
      this.btnTest.TabIndex = 1;
      this.btnTest.Text = "Test";
      this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
      // 
      // btnRestartService
      // 
      this.btnRestartService.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnRestartService.Location = new System.Drawing.Point(298, 15);
      this.btnRestartService.Margin = new System.Windows.Forms.Padding(2);
      this.btnRestartService.Name = "btnRestartService";
      this.btnRestartService.Size = new System.Drawing.Size(67, 19);
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
      this.gbxGeneralSettings.Dock = System.Windows.Forms.DockStyle.Top;
      this.gbxGeneralSettings.Location = new System.Drawing.Point(0, 86);
      this.gbxGeneralSettings.Margin = new System.Windows.Forms.Padding(2);
      this.gbxGeneralSettings.Name = "gbxGeneralSettings";
      this.gbxGeneralSettings.Padding = new System.Windows.Forms.Padding(2);
      this.gbxGeneralSettings.Size = new System.Drawing.Size(373, 126);
      this.gbxGeneralSettings.TabIndex = 2;
      this.gbxGeneralSettings.TabStop = false;
      this.gbxGeneralSettings.Text = "General settings";
      // 
      // lblShadowMode
      // 
      this.lblShadowMode.AutoSize = true;
      this.lblShadowMode.Location = new System.Drawing.Point(183, 82);
      this.lblShadowMode.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.lblShadowMode.Name = "lblShadowMode";
      this.lblShadowMode.Size = new System.Drawing.Size(130, 13);
      this.lblShadowMode.TabIndex = 8;
      this.lblShadowMode.Text = "Session Shadowing Mode";
      // 
      // lblAuthMode
      // 
      this.lblAuthMode.AutoSize = true;
      this.lblAuthMode.Location = new System.Drawing.Point(183, 41);
      this.lblAuthMode.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.lblAuthMode.Name = "lblAuthMode";
      this.lblAuthMode.Size = new System.Drawing.Size(105, 13);
      this.lblAuthMode.TabIndex = 6;
      this.lblAuthMode.Text = "Authentication Mode";
      // 
      // gbxStatus
      // 
      this.gbxStatus.Controls.Add(this.btnUninstall);
      this.gbxStatus.Controls.Add(this.btnRestartService);
      this.gbxStatus.Controls.Add(this.btnInstall);
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
      this.gbxStatus.Dock = System.Windows.Forms.DockStyle.Top;
      this.gbxStatus.Location = new System.Drawing.Point(0, 0);
      this.gbxStatus.Margin = new System.Windows.Forms.Padding(2);
      this.gbxStatus.Name = "gbxStatus";
      this.gbxStatus.Padding = new System.Windows.Forms.Padding(2);
      this.gbxStatus.Size = new System.Drawing.Size(373, 86);
      this.gbxStatus.TabIndex = 1;
      this.gbxStatus.TabStop = false;
      this.gbxStatus.Text = "Diagnostics";
      // 
      // btnUninstall
      // 
      this.btnUninstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnUninstall.Location = new System.Drawing.Point(298, 38);
      this.btnUninstall.Margin = new System.Windows.Forms.Padding(2);
      this.btnUninstall.Name = "btnUninstall";
      this.btnUninstall.Size = new System.Drawing.Size(67, 19);
      this.btnUninstall.TabIndex = 1;
      this.btnUninstall.Text = "Uninstall";
      this.btnUninstall.Visible = false;
      this.btnUninstall.Click += new System.EventHandler(this.btnUninstall_Click);
      // 
      // btnInstall
      // 
      this.btnInstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnInstall.Location = new System.Drawing.Point(298, 38);
      this.btnInstall.Margin = new System.Windows.Forms.Padding(2);
      this.btnInstall.Name = "btnInstall";
      this.btnInstall.Size = new System.Drawing.Size(67, 19);
      this.btnInstall.TabIndex = 10;
      this.btnInstall.Text = "Install";
      this.btnInstall.Visible = false;
      this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
      // 
      // btnGenerate
      // 
      this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnGenerate.Location = new System.Drawing.Point(298, 61);
      this.btnGenerate.Margin = new System.Windows.Forms.Padding(2);
      this.btnGenerate.Name = "btnGenerate";
      this.btnGenerate.Size = new System.Drawing.Size(67, 19);
      this.btnGenerate.TabIndex = 2;
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
      this.txtServiceVersion.Location = new System.Drawing.Point(183, 21);
      this.txtServiceVersion.Margin = new System.Windows.Forms.Padding(2);
      this.txtServiceVersion.Name = "txtServiceVersion";
      this.txtServiceVersion.ReadOnly = true;
      this.txtServiceVersion.Size = new System.Drawing.Size(111, 13);
      this.txtServiceVersion.TabIndex = 3;
      this.txtServiceVersion.TabStop = false;
      // 
      // lblSupported
      // 
      this.lblSupported.AutoSize = true;
      this.lblSupported.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblSupported.ForeColor = System.Drawing.Color.Red;
      this.lblSupported.Location = new System.Drawing.Point(180, 64);
      this.lblSupported.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.lblSupported.Name = "lblSupported";
      this.lblSupported.Size = new System.Drawing.Size(93, 13);
      this.lblSupported.TabIndex = 8;
      this.lblSupported.Text = "[not supported]";
      // 
      // lblListenerStateValue
      // 
      this.lblListenerStateValue.AutoSize = true;
      this.lblListenerStateValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblListenerStateValue.ForeColor = System.Drawing.Color.Red;
      this.lblListenerStateValue.Location = new System.Drawing.Point(100, 64);
      this.lblListenerStateValue.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.lblListenerStateValue.Name = "lblListenerStateValue";
      this.lblListenerStateValue.Size = new System.Drawing.Size(60, 13);
      this.lblListenerStateValue.TabIndex = 7;
      this.lblListenerStateValue.Text = "Unknown";
      // 
      // lblServiceStateValue
      // 
      this.lblServiceStateValue.AutoSize = true;
      this.lblServiceStateValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblServiceStateValue.ForeColor = System.Drawing.Color.Red;
      this.lblServiceStateValue.Location = new System.Drawing.Point(100, 21);
      this.lblServiceStateValue.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.lblServiceStateValue.Name = "lblServiceStateValue";
      this.lblServiceStateValue.Size = new System.Drawing.Size(60, 13);
      this.lblServiceStateValue.TabIndex = 1;
      this.lblServiceStateValue.Text = "Unknown";
      // 
      // lblWrapperVersion
      // 
      this.lblWrapperVersion.AutoSize = true;
      this.lblWrapperVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblWrapperVersion.Location = new System.Drawing.Point(180, 42);
      this.lblWrapperVersion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.lblWrapperVersion.Name = "lblWrapperVersion";
      this.lblWrapperVersion.Size = new System.Drawing.Size(47, 13);
      this.lblWrapperVersion.TabIndex = 5;
      this.lblWrapperVersion.Text = "1.0.0.0";
      // 
      // lblWrapperStateValue
      // 
      this.lblWrapperStateValue.AutoSize = true;
      this.lblWrapperStateValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblWrapperStateValue.ForeColor = System.Drawing.Color.Red;
      this.lblWrapperStateValue.Location = new System.Drawing.Point(100, 42);
      this.lblWrapperStateValue.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.lblWrapperStateValue.Name = "lblWrapperStateValue";
      this.lblWrapperStateValue.Size = new System.Drawing.Size(60, 13);
      this.lblWrapperStateValue.TabIndex = 4;
      this.lblWrapperStateValue.Text = "Unknown";
      // 
      // lblListenerState
      // 
      this.lblListenerState.AutoSize = true;
      this.lblListenerState.Location = new System.Drawing.Point(8, 64);
      this.lblListenerState.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.lblListenerState.Name = "lblListenerState";
      this.lblListenerState.Size = new System.Drawing.Size(73, 13);
      this.lblListenerState.TabIndex = 6;
      this.lblListenerState.Text = "Listener state:";
      // 
      // lblServiceState
      // 
      this.lblServiceState.AutoSize = true;
      this.lblServiceState.Location = new System.Drawing.Point(8, 21);
      this.lblServiceState.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.lblServiceState.Name = "lblServiceState";
      this.lblServiceState.Size = new System.Drawing.Size(72, 13);
      this.lblServiceState.TabIndex = 0;
      this.lblServiceState.Text = "Service state:";
      // 
      // lblWrapperState
      // 
      this.lblWrapperState.AutoSize = true;
      this.lblWrapperState.Location = new System.Drawing.Point(8, 42);
      this.lblWrapperState.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.lblWrapperState.Name = "lblWrapperState";
      this.lblWrapperState.Size = new System.Drawing.Size(77, 13);
      this.lblWrapperState.TabIndex = 3;
      this.lblWrapperState.Text = "Wrapper state:";
      // 
      // txtLog
      // 
      this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtLog.Location = new System.Drawing.Point(0, 212);
      this.txtLog.Margin = new System.Windows.Forms.Padding(2);
      this.txtLog.Name = "txtLog";
      this.txtLog.ReadOnly = true;
      this.txtLog.Size = new System.Drawing.Size(373, 55);
      this.txtLog.TabIndex = 0;
      this.txtLog.Text = "";
      // 
      // MainForm
      // 
      this.AcceptButton = this.btnApply;
      this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
      this.CancelButton = this.btnClose;
      this.ClientSize = new System.Drawing.Size(373, 297);
      this.Controls.Add(this.txtLog);
      this.Controls.Add(this.panActions);
      this.Controls.Add(this.gbxGeneralSettings);
      this.Controls.Add(this.gbxStatus);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Margin = new System.Windows.Forms.Padding(2);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "MainForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
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
    private System.Windows.Forms.Panel panActions;
    private System.Windows.Forms.GroupBox gbxGeneralSettings;
    private Label lblAuthMode;
    private Label lblShadowMode;
    private Button btnRestartService;
    private System.Windows.Forms.GroupBox gbxStatus;
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
    private rdpWrapper.SimplTextBox txtLog;
    private Button btnInstall;
    private Button btnUninstall;
  }
}