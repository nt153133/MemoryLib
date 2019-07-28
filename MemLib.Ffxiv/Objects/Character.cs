using System;
using MemLib.Ffxiv.Enums;
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

        public uint Level => m_Process.Read<byte>(BaseAddress + m_Process.Offsets.Character.ClassLevel);
        public ClassJobType ClassJob => (ClassJobType)m_Process.Read<byte>(BaseAddress + m_Process.Offsets.Character.ClassJob);

        public uint OwnerId => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.OwnerId);

        public virtual bool HasTarget => CurrentTargetId != 0 && CurrentTargetId != GameObjectManager.EmptyGameObject;
        public uint CurrentTargetId { get; }
        public StatusFlags StatusFlags { get; }
        public bool InCombat => StatusFlags.HasFlag(StatusFlags.InCombat);
        public bool IsNpc => NpcId > 0u;
        public Character TargetCharacter { get; }
        public GameObject TargetGameObject { get; }
        
        #region Overrides of GameObject

        public override uint CurrentHealth => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.Health);
        public override uint MaxHealth => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.Health + 4);
        public override float CurrentHealthPercent => (float) CurrentHealth / MaxHealth * 100f;

        public override uint NpcId => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.NpcId);

        #endregion

        public Character(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) { }
    }
}