using System;
using System.Reflection;
using System.Text;
using MemLib.Ffxiv.Enumerations;

namespace MemLib.Ffxiv.Objects {
    public class PartyMember : RemoteObject {
        protected internal int Index;
        public GameObject GameObject => ObjectId > 0u ? m_Process.GameObjects.GetObjectByObjectId(ObjectId) : null;
        public BattleCharacter BattleCharacter => GameObject as BattleCharacter;
        public bool IsInObjectManager => GameObject != null;
        public virtual bool IsMe => m_Process.GameObjects.LocalPlayer.ObjectId == ObjectId;
        public override bool IsValid => base.IsValid && ObjectId > 0u && ObjectId != 0xE0000000;
        public virtual ClassJobType ClassJob => m_Process.Read<ClassJobType>(BaseAddress + m_Process.Offsets.Party.ClassJob);
        public virtual uint Level => m_Process.Read<byte>(BaseAddress + m_Process.Offsets.Party.Level);
        public virtual uint CurrentHealth => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Party.Health);
        public virtual uint MaxHealth => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Party.Health + 4);
        public virtual uint CurrentMana => m_Process.Read<ushort>(BaseAddress + m_Process.Offsets.Party.Mana);
        public virtual uint MaxMana => m_Process.Read<ushort>(BaseAddress + m_Process.Offsets.Party.Mana + 2);
        public virtual string Name => m_Process.ReadString(BaseAddress + m_Process.Offsets.Party.Name, 64);
        public virtual uint ObjectId => m_Process.Read<uint>(BaseAddress + m_Process.Offsets.Party.ObjectId);
        public virtual ulong PartyObjectId => 0ul;
        public virtual bool IsPartyLeader { get; }
        public virtual bool IsXRealm => false;

        internal PartyMember(FfxivProcess process, IntPtr baseAddress, int index) : base(process, baseAddress) {
            Index = index;
        }

        #region Overrides of RemoteObject

        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine($"{GetType().Name}:");
            foreach (var propInfo in typeof(PartyMember).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy))
                sb.AppendFormat("    {0}:{1}\n", propInfo.Name.PadRight(20), propInfo.GetValue(this) ?? "null");
            return sb.ToString();
        }

        #endregion
    }
}