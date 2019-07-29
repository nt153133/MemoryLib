namespace MemLib.Ffxiv.Offsets.Structs {
    public class ItemOffsets {
        public int Slot { get; set; } = 0x04;
        public int RawItemId { get; set; } = 0x08;
        public int Count { get; set; } = 0x0C;
        public int SpiritBond { get; set; } = 0x10;
        public int Condition { get; set; } = 0x12;
        public int HqFlag { get; set; } = 0x14;
        public int MateriaIds { get; set; } = 0x20;
        public int MateriaRanks { get; set; } = 0x2A;
        public int DyeId { get; set; } = 0x2F;
        public int GlamourId { get; set; } = 0x30;
    }
}