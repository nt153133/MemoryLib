﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MemLib.Ffxiv.Enumerations;
using MemLib.Ffxiv.Objects;

namespace MemLib.Ffxiv.Managers {
    public sealed class GameObjectManager {
        private readonly ConcurrentDictionary<uint, GameObject> m_CachedEntities = new ConcurrentDictionary<uint, GameObject>();

        public const int MaxObjects = 424;
        public const uint EmptyGameObject = 0xE0000000;

        public IEnumerable<GameObject> GameObjects => m_CachedEntities.Values;

        private IntPtr m_LocalPlayerPtr;
        private LocalPlayer m_LocalPlayer;
        public LocalPlayer LocalPlayer {
            get {
                if (!Ffxiv.Memory.Read<IntPtr>(Ffxiv.Offsets.ObjectList, out var pointer) || pointer == IntPtr.Zero)
                    return null;
                if (m_LocalPlayer != null && m_LocalPlayerPtr == pointer)
                    return m_LocalPlayer;
                m_LocalPlayerPtr = pointer;
                m_LocalPlayer = new LocalPlayer(pointer);
                return m_LocalPlayer;
            }
        }
        public GameObject Target {
            get {
                var addr = Ffxiv.Offsets.Targeting + Ffxiv.Offsets.TargetOffsets.CurrentTargetId;
                if (!Ffxiv.Memory.Read<uint>(addr, out var targetId)
                    || targetId == 0
                    || targetId == EmptyGameObject)
                    return null;
                return m_CachedEntities.TryGetValue(targetId, out var target) ? target : null;
            }
        }
        public BattleCharacter CurrentPet {
            get {
                if (Ffxiv.Memory.Read<uint>(Ffxiv.Offsets.Pet, out var petId))
                    return GetObjectByObjectId<BattleCharacter>(petId);
                return null;
            }
        }
        public IEnumerable<BattleCharacter> Monsters => GetObjectsByObjectType<BattleCharacter>(GameObjectType.BattleNpc);
        public IEnumerable<BattleCharacter> Players => GetObjectsByObjectType<BattleCharacter>(GameObjectType.Player);
        public IEnumerable<GameObject> Npcs => GetObjectsByObjectTypes(NpcTypes);
        public HashSet<BattleCharacter> Attackers { get; private set; } = new HashSet<BattleCharacter>();
        
        public void Clear() {
            m_CachedEntities.Clear();
        }

        public void Update() {
            if (Ffxiv.Memory == null) return;
            foreach (var gameObject in m_CachedEntities.Values) {
                gameObject.UpdatePointer(IntPtr.Zero);
            }

            var attackerIds = new HashSet<uint>();
            if (Ffxiv.Memory.Read<int>(Ffxiv.Offsets.AttackerCount, out var attackersCount) && attackersCount > 0) {
                for (var i = 0; i < attackersCount; i++) {
                    var id = Ffxiv.Memory.Read<uint>(Ffxiv.Offsets.AttackerList + i * 0x48);
                    if (id > 0 && id != EmptyGameObject)
                        attackerIds.Add(id);
                }
            }

            Attackers = new HashSet<BattleCharacter>();
            foreach (var entity in GetRawEntities()) {
                if (entity == null || entity.BaseAddress == IntPtr.Zero) continue;
                var objId = entity.ObjectId;
                if (objId == 0 || objId == EmptyGameObject) continue;

                if (m_CachedEntities.TryGetValue(objId, out var gameObject)) {
                    gameObject.UpdatePointer(entity.BaseAddress);
                    if (!attackerIds.Contains(objId)) continue;
                    if (gameObject is BattleCharacter attacker)
                        Attackers.Add(attacker);
                } else {
                    m_CachedEntities.GetOrAdd(objId, entity);
                    if (!attackerIds.Contains(objId)) continue;
                    if (entity is BattleCharacter attacker)
                        Attackers.Add(attacker);
                }
            }

            var invalidObjKeys = m_CachedEntities.Where(kv => kv.Value.BaseAddress == IntPtr.Zero).Select(kv => kv.Key).ToList();
            foreach (var invalidKey in invalidObjKeys) {
                m_CachedEntities.TryRemove(invalidKey, out _);
            }
        }

