﻿using System;
using System.ComponentModel;
using MemLib.Native;

namespace MemLib.Memory {
    public sealed class MemoryProtection : IDisposable {
        private readonly RemoteProcess m_Process;
        public MemoryProtectionFlags NewProtection { get; }
        public MemoryProtectionFlags OldProtection { get; }
        public bool MustBeDisposed { get; set; }
        public IntPtr BaseAddress { get; }
        public long Size { get; }

        internal MemoryProtection(RemoteProcess process, IntPtr baseAddress, long size, MemoryProtectionFlags newProtect = MemoryProtectionFlags.ExecuteReadWrite, bool mustBeDisposed = true) {
            m_Process = process;
            MustBeDisposed = mustBeDisposed;
            BaseAddress = baseAddress;
            Size = size;
            NewProtection = newProtect;
            if(!NativeMethods.VirtualProtectEx(m_Process.Handle, baseAddress, size, newProtect, out var oldProtect))
                throw new Win32Exception();
            OldProtection = oldProtect;
        }

        public override string ToString() => $"BaseAddress=0x{BaseAddress.ToInt64():X} New={NewProtection} Old={OldProtection}";

        #region IDisposable

        public void Dispose() {
            NativeMethods.VirtualProtectEx(m_Process.Handle, BaseAddress, Size, OldProtection, out _);
            GC.SuppressFinalize(this);
        }

        ~MemoryProtection() {
            if (MustBeDisposed)
                Dispose();
        }

        #endregion
    }
}