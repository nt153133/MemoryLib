using System;
using System.Numerics;
using MemLib.Ffxiv.Enums;

namespace MemLib.Ffxiv.Objects {
    public class GameObject : RemoteObject, IEquatable<GameObject> {
        public string Name => m_Process.ReadString(BaseAddress + m_Process.Offsets.Character.Name, 64);
        public uint ObjectId => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.ObjectId);
        public virtual Vector3 Location => m_Process.Read<Vector3>(BaseAddress + m_Process.Offsets.Character.Location);
        public virtual uint NpcId => 0u;
        public virtual uint CurrentHealth => 0u;
        public virtual uint MaxHealth => 0u;
        public virtual float CurrentHealthPercent => 0f;
        public GameObjectType Type => (GameObjectType) m_Process.Read<byte>(BaseAddress + m_Process.Offsets.Character.ObjectType);

        public override bool IsValid => base.IsValid && ObjectId != 0u;
        public bool IsMe => ObjectId == m_Process.GameObjects.LocalPlayer.ObjectId;

        internal GameObject(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) { }
        
        #region Overrides of RemoteObject

        public override string ToString() {
            return $"{base.ToString()} - {Name.PadRight(25)}[{Type}]";
        }

        #endregion

        #region Equality members

        public bool Equals(GameObject other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && ObjectId == other.ObjectId;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((GameObject) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (base.GetHashCode() * 397) ^ (int) ObjectId;
            }
        }

        #endregion
    }
}