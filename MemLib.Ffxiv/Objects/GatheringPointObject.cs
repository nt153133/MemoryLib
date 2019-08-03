using System;

namespace MemLib.Ffxiv.Objects {
    public class GatheringPointObject : GameObject {
        public override uint NpcId => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.NpcId);
        
        //public bool CanGather => base.IsVisible;
        //public override uint NpcId => Core.Memory.Read<uint>(base.Pointer + Core.Offsets.struct125_0.int_2);
        //public new GatheringType Type => (GatheringType)Core.Memory.Read<byte>(base.Pointer + Core.Offsets.struct125_0.int_0);
        //public byte Level => Core.Memory.Read<byte>(base.Pointer + Core.Offsets.struct125_0.int_4);

        internal GatheringPointObject(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) { }
    }
}