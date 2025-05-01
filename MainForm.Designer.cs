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
      this.rgNLA = new System.Windows.Forms.GroupBox();
      this.rgNLAOptions = new System.Windows.Forms.ComboBox();
      this.rgShadow = new System.Windows.Forms.GroupBox();
      this.rgShadowOptions = new System.Windows.Forms.ComboBox();
      this.seRDPPort = new System.Windows.Forms.NumericUpDown();
      this.lRDPPort = new System.Windows.Forms.Label();
      this.rgNLA.SuspendLayout();
      this.rgShadow.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.seRDPPort)).BeginInit();
      this.SuspendLayout();
      // 
      // btnClose
      // 
      this.btnClose.Location = new System.Drawing.Point(177, 163);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(100, 35);
      this.btnClose.TabIndex = 0;
      this.btnClose.Text = "Close";
      this.btnClose.Click += new System.EventHandler(this.btnCloseClick);
      // 
      // btnApply
      // 
      this.btnApply.Location = new System.Drawing.Point(318, 163);
      this.btnApply.Name = "btnApply";
      this.btnApply.Size = new System.Drawing.Size(100, 35);
      this.btnApply.TabIndex = 1;
      this.btnApply.Text = "Apply";
      this.btnApply.Click += new System.EventHandler(this.btnApplyClick);
      // 
      // cbxSingleSessionPerUser
      // 
      this.cbxSingleSessionPerUser.AutoSize = true;
      this.cbxSingleSessionPerUser.Location = new System.Drawing.Point(12, 71);
      this.cbxSingleSessionPerUser.Name = "cbxSingleSessionPerUser";
      this.cbxSingleSessionPerUser.Size = new System.Drawing.Size(199, 24);
      this.cbxSingleSessionPerUser.TabIndex = 2;
      this.cbxSingleSessionPerUser.Text = "Single session per user";
      this.cbxSingleSessionPerUser.CheckedChanged += new System.EventHandler(this.onChanged);
      // 
      // cbxAllowTSConnections
      // 
      this.cbxAllowTSConnections.AutoSize = true;
      this.cbxAllowTSConnections.Location = new System.Drawing.Point(12, 12);
      this.cbxAllowTSConnections.Name = "cbxAllowTSConnections";
      this.cbxAllowTSConnections.Size = new System.Drawing.Size(210, 24);
      this.cbxAllowTSConnections.TabIndex = 3;
      this.cbxAllowTSConnections.Text = "Enable Remote Desktop";
      this.cbxAllowTSConnections.CheckedChanged += new System.EventHandler(this.onChanged);
      // 
      // cbDontDisplayLastUser
      // 
      this.cbDontDisplayLastUser.AutoSize = true;
      this.cbDontDisplayLastUser.Location = new System.Drawing.Point(12, 101);
      this.cbDontDisplayLastUser.Name = "cbDontDisplayLastUser";
      this.cbDontDisplayLastUser.Size = new System.Drawing.Size(239, 24);
      this.cbDontDisplayLastUser.TabIndex = 4;
      this.cbDontDisplayLastUser.Text = "Do not display last username";
      this.cbDontDisplayLastUser.CheckedChanged += new System.EventHandler(this.onChanged);
      // 
      // rgNLA
      // 
      this.rgNLA.Controls.Add(this.rgNLAOptions);
      this.rgNLA.Location = new System.Drawing.Point(283, 12);
      this.rgNLA.Name = "rgNLA";
      this.rgNLA.Size = new System.Drawing.Size(309, 67);
      this.rgNLA.TabIndex = 5;
      this.rgNLA.TabStop = false;
      this.rgNLA.Text = "Authentication Mode";
      // 
      // rgNLAOptions
      // 
      this.rgNLAOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.rgNLAOptions.Location = new System.Drawing.Point(6, 25);
      this.rgNLAOptions.Name = "rgNLAOptions";
      this.rgNLAOptions.Size = new System.Drawing.Size(297, 28);
      this.rgNLAOptions.TabIndex = 0;
      this.rgNLAOptions.SelectedIndexChanged += new System.EventHandler(this.onChanged);
      // 
      // rgShadow
      // 
      this.rgShadow.Controls.Add(this.rgShadowOptions);
      this.rgShadow.Location = new System.Drawing.Point(289, 85);
      this.rgShadow.Name = "rgShadow";
      this.rgShadow.Size = new System.Drawing.Size(303, 61);
      this.rgShadow.TabIndex = 6;
      this.rgShadow.TabStop = false;
      this.rgShadow.Text = "Session Shadowing Mode";
      // 
      // rgShadowOptions
      // 
      this.rgShadowOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.rgShadowOptions.Location = new System.Drawing.Point(0, 25);
      this.rgShadowOptions.Name = "rgShadowOptions";
      this.rgShadowOptions.Size = new System.Drawing.Size(297, 28);
      this.rgShadowOptions.TabIndex = 0;
      this.rgShadowOptions.SelectedIndexChanged += new System.EventHandler(this.onChanged);
      // 
      // seRDPPort
      // 
      this.seRDPPort.Location = new System.Drawing.Point(153, 39);
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
      this.lRDPPort.Location = new System.Drawing.Point(8, 42);
      this.lRDPPort.Name = "lRDPPort";
      this.lRDPPort.Size = new System.Drawing.Size(100, 23);
      this.lRDPPort.TabIndex = 8;
      this.lRDPPort.Text = "RDP Port:";
      // 
      // MainForm
      // 
      this.ClientSize = new System.Drawing.Size(604, 210);
      this.Controls.Add(this.btnClose);
      this.Controls.Add(this.rgShadow);
      this.Controls.Add(this.btnApply);
      this.Controls.Add(this.cbxSingleSessionPerUser);
      this.Controls.Add(this.cbxAllowTSConnections);
      this.Controls.Add(this.cbDontDisplayLastUser);
      this.Controls.Add(this.rgNLA);
      this.Controls.Add(this.seRDPPort);
      this.Controls.Add(this.lRDPPort);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "MainForm";
      this.Text = "RDP Wrapper";
      this.rgNLA.ResumeLayout(false);
      this.rgShadow.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.seRDPPort)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private Button btnClose;
    private Button btnApply;
    private CheckBox cbxSingleSessionPerUser;
    private CheckBox cbxAllowTSConnections;
    private CheckBox cbDontDisplayLastUser;
    private ComboBox rgNLAOptions;
    private ComboBox rgShadowOptions;
    private GroupBox rgNLA;
    private GroupBox rgShadow;
    private NumericUpDown seRDPPort;
    private Label lRDPPort;
  }
}