using System;
using MemLib.Ffxiv.Enumerations;
using MemLib.Ffxiv.Objects;

namespace MemLib.Ffxiv.Managers {
    public sealed class PetManager {
        private readonly FfxivProcess m_Process;

        public PetMovement PetMovement => m_Process.Read<PetMovement>(m_Process.Offsets.PetInfoPtr);
        public PetStance PetStance => m_Process.Read<PetStance>(m_Process.Offsets.PetInfoPtr + 1);
        public PetMode PetMode => m_Process.Read<PetMode>(m_Process.Offsets.PetInfoPtr + 2);
        public PetType ActivePetType {
            get {
                var ptr = m_Process.Read<IntPtr>(m_Process.Offsets.PetStatsPtr);
                return ptr == IntPtr.Zero ? 0 : m_Process.Read<PetType>(ptr + 0xC);
            }
        }
        public uint PetObjectId {
            get {
                var ptr = m_Process.Read<IntPtr>(m_Process.Offsets.PetStatsPtr);
                return ptr == IntPtr.Zero ? GameObjectManager.EmptyGameObject : m_Process.Read<uint>(ptr);
            }
        }
        public BattleCharacter CurrentPet {
            get {
                m_Process.GameObjects.Update();
                return m_Process.GameObjects.CurrentPet;
            }
        }
        
        internal PetManager(FfxivProcess process) {
            m_Process = process;
        }
    }
}