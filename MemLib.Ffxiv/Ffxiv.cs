using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using MemLib.Ffxiv.Managers;
using MemLib.Ffxiv.Objects;
using MemLib.Ffxiv.Offsets;

namespace MemLib.Ffxiv {
    public static class Ffxiv {
        private const string OffsetFile = "offsets.xml";
        private static RemoteProcess m_Memory;
        private static FfxivOffsets m_Offsets;

        public static RemoteProcess Memory {
            get {
                if (m_Memory != null) return m_Memory;
                var process = RemoteProcess.FindProcess("ffxiv_dx11");
                if (process == null) return null;
                m_Memory = new RemoteProcess(process);
                m_Memory.Native.Exited += NativeOnExited;
                if(Offsets == null)
                    LoadOffsets();
                return m_Memory;
            }
        }
        public static FfxivOffsets Offsets {
            get {
                if (m_Offsets != null) return m_Offsets;
                LoadOffsets();
                return m_Offsets;
            }
            private set => m_Offsets = value;
        }

        public static GameObjectManager Objects { get;} = new GameObjectManager();
        public static PartyManager Party { get; } = new PartyManager();
        public static InventoryManager Inventory { get; } = new InventoryManager();
        public static PetManager Pet { get; } = new PetManager();
        public static WorldManager World { get; } = new WorldManager();
        
        public static GameObject Target => Player?.CurrentTarget;
        public static LocalPlayer Player => Objects.LocalPlayer;
        public static bool IsInGame => Player != null;

        static Ffxiv() {
            AppDomain.CurrentDomain.ProcessExit += NativeOnExited;
            LoadOffsets();
        }

        public static void NativeOnExited(object sender, EventArgs e) {
            m_Memory?.Dispose();
            m_Memory = null;
            Offsets = null;
            Objects?.Clear();
            Party?.Clear();
        }

        private static void LoadOffsets() {
            if (!File.Exists(OffsetFile)) {
                Offsets = new FfxivOffsets();
                Offsets.Update();
                return;
            }
            Debug.WriteLine($"Loading offsets from file: {OffsetFile}");
            using (var fs = new FileStream(OffsetFile, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                var xml = new XmlSerializer(typeof(FfxivOffsets));
                Offsets = xml.Deserialize(fs) as FfxivOffsets;
            }
            Offsets?.Update();
        }

        //private static void SaveOffsetFile() {
        //    using (var fs = new FileStream(OffsetFile, FileMode.Create)) {
        //        var xml = new XmlSerializer(typeof(FfxivOffsets));
        //        xml.Serialize(fs, Offsets);
        //    }
        //}
    }
}