using sergiye.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace rdpWrapper {
  internal static class Program {

    private const int CodeException = -1;
    private const int CodeOk = 0;
    private const int CodeInvalidArgs = 1;

    //[DllImport("kernel32.dll")]
    //private static extern bool AttachConsole(int dwProcessId);
    //[DllImport("kernel32.dll")]
    //private static extern bool AllocConsole();
    //[DllImport("kernel32.dll")]
    //private static extern bool FreeConsole();
    // [DllImport("kernel32.dll")]
    // private static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);
    // [DllImport("kernel32.dll")]
    // static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate handlerRoutine, bool add);
    // delegate Boolean ConsoleCtrlDelegate(uint ctrlType);

    private static int result = CodeOk; //lets be a bit optimistic
    //private static bool consoleAllocated;
    private static FileLogger logger;

    [STAThread]
    private static void Main(string[] args) {

      Crasher.Listen();

      // new Wrapper(new FileLogger()).EncryptResources();
      // return;

      var consoleMode = args.Length > 0;
      if (consoleMode) {
        logger = new FileLogger();
        logger.OnNewLogEvent += AddToLog;
        //if (!AttachConsole(-1)) {
        //  consoleAllocated = AllocConsole();
        //}
        logger.Log($"{Updater.ApplicationTitle} {typeof(Program).Assembly.GetName().Version.ToString(3)} {(Environment.Is64BitProcess ? "x64" : "x32")}", Logger.StateKind.Info);
      }

      if (!OSHelper.IsCompatible(true, out var errorMessage, out var fixAction)) {
        if (consoleMode) {
          logger.Log(errorMessage, Logger.StateKind.Error);
        }
        else {
          if (fixAction != null) {
            if (MessageBox.Show(errorMessage, Updater.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
              fixAction();
            }
          }
          else {
            MessageBox.Show(errorMessage, Updater.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
          }
        }
        Environment.Exit(0);
      }

      if (WinApiHelper.CheckRunningInstances(true, true)) {
        // fallback
        MessageBox.Show($"{Updater.ApplicationName} is already running.", Updater.ApplicationName,
          MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        return;
      }

      if (consoleMode) {
        StartConsole(args);
      }
      else {
        StartWinForms();
      }

      Environment.Exit(result);
    }

    private static void StartConsole(string[] args) {
      try {
        // always check for an updated version of the application unless disabled by launch arguments
        // "-offline" should be the LAST parameter
        var offline = args.Any(a => a == "-offline");
        if (!offline) {
          Updater.CheckForUpdates(Updater.CheckUpdatesMode.NotifyOnNewVersion);
        }
        switch (args[0]) {
          case "-help":
            //todo: show help with supported options
            logger.Log("Usage:\n -x \t start UI and no wait for exit;\n -generate \t generate Ini;\n -install \t install wrapper;\n -uninstall \t uninstall wrapper;\n -patchtermsrv \t patch termsrv.dll for 10.0.26100.8521;\n -restoretermsrv \t restore latest termsrv.dll backup", Logger.StateKind.Info);
            break;
          case "-x":
            //start UI as a new separate process
            Process.Start(typeof(Program).Assembly.Location);
            logger.Log("Started UI in a new process", Logger.StateKind.Info);
            break;
          case "-generate": {
#if LITEVERSION
            logger.Log("No need for Ini file with TermWrap.");
#else
            //todo: check wrapper installed & wrapper is RdpWrap -> generate new Ini
            var wrapper = new Wrapper(logger);
            var wrapperIniPath = Path.Combine(wrapper.WrapperFolderPath, Wrapper.RdpWrapIniName);
            wrapper.GenerateIniFile(wrapperIniPath);
#endif
            break;
          }
          case "-install": {
            var wrapper = new Wrapper(logger);
            var settings = new PersistentSettings();
            settings.Load();
            if (!Enum.TryParse(settings.GetValue("preferredWrapper", "TermWrap"), out SupportedWrappers preferredWrapper)) {
              preferredWrapper = SupportedWrappers.TermWrap;
            }
            wrapper.Install(preferredWrapper, true);
            break;
          }
          case "-uninstall": {
            var wrapper = new Wrapper(logger);
            wrapper.Uninstall();
            break;
          }
          case "-start": {
            var wrapper = new Wrapper(logger);
            wrapper.StartService(TimeSpan.FromSeconds(10));
            break;
          }
          case "-stop": {
            var wrapper = new Wrapper(logger);
            wrapper.StopService(TimeSpan.FromSeconds(10));
            break;
          }
          case "-patchtermsrv": {
            var wrapper = new Wrapper(logger);
            logger.Log(wrapper.ApplyTermsrvPatch26100_8521(), Logger.StateKind.Info);
            break;
          }
          case "-restoretermsrv": {
            var wrapper = new Wrapper(logger);
            logger.Log(wrapper.RestoreTermsrvPatch26100_8521(), Logger.StateKind.Info);
            break;
          }
          default:
            result = CodeInvalidArgs;
            break;
        }
      }
      catch (Exception ex) {
        logger.Log("Error: " + ex.Message, Logger.StateKind.Error);
        result = CodeException;
      }
      finally {
        logger?.Dispose();

        // SetConsoleCtrlHandler(null, false);
        // GenerateConsoleCtrlEvent(0, 0);
        // Process.GetCurrentProcess().StandardOutput.Close();
        // Process.GetCurrentProcess().StandardInput.Close();

        //if (consoleAllocated) {
        //  FreeConsole();
        //}

        // Console.Out.Close();
        // Console.Error.Close();
      }
    }

    private static void StartWinForms() {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      using var form = new MainForm();
      form.FormClosed += delegate {
        Application.Exit();
      };
      Application.Run(form);
    }

    private static void AddToLog(string message, Logger.StateKind state, bool newLine) {

      //Console.BackgroundColor = ConsoleColor.Black;
      if (newLine) {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write($"\n{DateTime.Now:T} - ");
      }

      switch (state) {
        case Logger.StateKind.Error:
          Console.ForegroundColor = ConsoleColor.Red;
          Console.Write(message);
          break;
        case Logger.StateKind.Info:
          Console.ForegroundColor = ConsoleColor.White;
          Console.Write(message);
          break;
        default:
          Console.ForegroundColor = ConsoleColor.Gray;
          Console.Write(message);
          break;
      }
      Console.ResetColor();
    }
  }
}
