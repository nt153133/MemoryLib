using System;
using System.Numerics;
using MemLib.Ffxiv.Enumerations;
using MemLib.Ffxiv.Structures;

namespace MemLib.Ffxiv.Objects {
    public class SpellCastInfo : RemoteObject {
        public TimeSpan CastTime => TimeSpan.FromMilliseconds(m_Process.Read<float>(BaseAddress + m_Process.Offsets.Character.CastTime));
        public TimeSpan CurrentCastTime => TimeSpan.FromMilliseconds(m_Process.Read<float>(BaseAddress + m_Process.Offsets.Character.CurrentCastTime));
        public uint TargetId => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.CastingTargetId);
        public uint ActionId => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Character.CastingSpellId);
        public bool IsCasting => ActionId > 0u;
        public TimeSpan RemainingCastTime => CastTime - CurrentCastTime;

        public ActionType ActionType { get; }
        public bool IsSpell { get; }
        public bool Interuptible { get; }
        public Vector3 CastLocation { get; }
        
        internal SpellCastInfo(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) { }
    }
}