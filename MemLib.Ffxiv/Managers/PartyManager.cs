using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MemLib.Ffxiv.Objects;

namespace MemLib.Ffxiv.Managers {
    public sealed class PartyManager {
        private readonly ConcurrentDictionary<uint, PartyMember> m_CachedEntities = new ConcurrentDictionary<uint, PartyMember>();
        public IEnumerable<PartyMember> AllMembers => m_CachedEntities.Values;

        public bool IsInParty => NumMembers > 0u;
        public uint NumMembers => Ffxiv.Memory.Read<byte>(Ffxiv.Offsets.PartyCount);
        public PartyMember PartyLeader => null;
        public bool IsPartyLeader => NumMembers > 0u && PartyLeader.IsMe;
        public ulong PartyId => 0ul;
        internal int PartyLeaderIndex => 0;

        public List<PartyMember> RawMembers {
            get {
                var list = new List<PartyMember>();
                var count = NumMembers;
                for (var i = 0; i < count; i++) {
                    var addr = Ffxiv.Offsets.PartyList + i * Ffxiv.Offsets.PartyOffsets.MemberObjSize;
                    var member = new PartyMember(addr, i);
                    if (member.IsValid)
                        list.Add(member);
                }
                return list;
            }
        }

        internal PartyManager() { }

        public void Clear() {
            m_CachedEntities.Clear();
        }

        public void Update() {
            if (Ffxiv.Memory == null) return;
            foreach (var member in m_CachedEntities.Values) {
                member.UpdatePointer(IntPtr.Zero);
            }

            foreach (var member in RawMembers) {
                if (member == null || member.BaseAddress == IntPtr.Zero) continue;
                var objectId = member.ObjectId;
                if (m_CachedEntities.TryGetValue(objectId, out var obj))
                    obj.UpdatePointer(member.BaseAddress);
                else m_CachedEntities.GetOrAdd(objectId, member);
            }

            var invalidObjKeys = m_CachedEntities.Where(kv => kv.Value.BaseAddress == IntPtr.Zero).Select(kv => kv.Key).ToList();
            foreach (var invalidKey in invalidObjKeys) {
                m_CachedEntities.TryRemove(invalidKey, out _);
            }
        }
    }
}