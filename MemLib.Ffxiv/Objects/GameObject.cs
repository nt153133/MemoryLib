using System;
using MemLib.Ffxiv.Enumerations;
using MemLib.Ffxiv.Managers;
using MemLib.Ffxiv.Structures;

namespace MemLib.Ffxiv.Objects {
    public class GameObject : RemoteObject, IEquatable<GameObject> {
        public uint ObjectId {
            get {
                var id = m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.ObjectId);
                if (id == GameObjectManager.EmptyGameObject)
                    id = m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.ObjectId2);
                return id;
            }
        }
        public GameObjectType Type => m_Process.Read<GameObjectType>(BaseAddress + m_Process.Offsets.Character.ObjectType);
        public virtual string Name => m_Process.ReadString(BaseAddress + m_Process.Offsets.Character.Name, 64);
        public virtual uint NpcId => 0u;
        public override bool IsValid => base.IsValid && ObjectId != 0u;
        public bool IsMe => ObjectId == m_Process.GameObjects.LocalPlayer.ObjectId;
        public virtual Vector3 Location => m_Process.Read<Vector3>(BaseAddress + m_Process.Offsets.Character.Location);
        public float X => m_Process.Read<float>(BaseAddress + m_Process.Offsets.Character.Location);
        public float Y => m_Process.Read<float>(BaseAddress + m_Process.Offsets.Character.Location + 4);
        public float Z => m_Process.Read<float>(BaseAddress + m_Process.Offsets.Character.Location + 8);
        public virtual uint CurrentHealth => 0u;
        public virtual uint MaxHealth => 0u;
        public virtual float CurrentHealthPercent => 0f;

        internal GameObject(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) { }

        public float Distance() {
            return Distance(m_Process.GameObjects.LocalPlayer);
        }

        public float Distance(Vector3 vector) {
            return Location.Distance(vector);
        }

        public float Distance(GameObject gameObject) {
            return Location.Distance(gameObject.Location);
        }

        public float DistanceSqr() {
            return Location.DistanceSquared(m_Process.GameObjects.LocalPlayer.Location);
        }

        public float DistanceSqr(Vector3 vector) {
            return Location.DistanceSquared(vector);
        }

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