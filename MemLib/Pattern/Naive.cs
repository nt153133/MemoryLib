using System.Linq;

namespace MemLib.Pattern {
    internal static class Naive {
        private const int WildCard = byte.MaxValue + 1;

        public static int IndexOf(int[] pattern, byte[] data, int startIndex) {
            for (var offset = startIndex; offset < data.Length; offset++) {
                if (pattern.Where((m, b) => m != WildCard && pattern[b] != data[b + offset]).Any())
                    continue;
                return offset;
            }

            return -1;
        }
    }
}