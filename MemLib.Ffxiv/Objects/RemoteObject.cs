using System;
// ReSharper disable InconsistentNaming

namespace MemLib.Ffxiv.Objects {
    public class RemoteObject : IEquatable<RemoteObject> {
        protected readonly FfxivProcess m_Process;
        public IntPtr BaseAddress { get; internal set; }
        public IntPtr vTable => m_Process.Read<IntPtr>(BaseAddress);

        public virtual bool IsValid => BaseAddress != IntPtr.Zero;

        internal RemoteObject(FfxivProcess process, IntPtr baseAddress) {
            m_Process = process;
            BaseAddress = baseAddress;
        }

        #region Overrides of Object

        public override string ToString() {
            return $"0x{BaseAddress.ToInt64():X}";
        }

        #endregion

        #region Equality members

        public bool Equals(RemoteObject other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return m_Process.Equals(other.m_Process) && BaseAddress.Equals(other.BaseAddress);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((RemoteObject) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (m_Process.GetHashCode() * 397) ^ BaseAddress.GetHashCode();
            }
        }

        #endregion
    }
}