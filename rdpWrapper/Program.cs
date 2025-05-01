using sergiye.Common;
using System;
using System.Threading;
using System.Windows.Forms;

namespace rdpWrapper {
  internal static class Program {

    static readonly Mutex mutex = new(true, "{1D73AD65-1407-462C-AD0A-A5938F2FD9BB}");

    [STAThread]
    static void Main() {

      if (!VersionCompatibility.IsCompatible()) {
        MessageBox.Show("The application is not compatible with your region.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        Environment.Exit(0);
      }

      if (!mutex.WaitOne(TimeSpan.Zero, true)) {
        MessageBox.Show("Another instance of the application is already running.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        Environment.Exit(0);
      }

      Crasher.Listen();

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      using var form = new MainForm();
      form.FormClosed += delegate {
        Application.Exit();
      };
      Application.Run(form);
      mutex.ReleaseMutex();
    }
  }
}
