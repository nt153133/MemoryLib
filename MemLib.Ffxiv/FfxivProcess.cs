using MemLib.Ffxiv.Managers;
using MemLib.Ffxiv.Objects;
using MemLib.Ffxiv.Offsets;

namespace MemLib.Ffxiv {
    public class FfxivProcess : RemoteProcess {
        private OffsetManager m_Offsets;
        internal OffsetManager Offsets => m_Offsets ?? (m_Offsets = new OffsetManager(this));
        private GameObjectManager m_GameObjects;
        public GameObjectManager GameObjects => m_GameObjects ?? (m_GameObjects = new GameObjectManager(this));
        private InventoryManager m_Inventory;
        public InventoryManager Inventory => m_Inventory ?? (m_Inventory = new InventoryManager(this));

        public LocalPlayer Player => GameObjects.LocalPlayer;

        public FfxivProcess() : base(FindProcess("ffxiv_dx11")){}
    }
}