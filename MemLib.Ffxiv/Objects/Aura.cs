using System;
using MemLib.Ffxiv.Structures;

namespace MemLib.Ffxiv.Objects {
    public class Aura {
        private readonly FfxivProcess m_Process;
        private readonly AuraData m_Data;
        public int Index { get; }

        public uint Id => m_Data.AuraId;
        public uint Value => m_Data.AuraValue;
        public float TimeLeft => m_Data.TimeLeft;
        public TimeSpan TimespanLeft => TimeSpan.FromSeconds(m_Data.TimeLeft);
        public uint CasterId => m_Data.CasterId;
        public GameObject Caster => m_Process.GameObjects.GetObjectByObjectId(m_Data.CasterId);

        internal Aura(FfxivProcess process, AuraData data, int index) {
            m_Process = process;
            m_Data = data;
            Index = index;
        }

        #region Overrides of Object

        public override string ToString() {
            return $"AuraId:{Id} CasterId:{CasterId:X} Value:{Value} TimeLeft:{TimeLeft:F}";
        }

        #endregion
    }
}