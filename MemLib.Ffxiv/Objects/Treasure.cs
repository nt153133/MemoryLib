using System;

namespace MemLib.Ffxiv.Objects {
    public class Treasure : GameObject {
        //public bool IsOpen => Core.Memory.Read<int>(base.Pointer + Core.Offsets.struct158_0.int_0) == 2;
        //public int State => Core.Memory.Read<int>(base.Pointer + Core.Offsets.struct158_0.int_0);

        internal Treasure(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) { }
    }
}