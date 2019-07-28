using System;
using System.Linq;
using System.Xml.Serialization;

namespace MemLib.Ffxiv.Offsets {
    public class Signature {
        public string Key { get; set; }
        public string Value { get; set; }
        public int[] PointerPath { get; set; }

        [XmlIgnore]
        public IntPtr BaseAddress { get; set; } = IntPtr.Zero;

        public Signature(){}
        public Signature(string key, string value, int[] pointerPath = null) {
            Key = key;
            Value = value;
            PointerPath = pointerPath;
        }

        #region Overrides of Object

        public override string ToString() {
            return $"{Key.PadRight(15)}:{Value}" + 
                   (PointerPath == null || PointerPath.Length == 0 
                       ? "" 
                       : $" [{string.Join(",", PointerPath.Select(p => $"{p:X2}"))}]");
        }

        #endregion
    }
}