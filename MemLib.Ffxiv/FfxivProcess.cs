﻿using MemLib.Ffxiv.Managers;
using MemLib.Ffxiv.Objects;
using MemLib.Ffxiv.Offsets;

namespace MemLib.Ffxiv {
    public class FfxivProcess : RemoteProcess {
        private OffsetManager m_Offsets;
        private GameObjectManager m_GameObjects;
        private InventoryManager m_Inventory;
        private PartyManager m_Party;
        private PetManager m_Pet;
        private WorldManager m_World;

        public OffsetManager Offsets => m_Offsets ?? (m_Offsets = new OffsetManager(this));
        public GameObjectManager GameObjects => m_GameObjects ?? (m_GameObjects = new GameObjectManager(this));
        public InventoryManager Inventory => m_Inventory ?? (m_Inventory = new InventoryManager(this));
        public PartyManager Party => m_Party ?? (m_Party = new PartyManager(this));
        public PetManager Pet => m_Pet ?? (m_Pet = new PetManager(this));
        public WorldManager World => m_World ?? (m_World = new WorldManager(this));

        public LocalPlayer Player => GameObjects.LocalPlayer;
        
        public FfxivProcess() : base(FindProcess("ffxiv_dx11")){}

        public void Update() {
            Native?.Refresh();
            GameObjects?.Update();
            Party?.Update();
        }
    }
}