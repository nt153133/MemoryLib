using System;

namespace MemLib.Ffxiv {
    public static class Core {
        public static class Memory {
            public static T Read<T>(IntPtr addr) {
                return default;
            }
        }

        public static class Offsets {
            public static class Character {
                public static int Id0;
                public static int Id1;
                public static int Id2;
            }
        }
    }

    public class Test {
        private readonly IntPtr m_Pointer = IntPtr.Zero;
        public uint IdLocatiion { get; private set; }

        public uint Test0() {
            uint pointer0 = 0;
            uint pointer2 = 0;
            uint pointer3 = 0;
            for (;;) {
                var pointer1 = Core.Memory.Read<uint>(m_Pointer + Core.Offsets.Character.Id0);
                uint num;
                int num5;
                int num6;
                int num7;
                for (;;) {
                    uint num2;
                    num = num2 = pointer1;
                    var num3 = -536870912;
                    for (;;) {
                        var num4 = num5 = num3;
                        if (num4 == 0)
                            goto IL_CC;
                        if (num2 != (uint) num4)
                            goto break0;
                        pointer2 = Core.Memory.Read<uint>(m_Pointer + Core.Offsets.Character.Id1);
                        pointer3 = Core.Memory.Read<ushort>(m_Pointer + Core.Offsets.Character.Id2);
                        if (pointer2 == 0u)
                            goto break2;
                        num6 = (int) (num2 = num = pointer3);
                        num7 = num3 = 200;
                        if (num7 != 0)
                            goto Block_7;
                    }
                }

                IL_CC:
                if (num <= (uint) num5)
                    break;
                pointer0 = pointer2;
                IdLocatiion = 1u;
                goto Block_8;
                Block_7:
                num = (uint) (num6 - num7);
                num5 = 43;
                goto IL_CC;
            }

            break2:
            pointer0 = pointer3;
            IdLocatiion = 2u;
            Block_8:
            goto return_;
            break0:
            IdLocatiion = 0u;
            return_:
            var result = pointer0;
            return result;
        }
    }
}