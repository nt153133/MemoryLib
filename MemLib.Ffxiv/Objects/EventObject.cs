using System;

namespace MemLib.Ffxiv.Objects {
    public class EventObject : GameObject {
        public override uint NpcId { get; }
        public EventObject(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) { }
    }
}