// ReSharper disable InconsistentNaming
namespace MemLib.Ffxiv.Offsets.Structs {
    public class CharacterOffset {
        public int Name { get; set; } = 0x30;
        public int ObjectId { get; set; } = 0x74;
        public int NpcId { get; set; } = 0x78;
        public int OwnerId { get; set; } = 0x84;
        public int ObjectType { get; set; } = 0x8C;
        public int Distance { get; set; } = 0x92;
        public int Location { get; set; } = 0xA0;
        public int Heading { get; set; } = 0xB0;
        public int HitboxRadius { get; set; } = 0xC0;
        public int FcTag { get; set; } = 0x17FA;
        public int Health { get; set; } = 0x18A4;
        //public int MaxHealth { get; set; } = 0x18A8;
        public int Mana { get; set; } = 0x18AC;
        //public int MaxMana { get; set; } = 0x18B0;
        public int GP { get; set; } = 0x18B6;
        //public int MaxGP { get; set; } = 0x18B8;
        public int CP { get; set; } = 0x18BA;
        //public int MaxCP { get; set; } = 0x18BC;
        public int Title { get; set; } = 0x18BE;
        public int ClassJob { get; set; } = 0x18DC;
        public int ClassLevel { get; set; } = 0x18DE;
        public int Icon { get; set; } = 0x18E0;
        public int Status { get; set; } = 0x1908;
    }
}