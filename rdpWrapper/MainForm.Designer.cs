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
      this.panel1 = new System.Windows.Forms.Panel();
      this.gbxGeneralSettings = new System.Windows.Forms.GroupBox();
      this.lblShadowMode = new System.Windows.Forms.Label();
      this.lblAuthMode = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.seRDPPort)).BeginInit();
      this.panel1.SuspendLayout();
      this.gbxGeneralSettings.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Location = new System.Drawing.Point(645, 12);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(100, 35);
      this.btnClose.TabIndex = 0;
      this.btnClose.Text = "Close";
      this.btnClose.Click += new System.EventHandler(this.btnCloseClick);
      // 
      // btnApply
      // 
      this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnApply.Location = new System.Drawing.Point(529, 12);
      this.btnApply.Name = "btnApply";
      this.btnApply.Size = new System.Drawing.Size(100, 35);
      this.btnApply.TabIndex = 1;
      this.btnApply.Text = "Apply";
      this.btnApply.Click += new System.EventHandler(this.btnApplyClick);
      // 
      // cbxSingleSessionPerUser
      // 
      this.cbxSingleSessionPerUser.AutoSize = true;
      this.cbxSingleSessionPerUser.Location = new System.Drawing.Point(6, 93);
      this.cbxSingleSessionPerUser.Name = "cbxSingleSessionPerUser";
      this.cbxSingleSessionPerUser.Size = new System.Drawing.Size(199, 24);
      this.cbxSingleSessionPerUser.TabIndex = 2;
      this.cbxSingleSessionPerUser.Text = "Single session per user";
      this.cbxSingleSessionPerUser.CheckedChanged += new System.EventHandler(this.onChanged);
      // 
      // cbxAllowTSConnections
      // 
      this.cbxAllowTSConnections.AutoSize = true;
      this.cbxAllowTSConnections.Location = new System.Drawing.Point(6, 34);
      this.cbxAllowTSConnections.Name = "cbxAllowTSConnections";
      this.cbxAllowTSConnections.Size = new System.Drawing.Size(210, 24);
      this.cbxAllowTSConnections.TabIndex = 3;
      this.cbxAllowTSConnections.Text = "Enable Remote Desktop";
      this.cbxAllowTSConnections.CheckedChanged += new System.EventHandler(this.onChanged);
      // 
      // cbDontDisplayLastUser
      // 
      this.cbDontDisplayLastUser.AutoSize = true;
      this.cbDontDisplayLastUser.Location = new System.Drawing.Point(6, 123);
      this.cbDontDisplayLastUser.Name = "cbDontDisplayLastUser";
      this.cbDontDisplayLastUser.Size = new System.Drawing.Size(239, 24);
      this.cbDontDisplayLastUser.TabIndex = 4;
      this.cbDontDisplayLastUser.Text = "Do not display last username";
      this.cbDontDisplayLastUser.CheckedChanged += new System.EventHandler(this.onChanged);
      // 
      // rgNLAOptions
      // 
      this.rgNLAOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.rgNLAOptions.Location = new System.Drawing.Point(6, 216);
      this.rgNLAOptions.Name = "rgNLAOptions";
      this.rgNLAOptions.Size = new System.Drawing.Size(297, 28);
      this.rgNLAOptions.TabIndex = 0;
      this.rgNLAOptions.SelectedIndexChanged += new System.EventHandler(this.onChanged);
      // 
      // rgShadowOptions
      // 
      this.rgShadowOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.rgShadowOptions.Location = new System.Drawing.Point(6, 280);
      this.rgShadowOptions.Name = "rgShadowOptions";
      this.rgShadowOptions.Size = new System.Drawing.Size(297, 28);
      this.rgShadowOptions.TabIndex = 0;
      this.rgShadowOptions.SelectedIndexChanged += new System.EventHandler(this.onChanged);
      // 
      // seRDPPort
      // 
      this.seRDPPort.Location = new System.Drawing.Point(147, 61);
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
      this.seRDPPort.TabIndex = 7;
      this.seRDPPort.Value = new decimal(new int[] {
            3389,
            0,
            0,
            0});
      this.seRDPPort.ValueChanged += new System.EventHandler(this.onChanged);
      // 
      // lRDPPort
      // 
      this.lRDPPort.Location = new System.Drawing.Point(2, 64);
      this.lRDPPort.Name = "lRDPPort";
      this.lRDPPort.Size = new System.Drawing.Size(100, 23);
      this.lRDPPort.TabIndex = 8;
      this.lRDPPort.Text = "RDP Port:";
      // 
      // cbxHonorLegacy
      // 
      this.cbxHonorLegacy.AutoSize = true;
      this.cbxHonorLegacy.Location = new System.Drawing.Point(6, 155);
      this.cbxHonorLegacy.Name = "cbxHonorLegacy";
      this.cbxHonorLegacy.Size = new System.Drawing.Size(253, 24);
      this.cbxHonorLegacy.TabIndex = 9;
      this.cbxHonorLegacy.Text = "Allow to start custom programs";
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btnClose);
      this.panel1.Controls.Add(this.btnApply);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 319);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(757, 59);
      this.panel1.TabIndex = 10;
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
      this.gbxGeneralSettings.Dock = System.Windows.Forms.DockStyle.Left;
      this.gbxGeneralSettings.Location = new System.Drawing.Point(0, 0);
      this.gbxGeneralSettings.Name = "gbxGeneralSettings";
      this.gbxGeneralSettings.Size = new System.Drawing.Size(316, 319);
      this.gbxGeneralSettings.TabIndex = 11;
      this.gbxGeneralSettings.TabStop = false;
      this.gbxGeneralSettings.Text = "General settings";
      // 
      // lblShadowMode
      // 
      this.lblShadowMode.AutoSize = true;
      this.lblShadowMode.Location = new System.Drawing.Point(6, 257);
      this.lblShadowMode.Name = "lblShadowMode";
      this.lblShadowMode.Size = new System.Drawing.Size(193, 20);
      this.lblShadowMode.TabIndex = 11;
      this.lblShadowMode.Text = "Session Shadowing Mode";
      // 
      // lblAuthMode
      // 
      this.lblAuthMode.AutoSize = true;
      this.lblAuthMode.Location = new System.Drawing.Point(6, 193);
      this.lblAuthMode.Name = "lblAuthMode";
      this.lblAuthMode.Size = new System.Drawing.Size(156, 20);
      this.lblAuthMode.TabIndex = 10;
      this.lblAuthMode.Text = "Authentication Mode";
      // 
      // MainForm
      // 
      this.ClientSize = new System.Drawing.Size(757, 378);
      this.Controls.Add(this.gbxGeneralSettings);
      this.Controls.Add(this.panel1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "MainForm";
      this.Text = "RDP Wrapper";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
      ((System.ComponentModel.ISupportInitialize)(this.seRDPPort)).EndInit();
      this.panel1.ResumeLayout(false);
      this.gbxGeneralSettings.ResumeLayout(false);
      this.gbxGeneralSettings.PerformLayout();
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
    private Panel panel1;
    private GroupBox gbxGeneralSettings;
    private Label lblAuthMode;
    private Label lblShadowMode;
  }
}