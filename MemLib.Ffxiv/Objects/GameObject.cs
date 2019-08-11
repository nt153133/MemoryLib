using System;
using System.Numerics;
using MemLib.Ffxiv.Enumerations;
using MemLib.Ffxiv.Managers;

namespace MemLib.Ffxiv.Objects {
    public class GameObject : RemoteObject, IEquatable<GameObject> {
        private string m_Name;
        public virtual string Name {
            get {
                if (!string.IsNullOrEmpty(m_Name))
                    return m_Name;
                return m_Name = Ffxiv.Memory.ReadString(BaseAddress + Ffxiv.Offsets.CharacterOffsets.Name, 64);
            }
        }
        public GameObjectType Type => Ffxiv.Memory.Read<GameObjectType>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.ObjectType);

        public virtual Vector3 Location => Ffxiv.Memory.Read<Vector3>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.Location);
        public float X => Ffxiv.Memory.Read<float>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.Location);
        public float Y => Ffxiv.Memory.Read<float>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.Location + 4);
        public float Z => Ffxiv.Memory.Read<float>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.Location + 8);
        public virtual uint CurrentHealth => 0u;
        public virtual uint MaxHealth => 0u;
        public virtual float CurrentHealthPercent => 0f;
        public virtual uint NpcId => 0u;
        public override bool IsValid => base.IsValid && GetObjectId() == ObjectId;
        public bool IsMe => ObjectId == Ffxiv.Objects.LocalPlayer.ObjectId;
        public uint ObjectId { get; }

        internal GameObject(IntPtr baseAddress) : base(baseAddress) {
            ObjectId = GetObjectId();
        }

        private uint GetObjectId() {
            var id = Ffxiv.Memory.Read<uint>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.ObjectId);
            if (id == GameObjectManager.EmptyGameObject || id == 0u)
                id = Ffxiv.Memory.Read<uint>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.ObjectId2);
            if (id == GameObjectManager.EmptyGameObject || id == 0u)
                id = Ffxiv.Memory.Read<uint>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.ObjectId3);
            return id;
        }

        public float Distance() {
            return Distance(Ffxiv.Objects.LocalPlayer);
        }

        public float Distance(Vector3 vector) {
            return Vector3.Distance(Location, vector);
        }

        public float Distance(GameObject gameObject) {
            return Vector3.Distance(Location, gameObject.Location);
        }

        public float DistanceSqr() {
            return Vector3.DistanceSquared(Location, Ffxiv.Objects.LocalPlayer.Location);
        }

        public float DistanceSqr(Vector3 vector) {
            return Vector3.DistanceSquared(Location, vector);
        }

        #region Overrides of RemoteObject

        public override string ToString() {
            return $"0x{BaseAddress.ToInt64():X8}:{Name}";
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
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GameObject) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (base.GetHashCode() * 397) ^ (int) ObjectId;
            }
        }

        public static bool operator ==(GameObject left, GameObject right) {
            return Equals(left, right);
        }

        public static bool operator !=(GameObject left, GameObject right) {
            return !Equals(left, right);
        }

        #endregion
    }
}