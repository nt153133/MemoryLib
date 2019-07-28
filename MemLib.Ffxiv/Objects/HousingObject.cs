using System;

namespace MemLib.Ffxiv.Objects {
    public class HousingObject : GameObject {
        public override uint NpcId => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.NpcId);

        public HousingObject(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) { }
    }
}