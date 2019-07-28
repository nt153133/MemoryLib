using System;
using System.Runtime.InteropServices;
using System.Text;

namespace MemLib.Native {
    internal static class NativeMethods {
        #region Kernel32

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeMemoryHandle OpenProcess(ProcessAccessFlags dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(SafeMemoryHandle hObject);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(SafeMemoryHandle hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(SafeMemoryHandle hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);

        
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(SafeMemoryHandle hProcess, IntPtr lpAddress, long dwSize, MemoryAllocationFlags flAllocationType, MemoryProtectionFlags flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualFreeEx(SafeMemoryHandle hProcess, IntPtr lpAddress, long dwSize, MemoryReleaseFlags dwFreeType);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx(SafeMemoryHandle hProcess, IntPtr lpAddress, long dwSize, MemoryProtectionFlags flNewProtect, out MemoryProtectionFlags lpflOldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern long VirtualQueryEx(SafeMemoryHandle hProcess, IntPtr lpAddress, out MemoryBasicInformation lpBuffer, int dwLength);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);

        [DllImport("kernel32.dll", SetLastError = false)]
        public static extern void GetNativeSystemInfo(out SystemInfo lpSystemInfo);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetThreadId(SafeMemoryHandle hThread);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeMemoryHandle CreateRemoteThread(SafeMemoryHandle hProcess, IntPtr lpThreadAttributes, ulong dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, ThreadCreationFlags dwCreationFlags, out uint lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeMemoryHandle OpenThread(ThreadAccessFlags dwDesiredAccess, bool bInheritHandle, int dwThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint ResumeThread(SafeMemoryHandle hThread);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint SuspendThread(SafeMemoryHandle hThread);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool TerminateThread(SafeMemoryHandle hThread, int dwExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetExitCodeThread(SafeMemoryHandle hThread, out IntPtr lpExitCode);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern WaitValues WaitForSingleObject(SafeMemoryHandle hHandle, uint dwMilliseconds);

        #endregion

        #region NtDll

        [DllImport("ntdll.dll", SetLastError = false)]
        public static extern uint NtAllocateVirtualMemory(SafeMemoryHandle processHandle, [In, Out] ref IntPtr baseAddress, uint zeroBits, [In, Out] ref IntPtr regionSize, MemoryAllocationFlags allocationType, MemoryProtectionFlags protect);

        [DllImport("ntdll.dll")]
        public static extern uint NtQueryInformationThread(SafeMemoryHandle threadHandle, uint infoclass, ref ThreadBasicInformation threadinfo, int length, IntPtr bytesread);

        #endregion

        #region dbghelp

        [DllImport("dbghelp.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnDecorateSymbolName(string name, StringBuilder outputString, int maxStringLength, UnDecorateFlags flags);

        #endregion
    }
}