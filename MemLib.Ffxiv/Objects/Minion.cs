using System;

namespace MemLib.Ffxiv.Objects {
    public class Minion : GameObject {
        public override uint NpcId => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.NpcId);

        internal Minion(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) { }
    }
}