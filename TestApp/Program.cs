using System;
using System.Linq;
using MemLib;
using MemLib.Ffxiv;
using MemLib.Ffxiv.Enums;
using MemLib.Ffxiv.Objects;

namespace TestApp {
    internal class Program {
        private static void Test() {
            using (var ff = new FfxivProcess()) {
                ff.GameObjects.Update();
                foreach (var character in ff.GameObjects.GetObjectsOfType<BattleCharacter>(true, true).Where(o => o.IsMounted)) {
                    Print($"{character} -> {character.MountId}");
                }
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