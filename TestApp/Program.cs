using System;
using System.Linq;
using MemLib.Ffxiv;
using MemLib.Ffxiv.Enumerations;
using MemLib.Ffxiv.Objects;

namespace TestApp {
    internal class Program {
        private static void Test() {
            using (var ff = new FfxivProcess()) {
                ff.GameObjects.Update();
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