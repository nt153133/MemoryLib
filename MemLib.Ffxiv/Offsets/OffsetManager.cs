using System;
using System.Collections.Generic;
using System.IO;
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

        private Offsets m_Offsets = new Offsets();
        public CharacterOffsets Character => m_Offsets.Character;
        public PlayerInfoOffsets PlayerInfo => m_Offsets.PlayerInfo;
        public ItemOffsets Item => m_Offsets.Item;
        
        internal OffsetManager(FfxivProcess process) {
            m_Process = process;
            LoadOffsets();
            LoadSignatures();
            ResolveSignatures();
        }

        private void LoadOffsets() {
            if (!File.Exists(OffsetsFile)) {
                var dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                using (var fs = new FileStream(Path.Combine(dir, OffsetsFile), FileMode.Create)) {
                    var xml = new XmlSerializer(typeof(Offsets));
                    xml.Serialize(fs, m_Offsets);
                }
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
                    foreach (var offset in sig.PointerPath) {
                        sigStr += $" Add {offset:X} Read64";
                    }
                }
                var val = m_Process.Pattern.Find(sigStr);
                m_ResolvedSignatures.Add(sig.Key, val);
            }
        }

        private static List<Signature> GetDefaultSignatures() {
            return new List<Signature> {
                new Signature{Key = "PlayerInfo", Value = "83f9ff7412448b048e8bd3488d0d", Offset = 14},
                new Signature{Key = "ObjectList", Value = "488b420848c1e8033da701000077248bc0488d0d", Offset = 20},
                new Signature{Key = "Inventory", Value = "8d81********85c075584c8b05", Offset = 13},
                new Signature{Key = "InventoryIds", Value = "8BD94D85C974**33C94C8D15", Offset = 12},
                new Signature{Key = "Targeting", Value = "41bc000000e041bd01000000493bc47555488d0d", Offset = 20},
                new Signature{Key = "Pet", Value = "3B15********74**8915", Offset = 2},
            };
        }

        private void LoadSignatures() {
            m_Signatures = GetDefaultSignatures();
            if (!File.Exists(SignatureFile)) {
                var dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                using (var fs = new FileStream(Path.Combine(dir, SignatureFile), FileMode.Create)) {
                    var xml = new XmlSerializer(typeof(List<Signature>));
                    xml.Serialize(fs, m_Signatures);
                }
                return;
            }
            using (var fs = new FileStream(SignatureFile, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                var xml = new XmlSerializer(typeof(List<Signature>));
                m_Signatures = xml.Deserialize(fs) as List<Signature>;
            }
        }
    }
}