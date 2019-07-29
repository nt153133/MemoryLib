using System;
using MemLib.Ffxiv.Enums;

namespace MemLib.Ffxiv.Objects {
    public class BagSlot : RemoteObject {
        public InventoryBagId BagId => (InventoryBagId)m_Process.Read<uint>(BaseAddress);
        public uint RawItemId => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Item.RawItemId);
        public uint Count => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Item.Count);
        public ushort Slot => m_Process.Read<ushort>(BaseAddress + m_Process.Offsets.Item.Slot);
        public float Condition => m_Process.Read<float>(BaseAddress + m_Process.Offsets.Item.Condition);
        public float SpiritBond => m_Process.Read<float>(BaseAddress + m_Process.Offsets.Item.SpiritBond);
        public byte HqFlag => m_Process.Read<byte>(BaseAddress + m_Process.Offsets.Item.HqFlag);
        public ushort[] MateriaId => m_Process.Read<ushort>(BaseAddress + m_Process.Offsets.Item.MateriaIds, 5);
        public byte[] MateriaRank => m_Process.Read<byte>(BaseAddress + m_Process.Offsets.Item.MateriaRanks, 5);
        public byte DyeId => m_Process.Read<byte>(BaseAddress + m_Process.Offsets.Item.DyeId);
        public uint GlamourId => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Item.GlamourId);

        internal bool Valid = true;
        public override bool IsValid => Valid;
        public bool IsHq => (HqFlag & 1) == 1;
        public bool IsCollectable => (HqFlag & 8) > 0;
        public bool IsFilled => RawItemId > 0u;

        internal BagSlot(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) { }

        public override string ToString() {
            if (!IsValid)
                return $"0x{BaseAddress.ToInt64():X} - Invalid Item";
            return $"0x{BaseAddress.ToInt64():X} - ItemID={RawItemId}, Count={Count}, IsHq={IsHq}";
        }
    }
}