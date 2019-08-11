using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using MemLib.Ffxiv.Offsets.OffsetStructs;

namespace MemLib.Ffxiv.Offsets {
    public sealed class FfxivOffsets {
        private static readonly PropertyInfo[] PropertyCache;

        #region Signatures

        public HashSet<Signature> Signatures { get; set; } = new HashSet<Signature> {
            new Signature{Key = "PlayerInfo", Value = "83f9ff7412448b048e8bd3488d0d", Offset = 14},
            new Signature{Key = "ObjectList", Value = "488b420848c1e8033da701000077248bc0488d0d", Offset = 20},
            new Signature{Key = "Inventory", Value = "8d81********85c075584c8b05", Offset = 13},
            new Signature{Key = "InventoryIds", Value = "8BD94D85C974**33C94C8D15", Offset = 12},
            new Signature{Key = "Targeting", Value = "488B05********4885C07507488B05********C3", Offset = 3},
            new Signature{Key = "Pet", Value = "3B15********74**8915", Offset = 2},
            new Signature{Key = "PetStats", Value = "488D0D********E8********83BD********00488BC3448825", Offset = 3},
            new Signature{Key = "PetInfo", Value = "488D0D********E8********83BD********00488BC3448825", Offset = 25},
            new Signature{Key = "AttackerList", Value = "418BDF391D********0F8E********488D3D", Offset = 18},
            new Signature{Key = "AttackerCount", Value = "418BDF391D********0F8E********488D3D", Offset = 5},
            new Signature{Key = "PartyList", Value = "488D7C242066660F1F840000000000488B17488D0D", Offset = 21, PointerPath = new []{0x2F0}},
            new Signature{Key = "PartyCount", Value = "488D7C242066660F1F840000000000488B17488D0D", Offset = 21, PointerPath = new []{0x63DC}},
            new Signature{Key = "MapInfo", Value = "8B15********33C085D274**4C8D05", Offset = 2, PointerPath = new []{0x2C}},
            new Signature{Key = "SanctuaryFlag", Value = "0FB615********33C084C90F45C2C3", Offset = 3},
        };

        #endregion
        #region Signature Addresses

        // ReSharper disable UnusedAutoPropertyAccessor.Local
        [XmlIgnore] public IntPtr ObjectList { get; private set; }
        [XmlIgnore] public IntPtr PlayerInfo { get; private set; }
        [XmlIgnore] public IntPtr Inventory { get; private set; }
        [XmlIgnore] public IntPtr InventoryIds { get; private set; }
        [XmlIgnore] public IntPtr Targeting { get; private set; }
        [XmlIgnore] public IntPtr Pet { get; private set; }
        [XmlIgnore] public IntPtr PetStats { get; private set; }
        [XmlIgnore] public IntPtr PetInfo { get; private set; }
        [XmlIgnore] public IntPtr AttackerList { get; private set; }
        [XmlIgnore] public IntPtr AttackerCount { get; private set; }
        [XmlIgnore] public IntPtr PartyList { get; private set; }
        [XmlIgnore] public IntPtr PartyCount { get; private set; }
        [XmlIgnore] public IntPtr MapInfo { get; private set; }
        [XmlIgnore] public IntPtr SanctuaryFlag { get; private set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Local

        #endregion
        #region Offsets

        public CharacterOffsets CharacterOffsets = new CharacterOffsets();
        public ItemOffsets ItemOffsets = new ItemOffsets();
        public MapOffsets MapOffsets = new MapOffsets();
        public PartyOffsets PartyOffsets = new PartyOffsets();
        public TargetOffsets TargetOffsets = new TargetOffsets();
        public PlayerInfoOffsets PlayerOffsets = new PlayerInfoOffsets();

        #endregion

        [XmlIgnore]
        public Dictionary<string, IntPtr> SignatureResults { get; } = new Dictionary<string, IntPtr>();

        public IntPtr this[string key] => SignatureResults[key];

        static FfxivOffsets() {
            if (PropertyCache == null)
                PropertyCache = typeof(FfxivOffsets).GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }

        public void Update() {
            ResolveSignatures();
        }

        private void ResolveSignatures() {
            SignatureResults.Clear();
            if (Signatures.Count == 0 || Ffxiv.Memory == null) return;
            foreach (var sig in Signatures) {
                var addr = Resolve(sig);
                if (addr == IntPtr.Zero)
                    Debug.WriteLine($"[{DateTime.Now:T}] Failed to resolve Signature -> {sig}");
                if (SignatureResults.ContainsKey(sig.Key)) {
                    Debug.WriteLine($"[{DateTime.Now:T}] Duplicate signature key found, ignoring: {sig.Key}");
                    continue;
                }
                SignatureResults.Add(sig.Key, addr);
                var prop = PropertyCache.FirstOrDefault(f => f.Name.Equals(sig.Key, StringComparison.OrdinalIgnoreCase) && f.PropertyType == typeof(IntPtr));
                if (prop != null)
                    prop.SetValue(this, addr);
            }
        }

        private static IntPtr Resolve(Signature sig) {
            var sigStr = $"Search {sig.Value} Add {sig.Offset:X2} TraceRelative";
            if (sig.PointerPath != null && sig.PointerPath.Length > 0) {
                sigStr += $" Add {sig.PointerPath.FirstOrDefault():X2}";
                foreach (var offset in sig.PointerPath.Skip(1)) {
                    sigStr += $" Read64 Add {offset:X2}";
                }
            }
            return Ffxiv.Memory.Pattern.Find(sigStr);
        }
    }
}