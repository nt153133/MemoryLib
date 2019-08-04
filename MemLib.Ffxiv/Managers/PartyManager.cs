using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MemLib.Ffxiv.Objects;

namespace MemLib.Ffxiv.Managers {
    public class PartyManager {
        private readonly FfxivProcess m_Process;
        private readonly ConcurrentDictionary<uint, PartyMember> m_CachedEntities = new ConcurrentDictionary<uint, PartyMember>();
        public IEnumerable<PartyMember> AllMembers => m_CachedEntities.Values;

        public PartyManager(FfxivProcess process) {
            m_Process = process;
        }

        public void Clear() {
            m_CachedEntities.Clear();
        }

        public void Update() {
            foreach (var member in AllMembers) {
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

        public List<PartyMember> RawMembers {
            get {
                var list = new List<PartyMember>();
                for (var i = 0; i < 8; i++) {
                    var addr = m_Process.Offsets.PartyListPtr + i * m_Process.Offsets.Party.MemberObjSize;
                    var member = new PartyMember(m_Process, addr, i);
                    if(member.IsValid)
                        list.Add(member);
                }
                return list;
            }
        }
    }
}