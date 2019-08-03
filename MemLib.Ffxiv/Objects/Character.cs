using System;
using System.Linq;
using MemLib.Ffxiv.Enumerations;
using MemLib.Ffxiv.Managers;
// ReSharper disable InconsistentNaming

namespace MemLib.Ffxiv.Objects {
    public class Character : GameObject {
        public uint CurrentMana => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.Mana);
        public uint MaxMana => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.Mana + 4);
        public float CurrentManaPercent => (float) CurrentMana / MaxMana * 100f;

        public uint CurrentGP => (uint) m_Process.Read<short>(BaseAddress + m_Process.Offsets.Character.GP);
        public uint MaxGP => (uint) m_Process.Read<short>(BaseAddress + m_Process.Offsets.Character.GP + 2);
        public float CurrentGPPercent => (float) CurrentGP / MaxGP * 100f;
        
        public uint CurrentCP => (uint)m_Process.Read<short>(BaseAddress + m_Process.Offsets.Character.CP);
        public uint MaxCP => (uint)m_Process.Read<short>(BaseAddress + m_Process.Offsets.Character.CP + 2);
        public float CurrentCPPercent => (float) CurrentCP / MaxCP * 100f;

        public uint ClassLevel => m_Process.Read<byte>(BaseAddress + m_Process.Offsets.Character.ClassLevel);
        public ClassJobType CurrentJob => m_Process.Read<ClassJobType>(BaseAddress + m_Process.Offsets.Character.ClassJob);

        public uint OwnerId => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.OwnerId);

        public virtual bool HasTarget {
            get {
                var id = CurrentTargetId;
                return id != 0 && id != GameObjectManager.EmptyGameObject;
            }
        }

        public virtual uint CurrentTargetId => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.TargetId);
        public Character TargetCharacter => HasTarget ? m_Process.GameObjects.GetObjectByObjectId(CurrentTargetId) as Character : null;
        public GameObject TargetGameObject => HasTarget ? m_Process.GameObjects.GetObjectByObjectId(CurrentTargetId) : null;

        public StatusFlags StatusFlags => m_Process.Read<StatusFlags>(BaseAddress + m_Process.Offsets.Character.Status);
        public bool InCombat => StatusFlags.HasFlag(StatusFlags.InCombat);
        
        public override uint CurrentHealth => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.Health);
        public override uint MaxHealth => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.Health + 4);
        public override float CurrentHealthPercent => (float) CurrentHealth / MaxHealth * 100f;

        public override uint NpcId => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.NpcId);
        public bool IsNpc => NpcId > 0u;

        private SpellCastInfo m_SpellCastInfo;
        public SpellCastInfo SpellCastInfo => m_SpellCastInfo ?? (m_SpellCastInfo = new SpellCastInfo(m_Process, BaseAddress + 0));
        public bool IsCasting => CastingSpellId > 0u;
        public uint CastingSpellId => SpellCastInfo.ActionId;

        private Auras m_Auras;
        public Auras CharacterAuras => m_Auras ?? (m_Auras = new Auras(m_Process, BaseAddress + m_Process.Offsets.Character.AuraList));
        public bool HasAuras => CharacterAuras.AuraList.Count != 0;

        public bool HasMyAura(uint auraId) {
            return CharacterAuras.Any(a => a.Id == auraId && a.CasterId == m_Process.GameObjects.LocalPlayer.ObjectId);
        }

        public bool HasAura(uint auraId) {
            return CharacterAuras.Any(a => a.Id == auraId);
        }

        public Aura GetAuraById(uint auraId) {
            return CharacterAuras.FirstOrDefault(a => a.Id == auraId);
        }

        internal Character(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) { }
    }
}