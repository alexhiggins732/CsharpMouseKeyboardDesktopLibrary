using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopStream
{
    using System;
    using System.Runtime.InteropServices;

    public class WindowsLoginHelper
    {
        public static void ExecuteAsInteractive(string command = @"desktopstream.exe")
        {
            IntPtr UserTokenHandle = IntPtr.Zero;
            var sessionId = WindowsApi.WTSGetActiveConsoleSessionId();
            bool createdUserToken = WindowsApi.WTSQueryUserToken(0, ref UserTokenHandle);
            var lastError = Marshal.GetLastWin32Error();
            if (!createdUserToken)
            {

                var exception = new System.ComponentModel.Win32Exception(lastError).Message;
                Console.WriteLine($"Error creating user token: {exception}");
            }
            WindowsApi.PROCESS_INFORMATION ProcInfo = new WindowsApi.PROCESS_INFORMATION();
            WindowsApi.STARTUPINFOW StartInfo = new WindowsApi.STARTUPINFOW();
            StartInfo.cb = System.Convert.ToUInt32(System.Runtime.InteropServices.Marshal.SizeOf(StartInfo));
         
            WindowsApi.CreateProcessAsUser(UserTokenHandle,
                command,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                false, 0,
                IntPtr.Zero,
                null/* TODO Change to default(_) if this is not a reference type */,
                ref StartInfo,
                out ProcInfo);
            lastError = Marshal.GetLastWin32Error();
            if (lastError == 0)
            {
                var exception = new System.ComponentModel.Win32Exception(lastError).Message;
                Console.WriteLine($"Error creating process: {exception}");
            } else
            {
                Console.WriteLine($"Started process: {ProcInfo.dwProcessId}");
            }
            if (UserTokenHandle != IntPtr.Zero)
                WindowsApi.CloseHandle(UserTokenHandle);
        }

    }
    public class PrivilegeHelper
    {



        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]

        internal static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall,

        ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);
        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]

        internal static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);
        [DllImport("advapi32.dll", SetLastError = true)]

        internal static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);
        [StructLayout(LayoutKind.Sequential, Pack = 1)]

        internal struct TokPriv1Luid

        {
            public int Count;
            public long Luid;
            public int Attr;

        }
        internal const int SE_PRIVILEGE_ENABLED = 0x00000002;
        internal const int SE_PRIVILEGE_DISABLED = 0x00000000;
        internal const int TOKEN_QUERY = 0x00000008;
        internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;

        public static bool EnablePrivilege(long processHandle, string privilege, bool enabled)
        {

            bool retVal;
            TokPriv1Luid tp;
            IntPtr hproc = new IntPtr(processHandle);
            IntPtr htok = IntPtr.Zero;

            retVal = OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok);
            Console.WriteLine($"Opened Process token: {retVal}");
            tp.Count = 1;
            tp.Luid = 0;
            if (enabled)
            {
                tp.Attr = SE_PRIVILEGE_ENABLED;
            }
            else
            {
                tp.Attr = SE_PRIVILEGE_DISABLED;
            }
            retVal = LookupPrivilegeValue(null, privilege, ref tp.Luid);
            Console.WriteLine($"LookupPrivilegeValue: {retVal}");
            retVal = AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
            Console.WriteLine($"AdjustTokenPrivileges: {retVal}");
            return retVal;

        }

    }
    public class Privileges
    {
        public const string SeAssignPrimaryTokenPrivilege = "SeAssignPrimaryTokenPrivilege";
        public const string SeAuditPrivilege = "SeAuditPrivilege";
        public const string SeBackupPrivilege = "SeBackupPrivilege";
        public const string SeChangeNotifyPrivilege = "SeChangeNotifyPrivilege";
        public const string SeCreateGlobalPrivilege = "SeCreateGlobalPrivilege";
        public const string SeCreatePagefilePrivilege = "SeCreatePagefilePrivilege";
        public const string SeCreatePermanentPrivilege = "SeCreatePermanentPrivilege";
        public const string SeCreateSymbolicLinkPrivilege = "SeCreateSymbolicLinkPrivilege";
        public const string SeCreateTokenPrivilege = "SeCreateTokenPrivilege";
        public const string SeDebugPrivilege = "SeDebugPrivilege";
        public const string SeEnableDelegationPrivilege = "SeEnableDelegationPrivilege";
        public const string SeImpersonatePrivilege = "SeImpersonatePrivilege";
        public const string SeIncreaseBasePriorityPrivilege = "SeIncreaseBasePriorityPrivilege";
        public const string SeIncreaseQuotaPrivilege = "SeIncreaseQuotaPrivilege";
        public const string SeIncreaseWorkingSetPrivilege = "SeIncreaseWorkingSetPrivilege";
        public const string SeLoadDriverPrivilege = "SeLoadDriverPrivilege";
        public const string SeLockMemoryPrivilege = "SeLockMemoryPrivilege";
        public const string SeMachineAccountPrivilege = "SeMachineAccountPrivilege";
        public const string SeManageVolumePrivilege = "SeManageVolumePrivilege";
        public const string SeProfileSingleProcessPrivilege = "SeProfileSingleProcessPrivilege";
        public const string SeRelabelPrivilege = "SeRelabelPrivilege";
        public const string SeRemoteShutdownPrivilege = "SeRemoteShutdownPrivilege";
        public const string SeRestorePrivilege = "SeRestorePrivilege";
        public const string SeSecurityPrivilege = "SeSecurityPrivilege";
        public const string SeShutdownPrivilege = "SeShutdownPrivilege";
        public const string SeSyncAgentPrivilege = "SeSyncAgentPrivilege";
        public const string SeSystemEnvironmentPrivilege = "SeSystemEnvironmentPrivilege";
        public const string SeSystemProfilePrivilege = "SeSystemProfilePrivilege";
        public const string SeSystemtimePrivilege = "SeSystemtimePrivilege";
        public const string SeTakeOwnershipPrivilege = "SeTakeOwnershipPrivilege";
        public const string SeTcbPrivilege = "SeTcbPrivilege";
        public const string SeTimeZonePrivilege = "SeTimeZonePrivilege";
        public const string SeTrustedCredManAccessPrivilege = "SeTrustedCredManAccessPrivilege";
        public const string SeUndockPrivilege = "SeUndockPrivilege";
        public const string SeUnsolicitedInputPrivilege = "SeUnsolicitedInputPrivilege";

        public static string[] AllPrivilages = new[] { "SeAssignPrimaryTokenPrivilege", "SeAuditPrivilege", "SeBackupPrivilege",
        "SeChangeNotifyPrivilege", "SeCreateGlobalPrivilege", "SeCreatePagefilePrivilege",
        "SeCreatePermanentPrivilege", "SeCreateSymbolicLinkPrivilege", "SeCreateTokenPrivilege",
        "SeDebugPrivilege", "SeEnableDelegationPrivilege", "SeImpersonatePrivilege", "SeIncreaseBasePriorityPrivilege",
        "SeIncreaseQuotaPrivilege", "SeIncreaseWorkingSetPrivilege", "SeLoadDriverPrivilege",
        "SeLockMemoryPrivilege", "SeMachineAccountPrivilege", "SeManageVolumePrivilege",
        "SeProfileSingleProcessPrivilege", "SeRelabelPrivilege", "SeRemoteShutdownPrivilege",
        "SeRestorePrivilege", "SeSecurityPrivilege", "SeShutdownPrivilege", "SeSyncAgentPrivilege",
        "SeSystemEnvironmentPrivilege", "SeSystemProfilePrivilege", "SeSystemtimePrivilege",
        "SeTakeOwnershipPrivilege", "SeTcbPrivilege", "SeTimeZonePrivilege", "SeTrustedCredManAccessPrivilege",
        "SeUndockPrivilege", "SeUnsolicitedInputPrivilege"};


    }

    public class WindowsApi
    {
        [DllImport("kernel32.dll", EntryPoint = "WTSGetActiveConsoleSessionId", SetLastError = true)]
        public static extern uint WTSGetActiveConsoleSessionId();


        [DllImport("Wtsapi32.dll", EntryPoint = "WTSQueryUserToken", SetLastError = true)]
        public static extern bool WTSQueryUserToken(uint SessionId, ref IntPtr phToken);


        [DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true)]
        public static extern bool CloseHandle([In()] IntPtr hObject);


        [DllImport("advapi32.dll", EntryPoint = "CreateProcessAsUserW", SetLastError = true)]
        public static extern bool CreateProcessAsUser([In()] IntPtr hToken, [In()][MarshalAs(UnmanagedType.LPWStr)] string lpApplicationName, System.IntPtr lpCommandLine, [In()] IntPtr lpProcessAttributes, [In()] IntPtr lpThreadAttributes, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandles, uint dwCreationFlags, [In()] IntPtr lpEnvironment, [In()][MarshalAs(UnmanagedType.LPWStr)] string lpCurrentDirectory, [In()] ref STARTUPINFOW lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);


        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public uint nLength;
            public IntPtr lpSecurityDescriptor;
            [MarshalAs(UnmanagedType.Bool)]
            public bool bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFOW
        {
            public uint cb;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpReserved;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpDesktop;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public ushort wShowWindow;
            public ushort cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }
    }

}
