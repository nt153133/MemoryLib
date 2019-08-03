using System;
using System.Linq;
using MemLib.Ffxiv;
using MemLib.Ffxiv.Objects;

namespace TestApp {
    internal class Program {
        private static void Test() {
            using (var ff = new FfxivProcess()) {
                ff.GameObjects.Update();
                var list = ff.GameObjects.GetObjectsOfType<Character>(true, true);
                foreach (var obj in list.Where(o => o.HasMyAura(158))) {
                    Print($"{obj}\n" +
                          $"{string.Join("\n", obj.CharacterAuras.Select(a => $"\t{a}"))}");
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