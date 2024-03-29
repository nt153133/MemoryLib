﻿using System;
using MemLib.Native;

namespace MemLib.Memory {
    public class RemoteRegion : RemotePointer, IEquatable<RemoteRegion> {
        public MemoryBasicInformation Information => MemoryManager.Query(m_Process.Handle, BaseAddress);
        public override bool IsValid => base.IsValid && Information.State != MemoryStateFlags.Free;

        public RemoteRegion(RemoteProcess process, IntPtr address) : base(process, address) { }

        public void Release() {
            MemoryManager.Free(m_Process.Handle, BaseAddress);
            BaseAddress = IntPtr.Zero;
        }

        public MemoryProtection ChangeProtection(MemoryProtectionFlags protection = MemoryProtectionFlags.ExecuteReadWrite, bool mustBeDisposed = true) {
            return new MemoryProtection(m_Process, BaseAddress, Information.RegionSize.ToInt64(), protection, mustBeDisposed);
        }

        public override string ToString() {
            var info = Information;
            if (!base.IsValid && info.State == MemoryStateFlags.Free)
                return "Invalid Region";
            return $"{base.ToString()} Size=0x{info.RegionSize.ToInt64():X} Protection={info.Protect}";
        }

        #region Equality members

        public bool Equals(RemoteRegion other) {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return BaseAddress.Equals(other.BaseAddress) &&
                   m_Process.Equals(other.m_Process) &&
                   Information.RegionSize.Equals(other.Information.RegionSize);
        }

        public override bool Equals(object obj) {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((RemoteRegion) obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode() ^ Information.RegionSize.GetHashCode();
        }

        #endregion
    }
}