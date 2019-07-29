using System;
using System.Collections.Generic;
using MemLib.Ffxiv.Enums;

namespace MemLib.Ffxiv.Objects {
    public sealed class LocalPlayer : BattleCharacter {
        public BattleCharacter Pet => m_Process.GameObjects.CurrentPet;
        public Stats Stats => m_Process.Read<Stats>(m_Process.Offsets.PlayerInfoPtr + m_Process.Offsets.PlayerInfo.StatsTable);

        public Dictionary<ClassJobType, ushort> Levels {
            get {
                var addr = m_Process.Offsets.PlayerInfoPtr + m_Process.Offsets.PlayerInfo.ClassLevelTable;
                var levels = m_Process.Read<ushort>(addr, m_ClassJobTypes.Length);
                var result = new Dictionary<ClassJobType, ushort>(m_ClassJobTypes.Length);
                for(var i = 0; i < m_ClassJobTypes.Length; i++)
                    result.Add(m_ClassJobTypes[i], levels[i]);
                return result;
            }
        }

        public Dictionary<ClassJobType, uint> Experience {
            get {
                var addr = m_Process.Offsets.PlayerInfoPtr + m_Process.Offsets.PlayerInfo.ClassExpTable;
                var exp = m_Process.Read<uint>(addr, m_ClassJobTypes.Length);
                var result = new Dictionary<ClassJobType, uint>(m_ClassJobTypes.Length);
                for (var i = 0; i < m_ClassJobTypes.Length; i++)
                    result.Add(m_ClassJobTypes[i], exp[i]);
                return result;
            }
        }

        public override GameObject TargetGameObject => m_Process.GameObjects.Target;
        public override Character TargetCharacter => m_Process.GameObjects.GetObjectByObjectId<Character>(CurrentTargetId);
        public override uint CurrentTargetId => m_Process.Read<uint>(m_Process.Offsets.TargetingPtr + 0x160);
        public override bool IsValid => BaseAddress != IntPtr.Zero;

        private static ClassJobType[] m_ClassJobTypes;

        internal LocalPlayer(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) {
            if (m_ClassJobTypes == null) {
                m_ClassJobTypes = new[] {
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
    }
}