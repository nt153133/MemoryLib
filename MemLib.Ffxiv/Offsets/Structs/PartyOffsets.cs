namespace MemLib.Ffxiv.Offsets.Structs {
    public class PartyOffsets {
        public int MemberObjSize { get; set; } = 0x390;
        public int ObjectId { get; set; } = 0x18;
        public int Health { get; set; } = 0x24;
        public int Mana { get; set; } = 0x2C;
        public int Name { get; set; } = 0x34;
        public int ClassJob { get; set; } = 0x75;
        public int Level { get; set; } = 0x76;
    }
}