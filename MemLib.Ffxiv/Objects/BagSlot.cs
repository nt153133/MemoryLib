using System;
using MemLib.Ffxiv.Enumerations;

namespace MemLib.Ffxiv.Objects {
    public class BagSlot : RemoteObject {
        public InventoryBagId BagId => Ffxiv.Memory.Read<InventoryBagId>(BaseAddress);
        public uint RawItemId => Ffxiv.Memory.Read<uint>(BaseAddress + Ffxiv.Offsets.ItemOffsets.RawItemId);
        public uint Count => Ffxiv.Memory.Read<uint>(BaseAddress + Ffxiv.Offsets.ItemOffsets.Count);
        public ushort Slot => Ffxiv.Memory.Read<ushort>(BaseAddress + Ffxiv.Offsets.ItemOffsets.Slot);
        public float Condition => Ffxiv.Memory.Read<float>(BaseAddress + Ffxiv.Offsets.ItemOffsets.Condition);
        public float SpiritBond => Ffxiv.Memory.Read<float>(BaseAddress + Ffxiv.Offsets.ItemOffsets.SpiritBond);
        public byte HqFlag => Ffxiv.Memory.Read<byte>(BaseAddress + Ffxiv.Offsets.ItemOffsets.HqFlag);
        public MateriaType[] MateriaTypes => Ffxiv.Memory.Read<MateriaType>(BaseAddress + Ffxiv.Offsets.ItemOffsets.MateriaIds, 5);
        public byte[] MateriaRanks => Ffxiv.Memory.Read<byte>(BaseAddress + Ffxiv.Offsets.ItemOffsets.MateriaRanks, 5);
        public byte DyeId => Ffxiv.Memory.Read<byte>(BaseAddress + Ffxiv.Offsets.ItemOffsets.DyeId);
        public uint GlamourId => Ffxiv.Memory.Read<uint>(BaseAddress + Ffxiv.Offsets.ItemOffsets.GlamourId);

        internal bool Valid = true;
        public override bool IsValid => Valid;
        public bool IsHighQuality => (HqFlag & 1) == 1;
        public bool IsCollectable => (HqFlag & 8) > 0;
        public bool IsFilled => RawItemId > 0u;

        internal BagSlot(IntPtr baseAddress) : base(baseAddress) { }

        #region Overrides of RemoteObject

        public override string ToString() {
            return IsValid ?
                $"0x{BaseAddress.ToInt64():X} ItemID:{RawItemId}" :
                $"Invalid Item {RawItemId}";
        }

        #endregion
    }
}