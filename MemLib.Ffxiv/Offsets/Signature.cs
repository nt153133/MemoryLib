using System;
using System.Linq;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace MemLib.Ffxiv.Offsets {
    public class Signature : IEquatable<Signature> {
        public string Key;
        public string Value;
        public int Offset;
        public int[] PointerPath;

        #region Overrides of Object

        public override string ToString() {
            var str = $"Key:{Key} Value:{Value} Offset:{Offset}";
            if (PointerPath != null && PointerPath.Length > 0)
                str += $" Path:[{string.Join(", ", PointerPath.Select(v => $"0x{v:X2}"))}]";
            return str;
        }

        #endregion

        #region Equality members

        public bool Equals(Signature other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Key == other.Key;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Signature) obj);
        }

        public override int GetHashCode() {
            return Key != null ? Key.GetHashCode() : 0;
        }

        public static bool operator ==(Signature left, Signature right) {
            return Equals(left, right);
        }

        public static bool operator !=(Signature left, Signature right) {
            return !Equals(left, right);
        }

        #endregion
    }
}