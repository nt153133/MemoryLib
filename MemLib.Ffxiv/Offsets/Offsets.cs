using System.Reflection;
using System.Text;
using MemLib.Ffxiv.Offsets.Structs;

namespace MemLib.Ffxiv.Offsets {
    public sealed class Offsets {
        public PlayerInfoOffsets PlayerInfo { get; set; } = new PlayerInfoOffsets();
        public CharacterOffsets Character { get; set; } = new CharacterOffsets();
        public ItemOffsets Item { get; set; } = new ItemOffsets();

        #region Overrides of Object

        public override string ToString() {
            var sb = new StringBuilder();
            foreach (var property in typeof(Offsets).GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
                sb.AppendFormat("[{0}]\n", property.PropertyType.Name);
                foreach (var prop in property.PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
                    sb.AppendFormat("  {0}:{1:X}\n", prop.Name.PadRight(15), (int)prop.GetValue(property.GetValue(this)));
                }
            }
            return sb.ToString();
        }

        #endregion
    }
}