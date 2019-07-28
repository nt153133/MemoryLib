using System;
using System.Text;

namespace MemLib.Memory {
    public class RemotePointer : IEquatable<RemotePointer> {
        protected readonly RemoteProcess m_Process;
        public IntPtr BaseAddress { get; protected set; }
        
        public virtual bool IsValid => m_Process.IsRunning && BaseAddress != IntPtr.Zero;

        public RemotePointer(RemoteProcess process, IntPtr address) {
            m_Process = process;
            BaseAddress = address;
        }
        
        public override string ToString() => $"0x{BaseAddress.ToInt64():X}";

        #region Read Memory

        public T Read<T>() {
            return Read<T>(0);
        }

        public T Read<T>(int offset) {
            return m_Process.Read<T>(BaseAddress + offset);
        }

        public T[] Read<T>(int offset, int count) {
            return m_Process.Read<T>(BaseAddress + offset, count);
        }

        public string ReadString() {
            return ReadString(0, Encoding.UTF8);
        }

        public string ReadString(int offset, Encoding encoding, int maxLength = 512) {
            return m_Process.ReadString(BaseAddress + offset, encoding, maxLength);
        }

        public string ReadString(Encoding encoding, int maxLength = 512) {
            return ReadString(0, encoding, maxLength);
        }

        //public T Read<T>(Enum offset) {
        //    return Read<T>(Convert.ToInt32(offset));
        //}

        //public T[] Read<T>(Enum offset, int count) {
        //    return Read<T>(Convert.ToInt32(offset), count);
        //}

        //public string ReadString(Enum offset, Encoding encoding, int maxLength = 512) {
        //    return ReadString(Convert.ToInt32(offset), encoding, maxLength);
        //}

        #endregion
        #region Write Memory

        public void Write<T>(T value) {
            Write(0, value);
        }

        public void Write<T>(int offset, T value) {
            m_Process.Write(BaseAddress + offset, value);
        }

        public void Write<T>(T[] array) {
            Write(0, array);
        }

        public void Write<T>(int offset, T[] array) {
            m_Process.Write(BaseAddress + offset, array);
        }

        public void WriteString(string text) {
            WriteString(0, text);
        }

        public void WriteString(int offset, string text, Encoding encoding) {
            m_Process.WriteString(BaseAddress + offset, text, encoding);
        }

        public void WriteString(string text, Encoding encoding) {
            WriteString(0, text, encoding);
        }

        public void WriteString(int offset, string text) {
            m_Process.WriteString(BaseAddress + offset, text);
        }

        //public void Write<T>(Enum offset, T value) {
        //    Write(Convert.ToInt32(offset), value);
        //}

        //public void Write<T>(Enum offset, T[] array) {
        //    Write(Convert.ToInt32(offset), array);
        //}

        //public void WriteString(Enum offset, string text, Encoding encoding) {
        //    WriteString(Convert.ToInt32(offset), text, encoding);
        //}

        //public void WriteString(Enum offset, string text) {
        //    WriteString(Convert.ToInt32(offset), text);
        //}

        #endregion
        #region Equality members

        public bool Equals(RemotePointer other) {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(m_Process, other.m_Process) && BaseAddress.Equals(other.BaseAddress);
        }

        public override bool Equals(object obj) {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((RemotePointer) obj);
        }

        public override int GetHashCode() {
            unchecked {
                // ReSharper disable once NonReadonlyMemberInGetHashCode
                return ((m_Process != null ? m_Process.GetHashCode() : 0) * 397) ^ BaseAddress.GetHashCode();
            }
        }

        #endregion
    }
}