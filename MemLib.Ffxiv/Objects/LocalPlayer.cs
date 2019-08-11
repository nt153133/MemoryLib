using System;
using System.Collections.Generic;
using MemLib.Ffxiv.Enumerations;

namespace MemLib.Ffxiv.Objects {
    public sealed class LocalPlayer : BattleCharacter {
        public Dictionary<ClassJobType, ushort> Levels {
            get {
                var addr = Ffxiv.Offsets.PlayerInfo + Ffxiv.Offsets.PlayerOffsets.ClassLevelTable;
                var levels = Ffxiv.Memory.Read<ushort>(addr, ClassJobTypesList.Length);
                var result = new Dictionary<ClassJobType, ushort>(ClassJobTypesList.Length);
                for (var i = 0; i < ClassJobTypesList.Length; i++)
                    result.Add(ClassJobTypesList[i], levels[i]);
                return result;
            }
        }
        public Dictionary<ClassJobType, uint> Experience {
            get {
                var addr = Ffxiv.Offsets.PlayerInfo + Ffxiv.Offsets.PlayerOffsets.ClassExpTable;
                var exp = Ffxiv.Memory.Read<uint>(addr, ClassJobTypesList.Length);
                var result = new Dictionary<ClassJobType, uint>(ClassJobTypesList.Length);
                for (var i = 0; i < ClassJobTypesList.Length; i++)
                    result.Add(ClassJobTypesList[i], exp[i]);
                return result;
            }
        }
        public override bool IsValid => BaseAddress != IntPtr.Zero;
        public BattleCharacter Pet => Ffxiv.Objects.CurrentPet;
        public PlayerStats Stats => Ffxiv.Memory.Read<PlayerStats>(Ffxiv.Offsets.PlayerInfo + Ffxiv.Offsets.PlayerOffsets.StatsTable);

        public GameObject CurrentTarget => Ffxiv.Objects.Target;
        public uint CurrentTargetObjId => HasTarget ? CurrentTarget.ObjectId : 0u;

        public override bool HasTarget {
            get {
                var addr = Ffxiv.Offsets.Targeting + Ffxiv.Offsets.TargetOffsets.CurrentTargetId;
                return Ffxiv.Memory.Read<uint>(addr, out var targetId) && targetId != 0 && targetId != 0xE0000000;
            }
        }

        internal LocalPlayer(IntPtr baseAddress) : base(baseAddress) { }

        private static readonly ClassJobType[] ClassJobTypesList = {
            ClassJobType.Pugilist,
            ClassJobType.Gladiator,
            ClassJobType.Marauder,
            ClassJobType.Archer,
            ClassJobType.Lancer,
            ClassJobType.Thaumaturge,
            ClassJobType.Conjurer,
            ClassJobType.Carpenter,
            ClassJobType.Blacksmith,
            ClassJobType.Armorer,
            ClassJobType.Goldsmith,
            ClassJobType.Leatherworker,
            ClassJobType.Weaver,
            ClassJobType.Alchemist,
            ClassJobType.Culinarian,
            ClassJobType.Miner,
            ClassJobType.Botanist,
            ClassJobType.Fisher,
            ClassJobType.Arcanist,
            ClassJobType.Rogue,
            ClassJobType.Machinist,
            ClassJobType.DarkKnight,
            ClassJobType.Astrologian,
            ClassJobType.Samurai,
            ClassJobType.RedMage,
            ClassJobType.BlueMage,
            ClassJobType.Gunbreaker,
            ClassJobType.Dancer
        };
    }
}