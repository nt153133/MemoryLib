using System;
using System.Collections.Generic;
using MemLib.Ffxiv.Enumerations;
using MemLib.Ffxiv.Structures;

namespace MemLib.Ffxiv.Objects {
    public sealed class LocalPlayer : BattleCharacter {
        public Dictionary<ClassJobType, ushort> Levels {
            get {
                var addr = m_Process.Offsets.PlayerInfoPtr + m_Process.Offsets.PlayerInfo.ClassLevelTable;
                var levels = m_Process.Read<ushort>(addr, ClassJobTypesList.Length);
                var result = new Dictionary<ClassJobType, ushort>(ClassJobTypesList.Length);
                for(var i = 0; i < ClassJobTypesList.Length; i++)
                    result.Add(ClassJobTypesList[i], levels[i]);
                return result;
            }
        }
        public Dictionary<ClassJobType, uint> Experience {
            get {
                var addr = m_Process.Offsets.PlayerInfoPtr + m_Process.Offsets.PlayerInfo.ClassExpTable;
                var exp = m_Process.Read<uint>(addr, ClassJobTypesList.Length);
                var result = new Dictionary<ClassJobType, uint>(ClassJobTypesList.Length);
                for (var i = 0; i < ClassJobTypesList.Length; i++)
                    result.Add(ClassJobTypesList[i], exp[i]);
                return result;
            }
        }
        public override bool IsValid => BaseAddress != IntPtr.Zero;
        public BattleCharacter Pet => m_Process.GameObjects.CurrentPet;
        public Stats Stats => m_Process.Read<Stats>(m_Process.Offsets.PlayerInfoPtr + m_Process.Offsets.PlayerInfo.StatsTable);
        //public new Character Mount => null;

        public GameObject CurrentTarget => m_Process.GameObjects.Target;
        public uint CurrentTargetObjId => HasTarget ? CurrentTarget.ObjectId : 0u;

        public override bool HasTarget {
            get {
                var addr = m_Process.Offsets.TargetingPtr + m_Process.Offsets.Target.CurrentTargetId;
                return m_Process.Read<uint>(addr, out var targetId) && targetId != 0 && targetId != 0xE0000000;
            }
        }

        internal LocalPlayer(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) { }

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