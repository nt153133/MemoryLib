using System;

namespace MemLib.Ffxiv.Objects {
    public class EventObject : GameObject {
        public override uint NpcId => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.NpcId);

        public EventObject(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) { }
    }
}