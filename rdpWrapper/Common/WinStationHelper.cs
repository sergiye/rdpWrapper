using System;
using System.Runtime.InteropServices;

namespace sergiye.Common {

  internal static class WinStationHelper {
    private const string WINSTADLL = "winsta.dll";

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WtsSessionInfo {
      public int SessionId;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
      public string Name;
      public int State;
    }

    [DllImport(WINSTADLL, CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool WinStationEnumerateW(
        IntPtr hServer,
        out IntPtr ppSessionInfo,
        out int pCount
    );

    [DllImport(WINSTADLL, SetLastError = true)]
    private static extern bool WinStationFreeMemory(IntPtr pMemory);

    internal static bool IsListenerWorking() {
      if (!WinStationEnumerateW(IntPtr.Zero, out var ppSessionInfo, out var count))
        return false;

      try {
        var size = Marshal.SizeOf(typeof(WtsSessionInfo));
        for (var i = 0; i < count; i++) {
          var pItem = IntPtr.Add(ppSessionInfo, i * size);
          var sessionInfo = Marshal.PtrToStructure<WtsSessionInfo>(pItem);
          if (sessionInfo.Name == "RDP-Tcp")
            return true;
        }
      }
      finally {
        WinStationFreeMemory(ppSessionInfo);
      }

      return false;
    }
  }
}
