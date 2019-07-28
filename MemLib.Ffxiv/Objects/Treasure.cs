using System;

namespace MemLib.Ffxiv.Objects {
    public class Treasure : GameObject {
        public bool IsOpen { get; }
        public Treasure(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) { }
    }
}