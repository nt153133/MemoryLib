using System;
using MemLib.Ffxiv.Enums;

namespace MemLib.Ffxiv.Objects {
    public class GatheringPointObject : GameObject {
        public override uint NpcId => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.NpcId);
        public new GatheringType Type => (GatheringType)(-1);

        internal GatheringPointObject(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) { }
    }
}