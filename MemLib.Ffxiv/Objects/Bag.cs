﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MemLib.Ffxiv.Enumerations;

namespace MemLib.Ffxiv.Objects {
    public class Bag : RemoteObject, IEnumerable<BagSlot> {
        private List<BagSlot> m_BagSlots = new List<BagSlot>();

        public uint BagId => Ffxiv.Memory.Read<uint>(BaseAddress + 8);
        public InventoryBagId InventoryBagId => (InventoryBagId) BagId;
        public uint TotalSlots => Ffxiv.Memory.Read<uint>(BaseAddress + 12);
        public uint FreeSlots => TotalSlots - UsedSlots;
        public uint UsedSlots => (uint) m_BagSlots.Sum(s => s.RawItemId == 0 ? 0 : 1);
        public List<BagSlot> FilledSlots => m_BagSlots.Where(s => s.IsFilled).ToList();

        private bool m_Valid;
        public override bool IsValid => m_Valid;

        public BagSlot this[int i] => m_BagSlots[i];
        public BagSlot this[EquipmentSlot i] => m_BagSlots[(int) i];

        internal Bag(IntPtr baseAddress) : base(baseAddress) {
            var ptr = Ffxiv.Memory.Read<IntPtr>(baseAddress);
            var slots = TotalSlots;
            for (var i = 0; i < slots; i++) {
                m_BagSlots.Add(new BagSlot(ptr + i * 56));
            }
            m_Valid = true;
        }

        internal void Invalidate() {
            m_Valid = false;
            m_BagSlots.ForEach(b => b.Valid = false);
            m_BagSlots.Clear();
            m_BagSlots = null;
        }

        public bool FindItem(uint rawItemId, out BagSlot slot) {
            slot = m_BagSlots.FirstOrDefault(b => b.RawItemId == rawItemId);
            return slot != null;
        }

        #region Implementation of IEnumerable

        public IEnumerator<BagSlot> GetEnumerator() {
            return m_BagSlots.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion

        #region Overrides of RemoteObject

        public override string ToString() {
            return $"{InventoryBagId}:\n{string.Join("\n", FilledSlots.Select(s => $"{s.ToString().PadLeft(4)}"))}";
        }
        
        #endregion
    }
}