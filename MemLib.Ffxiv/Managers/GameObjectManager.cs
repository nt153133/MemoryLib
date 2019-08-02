using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MemLib.Ffxiv.Enums;
using MemLib.Ffxiv.Objects;

namespace MemLib.Ffxiv.Managers {
    public sealed class GameObjectManager {
        private readonly FfxivProcess m_Process;
        private readonly Dictionary<uint, GameObject> m_CachedEntities = new Dictionary<uint, GameObject>();

        public const int MaxObjects = 424;
        public const uint EmptyGameObject = 0xE0000000;

        public IEnumerable<GameObject> GameObjects => m_CachedEntities.Values;

        private IntPtr m_LocalPlayerPtr;
        private LocalPlayer m_LocalPlayer;
        public LocalPlayer LocalPlayer {
            get {
                if (!m_Process.Read<IntPtr>(m_Process.Offsets.ObjectListPtr, out var pointer) || pointer == IntPtr.Zero)
                    return null;
                if (m_LocalPlayer != null && m_LocalPlayerPtr == pointer)
                    return m_LocalPlayer;
                m_LocalPlayerPtr = pointer;
                m_LocalPlayer = new LocalPlayer(m_Process, pointer);
                return m_LocalPlayer;
            }
        }

        public GameObject Target => GetTarget();
        public BattleCharacter CurrentPet => GetPet();
        //public HashSet<BattleCharacter> Attackers { get; }

        internal GameObjectManager(FfxivProcess process) {
            m_Process = process;
        }

        public void Clear() {
            m_CachedEntities.Clear();
        }

        public void Update() {
            foreach (var gameObject in GameObjects) {
                gameObject.UpdatePointer(IntPtr.Zero);
            }

            foreach (var entity in GetRawEntities()) {
                if (entity == null || entity.BaseAddress == IntPtr.Zero) continue;
                var objId = entity.ObjectId;
                if (m_CachedEntities.TryGetValue(objId, out var gameObject))
                    gameObject.UpdatePointer(entity.BaseAddress);
                else m_CachedEntities.Add(objId, entity);
            }

            var invalidObjKeys = m_CachedEntities.Where(kv => kv.Value.BaseAddress == IntPtr.Zero).Select(kv => kv.Key).ToList();
            foreach (var invalidKey in invalidObjKeys) {
                m_CachedEntities.Remove(invalidKey);
            }
            Debug.WriteLine($"Cached: {m_CachedEntities.Count}");
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

        private IEnumerable<GameObject> GetRawEntities() {
            if (!m_Process.Read<IntPtr>(m_Process.Offsets.ObjectListPtr, out var ptrArray, MaxObjects))
                yield break;
            foreach (var ptr in ptrArray.Where(p => p != IntPtr.Zero).Distinct()) {
                var type = (GameObjectType)m_Process.Read<byte>(ptr + m_Process.Offsets.Character.ObjectType);
                switch (type) {
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
                        yield return new GameObject(m_Process, ptr);
                        break;
                }
            }
        }

        internal GameObject GetObjectByPtr(IntPtr ptr) {
            return GameObjects.FirstOrDefault(o => o.BaseAddress == ptr);
        }

        public GameObject GetObjectByName(string name, bool matchPartial = false) {
            if(matchPartial)
                return GameObjects.FirstOrDefault(o => o.Name.ToUpper().Contains(name.ToUpper()));
            return GameObjects.FirstOrDefault(o => o.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<GameObject> GetObjectsByName(string name, bool matchPartial = false) {
            if (matchPartial)
                return GameObjects.Where(o => o.Name.ToUpper().Contains(name.ToUpper()));
            return GameObjects.Where(o => o.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public GameObject GetObjectByObjectId(uint objectId) {
            return m_CachedEntities.TryGetValue(objectId, out var gameObject) ? gameObject : null;
        }

        public T GetObjectByObjectId<T>(uint objectId) where T : GameObject {
            return m_CachedEntities.TryGetValue(objectId, out var gameObject) ? gameObject as T : null;
        }

        public GameObject GetObjectByNpcId(uint npcId) {
            return npcId == 0u ? null : GameObjects.FirstOrDefault(o => o.NpcId == npcId);
        }

        public T GetObjectByNpcId<T>(uint npcId) where T : GameObject {
            return npcId == 0u ? null : GameObjects.FirstOrDefault(o => o.NpcId == npcId) as T;
        }

        public IEnumerable<GameObject> GetObjectsByNpcId(uint npcId) {
            return npcId == 0u ? Enumerable.Empty<GameObject>() : GameObjects.Where(o => o.NpcId == npcId);
        }

        public IEnumerable<T> GetObjectsByNpcId<T>(uint npcId) where T : GameObject {
            return npcId == 0u ? Enumerable.Empty<T>() : GameObjects.Where(o => o.NpcId == npcId).Select(o => o as T);
        }

        public T[] GetObjectsByNpcIds<T>(uint[] npcIds) where T : GameObject {
            return GameObjects.Where(o => npcIds.Contains(o.NpcId)).Select(o => o as T).ToArray();
        }

        public IEnumerable<GameObject> GetObjectsByObjectIds(IEnumerable<uint> objectIds) {
            return GameObjects.Where(o => objectIds.Contains(o.ObjectId));
        }

        public IEnumerable<T> GetObjectsOfType<T>(bool allowInheritance = false, bool includeMeIfFound = false) where T : GameObject {
            var localPlayerId = LocalPlayer.ObjectId;
            foreach (var gameObject in GameObjects) {
                if ((!allowInheritance || !(gameObject is T)) && typeof(T) != gameObject.GetType()) continue;
                if (includeMeIfFound || gameObject.ObjectId != localPlayerId)
                    yield return gameObject as T;
            }
        }
    }
}