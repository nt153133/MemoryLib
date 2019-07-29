using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MemLib.Ffxiv.Enums;

namespace MemLib.Ffxiv.Objects {
    public class Bag : RemoteObject, IEnumerable<BagSlot> {
        private List<BagSlot> m_BagSlots = new List<BagSlot>();

        public uint BagId => m_Process.Read<uint>(BaseAddress + 0x08);
        public InventoryBagId InventoryBagId => (InventoryBagId) BagId;
        public uint TotalSlots => m_Process.Read<uint>(BaseAddress + 0x0C);
        public uint FreeSlots => TotalSlots - UsedSlots;
        public uint UsedSlots => (uint) m_BagSlots.Sum(s => s.RawItemId == 0 ? 0 : 1);
        public List<BagSlot> FilledSlots => m_BagSlots.Where(b => b.IsFilled).ToList();

        private bool m_Valid;
        public override bool IsValid => m_Valid;

        public BagSlot this[int i] => m_BagSlots[i];
        public BagSlot this[EquipmentSlot i] => m_BagSlots[(int) i];

        internal Bag(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) {
            var ptr = process.Read<IntPtr>(baseAddress);
            var slots = TotalSlots;
            for (var i = 0; i < slots; i++) {
                m_BagSlots.Add(new BagSlot(process, ptr + i * 56));
            }
            m_Valid = true;
        }

        internal void Invalidate() {
            m_Valid = false;
            m_BagSlots.ForEach(b => b.Valid = false);
            m_BagSlots.Clear();
            m_BagSlots = null;
        }

        public IEnumerator<BagSlot> GetEnumerator() {
            return m_BagSlots.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public override string ToString() {
            return $"{InventoryBagId}:\n{string.Join("\n", FilledSlots.Select(s => $"{s.ToString().PadLeft(4)}"))}";
        }
    }
}