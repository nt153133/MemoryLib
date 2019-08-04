using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using MemLib.Ffxiv.Offsets.Structs;

namespace MemLib.Ffxiv.Offsets {
    public sealed class OffsetManager {
        private readonly FfxivProcess m_Process;
        private const string SignatureFile = "signatures.xml";
        private const string OffsetsFile = "offsets.xml";

        private List<Signature> m_Signatures;
        private Dictionary<string, IntPtr> m_ResolvedSignatures;
        public IntPtr ObjectListPtr => m_ResolvedSignatures["ObjectList"];
        public IntPtr PlayerInfoPtr => m_ResolvedSignatures["PlayerInfo"];
        public IntPtr TargetingPtr => m_ResolvedSignatures["Targeting"];
        public IntPtr InventoryPtr => m_ResolvedSignatures["Inventory"];
        public IntPtr InventoryIdsPtr => m_ResolvedSignatures["InventoryIds"];
        public IntPtr PetPtr => m_ResolvedSignatures["Pet"];
        public IntPtr AttackerListPtr => m_ResolvedSignatures["AttackerList"];
        public IntPtr AttackerCountPtr => m_ResolvedSignatures["AttackerCount"];
        public IntPtr PartyListPtr => m_ResolvedSignatures["PartyList"];

        private Offsets m_Offsets = new Offsets();
        public CharacterOffsets Character => m_Offsets.Character;
        public PlayerInfoOffsets PlayerInfo => m_Offsets.PlayerInfo;
        public ItemOffsets Item => m_Offsets.Item;
        public TargetOffsets Target => m_Offsets.Target;
        public PartyOffsets Party => m_Offsets.Party;

        internal OffsetManager(FfxivProcess process) {
            m_Process = process;
            LoadOffsets();
            LoadSignatures();
            ResolveSignatures();
        }

        private static List<Signature> GetDefaultSignatures() {
            return new List<Signature> {
                new Signature{Key = "PlayerInfo", Value = "83f9ff7412448b048e8bd3488d0d", Offset = 14},
                new Signature{Key = "ObjectList", Value = "488b420848c1e8033da701000077248bc0488d0d", Offset = 20},
                new Signature{Key = "Inventory", Value = "8d81********85c075584c8b05", Offset = 13},
                new Signature{Key = "InventoryIds", Value = "8BD94D85C974**33C94C8D15", Offset = 12},
                new Signature{Key = "Targeting", Value = "488B05********4885C07507488B05********C3", Offset = 3},
                new Signature{Key = "Pet", Value = "3B15********74**8915", Offset = 2},
                new Signature{Key = "AttackerList", Value = "418BDF391D********0F8E********488D3D", Offset = 18},
                new Signature{Key = "AttackerCount", Value = "418BDF391D********0F8E********488D3D", Offset = 5},
                new Signature{Key = "PartyList", Value = "488D7C242066660F1F840000000000488B17488D0D", Offset = 21, PointerPath = new []{0x2F0}},
            };
        }

        private void LoadOffsets() {
            if (!File.Exists(OffsetsFile)) {
#if DEBUG
                var dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                using (var fs = new FileStream(Path.Combine(dir, OffsetsFile), FileMode.Create)) {
                    var xml = new XmlSerializer(typeof(Offsets));
                    xml.Serialize(fs, m_Offsets);
                }
#endif
                return;
            }
            using (var fs = new FileStream(OffsetsFile, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                var xml = new XmlSerializer(typeof(Offsets));
                m_Offsets = xml.Deserialize(fs) as Offsets;
            }
        }

        private void ResolveSignatures() {
            m_ResolvedSignatures = new Dictionary<string, IntPtr>(m_Signatures.Count);
            foreach (var sig in m_Signatures) {
                var sigStr = $"Search {sig.Value} Add {sig.Offset:X} TraceRelative";
                if (sig.PointerPath != null && sig.PointerPath.Length > 0) {
                    sigStr += $" Add {sig.PointerPath.FirstOrDefault():X}";
                    foreach (var offset in sig.PointerPath.Skip(1)) {
                        sigStr += $" Add {offset:X} Read64";
                    }
                }
                var val = m_Process.Pattern.Find(sigStr);
                m_ResolvedSignatures.Add(sig.Key, val);
            }
        }

        private void LoadSignatures() {
            m_Signatures = GetDefaultSignatures();
            if (!File.Exists(SignatureFile)) {
#if DEBUG
                var dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                using (var fs = new FileStream(Path.Combine(dir, SignatureFile), FileMode.Create)) {
                    var xml = new XmlSerializer(typeof(List<Signature>));
                    xml.Serialize(fs, m_Signatures);
                }
#endif
                return;
            }
            using (var fs = new FileStream(SignatureFile, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                var xml = new XmlSerializer(typeof(List<Signature>));
                m_Signatures = xml.Deserialize(fs) as List<Signature>;
            }
        }
    }
}