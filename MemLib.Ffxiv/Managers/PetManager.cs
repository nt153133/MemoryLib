using System;
using MemLib.Ffxiv.Enumerations;
using MemLib.Ffxiv.Objects;

namespace MemLib.Ffxiv.Managers {
    public sealed class PetManager {
        public PetMovement PetMovement => Ffxiv.Memory.Read<PetMovement>(Ffxiv.Offsets.PetInfo);
        public PetStance PetStance => Ffxiv.Memory.Read<PetStance>(Ffxiv.Offsets.PetInfo + 1);
        public PetMode PetMode => Ffxiv.Memory.Read<PetMode>(Ffxiv.Offsets.PetInfo + 2);
        public PetType ActivePetType {
            get {
                var ptr = Ffxiv.Memory.Read<IntPtr>(Ffxiv.Offsets.PetStats);
                return ptr == IntPtr.Zero ? 0 : Ffxiv.Memory.Read<PetType>(ptr + 0xC);
            }
        }
        public uint PetObjectId {
            get {
                var ptr = Ffxiv.Memory.Read<IntPtr>(Ffxiv.Offsets.PetStats);
                return ptr == IntPtr.Zero ? GameObjectManager.EmptyGameObject : Ffxiv.Memory.Read<uint>(ptr);
            }
        }
        public BattleCharacter CurrentPet {
            get {
                Ffxiv.Objects.Update();
                return Ffxiv.Objects.CurrentPet;
            }
        }

        internal PetManager() { }
    }
}