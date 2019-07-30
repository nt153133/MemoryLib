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

        public virtual bool HasTarget {
            get {
                var id = CurrentTargetId;
                return id != 0 && id != GameObjectManager.EmptyGameObject;
            }
        }

        public StatusFlags StatusFlags => (StatusFlags)m_Process.Read<byte>(BaseAddress + m_Process.Offsets.Character.Status);
        public bool InCombat => StatusFlags.HasFlag(StatusFlags.InCombat);
        public bool IsNpc => NpcId > 0u;

        public virtual uint CurrentTargetId => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.TargetId);
        public virtual Character TargetCharacter => m_Process.GameObjects.GetObjectByObjectId<Character>(CurrentTargetId);
        public virtual GameObject TargetGameObject => m_Process.GameObjects.GetObjectByObjectId(CurrentTargetId);

        public override uint CurrentHealth => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.Health);
        public override uint MaxHealth => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.Health + 4);
        public override float CurrentHealthPercent => (float) CurrentHealth / MaxHealth * 100f;

        public override uint NpcId => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.NpcId);

        internal Character(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) { }
    }
}