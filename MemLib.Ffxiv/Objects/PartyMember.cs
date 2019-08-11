using System;
using MemLib.Ffxiv.Enumerations;

namespace MemLib.Ffxiv.Objects {
    public class PartyMember : RemoteObject {
        protected internal int Index;
        public GameObject GameObject => ObjectId > 0u ? Ffxiv.Objects.GetObjectByObjectId(ObjectId) : null;
        public BattleCharacter BattleCharacter => GameObject as BattleCharacter;
        public bool IsInObjectManager => GameObject != null;
        public virtual bool IsMe => Ffxiv.Objects.LocalPlayer.ObjectId == ObjectId;
        public override bool IsValid => base.IsValid && ObjectId > 0u && ObjectId != 0xE0000000;
        public virtual ClassJobType ClassJob => Ffxiv.Memory.Read<ClassJobType>(BaseAddress + Ffxiv.Offsets.PartyOffsets.ClassJob);
        public virtual uint Level => Ffxiv.Memory.Read<byte>(BaseAddress + Ffxiv.Offsets.PartyOffsets.Level);
        public virtual uint CurrentHealth => Ffxiv.Memory.Read<uint>(BaseAddress + Ffxiv.Offsets.PartyOffsets.Health);
        public virtual uint MaxHealth => Ffxiv.Memory.Read<uint>(BaseAddress + Ffxiv.Offsets.PartyOffsets.Health + 4);
        public virtual uint CurrentMana => Ffxiv.Memory.Read<ushort>(BaseAddress + Ffxiv.Offsets.PartyOffsets.Mana);
        public virtual uint MaxMana => Ffxiv.Memory.Read<ushort>(BaseAddress + Ffxiv.Offsets.PartyOffsets.Mana + 2);
        public virtual string Name => Ffxiv.Memory.ReadString(BaseAddress + Ffxiv.Offsets.PartyOffsets.Name, 64);
        public virtual uint ObjectId => Ffxiv.Memory.Read<uint>(BaseAddress + Ffxiv.Offsets.PartyOffsets.ObjectId);
        public virtual ulong PartyObjectId => 0ul;
        public virtual bool IsPartyLeader { get; }
        public virtual bool IsXRealm => false;

        internal PartyMember(IntPtr baseAddress, int index) : base(baseAddress) {
            Index = index;
        }

        #region Overrides of RemoteObject

        public override string ToString() {
            return Helper.ObjectToString(this, "BaseAddress", "vTable");
        }

        #endregion
    }
}