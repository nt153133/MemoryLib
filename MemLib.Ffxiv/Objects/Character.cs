using System;
using System.Linq;
using MemLib.Ffxiv.Enumerations;
using MemLib.Ffxiv.Managers;
// ReSharper disable InconsistentNaming

namespace MemLib.Ffxiv.Objects {
    public class Character : GameObject {
        public uint CurrentMana => Ffxiv.Memory.Read<uint>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.Mana);
        public uint MaxMana => Ffxiv.Memory.Read<uint>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.Mana + 4);
        public float CurrentManaPercent => (float) CurrentMana / MaxMana * 100f;

        public uint CurrentGP => (uint)Ffxiv.Memory.Read<short>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.GP);
        public uint MaxGP => (uint)Ffxiv.Memory.Read<short>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.GP + 2);
        public float CurrentGPPercent => (float) CurrentGP / MaxGP * 100f;
        
        public uint CurrentCP => (uint)Ffxiv.Memory.Read<short>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.CP);
        public uint MaxCP => (uint)Ffxiv.Memory.Read<short>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.CP + 2);
        public float CurrentCPPercent => (float) CurrentCP / MaxCP * 100f;

        public uint ClassLevel => Ffxiv.Memory.Read<byte>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.ClassLevel);
        public ClassJobType CurrentJob => Ffxiv.Memory.Read<ClassJobType>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.ClassJob);

        public uint OwnerId => Ffxiv.Memory.Read<uint>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.OwnerId);

        public virtual bool HasTarget {
            get {
                var id = CurrentTargetId;
                return id != 0 && id != GameObjectManager.EmptyGameObject;
            }
        }

        public virtual uint CurrentTargetId => Ffxiv.Memory.Read<uint>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.TargetId);
        public Character TargetCharacter => HasTarget ? Ffxiv.Objects.GetObjectByObjectId(CurrentTargetId) as Character : null;
        public GameObject TargetGameObject => HasTarget ? Ffxiv.Objects.GetObjectByObjectId(CurrentTargetId) : null;

        public StatusFlags StatusFlags => Ffxiv.Memory.Read<StatusFlags>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.Status);
        public bool InCombat => StatusFlags.HasFlag(StatusFlags.InCombat);
        
        public override uint CurrentHealth => Ffxiv.Memory.Read<uint>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.Health);
        public override uint MaxHealth => Ffxiv.Memory.Read<uint>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.Health + 4);
        public override float CurrentHealthPercent => (float) CurrentHealth / MaxHealth * 100f;

        public override uint NpcId => Ffxiv.Memory.Read<uint>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.NpcId);
        public bool IsNpc => NpcId > 0u;

        private SpellCastInfo m_SpellCastInfo;
        public SpellCastInfo SpellCastInfo => m_SpellCastInfo ?? (m_SpellCastInfo = new SpellCastInfo(BaseAddress + 0));
        public bool IsCasting => CastingSpellId > 0u;
        public uint CastingSpellId => SpellCastInfo.ActionId;

        private Auras m_Auras;
        public Auras CharacterAuras => m_Auras ?? (m_Auras = new Auras(BaseAddress + Ffxiv.Offsets.CharacterOffsets.AuraList));
        public bool HasAuras => CharacterAuras.AuraList.Count != 0;

        internal Character(IntPtr baseAddress) : base(baseAddress) { }

        public bool HasMyAura(uint auraId) {
            return CharacterAuras.Any(a => a.Id == auraId && a.CasterId == Ffxiv.Objects.LocalPlayer.ObjectId);
        }

        public bool HasAura(uint auraId) {
            return CharacterAuras.Any(a => a.Id == auraId);
        }

        public Aura GetAuraById(uint auraId) {
            return CharacterAuras.FirstOrDefault(a => a.Id == auraId);
        }
    }
}