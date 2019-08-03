using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace MemLib.Ffxiv.Structures {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Stats {
        public override string ToString() {
            var stringBuilder = new StringBuilder();
            foreach (var fieldInfo in typeof(Stats).GetFields(BindingFlags.Instance | BindingFlags.Public))
                stringBuilder.AppendFormat("{0}:{1}\n", fieldInfo.Name.PadRight(25, ' '), fieldInfo.GetValue(this));
            return stringBuilder.ToString();
        }

        public uint StrengthBase;
        public uint DexterityBase;
        public uint VitalityBase;
        public uint IntelligenceBase;
        public uint MindBase;
        public uint PietyBase;
        private readonly uint padding0;
        public uint Strength;
        public uint Dexterity;
        public uint Vitality;
        public uint Intelligence;
        public uint Mind;
        public uint Piety;
        public uint HP;
        public uint MP;
        public uint TP;
        public uint GP;
        public uint CP;
        private readonly uint padding1;
        private readonly uint padding2;
        private readonly uint padding3;
        private readonly uint padding4;
        private readonly uint padding5;
        private readonly uint padding6;
        private readonly uint padding7;
        public uint Tenacity;
        public uint AttackPower;
        public uint Defense;
        public uint DirectHitRate;
        private readonly uint padding8;
        public uint MagicDefense;
        private readonly uint padding9;
        private readonly uint padding10;
        public uint CriticalHit;
        private readonly uint padding11;
        private readonly uint padding12;
        private readonly uint padding13;
        private readonly uint padding14;
        private readonly uint padding15;
        public uint AttackMagicPotency;
        public uint HealingMagicPotency;
        private readonly uint padding16;
        private readonly uint padding17;
        private readonly uint padding18;
        private readonly uint padding19;
        private readonly uint padding20;
        private readonly uint padding21;
        private readonly uint padding22;
        private readonly uint padding23;
        private readonly uint padding24;
        private readonly uint padding25;
        public uint SkillSpeed;
        public uint SpellSpeed;
        private readonly uint padding26;
        private readonly uint padding27;
        private readonly uint padding28;
        private readonly uint padding29;
        private readonly uint padding30;
        private readonly uint padding31;
        private readonly uint padding32;
        private readonly uint padding33;
        private readonly uint padding34;
        private readonly uint padding35;
        private readonly uint padding36;
        private readonly uint padding37;
        private readonly uint padding38;
        private readonly uint padding39;
        private readonly uint padding40;
        private readonly uint padding41;
        private readonly uint padding42;
        private readonly uint padding43;
        private readonly uint padding44;
        private readonly uint padding45;
        private readonly uint padding46;
        private readonly uint padding47;
        private readonly uint padding48;
        public uint Craftsmanship;
        public uint Control;
        public uint Gathering;
        public uint Perception;
    }
}