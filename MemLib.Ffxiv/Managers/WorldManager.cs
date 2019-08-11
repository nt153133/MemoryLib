using System;

namespace MemLib.Ffxiv.Managers {
    public sealed class WorldManager {
        public DateTime EorzeaTime => DateTimeOffset.FromUnixTimeSeconds((long)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 20.5714285714)).DateTime;
        public uint RawZoneId {
            get {
                var ptr = Ffxiv.Memory.Read<IntPtr>(Ffxiv.Offsets.MapInfo);
                return ptr != IntPtr.Zero ? Ffxiv.Memory.Read<ushort>(ptr + Ffxiv.Offsets.MapOffsets.MapId) : 0u;
            }
        }

        internal WorldManager() { }
    }
}