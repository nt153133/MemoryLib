using System;
using System.Linq;
using MemLib.Ffxiv;
using MemLib.Ffxiv.Enums;

namespace TestApp {
    internal class Program {
        private static void Test() {
            using (var proc = new FfxivProcess()) {

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