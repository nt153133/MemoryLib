using System;
using System.Collections.Generic;
using System.Linq;
using MemLib.Ffxiv.Enums;
using MemLib.Ffxiv.Objects;

namespace MemLib.Ffxiv.Managers {
    public sealed class GameObjectManager {
        private readonly FfxivProcess m_Process;
        public const int MaxObjects = 424;
        public const uint EmptyGameObject = 0xE0000000;

        public IEnumerable<GameObject> AllGameObjects => GetRawEntities();
        public LocalPlayer LocalPlayer => GetLocalPlayer();
        public GameObject Target => GetTarget();
        public BattleCharacter CurrentPet => GetPet();
        //public HashSet<BattleCharacter> Attackers { get; }

        internal GameObjectManager(FfxivProcess process) {
            m_Process = process;
        }

        private BattleCharacter GetPet() {
            if (m_Process.Read<uint>(m_Process.Offsets.PetPtr, out var petId))
                return GetObjectByObjectId<BattleCharacter>(petId);
            return null;
        }

        private GameObject GetTarget() {
            if(m_Process.Read<IntPtr>(m_Process.Offsets.TargetingPtr + 0x88, out var ptr) && ptr != IntPtr.Zero)
                return new GameObject(m_Process, ptr);
            return null;
        }

        internal GameObject GetObjectByPtr(IntPtr ptr) {
            return AllGameObjects.FirstOrDefault(o => o.BaseAddress == ptr);
        }

        private IEnumerable<GameObject> GetRawEntities() {
            if (!m_Process.Read<IntPtr>(m_Process.Offsets.ObjectListPtr, out var ptrArray, MaxObjects))
                yield break;
            foreach (var ptr in ptrArray.Where(p => p != IntPtr.Zero).Distinct()) {
                var type = (GameObjectType) m_Process.Read<byte>(ptr + m_Process.Offsets.Character.ObjectType);
                //yield return new Character(m_Process, ptr);
                //continue;
                switch (type) {
                    case GameObjectType.Unknown:
                    case GameObjectType.None:
                        yield return new GameObject(m_Process, ptr);
                        break;
                    case GameObjectType.Pc:
                    case GameObjectType.BattleNpc:
                        yield return new BattleCharacter(m_Process, ptr);
                        break;
                    case GameObjectType.Minion:
                        yield return new Minion(m_Process, ptr);
                        break;
                    case GameObjectType.AetheryteObject:
                        yield return new Aetheryte(m_Process, ptr);
                        break;
                    case GameObjectType.Treasure:
                        yield return new Treasure(m_Process, ptr);
                        break;
                    case GameObjectType.EventObject:
                    case GameObjectType.EventNpc:
                        yield return new EventObject(m_Process, ptr);
                        break;
                    case GameObjectType.GatheringPoint:
                        yield return new GatheringPointObject(m_Process, ptr);
                        break;
                    case GameObjectType.Mount:
                    case GameObjectType.Retainer:
                        yield return new Character(m_Process, ptr);
                        break;
                    case GameObjectType.HousingEventObject:
                        yield return new HousingObject(m_Process, ptr);
                        break;
                    default:
                        yield break;
                }
            }
        }

        private LocalPlayer GetLocalPlayer() {
            var addr = m_Process.Offsets.PlayerInfoPtr + m_Process.Offsets.PlayerInfo.ObjectId;
            if (m_Process.Read<uint>(addr, out var id) && id != 0u) {
                return new LocalPlayer(m_Process, GetObjectByObjectId(id).BaseAddress);
            }
            return null;
        }

        public GameObject GetObjectByName(string name, bool matchPartial = false) {
            if(matchPartial)
                return AllGameObjects.FirstOrDefault(o => o.Name.ToUpper().Contains(name.ToUpper()));
            return AllGameObjects.FirstOrDefault(o => o.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public GameObject GetObjectByObjectId(uint objectId) {
            return objectId == 0u ? null : AllGameObjects.FirstOrDefault(o => o.ObjectId == objectId);
        }

        public T GetObjectByObjectId<T>(uint objectId) where T : GameObject {
            if (objectId == 0u) return null;
            return AllGameObjects.FirstOrDefault(o => o.ObjectId == objectId) as T;
        }

        public GameObject GetObjectByNpcId(uint npcId) {
            return npcId == 0u ? null : AllGameObjects.FirstOrDefault(o => o.NpcId == npcId);
        }

        public T GetObjectByNpcId<T>(uint npcId) where T : GameObject {
            if (npcId == 0u) return null;
            return AllGameObjects.FirstOrDefault(o => o.NpcId == npcId) as T;
        }

        public IEnumerable<GameObject> GetObjectsByNpcId(uint npcId) {
            return npcId == 0u ? Enumerable.Empty<GameObject>() : AllGameObjects.Where(o => o.NpcId == npcId);
        }

        public IEnumerable<T> GetObjectsByNpcId<T>(uint npcId) where T : GameObject {
            if (npcId == 0u) return Enumerable.Empty<T>();
            return AllGameObjects.Where(o => o.NpcId == npcId).Select(o => o as T);
        }

        public IEnumerable<T> GetObjectsByNpcIds<T>(params uint[] npcIds) where T : GameObject {
            return AllGameObjects.Where(o => npcIds.Contains(o.NpcId)).Select(o => o as T);
        }

        public IEnumerable<T> GetObjectsByObjectType<T>(GameObjectType type) where T : GameObject {
            return AllGameObjects.Where(o => o.Type == type).Select(o => o as T);
        }

        public IEnumerable<T> GetObjectsByType<T>() where T : GameObject {
            return AllGameObjects.OfType<T>();
        }
    }
}