namespace MemLib.Pattern {
    internal static class BoyerMooreHorspool {
        private const int WildCard = byte.MaxValue + 1;

        private static int[] BuildBadCharTable(int[] pPattern) {
            int idx;
            var last = pPattern.Length - 1;
            var badShift = new int[257];

            for (idx = last; idx > 0 && pPattern[idx] != WildCard; --idx) { }
            var diff = last - idx;
            if (diff == 0)
                diff = 1;
            
            for (idx = 0; idx <= 256; ++idx)
                badShift[idx] = diff;
            for (idx = last - diff; idx < last; ++idx)
                badShift[pPattern[idx]] = last - idx;
            return badShift;
        }

        public static int IndexOf(byte[] buffer, int[] pattern, int startIndex) {
            if (pattern.Length > buffer.Length) return -1;
            var badShift = BuildBadCharTable(pattern);
            var offset = startIndex;
            var last = pattern.Length - 1;
            var maxoffset = buffer.Length - pattern.Length;
            while (offset <= maxoffset) {
                int position;
                for (position = last; pattern[position] == buffer[position + offset] || pattern[position] == WildCard; position--)
                    if (position == 0)
                        return offset;
                offset += badShift[buffer[offset + last]];
            }
            return -1;
        }
    }
}