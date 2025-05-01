using System;
using System.Runtime.InteropServices;

namespace rdpWrapper {

  internal static class WinStationHelper {
    private const string WINSTADLL = "winsta.dll";

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WTS_SESSION_INFO {
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
      if (!WinStationEnumerateW(IntPtr.Zero, out IntPtr ppSessionInfo, out int count))
        return false;

      try {
        int size = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
        for (int i = 0; i < count; i++) {
          IntPtr pItem = IntPtr.Add(ppSessionInfo, i * size);
          WTS_SESSION_INFO sessionInfo = Marshal.PtrToStructure<WTS_SESSION_INFO>(pItem);
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
