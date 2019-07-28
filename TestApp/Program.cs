using System;
using System.Linq;
using MemLib;
using MemLib.Ffxiv;
using MemLib.Ffxiv.Enums;
using MemLib.Ffxiv.Objects;
using MemLib.Ffxiv.Offsets;

namespace TestApp {
    internal class Program {
        private static void Test() {
            using (var proc = new FfxivProcess()) {
                //Print(string.Join("\n", proc.Offsets.m_ResolvedSignatures.Select(kv => $"{kv.Key}:0x{kv.Value.ToInt64():X}")));
                //Print(proc.Offsets.m_Offsets);
                //Print(string.Join("\n", proc.GameObjects.GameObjects.Select(o => $"{o}")));
                //Print(proc.Player);
                //Print(string.Join("\n", proc.Player.Levels.Select(kv => $"{kv.Key.ToString().PadRight(15)}:{kv.Value}")));
                //var objs = proc.GameObjects.GetObjectsByType<Character>();
                //Print(string.Join("\n", objs.Select(o => $"{o} [{o.CurrentTargetId:X}]")));
            }
        }

        private static void Main() {
            Test();
            Console.ReadLine();
        }

        private static void Print(object obj) {
            Console.WriteLine(obj);
        }
    }
}