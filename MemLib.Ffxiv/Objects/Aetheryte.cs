using System;

namespace MemLib.Ffxiv.Objects {
    public class Aetheryte : GameObject {
        public override uint NpcId => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.NpcId);
        //public override Vector3 Location { get; }

        internal Aetheryte(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) { }
    }
}