using System;
using System.Numerics;

namespace MemLib.Ffxiv.Objects {
    public class Aetheryte : GameObject {
        public override uint NpcId { get; }
        public override Vector3 Location { get; }

        public Aetheryte(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) { }
    }
}