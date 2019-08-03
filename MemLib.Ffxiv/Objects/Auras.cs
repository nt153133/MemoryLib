using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MemLib.Ffxiv.Structures;

namespace MemLib.Ffxiv.Objects {
    public class Auras : RemoteObject, IEnumerable<Aura> {
        private readonly List<Aura> m_AuraList = new List<Aura>();
        public List<Aura> AuraList {
            get {
                m_AuraList.Clear();
                var list = m_Process.Read<AuraData>(BaseAddress, 30);
                for (var i = 0; i < 30; i++) {
                    if(list[i].AuraId > 0)
                        m_AuraList.Add(new Aura(m_Process, list[i], i));
                }
                return m_AuraList;
            }
        }

        internal Auras(FfxivProcess process, IntPtr baseAddress) : base(process, baseAddress) { }

        public int GetAuraStacksById(uint id) {
            var aura = AuraList.FirstOrDefault(a => a.Id == id);
            return aura != null ? (int) aura.Value : -1;
        }

        #region Implementation of IEnumerable

        public IEnumerator<Aura> GetEnumerator() {
            return AuraList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion
    }
}