using System;
// ReSharper disable InconsistentNaming

namespace MemLib.Ffxiv.Objects {
    public class RemoteObject : IEquatable<RemoteObject> {
        public IntPtr BaseAddress { get; private set; }
        public IntPtr vTable => Ffxiv.Memory.Read<IntPtr>(BaseAddress);

        public virtual bool IsValid => BaseAddress != IntPtr.Zero;

        protected RemoteObject(IntPtr baseAddress) {
            BaseAddress = baseAddress;
        }

        internal void UpdatePointer(IntPtr address) {
            BaseAddress = address;
        }

        #region Overrides of Object

        public override string ToString() {
            return $"0x{BaseAddress.ToInt64():X8}";
        }

        #endregion

        #region Equality members

        public bool Equals(RemoteObject other) {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || BaseAddress.Equals(other.BaseAddress);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((RemoteObject) obj);
        }

        public override int GetHashCode() {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return BaseAddress.GetHashCode();
        }

        public static bool operator ==(RemoteObject left, RemoteObject right) {
            return Equals(left, right);
        }

        public static bool operator !=(RemoteObject left, RemoteObject right) {
            return !Equals(left, right);
        }

        #endregion
    }
}