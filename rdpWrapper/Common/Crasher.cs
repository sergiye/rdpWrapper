using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace sergiye.Common
{
    public static class Crasher
    {
        public static event EventHandler SaveState;

        public static void Listen(bool disableCrashDialog = true)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (disableCrashDialog)
            {
                // disable Windows Error Reporting (crash dialog)
                ErrorModes dwMode = SetErrorMode(ErrorModes.SEM_NOGPFAULTERRORBOX);
                SetErrorMode(dwMode | ErrorModes.SEM_NOGPFAULTERRORBOX);
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception)
            {
                var details = ((Exception)e.ExceptionObject).TraceException();
                var path = Path.Combine(Path.GetDirectoryName(typeof(Crasher).Assembly.Location), "Crash_" + DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                File.WriteAllText(path, details);
            }

            if (SaveState != null)
                SaveState(null, EventArgs.Empty);

            Environment.Exit(-1);
        }

        [Flags]
        private enum ErrorModes : uint
        {
            SYSTEM_DEFAULT = 0x0,
            SEM_FAILCRITICALERRORS = 0x0001,
            SEM_NOALIGNMENTFAULTEXCEPT = 0x0004,
            SEM_NOGPFAULTERRORBOX = 0x0002,
            SEM_NOOPENFILEERRORBOX = 0x8000
        }

        [DllImport("kernel32.dll")]
        private static extern ErrorModes SetErrorMode(ErrorModes uMode);

        public static string TraceException(this Exception ex)
        {
            var text = new StringBuilder();
            while (ex != null)
            {
                text.AppendLine(ex.Message);
                text.AppendLine(ex.GetType().Name);
                text.AppendLine(ex.StackTrace);
                ex = ex.InnerException;
            }
            return text.ToString();
        }
    }
}
