using System;

namespace MemLib.Ffxiv.Enumerations {
    [Flags]
    public enum StatusFlags : byte {
        None = 0,
        Hostile = 1,
        InCombat = 2,
        WeaponOut = 4,
        PartyMember = 16,
        AllianceMember = 32,
        Friend = 64
    }
}