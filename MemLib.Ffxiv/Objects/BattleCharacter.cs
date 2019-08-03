using System;
using MemLib.Ffxiv.Enumerations;

namespace MemLib.Ffxiv.Objects {
    public class BattleCharacter : Character {
        public PlayerIcon Icon => m_Process.Read<PlayerIcon>(BaseAddress + m_Process.Offsets.Character.Icon);
        public byte MountId => m_Process.Read<byte>(BaseAddress + m_Process.Offsets.Character.MountId);
        public bool IsMounted => MountId > 0;
        //public Character Mount => GameObjectManager.GetObjectsOfType<Character>().FirstOrDefault((Character character_0) => character_0.SummonerObjectId == base.ObjectId);
        //public int ElementalLevel { get; }
        //public EurekaElement Element { get; }
        //public bool IsFate { get; }

        public World CurrentWorld => m_Process.Read<World>(BaseAddress + m_Process.Offsets.Character.World);
        public World HomeWorld => m_Process.Read<World>(BaseAddress + m_Process.Offsets.Character.HomeWorld);
        public bool IsCrossWorld => CurrentWorld != HomeWorld;

        public override uint NpcId => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.BNpcNameId);

        internal BattleCharacter(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) { }
    }
}