using System;
using System.Linq;
using MemLib.Ffxiv.Enums;

namespace MemLib.Ffxiv.Objects {
    public class BattleCharacter : Character {
        public PlayerIcon Icon => (PlayerIcon) m_Process.Read<byte>(BaseAddress + m_Process.Offsets.Character.Icon);
        public byte MountId => m_Process.Read<byte>(BaseAddress + m_Process.Offsets.Character.MountId);
        public bool IsMounted => MountId > 0;

        public Character Mount {
            get {
                if (!m_Process.Read<IntPtr>(BaseAddress + m_Process.Offsets.Character.Mount, out var mountPtr) || mountPtr == IntPtr.Zero)
                    return null;
                var list = m_Process.GameObjects.GetObjectsByObjectType<Character>(GameObjectType.Mount);
                return list.FirstOrDefault(o => o.BaseAddress == mountPtr);
            }
        }

        //public int ElementalLevel { get; }
        //public EurekaElement Element { get; }
        //public bool IsFate { get; }

        public World CurrentWorld => (World)m_Process.Read<byte>(BaseAddress + m_Process.Offsets.Character.World);
        public World HomeWorld => (World)m_Process.Read<byte>(BaseAddress + m_Process.Offsets.Character.HomeWorld);
        public bool IsCrossWorld => CurrentWorld != HomeWorld;

        public override uint NpcId => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.NpcId2);

        internal BattleCharacter(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) { }
    }
}