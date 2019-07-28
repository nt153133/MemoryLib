using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MemLib.Modules;

namespace MemLib.Pattern {
    public class PatternFinder {
        private readonly RemoteProcess m_Process;
        private readonly RemoteModule m_Module;
        private byte[] m_Data;
        public byte[] Data => m_Data ?? (m_Data = m_Process.Read<byte>(m_Module.BaseAddress, m_Module.Size));
        private readonly HashSet<string> m_Tokens = new HashSet<string> {
            "Search", "TraceRelative",
            "Read8", "Read16", "Read32", "Read64",
            "Add", "Sub"
        };
        
        public IntPtr BaseAddress => m_Module.BaseAddress;
        public SearchAlgorithm SearchAlgorithm { get; set; } = SearchAlgorithm.BoyerMooreHorspool;

        public PatternFinder(RemoteProcess process) {
            m_Process = process;
            m_Module = process.MainModule;
        }

        public PatternFinder(RemoteProcess process, string moduleName) {
            m_Process = process;
            var module = process[moduleName];
            m_Module = module ?? throw new ArgumentException($"Module '{moduleName}' not found.", nameof(moduleName));
        }

        public IntPtr Find(string pattern) {
            var results = FindMany(pattern, true);
            if(results.Count == 0)
                throw new ArgumentException("Pattern not found.", nameof(pattern));
            return results[0];
        }

        public List<IntPtr> FindMany(string pattern, bool matchFirstOnly = false) {
            if (!pattern.StartsWith("Search")) pattern = "Search " + pattern;
            var results = new List<IntPtr>();
            var tokens = pattern.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            var index = 0;
            do {
                var text = tokens[index];
                if (text == "Search") {
                    var intPattern = new List<int>();
                    index++;
                    var byteString = tokens[index];
                    string[] splitBytes = null;
                    var splitindex = 0;
                    if (byteString.Length > 2) {
                        splitBytes = SplitByteString(byteString).ToArray();
                        byteString = splitBytes[splitindex];
                        index++;
                    }
                    while (!IsToken(byteString)) {
                        if (IsWildcard(byteString)) {
                            intPattern.Add(byte.MaxValue + 1); // wildcard = 256
                        } else {
                            if (byte.TryParse(byteString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var val))
                                intPattern.Add(val);
                            else
                                throw new ArgumentException("Could not parse search byte: " + byteString, nameof(pattern));
                        }
                        if (splitBytes == null) {
                            index++;
                            if (index >= tokens.Length) break;
                            byteString = tokens[index];
                        } else {
                            splitindex++;
                            if (splitindex >= splitBytes.Length) break;
                            byteString = splitBytes[splitindex];
                        }
                    }
                    if (intPattern.Count == 0)
                        throw new ArgumentException("Empty 'Search' token found.", nameof(pattern));
                    results = FindMany(intPattern.ToArray(), 0, matchFirstOnly);
                    if (results.Count == 0)
                        return null;
                    for (var i = 0; i < results.Count; i++)
                        results[i] = new IntPtr(BaseAddress.ToInt64() + results[i].ToInt64());
                } else if (text == "Add") {
                    index++;
                    if (int.TryParse(tokens[index], NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                        out var val)) {
                        for (var i = 0; i < results.Count; i++)
                            results[i] = new IntPtr(results[i].ToInt64() + val);
                        index++;
                    } else {
                        throw new ArgumentException("Could not parse 'Add' arg: " + tokens[index], nameof(pattern));
                    }
                } else if (text == "Sub") {
                    index++;
                    if (int.TryParse(tokens[index], NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                        out var val)) {
                        for (var i = 0; i < results.Count; i++)
                            results[i] = new IntPtr(results[i].ToInt64() - val);
                        index++;
                    } else {
                        throw new ArgumentException("Could not parse 'Sub' arg: " + tokens[index], nameof(pattern));
                    }
                } else if (text == "TraceRelative") {
                    for (var i = 0; i < results.Count; i++) {
                        if(results[i] == IntPtr.Zero) continue;
                        var pointer = results[i];
                        var offset = (int) (pointer.ToInt64() - BaseAddress.ToInt64());
                        if (offset + 4 >= m_Data.Length)
                            throw new ArgumentException("Address pointer was at an invalid address when 'TraceRelative' was encountered", nameof(pattern));
                        results[i] = pointer + 4 + BitConverter.ToInt32(m_Data, offset);
                    }
                    index++;
                } else if (text == "Read8") {
                    for (var i = 0; i < results.Count; i++) {
                        if(results[i] != IntPtr.Zero && m_Process.Read<byte>(results[i], out var val8))
                            results[i] = new IntPtr(val8);
                        else results[i] = IntPtr.Zero;
                    }
                    index++;
                } else if (text == "Read16") {
                    for (var i = 0; i < results.Count; i++) {
                        if(results[i] != IntPtr.Zero && m_Process.Read<short>(results[i], out var val16))
                            results[i] = new IntPtr(val16);
                        else results[i] = IntPtr.Zero;
                    }
                    index++;
                } else if (text == "Read32") {
                    for (var i = 0; i < results.Count; i++) {
                        if(results[i] != IntPtr.Zero && m_Process.Read<int>(results[i], out var val32))
                            results[i] = new IntPtr(val32);
                        else results[i] = IntPtr.Zero;
                    }
                    index++;
                } else if (text == "Read64") {
                    for (var i = 0; i < results.Count; i++) {
                        if(results[i] != IntPtr.Zero && m_Process.Read<long>(results[i], out var val64))
                            results[i] = new IntPtr(val64);
                        else results[i] = IntPtr.Zero;
                    }
                    index++;
                }
            } while (index < tokens.Length);

            return results;
        }

        private List<IntPtr> FindMany(int[] pattern, int startIndex, bool matchFirstOnly) {
            var results = new List<IntPtr>();
            do {
                var index = IndexOf(Data, pattern, startIndex, SearchAlgorithm);
                if (index == -1) break;
                results.Add(new IntPtr(index));
                if (matchFirstOnly) break;
                startIndex += index + pattern.Length;
            } while (startIndex < Data.Length);
            return results;
        }

        public static int IndexOf(byte[] data, int[] pattern, int startIndex = 0, SearchAlgorithm algorithm = SearchAlgorithm.BoyerMooreHorspool) {
            switch (algorithm) {
                case SearchAlgorithm.Naive:
                    return Naive.IndexOf(pattern, data, startIndex);
                case SearchAlgorithm.BoyerMooreHorspool:
                    return BoyerMooreHorspool.IndexOf(data, pattern, startIndex);
                default:
                    throw new ArgumentException("Unknown Search Algorithm.", nameof(algorithm));
            }
        }

        private static IEnumerable<string> SplitByteString(string str) {
            return Enumerable.Range(0, (int)Math.Ceiling(str.Length / 2D))
                .Select(i => string.Concat(str.Skip(i * 2).Take(2)));
        }

        private bool IsToken(string text) {
            return m_Tokens.Contains(text.Trim());
        }

        private static bool IsWildcard(string text) {
            return text.Equals("?", StringComparison.Ordinal) ||
                   text.Equals("??", StringComparison.Ordinal) ||
                   text.Equals("*", StringComparison.Ordinal) ||
                   text.Equals("**", StringComparison.Ordinal);
        }
    }
}