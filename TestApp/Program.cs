using System;
using System.Linq;
using MemLib.Ffxiv;
using MemLib.Ffxiv.Objects;

namespace TestApp {
    internal class Program {
        private static void Test() {
            using (var ff = new FfxivProcess()) {
                ff.GameObjects.Update();
                foreach (var obj in ff.GameObjects.GetObjectsOfType<Character>(true, true)) {
                    Print($"{obj}");
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