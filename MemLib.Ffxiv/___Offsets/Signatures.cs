using System.Reflection;
using System.Text;

namespace MemLib.Ffxiv.Offsets {
    public sealed class Signatures {
        public Signature ObjectList { get; set; } = new Signature("ObjectList", "488b420848c1e8033da701000077248bc0488d0d");
        public Signature PlayerInfo { get; set; } = new Signature("PlayerInfo", "83f9ff7412448b048e8bd3488d0d");
        public Signature Inventory { get; set; } = new Signature("Inventory", "8d81********85c075584c8b05");

        #region Overrides of Object

        public override string ToString() {
            var sb = new StringBuilder();
            foreach (var prop in typeof(Signatures).GetProperties(BindingFlags.Instance | BindingFlags.Public))
                sb.AppendFormat("[{0}] {1}\n", prop.Name.PadRight(15), prop.GetValue(this));
            return sb.ToString();
        }

        #endregion
    }
}