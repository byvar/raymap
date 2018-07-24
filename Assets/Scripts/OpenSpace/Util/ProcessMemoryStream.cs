#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
#define ISWINDOWS
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace OpenSpace {
    public class ProcessMemoryStream : Stream {
#if ISWINDOWS
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref UIntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref UIntPtr lpNumberOfBytesWritten);
#endif

        const int PROCESS_ALL_ACCESS = 0x1F0FFF;
        const int PROCESS_WM_READ = 0x0010;
        const int PROCESS_VM_WRITE = 0x0020;
        const int PROCESS_VM_OPERATION = 0x0008;
        public enum Mode {
            Read,
            Write,
            AllAccess
        }

        Process process;
        IntPtr processHandle = IntPtr.Zero;
        long currentAddress = 0;
        Mode mode = Mode.Read;

        public ProcessMemoryStream(string name, Mode mode) {
#if ISWINDOWS
            Process[] processes = Process.GetProcessesByName(name.Replace(".exe",""));
            if (processes.Length == 0) throw new FileNotFoundException("Process not found");
            for (int i = 1; i < processes.Length; i++) {
                processes[i].Dispose();
            }
            process = processes[0];
#endif
            this.mode = mode;

            int accessLevel = PROCESS_WM_READ;
            switch (mode) {
                case Mode.Read:
                    accessLevel = PROCESS_WM_READ; break;
                case Mode.Write:
                    accessLevel = PROCESS_VM_WRITE; break;
                case Mode.AllAccess:
                    accessLevel = PROCESS_ALL_ACCESS; break;
            }
#if ISWINDOWS
            processHandle = OpenProcess(accessLevel, false, process.Id);
#endif
        }

        public int BaseAddress {
            get { return process.MainModule.BaseAddress.ToInt32(); }
        }

        public int MemorySize {
            get { return process.MainModule.ModuleMemorySize; } 
        }

        public override bool CanRead {
            get { return (mode == Mode.Read || mode == Mode.AllAccess) && processHandle != IntPtr.Zero; }
        }

        public override bool CanSeek {
            get { return processHandle != IntPtr.Zero; }
        }

        public override bool CanWrite {
            get { return (mode == Mode.Write || mode == Mode.AllAccess) && processHandle != IntPtr.Zero; }
        }

        public override long Length {
            get {
                if (!CanSeek) throw new NotSupportedException();
                return MemorySize;
            }
        }

        public override long Position {
            get {
                if (!CanSeek) throw new NotSupportedException();
                return currentAddress;
            }
            set {
                if (!CanSeek) throw new NotSupportedException();
                currentAddress = value;
            }
        }

        public override void Flush() {
            return; // We're writing to RAM. No flushing necessary.
        }

        public override int Read(byte[] buffer, int offset, int count) {
            if (!CanRead) throw new NotSupportedException();
            UIntPtr numBytesRead = UIntPtr.Zero;
#if ISWINDOWS
            byte[] tempBuf = new byte[count];
            bool success = ReadProcessMemory(processHandle, (int)currentAddress, tempBuf, count, ref numBytesRead);
            if (numBytesRead != UIntPtr.Zero) {
                Seek(numBytesRead.ToUInt32(), SeekOrigin.Current);
                Array.Copy(tempBuf, 0, buffer, offset, numBytesRead.ToUInt32());
                return (int)numBytesRead.ToUInt32();
            } else {
                return 0;
            }
#else
            throw new NotImplementedException();
#endif
        }

        public override long Seek(long offset, SeekOrigin origin) {
            if (!CanSeek) throw new NotSupportedException();
            switch (origin) {
                case SeekOrigin.Begin:
                    currentAddress = offset;
                    break;
                case SeekOrigin.Current:
                    currentAddress += offset;
                    break;
                case SeekOrigin.End:
                    currentAddress = BaseAddress + MemorySize - offset;
                    break;
            }
            return currentAddress;
        }

        public override void SetLength(long value) {
            if (!CanSeek || !CanWrite) throw new NotSupportedException();
            // Actually we won't support it in general, so...
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count) {
            if (!CanWrite) throw new NotSupportedException();
            UIntPtr numBytesWritten = UIntPtr.Zero;
#if ISWINDOWS
            byte[] tempBuf = new byte[count];
            Array.Copy(buffer, offset, tempBuf, 0, count);
            bool success = WriteProcessMemory(processHandle, (int)currentAddress, tempBuf, count, ref numBytesWritten);
            if (numBytesWritten != UIntPtr.Zero) {
                Seek(numBytesWritten.ToUInt32(), SeekOrigin.Current);
            }
#else
            throw new NotImplementedException();
#endif
        }

        ~ProcessMemoryStream() {
            Dispose(false);
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
#if ISWINDOWS
            if (processHandle != IntPtr.Zero) {
                CloseHandle(processHandle);
                processHandle = IntPtr.Zero;
            }
            if (process != null) {
                process.Dispose();
                process = null;
            }
#endif
        }
    }
}
