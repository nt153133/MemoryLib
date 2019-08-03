namespace MemLib.Ffxiv.Offsets.Structs {
    public class TargetOffsets {
        public int CurrentTarget { get; set; } = 0x08;
        public int MouseOverTarget { get; set; } = 0x30;
        public int FocusTarget { get; set; } = 0x68;
        public int PreviousTarget { get; set; } = 0x80;
        public int CurrentTargetId { get; set; } = 0xB0;
        public int NumTargetableObj { get; set; } = 0xB8;
        public int TargetableObjList { get; set; } = 0xC0;
    }
}