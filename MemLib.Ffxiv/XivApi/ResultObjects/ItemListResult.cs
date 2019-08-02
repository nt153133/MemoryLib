using System.Collections.Generic;

namespace MemLib.Ffxiv.XivApi.ResultObjects {
    internal class ItemListResult {
        public Pagination Pagination { get; set; }
        public List<ItemListResultEntry> Results { get; set; }
    }
}