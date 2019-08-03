using System;
using MemLib.Ffxiv;

namespace TestApp {
    internal class Program {
        private static void Test() {
            using (var ff = new FfxivProcess()) {
                ff.GameObjects.Update();
                var list = ff.Inventory.EquippedSlots;
                foreach (var obj in list) Print($"{obj}");
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