        private IEnumerable<GameObject> GetRawEntities() {
            if (!Ffxiv.Memory.Read<IntPtr>(Ffxiv.Offsets.ObjectList, out var ptrArray, MaxObjects))
                yield break;
            foreach (var ptr in ptrArray.Where(p => p != IntPtr.Zero).Distinct()) {
                var type = (GameObjectType)Ffxiv.Memory.Read<byte>(ptr + Ffxiv.Offsets.CharacterOffsets.ObjectType);
                switch (type) {
                    case GameObjectType.Player:
                    case GameObjectType.BattleNpc:
                        yield return new BattleCharacter(ptr);
                        break;
                    //case GameObjectType.Minion:
                    //    yield return new Minion(m_Process, ptr);
                    //    break;
                    //case GameObjectType.Aetheryte:
                    //    yield return new Aetheryte(m_Process, ptr);
                    //    break;
                    //case GameObjectType.Treasure:
                    //    yield return new Treasure(m_Process, ptr);
                    //    break;
                    //case GameObjectType.EventObj:
                    //case GameObjectType.EventNpc:
                    //    yield return new EventObject(m_Process, ptr);
                    //    break;
                    //case GameObjectType.GatheringPoint:
                    //    yield return new GatheringPointObject(m_Process, ptr);
                    //    break;
                    //case GameObjectType.Mount:
                    //case GameObjectType.Retainer:
                    //    yield return new Character(m_Process, ptr);
                    //    break;
                    //case GameObjectType.Housing:
                    //    yield return new HousingObject(m_Process, ptr);
                    //    break;
                    default:
                        yield return new GameObject(ptr);
                        break;
                }
            }
        }

        internal GameObject GetObjectByPtr(IntPtr ptr) {
            return GameObjects.FirstOrDefault(o => o.BaseAddress == ptr);
        }

        #region ByName

        public GameObject GetObjectByName(string name, bool matchPartial = false) {
            if (matchPartial)
                return GameObjects.FirstOrDefault(o => o.Name.ToUpper().Contains(name.ToUpper()));
            return GameObjects.FirstOrDefault(o => o.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<GameObject> GetObjectsByName(string name, bool matchPartial = false) {
            if (matchPartial)
                return GameObjects.Where(o => o.Name.ToUpper().Contains(name.ToUpper()));
            return GameObjects.Where(o => o.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region ByObjectId

        public GameObject GetObjectByObjectId(uint objectId) {
            return m_CachedEntities.TryGetValue(objectId, out var gameObject) ? gameObject : null;
        }

        public T GetObjectByObjectId<T>(uint objectId) where T : GameObject {
            return m_CachedEntities.TryGetValue(objectId, out var gameObject) ? gameObject as T : null;
        }


        public IEnumerable<GameObject> GetObjectsByObjectIds(params uint[] objectIds) {
            return m_CachedEntities.Where(kv => objectIds.Contains(kv.Key)).Select(kv => kv.Value);
        }

        #endregion

        #region ByNpcId

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

        #endregion

        #region ByType

        public IEnumerable<T> GetObjectsByObjectType<T>(GameObjectType type) where T : GameObject {
            return GameObjects.Where(o => o.Type == type).Select(o => o as T);
        }

        public IEnumerable<GameObject> GetObjectsByObjectType(GameObjectType type) {
            return GameObjects.Where(o => o.Type == type);
        }

        public IEnumerable<T> GetObjectsByObjectTypes<T>(params GameObjectType[] types) where T : GameObject {
            return GameObjects.Where(o => types.Contains(o.Type)).Select(o => o as T);
        }

        public IEnumerable<GameObject> GetObjectsByObjectTypes(params GameObjectType[] types) {
            return GameObjects.Where(o => types.Contains(o.Type));
        }

        public IEnumerable<T> GetObjectsOfType<T>(bool allowInheritance = false, bool includeMeIfFound = false) where T : GameObject {
            var localPlayerId = LocalPlayer.ObjectId;
            foreach (var gameObject in GameObjects) {
                if ((!allowInheritance || !(gameObject is T)) && typeof(T) != gameObject.GetType()) continue;
                if (includeMeIfFound || gameObject.ObjectId != localPlayerId)
                    yield return gameObject as T;
            }
        }

        #endregion

        #region ObjectTypeArrays

        private static readonly GameObjectType[] NpcTypes = {
            GameObjectType.Aetheryte,
            GameObjectType.Area,
            GameObjectType.CardStand,
            GameObjectType.Cutscene,
            GameObjectType.EventNpc,
            GameObjectType.EventObj,
            GameObjectType.GatheringPoint,
            GameObjectType.Housing,
            GameObjectType.Minion,
            GameObjectType.Mount,
            GameObjectType.Retainer,
            GameObjectType.Treasure,
        };

        #endregion
    }
}