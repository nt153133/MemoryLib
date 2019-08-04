using System;
using MemLib.Ffxiv.Enumerations;
using MemLib.Ffxiv.Managers;
using MemLib.Ffxiv.Structures;

namespace MemLib.Ffxiv.Objects {
    public class GameObject : RemoteObject, IEquatable<GameObject> {
        public GameObjectType Type => m_Process.Read<GameObjectType>(BaseAddress + m_Process.Offsets.Character.ObjectType);
        public virtual string Name => m_Process.ReadString(BaseAddress + m_Process.Offsets.Character.Name, 64);

        public virtual Vector3 Location => m_Process.Read<Vector3>(BaseAddress + m_Process.Offsets.Character.Location);
        public float X => m_Process.Read<float>(BaseAddress + m_Process.Offsets.Character.Location);
        public float Y => m_Process.Read<float>(BaseAddress + m_Process.Offsets.Character.Location + 4);
        public float Z => m_Process.Read<float>(BaseAddress + m_Process.Offsets.Character.Location + 8);
        public virtual uint CurrentHealth => 0u;
        public virtual uint MaxHealth => 0u;
        public virtual float CurrentHealthPercent => 0f;
        public virtual uint NpcId => 0u;
        public override bool IsValid => base.IsValid && GetObjectId() == ObjectId;
        public bool IsMe => ObjectId == m_Process.GameObjects.LocalPlayer.ObjectId;
        public uint ObjectId { get; }

        internal GameObject(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) {
            ObjectId = GetObjectId();
        }

        private uint GetObjectId() {
            var id = m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.ObjectId);
            if (id == GameObjectManager.EmptyGameObject || id == 0u)
                id = m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.ObjectId2);
            if (id == GameObjectManager.EmptyGameObject || id == 0u)
                id = m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.ObjectId3);
            return id;
        }

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
            return $"{Name}:0x{BaseAddress.ToInt64():X}";
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