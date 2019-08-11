using System.Runtime.InteropServices;

namespace MemLib.Ffxiv.Objects {
    [StructLayout(LayoutKind.Sequential, Size = 12)]
    public struct AuraData {
        public ushort AuraId;
        public ushort AuraValue;
        public float TimeLeft;
        public uint CasterId;
    }
}