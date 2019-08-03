using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MemLib.Ffxiv.Enumerations;
using MemLib.Ffxiv.Objects;

namespace MemLib.Ffxiv.Managers {
    public sealed class InventoryManager {
        private readonly FfxivProcess m_Process;
        private static IntPtr m_InventoryPtr = IntPtr.Zero;
        private readonly ConcurrentDictionary<InventoryBagId, Bag> m_Bags = new ConcurrentDictionary<InventoryBagId, Bag>();

        public uint FreeSlots => (uint) GetBagsByInventoryId(InventoryIds).Sum(b => b.FreeSlots);
        public IEnumerable<BagSlot> EquippedSlots => GetBagByInventoryId(InventoryBagId.EquippedItems);
        public IEnumerable<BagSlot> ExamineSlots => GetBagByInventoryId(InventoryBagId.Examine);
        public IEnumerable<BagSlot> FilledInventorySlots => GetSlots(InventoryKeyIds);
        public IEnumerable<BagSlot> FilledArmorySlots => GetSlots(ArmoryIds);
        public IEnumerable<BagSlot> FilledInventoryAndArmory => GetSlots(InventoryAndArmoryIds);
        
        public Bag this[InventoryBagId id] => GetBagByInventoryId(id);
        
        internal InventoryManager(FfxivProcess process) {
            m_Process = process;
        }

        private IEnumerable<BagSlot> GetSlots(params InventoryBagId[] bagIds) {
            var list = new List<BagSlot>();
            foreach (var bag in GetBagsByInventoryId(bagIds))
                list.AddRange(bag.FilledSlots);
            return list;
        }

        private void Update() {
            var invPtr = m_Process.Read<IntPtr>(m_Process.Offsets.InventoryPtr);
            if (invPtr != m_InventoryPtr) {
                foreach (var bag in m_Bags.Values) {
                    bag.Invalidate();
                }
                m_InventoryPtr = invPtr;
            }
            var idArray = m_Process.Read<uint>(m_Process.Offsets.InventoryIdsPtr, 74);
            for (var i = 0; i < idArray.Length; i++) {
                if(!Enum.IsDefined(typeof(InventoryBagId), idArray[i])) continue;
                if (m_Process.Read<IntPtr>(invPtr + i * 24, out var ptr) && ptr != IntPtr.Zero)
                    m_Bags[(InventoryBagId) idArray[i]] = new Bag(m_Process, invPtr + i * 24);
                else m_Bags.TryRemove((InventoryBagId) idArray[i], out _);
            }
        }

        public Bag GetBagByInventoryId(InventoryBagId id) {
            Update();
            return m_Bags.TryGetValue(id, out var bag) ? bag : null;
        }

        public IEnumerable<Bag> GetBagsByInventoryId(params InventoryBagId[] bagIds) {
            Update();
            foreach (var id in bagIds) {
                if (m_Bags.TryGetValue(id, out var bag))
                    yield return bag;
            }
        }

        private static readonly InventoryBagId[] InventoryIds = {
            InventoryBagId.Bag1,
            InventoryBagId.Bag2,
            InventoryBagId.Bag3,
            InventoryBagId.Bag4
        };

        private static readonly InventoryBagId[] InventoryKeyIds = {
            InventoryBagId.Bag1,
            InventoryBagId.Bag2,
            InventoryBagId.Bag3,
            InventoryBagId.Bag4,
            InventoryBagId.KeyItems
        };

        private static readonly InventoryBagId[] ArmoryIds = {
            InventoryBagId.EquippedItems,
            InventoryBagId.ArmoryMainHand,
            InventoryBagId.ArmoryOffHand,
            InventoryBagId.ArmoryHelmet,
            InventoryBagId.ArmoryChest,
            InventoryBagId.ArmoryGlove,
            InventoryBagId.ArmoryBelt,
            InventoryBagId.ArmoryPants,
            InventoryBagId.ArmoryBoots,
            InventoryBagId.ArmoryEarrings,
            InventoryBagId.ArmoryNecklace,
            InventoryBagId.ArmoryWrits,
            InventoryBagId.ArmoryRings,
            InventoryBagId.ArmorySouls
        };

        private static readonly InventoryBagId[] InventoryAndArmoryIds = {
            InventoryBagId.EquippedItems,
            InventoryBagId.ArmoryMainHand,
            InventoryBagId.ArmoryOffHand,
            InventoryBagId.ArmoryHelmet,
            InventoryBagId.ArmoryChest,
            InventoryBagId.ArmoryGlove,
            InventoryBagId.ArmoryBelt,
            InventoryBagId.ArmoryPants,
            InventoryBagId.ArmoryBoots,
            InventoryBagId.ArmoryEarrings,
            InventoryBagId.ArmoryNecklace,
            InventoryBagId.ArmoryWrits,
            InventoryBagId.ArmoryRings,
            InventoryBagId.ArmorySouls,
            InventoryBagId.Bag1,
            InventoryBagId.Bag2,
            InventoryBagId.Bag3,
            InventoryBagId.Bag4,
            InventoryBagId.KeyItems
        };
    }
}