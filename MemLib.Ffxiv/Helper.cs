using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MemLib.Ffxiv {
    internal static class Helper {
        public static string ObjectToString(object obj, params string[] filter) {
            var sb = new StringBuilder();
            sb.AppendLine($"{obj.GetType().Name}:");
            var props = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            foreach (var propInfo in props.Where(p => !filter.Any(s => s.Equals(p.Name, StringComparison.OrdinalIgnoreCase))))
                sb.AppendFormat("{0}:{1}\n", propInfo.Name.PadRight(20), propInfo.GetValue(obj) ?? "null");
            return sb.ToString();
        }
    }
}