using System;

namespace rdpWrapper {

  internal class Logger {

    internal enum StateKind {
      Log,
      Info,
      Warning,
      Error,
    }

    public event Action<string, StateKind, bool> OnNewLogEvent;

    public void Log(string message, StateKind kind = StateKind.Log, bool newLine = true) {
      OnNewLogEvent?.Invoke(message, kind, newLine);
    }
  }
}
