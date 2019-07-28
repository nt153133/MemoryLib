// ReSharper disable InconsistentNaming
namespace MemLib.Ffxiv.Offsets.Structs {
    public class CharacterOffsets {
        public int Name { get; set; } = 0x30;
        public int ObjectId { get; set; } = 0x74;
        //public int NpcId1 { get; set; } = 0x78;
        public int NpcId { get; set; } = 0x80;
        public int NpcId2 { get; set; } = 0x187C;
        public int OwnerId { get; set; } = 0x84;
        public int ObjectType { get; set; } = 0x8C;
        public int Distance { get; set; } = 0x92;
        public int Location { get; set; } = 0xA0;
        public int Heading { get; set; } = 0xB0;
        public int HitboxRadius { get; set; } = 0xC0;
        public int FcTag { get; set; } = 0x17FA;
        public int Health { get; set; } = 0x18A4;
        public int Mana { get; set; } = 0x18AC;
        public int GP { get; set; } = 0x18B6;
        public int CP { get; set; } = 0x18BA;
        public int Title { get; set; } = 0x18BE;
        public int ClassJob { get; set; } = 0x18DC;
        public int ClassLevel { get; set; } = 0x18DE;
        public int Icon { get; set; } = 0x18E0;
        public int World { get; set; } = 0x1898;
        public int HomeWorld { get; set; } = 0x189A;
        public int Status { get; set; } = 0x1909;
    }
}