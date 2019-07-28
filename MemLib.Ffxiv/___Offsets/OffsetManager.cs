using System.IO;
using System.Xml.Serialization;

namespace MemLib.Ffxiv.Offsets {
    public sealed class OffsetManager {
        private readonly FfxivProcess m_Process;
        private const string SignatureFile = "signatures.xml";
        private const string OffsetsFile = "offsets.xml";
        
        public Signatures Signatures { get; private set; } = new Signatures();
        public Offsets Offsets { get; private set; } = new Offsets();

        internal OffsetManager(FfxivProcess process) {
            m_Process = process;
            LoadSignatures();
            LoadOffsets();
        }

        private void LoadOffsets() {
            if (!File.Exists(OffsetsFile)) {
                using (var fs = new FileStream(@"C:\Users\Pohky\Desktop\offsets.xml", FileMode.OpenOrCreate)) {
                    var xml = new XmlSerializer(typeof(Offsets));
                    xml.Serialize(fs, Offsets);
                }
                return;
            }
            using (var fs = new FileStream(OffsetsFile, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                var xml = new XmlSerializer(typeof(Offsets));
                Offsets = xml.Deserialize(fs) as Offsets;
            }
        }

        private void LoadSignatures() {
            if (!File.Exists(SignatureFile)) {
                using (var fs = new FileStream(@"C:\Users\Pohky\Desktop\signatures.xml", FileMode.OpenOrCreate)) {
                    var xml = new XmlSerializer(typeof(Signatures));
                    xml.Serialize(fs, Signatures);
                }
                return;
            }
            using (var fs = new FileStream(SignatureFile, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                var xml = new XmlSerializer(typeof(Signatures));
                Signatures = xml.Deserialize(fs) as Signatures;
            }
        }
    }
}