using System;

namespace MemLib.Ffxiv.Objects {
    public class Aura {
        private readonly AuraData m_Data;
        public int Index { get; }

        public uint Id => m_Data.AuraId;
        public uint Value => m_Data.AuraValue;
        public float TimeLeft => m_Data.TimeLeft;
        public TimeSpan TimespanLeft => TimeSpan.FromSeconds(m_Data.TimeLeft);
        public uint CasterId => m_Data.CasterId;
        public GameObject Caster => Ffxiv.Objects.GetObjectByObjectId(m_Data.CasterId);

        internal Aura(AuraData data, int index) {
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