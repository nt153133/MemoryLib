using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MemLib.Memory;
using MemLib.PeHeader;

namespace MemLib.Modules {
    public class RemoteModule : RemoteRegion {
        public ProcessModule Native { get; }
        private PeHeaderReader m_PeHeader;
        public PeHeaderReader PeHeader => m_PeHeader ?? (m_PeHeader = new PeHeaderReader(m_Process, BaseAddress));
        public string Name => Native.ModuleName;
        public string Path => Native.FileName;
        public int Size => Native.ModuleMemorySize;
        public bool IsMainModule => m_Process.MainModule.BaseAddress == BaseAddress;

        public IEnumerable<RemoteFunction> Exports => PeHeader.ExportFunctions.Select(ExportToRemote);

        public override bool IsValid => base.IsValid && m_Process.Modules.NativeModules.Any(m => m.BaseAddress == BaseAddress && m.ModuleName == Name);
        
        public RemoteFunction this[string functionName] => FindFunction(functionName);

        internal RemoteModule(RemoteProcess process, ProcessModule module) : base(process, module.BaseAddress) {
            Native = module;
        }

        private RemoteFunction ExportToRemote(ExportFunction func) {
            return new RemoteFunction(m_Process, BaseAddress + func.RelativeAddress, func.Name);
        }

        public void Eject() {
            m_Process.Modules.Eject(this);
            BaseAddress = IntPtr.Zero;
        }

        private RemoteFunction FindFunction(string functionName) {
            var function = Exports.FirstOrDefault(f => f.Name == functionName || f.UndecoratedName == functionName);
            return function;
        }
        
        public override string ToString() => $"BaseAddress=0x{BaseAddress.ToInt64():X} Size=0x{Size:X} Name={Name}";
    }
}