using System;
using MemLib.Ffxiv.Enums;

namespace MemLib.Ffxiv.Objects {
    public class GatheringPointObject : GameObject {
        public override uint NpcId { get; }
        public new GatheringType Type => (GatheringType)(-1);

        public GatheringPointObject(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) { }
    }
}