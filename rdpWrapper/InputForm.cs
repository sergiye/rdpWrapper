using sergiye.Common;
using System.Drawing;
using System.Windows.Forms;

namespace rdpWrapper {

  internal partial class InputForm : Form {

    private InputForm() {
      InitializeComponent();
      Icon = Icon.ExtractAssociatedIcon(typeof(MainForm).Assembly.Location);

      txtInput.TextChanged += (s, e) => btnOk.Enabled = !txtInput.Text.IsNullOrEmpty();
    }

    internal static DialogResult GetValue(string title, string promptText, out string input, bool secret = false) {

      var form = new InputForm();
      form.lblText.Text = promptText;
      form.txtInput.UseSystemPasswordChar = secret;
      form.Text = title;
      Theme.Current.Apply(form);
      var dialogResult = form.ShowDialog();
      input = form.txtInput.Text;
      return dialogResult;
    }
  }
}
