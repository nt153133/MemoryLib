namespace MemLib.Ffxiv.Offsets.Structs {
    public class PlayerInfoOffsets {
        public int Name { get; set; } = 0x1;
        public int ObjectId { get; set; } = 0x54;
        public int ClassJob { get; set; } = 0x66;
        public int ClassLevel { get; set; } = 0x68;
        public int ClassLevelTable { get; set; } = 0x6A;
        public int ClassExpTable { get; set; } = 0xA4;
        public int StatsTable { get; set; } = 0x138;
        public int InspectedPlayer { get; set; } = 0x618;
    }
}