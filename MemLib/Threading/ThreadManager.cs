using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using MemLib.Internals;
using MemLib.Native;

namespace MemLib.Threading {
    public class ThreadManager : IDisposable {
        private readonly RemoteProcess m_Process;

        internal IEnumerable<ProcessThread> NativeThreads {
            get {
                m_Process.Native.Refresh();
                return m_Process.Native.Threads.Cast<ProcessThread>();
            }
        }

        public IEnumerable<RemoteThread> RemoteThreads => NativeThreads.Select(t => new RemoteThread(m_Process, t));
        public RemoteThread MainThread => new RemoteThread(m_Process,
            NativeThreads.Aggregate((current, next) => next.StartTime < current.StartTime ? next : current));

        public RemoteThread this[int threadId] => GetThreadById(threadId);

        internal ThreadManager(RemoteProcess proc) {
            m_Process = proc;
        }

        public RemoteThread GetThreadById(int id) {
            var native = NativeThreads.FirstOrDefault(t => t.Id == id);
            return native == null ? null : new RemoteThread(m_Process, native);
        }

        #region Create

        public RemoteThread Create(IntPtr address, bool isStarted = true) {
            var tbi = NtQueryInformationThread(
                CreateRemoteThread(m_Process.Handle, address, IntPtr.Zero, ThreadCreationFlags.Suspended)
            );

            ProcessThread nativeThread;
            do {
                nativeThread = m_Process.Threads.NativeThreads.FirstOrDefault(t => t.Id == tbi.ThreadId.ToInt32());
            } while (nativeThread == null);

            var result = new RemoteThread(m_Process, nativeThread);

            if (isStarted)
                result.Resume();
            return result;
        }

        public RemoteThread Create(IntPtr address, dynamic parameter, bool isStarted = true) {
            var marshalledParameter = MarshalValue.Marshal(m_Process, parameter);

            ThreadBasicInformation tbi = NtQueryInformationThread(
                CreateRemoteThread(m_Process.Handle, address, marshalledParameter.Reference,
                    ThreadCreationFlags.Suspended)
            );

            ProcessThread nativeThread;
            do {
                nativeThread = m_Process.Threads.NativeThreads.FirstOrDefault(t => t.Id == tbi.ThreadId.ToInt32());
            } while (nativeThread == null);

            var result = new RemoteThread(m_Process, nativeThread, marshalledParameter);

            if (isStarted)
                result.Resume();
            return result;
        }

        #endregion
        #region CreateAndJoin

        public RemoteThread CreateAndJoin(IntPtr address, dynamic parameter) {
            var ret = Create(address, parameter);
            ret.Join();
            return ret;
        }

        public RemoteThread CreateAndJoin(IntPtr address) {
            var ret = Create(address);
            ret.Join();
            return ret;
        }

        #endregion
        #region ResumeAll

        public void ResumeAll() {
            foreach (var thread in RemoteThreads) {
                thread.Resume();
            }
        }

        #endregion
        #region SuspendAll

        public void SuspendAll() {
            foreach (var thread in RemoteThreads) {
                thread.Suspend();
            }
        }

        #endregion
        #region Statics

        public static SafeMemoryHandle OpenThread(ThreadAccessFlags accessFlags, int threadId) {
            var handle = NativeMethods.OpenThread(accessFlags, false, threadId);
            if (handle.IsInvalid || handle.IsClosed)
                throw new Win32Exception();
            return handle;
        }

        public static SafeMemoryHandle CreateRemoteThread(SafeMemoryHandle processHandle, IntPtr startAddress,
            IntPtr parameter, ThreadCreationFlags creationFlags = ThreadCreationFlags.Run) {
            var handle = NativeMethods.CreateRemoteThread(processHandle, IntPtr.Zero, 0, startAddress, parameter,
                creationFlags, out _);
            if (handle.IsInvalid || handle.IsClosed)
                throw new Win32Exception();
            return handle;
        }

        private static readonly IntPtr StillActive = new IntPtr(259);

        public static IntPtr? GetExitCodeThread(SafeMemoryHandle threadHandle) {
            if (!NativeMethods.GetExitCodeThread(threadHandle, out var exitCode))
                throw new Win32Exception();
            if (exitCode == StillActive && NativeMethods.WaitForSingleObject(threadHandle, 0) == WaitValues.Timeout)
                return null;
            return exitCode;
        }

        public static ThreadBasicInformation NtQueryInformationThread(SafeMemoryHandle threadHandle) {
            var info = new ThreadBasicInformation();
            var ret = NativeMethods.NtQueryInformationThread(threadHandle, 0, ref info,
                MarshalType<ThreadBasicInformation>.Size, IntPtr.Zero);
            if (ret == 0)
                return info;
            throw new Win32Exception();
        }

        public static uint ResumeThread(SafeMemoryHandle threadHandle) {
            var ret = NativeMethods.ResumeThread(threadHandle);
            if (ret == uint.MaxValue)
                throw new Win32Exception();
            return ret;
        }

        public static uint SuspendThread(SafeMemoryHandle threadHandle) {
            var ret = NativeMethods.SuspendThread(threadHandle);
            if (ret == uint.MaxValue)
                throw new Win32Exception();
            return ret;
        }

        public static void TerminateThread(SafeMemoryHandle threadHandle, int exitCode) {
            var ret = NativeMethods.TerminateThread(threadHandle, exitCode);
            if (!ret)
                throw new Win32Exception();
        }

        public static WaitValues WaitForSingleObject(SafeMemoryHandle handle, uint timeout) {
            var ret = NativeMethods.WaitForSingleObject(handle, timeout);
            if (ret == WaitValues.Failed)
                throw new Win32Exception();
            return ret;
        }

        #endregion
        #region IDisposable

        void IDisposable.Dispose() { }

        ~ThreadManager() {
            ((IDisposable) this).Dispose();
        }

        #endregion
    }
}