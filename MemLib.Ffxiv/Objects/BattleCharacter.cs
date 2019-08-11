using System;
using MemLib.Ffxiv.Enumerations;

namespace MemLib.Ffxiv.Objects {
    public class BattleCharacter : Character {
        public PlayerIcon Icon => Ffxiv.Memory.Read<PlayerIcon>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.Icon);
        public byte MountId => Ffxiv.Memory.Read<byte>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.MountId);
        public bool IsMounted => MountId > 0;
        //public Character Mount => GameObjectManager.GetObjectsOfType<Character>().FirstOrDefault((Character character_0) => character_0.SummonerObjectId == base.ObjectId);
        //public int ElementalLevel { get; }
        //public EurekaElement Element { get; }
        //public bool IsFate { get; }

        public World CurrentWorld => Ffxiv.Memory.Read<World>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.World);
        public World HomeWorld => Ffxiv.Memory.Read<World>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.HomeWorld);
        public bool IsCrossWorld => CurrentWorld != HomeWorld;

        public override uint NpcId => Ffxiv.Memory.Read<uint>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.BNpcNameId);

        internal BattleCharacter(IntPtr baseAddress) : base(baseAddress) { }
    }
}