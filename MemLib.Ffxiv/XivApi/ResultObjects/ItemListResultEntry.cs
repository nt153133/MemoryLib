namespace MemLib.Ffxiv.XivApi.ResultObjects {
    public class ItemListResultEntry {
        public uint ID { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }

        public override string ToString() => $"ID={ID}, Name=\"{Name}\", Url=\"{Url}\", Icon=\"{Icon}\"";
    }
}