using System;

namespace MemLib.Ffxiv.Managers {
    public class WorldManager {
        private readonly FfxivProcess m_Process;
        public DateTime EorzeaTime => DateTimeOffset.FromUnixTimeSeconds((long)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 20.5714285714)).DateTime;
        public uint RawZoneId {
            get {
                var ptr = m_Process.Read<IntPtr>(m_Process.Offsets.MapInfoPtr);
                return ptr != IntPtr.Zero ? m_Process.Read<ushort>(ptr + m_Process.Offsets.Map.MapId) : 0u;
            }
        }

        internal WorldManager(FfxivProcess process) {
            m_Process = process;
        }
    }
}