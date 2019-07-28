namespace MemLib.Ffxiv.Offsets {
    public class Signature {
        public string Key { get; set; }
        public string Value { get; set; }
        public int Offset { get; set; }
        public int[] PointerPath { get; set; }
    }
}