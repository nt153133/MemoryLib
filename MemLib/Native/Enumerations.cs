using System;

namespace MemLib.Native {
    #region Flags

    [Flags]
    public enum ProcessAccessFlags {
        AllAccess = 0x001F0FFF,
        CreateProcess = 0x0080,
        CreateThread = 0x0002,
        DupHandle = 0x0040,
        QueryInformation = 0x0400,
        QueryLimitedInformation = 0x1000,
        SetInformation = 0x0200,
        SetQuota = 0x0100,
        SuspendResume = 0x0800,
        Terminate = 0x0001,
        VmOperation = 0x0008,
        VmRead = 0x0010,
        VmWrite = 0x0020,
        Synchronize = 0x00100000
    }

    [Flags]
    public enum MemoryAllocationFlags {
        Commit = 0x00001000,
        Reserve = 0x00002000,
        Reset = 0x00080000,
        ResetUndo = 0x1000000,
        LargePages = 0x20000000,
        Physical = 0x00400000,
        TopDown = 0x00100000
    }

    [Flags]
    public enum MemoryProtectionFlags {
        ZeroAccess = 0x0,
        Execute = 0x10,
        ExecuteRead = 0x20,
        ExecuteReadWrite = 0x40,
        ExecuteWriteCopy = 0x80,
        NoAccess = 0x01,
        ReadOnly = 0x02,
        ReadWrite = 0x04,
        WriteCopy = 0x08,
        Guard = 0x100,
        NoCache = 0x200,
        WriteCombine = 0x400
    }

    [Flags]
    public enum MemoryReleaseFlags {
        Decommit = 0x4000,
        Release = 0x8000
    }

    [Flags]
    public enum MemoryStateFlags {
        Commit = 0x1000,
        Free = 0x10000,
        Reserve = 0x2000
    }

    [Flags]
    public enum MemoryTypeFlags {
        None = 0x0,
        Image = 0x1000000,
        Mapped = 0x40000,
        Private = 0x20000
    }

    [Flags]
    public enum UnDecorateFlags : uint {
        Complete = 0x0000,
        NoLeadingUnderscores = 0x0001,
        NoMsKeywords = 0x0002,
        NoFunctionReturns = 0x0004,
        NoAllocationModel = 0x0008,
        NoAllocationLanguage = 0x0010,
        NoMsThisType = 0x0020,
        NoCvThisType = 0x0040,
        NoThisType = 0x0060,
        NoAccessSpecifiers = 0x0080,
        NoThrowSignatures = 0x0100,
        NoMemberType = 0x0200,
        NoReturnUdtModel = 0x0400,

        //_32BitDecode = 0x0800,
        NameOnly = 0x1000,
        NoArguments = 0x2000,
        NoSpecialSyms = 0x4000
    }

    [Flags]
    public enum ThreadAccessFlags {
        Synchronize = 0x00100000,
        AllAccess = 0x001F0FFF,
        DirectImpersonation = 0x0200,
        GetContext = 0x0008,
        Impersonate = 0x0100,
        QueryInformation = 0x0040,
        QueryLimitedInformation = 0x0800,
        SetContext = 0x0010,
        SetInformation = 0x0020,
        SetLimitedInformation = 0x0400,
        SetThreadToken = 0x0080,
        SuspendResume = 0x0002,
        Terminate = 0x0001
    }

    [Flags]
    public enum ThreadCreationFlags {
        Run = 0x0,
        Suspended = 0x04,
        StackSizeParamIsAReservation = 0x10000
    }

    #endregion

    #region Enums

    public enum WaitValues : uint {
        Abandoned = 0x80,
        Signaled = 0x0,
        Timeout = 0x102,
        Failed = 0xFFFFFFFF
    }

    #endregion
}