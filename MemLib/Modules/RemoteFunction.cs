using System;
using MemLib.Memory;

namespace MemLib.Modules {
    public class RemoteFunction : RemotePointer {
        public string Name { get; }
        public string UndecoratedName => ModuleManager.UnDecorateSymbolName(Name);

        public RemoteFunction(RemoteProcess process, IntPtr address, string name) : base(process, address) {
            Name = name;
        }

        public override string ToString() {
            return $"Address = 0x{BaseAddress.ToInt64():X8} Name = {Name}";
        }
    }
}