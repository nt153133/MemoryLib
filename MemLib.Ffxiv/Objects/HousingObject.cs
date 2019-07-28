using System;

namespace MemLib.Ffxiv.Objects {
    public class HousingObject : GameObject {
        public override uint NpcId { get; }
        public HousingObject(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) { }
    }
}