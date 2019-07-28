using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using MemLib.Native;

namespace MemLib.Modules {
    public class ModuleManager : IDisposable {
        private readonly RemoteProcess m_Process;
        private readonly HashSet<InjectedModule> m_InjectedModules = new HashSet<InjectedModule>();
        internal IEnumerable<ProcessModule> NativeModules => m_Process.Native.Modules.Cast<ProcessModule>();
        private RemoteModule m_MainModule;
        public RemoteModule MainModule => m_MainModule ?? (m_MainModule = FetchModule(m_Process.Native.MainModule?.ModuleName));
        public IEnumerable<RemoteModule> RemoteModules => NativeModules.Select(m => new RemoteModule(m_Process, m));

        public RemoteModule this[string moduleName] => FetchModule(moduleName);

        public ModuleManager(RemoteProcess process) {
            m_Process = process;
        }

        private RemoteModule FetchModule(string moduleName) {
            if (!Path.HasExtension(moduleName))
                moduleName += ".dll";
            var nativeMod = NativeModules.FirstOrDefault(m => m.ModuleName.Equals(moduleName, StringComparison.OrdinalIgnoreCase));
            return nativeMod == null ? null : new RemoteModule(m_Process, nativeMod);
        }

        #region Inject
        
        public InjectedModule Inject(string moduleFile, bool mustBeDisposed = true) {
            if(!File.Exists(moduleFile))
                throw new FileNotFoundException("File not found.", moduleFile);
            var module = InternalInject(moduleFile, mustBeDisposed);
            if (module != null && !m_InjectedModules.Contains(module)) 
                m_InjectedModules.Add(module);
            return module;
        }

        private InjectedModule InternalInject(string path, bool mustBeDisposed) {
            var thread = m_Process.Threads.CreateAndJoin(m_Process["kernel32"]["LoadLibraryA"].BaseAddress, path);
            var exitCode = thread.GetExitCode<int>();
            if (exitCode == 0) return null;
            var moduleName = Path.GetFileName(path);
            var nativeMod = m_Process.Modules.NativeModules.First(m => m.ModuleName.Equals(moduleName, StringComparison.OrdinalIgnoreCase));
            return new InjectedModule(m_Process, nativeMod, mustBeDisposed);
        }

        #endregion
        #region Eject

        public void Eject(RemoteModule module) {
            if (!module.IsValid) return;
            
            var injected = m_InjectedModules.FirstOrDefault(m => m.Equals(module));
            if (injected != null)
                m_InjectedModules.Remove(injected);

            InternalEject(module);
        }

        public void Eject(string moduleName) {
            var module = RemoteModules.FirstOrDefault(m => m.Name.Equals(moduleName, StringComparison.OrdinalIgnoreCase));
            if (module != null)
                InternalEject(module);
        }

        private void InternalEject(RemoteModule module) {
            m_Process.Threads.CreateAndJoin(m_Process["kernel32"]["FreeLibrary"].BaseAddress, module.BaseAddress);
        }

        #endregion
        #region Statics

        public static string UnDecorateSymbolName(string name, UnDecorateFlags flags = UnDecorateFlags.NameOnly) {
            var sb = new StringBuilder(260);
            return NativeMethods.UnDecorateSymbolName(name, sb, sb.Capacity, flags) ? sb.ToString() : name;
        }

        #endregion
        #region IDisposable

        void IDisposable.Dispose() {
            foreach (var module in m_InjectedModules.Where(m => m.MustBeDisposed).ToList()) {
                module.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        ~ModuleManager() {
            ((IDisposable) this).Dispose();
        }

        #endregion
    }
}