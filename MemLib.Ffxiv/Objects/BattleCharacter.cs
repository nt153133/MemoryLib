using System;
using MemLib.Ffxiv.Enums;

namespace MemLib.Ffxiv.Objects {
    public class BattleCharacter : Character {
        public PlayerIcon Icon => (PlayerIcon) m_Process.Read<byte>(BaseAddress + m_Process.Offsets.Character.Icon);
        public int ElementalLevel { get; }
        public EurekaElement Element{ get; }
        public bool IsMounted { get; }
        public Character Mount { get; }
        public bool IsFate { get; }

        public World CurrentWorld => (World)m_Process.Read<byte>(BaseAddress + m_Process.Offsets.Character.World);
        public World HomeWorld => (World)m_Process.Read<byte>(BaseAddress + m_Process.Offsets.Character.HomeWorld);
        public bool IsCrossWorld => CurrentWorld != HomeWorld;

        #region Overrides of Character

        public override uint NpcId { get; }

        #endregion

        public BattleCharacter(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) { }
    }
}