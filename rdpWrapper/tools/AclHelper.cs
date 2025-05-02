using System;
using System.Runtime.InteropServices;

namespace rdpWrapper {
  
  internal static class AclHelper {

    private const int ErrorSuccess = 0;
    private const int GenericAll = 0x10000000;
    private const int DaclSecurityInformation = 0x00000004;
    private const int SeFileObject = 1;

    private const int GrantAccess = 1;
    private const int NoMultipleTrustee = 0;
    private const int TrusteeIsSid = 0;
    private const int TrusteeIsWellKnownGroup = 5;
    private const int SubContainersAndObjectsInherit = 0x3;

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool ConvertStringSidToSid(string stringSid, out IntPtr sid);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern uint SetEntriesInAcl(
      int cCountOfExplicitEntries,
      [In] ref ExplicitAccess pListOfExplicitEntries,
      IntPtr oldAcl,
      out IntPtr newAcl);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern uint SetNamedSecurityInfo(
      string pObjectName,
      int objectType,
      int securityInfo,
      IntPtr psidOwner,
      IntPtr psidGroup,
      IntPtr pDacl,
      IntPtr pSacl);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr LocalFree(IntPtr hMem);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct ExplicitAccess {
      public int grfAccessPermissions;
      public int grfAccessMode;
      public int grfInheritance;
      public Trustee Trustee;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct Trustee {
      public IntPtr pMultipleTrustee;
      public int MultipleTrusteeOperation;
      public int TrusteeForm;
      public int TrusteeType;
      public IntPtr ptstrName;
    }

    internal static void GrantSidFullAccess(string path, string sidString, Logger logger = null) {
      
      if (!ConvertStringSidToSid(sidString, out var pSid)) {
        logger?.Log($"ConvertStringSidToSid failed. Code: {Marshal.GetLastWin32Error()}", Logger.StateKind.Error);
        return;
      }

      var ea = new ExplicitAccess {
        grfAccessPermissions = GenericAll,
        grfAccessMode = GrantAccess,
        grfInheritance = SubContainersAndObjectsInherit,
        Trustee = new Trustee {
          pMultipleTrustee = IntPtr.Zero,
          MultipleTrusteeOperation = NoMultipleTrustee,
          TrusteeForm = TrusteeIsSid,
          TrusteeType = TrusteeIsWellKnownGroup,
          ptstrName = pSid
        }
      };

      var result = SetEntriesInAcl(1, ref ea, IntPtr.Zero, out var pDacl);
      if (result == ErrorSuccess) {
        var setResult = SetNamedSecurityInfo(path, SeFileObject, DaclSecurityInformation, IntPtr.Zero, IntPtr.Zero,
          pDacl, IntPtr.Zero);
        if (setResult != ErrorSuccess) {
          logger?.Log($"SetNamedSecurityInfo failed. Code: {Marshal.GetLastWin32Error()}", Logger.StateKind.Error);
        }

        LocalFree(pDacl);
      }
      else {
        logger?.Log($"SetEntriesInAcl failed. Code: {Marshal.GetLastWin32Error()}", Logger.StateKind.Error);
      }

      LocalFree(pSid);
    }
  }
}