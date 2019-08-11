using System;
using System.Numerics;
using MemLib.Ffxiv.Enumerations;

namespace MemLib.Ffxiv.Objects {
    public class SpellCastInfo : RemoteObject {
        public TimeSpan CastTime => TimeSpan.FromSeconds(Ffxiv.Memory.Read<float>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.CastTime));
        public TimeSpan CurrentCastTime => TimeSpan.FromSeconds(Ffxiv.Memory.Read<float>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.CurrentCastTime));
        public TimeSpan RemainingCastTime => CastTime - CurrentCastTime;

        public uint TargetId => Ffxiv.Memory.Read<uint>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.CastingTargetId);
        public uint ActionId => Ffxiv.Memory.Read<uint>(BaseAddress + Ffxiv.Offsets.CharacterOffsets.CastingSpellId);
        public bool IsCasting => ActionId != 0u;
        public bool IsSpell => ActionType == ActionType.Spell;

        public ActionType ActionType { get; }
        public bool Interuptible { get; }
        public Vector3 CastLocation { get; }
        
        internal SpellCastInfo(IntPtr baseAddress) : base(baseAddress) { }
    }
}