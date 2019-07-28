using System;

namespace MemLib.Ffxiv.Objects {
    public class Minion : GameObject {
        public Minion(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) { }
    }
}