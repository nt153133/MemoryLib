using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MemLib.Internals;
using MemLib.Native;

namespace MemLib.Memory {
    public class MemoryManager : IDisposable {
        private readonly RemoteProcess m_Process;
        private readonly HashSet<RemoteAllocation> m_RemoteAllocations = new HashSet<RemoteAllocation>();
        private readonly IntPtr m_RegionStart;
        private readonly IntPtr m_RegionEnd;

        public List<RemoteAllocation> RemoteAllocations => m_RemoteAllocations.ToList();
        public IEnumerable<RemoteRegion> Regions => QueryRegions(m_RegionStart, m_RegionEnd);

        public MemoryManager(RemoteProcess process) {
            m_Process = process;
            m_RegionStart = new IntPtr(0x10_000);
            m_RegionEnd = process.Is64Bit ? new IntPtr(0x7FFF_FFFE_FFFF) : new IntPtr(0x7FFE_FFFF);
        }

        private IEnumerable<RemoteRegion> QueryRegions(IntPtr start, IntPtr end) {
            return Query(m_Process.Handle, start, end).Select(region => new RemoteRegion(m_Process, region.BaseAddress));
        }

        #region Allocate

        public RemoteAllocation Allocate(int size, bool mustBeDisposed = true) {
            return Allocate(size, MemoryProtectionFlags.ExecuteReadWrite, mustBeDisposed);
        }

        public RemoteAllocation Allocate(int size, MemoryProtectionFlags protection, bool mustBeDisposed = true) {
            var memory = new RemoteAllocation(m_Process, size, protection, mustBeDisposed);
            m_RemoteAllocations.Add(memory);
            return memory;
        }

        #endregion
        #region Deallocate

        public void Deallocate(RemoteAllocation allocation) {
            if (m_RemoteAllocations.Contains(allocation))
                m_RemoteAllocations.Remove(allocation);
            if (!allocation.IsDisposed)
                allocation.Dispose();
        }

        #endregion
        #region Statics

        public static IntPtr Allocate(SafeMemoryHandle handle, int size, MemoryProtectionFlags protectionFlags = MemoryProtectionFlags.ExecuteReadWrite, MemoryAllocationFlags allocationFlags = MemoryAllocationFlags.Commit) {
            var address = IntPtr.Zero;
            var regionSize = new IntPtr(size);
            if (NativeMethods.NtAllocateVirtualMemory(handle, ref address, 0x7FFF_FFFF, ref regionSize, allocationFlags, protectionFlags) == 0)
                return address;
            return NativeMethods.VirtualAllocEx(handle, IntPtr.Zero, size, allocationFlags, protectionFlags);
        }

        public static bool Free(SafeMemoryHandle handle, IntPtr address) {
            return NativeMethods.VirtualFreeEx(handle, address, 0, MemoryReleaseFlags.Release);
        }

        public static MemoryBasicInformation Query(SafeMemoryHandle handle, IntPtr baseAddress) {
            if (NativeMethods.VirtualQueryEx(handle, baseAddress, out var mbi, MarshalType<MemoryBasicInformation>.Size) == 0)
                throw new Win32Exception();
            return mbi;
        }

        public static IEnumerable<MemoryBasicInformation> Query(SafeMemoryHandle handle, IntPtr start, IntPtr end) {
            var address = start.ToInt64();
            var limit = end.ToInt64();

            if (address >= limit)
                throw new ArgumentOutOfRangeException(nameof(start));

            long ret;
            var mbiSize = MarshalType<MemoryBasicInformation>.Size;
            do {
                ret = NativeMethods.VirtualQueryEx(handle, new IntPtr(address), out var mbi, mbiSize);
                address += mbi.RegionSize.ToInt64();
                if (mbi.State != MemoryStateFlags.Free && ret != 0)
                    yield return mbi;
            } while (address < limit && ret != 0);
        }

        #endregion
        #region IDisposable

        void IDisposable.Dispose() {
            foreach (var allocation in m_RemoteAllocations.Where(r => r.MustBeDisposed).ToList()) {
                allocation.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        ~MemoryManager() {
            ((IDisposable) this).Dispose();
        }

        #endregion
    }
}