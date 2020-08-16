#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
#define ISWINDOWS
#endif

#if (UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX)
#define ISLINUX
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

#if ISLINUX
using c_uint = System.UInt32;
using pid_t = System.Int32;
#if UNITY_64
using c_ptr = System.UInt64;
using c_long = System.Int64; // Mandated to be same size as pointer on Linux
#else
using c_ptr = System.UInt32;
using c_long = System.Int32; // Mandated to be same size as pointer on Linux
#endif
#endif

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
#elif ISLINUX
        // It so happens that the one syscall needed here is still "TODO" in
        // Mono.Posix.NETStandard, so here's a manual wrapping...
        [DllImport("libc", SetLastError = true)]
        public static extern c_long ptrace(c_uint request, pid_t pid, c_ptr addr, c_ptr data);
        
        // This one's available in Mono.Unix.Native, but why bother with that
        // when I've already manually wrapped the most important one?
        [DllImport("libc", SetLastError = true)]
        public static extern pid_t waitpid(pid_t pid, out int status, int options);

        static c_uint PTRACE_PEEKDATA = 2;
        static c_uint PTRACE_POKEDATA = 5;
        static c_uint PTRACE_ATTACH = 16;
        static c_uint PTRACE_DETACH = 17;
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
        string exeFile = "";

        public string ExeFile => exeFile;

        public ProcessMemoryStream(string name, Mode mode) {
#if (ISWINDOWS || ISLINUX)
            Process[] processes = Process.GetProcessesByName(name.Replace(".exe",""));
#if ISLINUX
            // If something's running through Wine, then Mono identifies it as "wine-preloader", because this is
            // what it finds when it does a readlink on "/proc/{pid}/exe".
            // Only solution I see is to search for all processes with that name and then use /proc/{pid}/comm to
            // find their real names...
            if ((processes.Length == 0) && name.EndsWith(".exe")) {
                Process[] wineProcesses = Process.GetProcessesByName("wine-preloader");
                foreach (var wineProcess in wineProcesses) {
                    string cmdLine = File.ReadAllLines($"/proc/{wineProcess.Id}/cmdline")[0].Split(new Char[] {'\0'})[0];
                    string realName = cmdLine.Split(new Char[] {'/'}).Last();
                    if (String.Equals(realName,name)) {
                        processes = new Process[] {wineProcess};
                        if (cmdLine.StartsWith("/"))
                            // It's an absolute path.
                            exeFile = cmdLine;
                        else 
                            // TODO: Maybe make this not assume that the traced process never "chdir"s...
                            exeFile = $"/proc/{wineProcess.Id}/cwd/{cmdLine}";
                        break;
                    }
                }
            }
            // Could also be 64-bit...
            if ((processes.Length == 0) && name.EndsWith(".exe")) {
                Process[] wineProcesses = Process.GetProcessesByName("wine64-preloader");
                foreach (var wineProcess in wineProcesses) {
                    string cmdLine = File.ReadAllLines($"/proc/{wineProcess.Id}/cmdline")[0].Split(new Char[] {'\0'})[0];
                    string realName = cmdLine.Split(new Char[] {'/'}).Last();
                    if (String.Equals(realName,name)) {
                        processes = new Process[] {wineProcess};
                        if (cmdLine.StartsWith("/"))
                            // It's an absolute path.
                            exeFile = cmdLine;
                        else 
                            // TODO: Maybe make this not assume that the traced process never "chdir"s...
                            exeFile = $"/proc/{wineProcess.Id}/cwd/{cmdLine}";
                        break;
                    }
                }
            }
#endif
            if (processes.Length == 0) throw new FileNotFoundException("Process not found");
            for (int i = 1; i < processes.Length; i++) {
                processes[i].Dispose();
            }
            process = processes[0];
            if (String.Equals(exeFile,"")) exeFile = process.MainModule.FileName;
#endif
            this.mode = mode;

#if ISWINDOWS
            int accessLevel = PROCESS_WM_READ;
            switch (mode) {
                case Mode.Read:
                    accessLevel = PROCESS_WM_READ; break;
                case Mode.Write:
                    accessLevel = PROCESS_VM_WRITE; break;
                case Mode.AllAccess:
                    accessLevel = PROCESS_ALL_ACCESS; break;
            }
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
#if ISWINDOWS
            get { return (mode == Mode.Read || mode == Mode.AllAccess) && processHandle != IntPtr.Zero; }
#elif ISLINUX
            get { return (mode == Mode.Read || mode == Mode.AllAccess) && process.Id != 0; }
#endif
        }

        public override bool CanSeek {
#if ISWINDOWS
            get { return processHandle != IntPtr.Zero; }
#elif ISLINUX
	    get { return process.Id != 0; } // No handle on Linux - we just ptrace every time we want to do something.
#endif
        }

        public override bool CanWrite {
#if ISWINDOWS
            get { return (mode == Mode.Write || mode == Mode.AllAccess) && processHandle != IntPtr.Zero; }
#elif ISLINUX
            get { return (mode == Mode.Write || mode == Mode.AllAccess) && process.Id != 0; }
#endif
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
#if ISWINDOWS
            UIntPtr numBytesRead = UIntPtr.Zero;
            byte[] tempBuf = new byte[count];
            bool success = ReadProcessMemory(processHandle, (int)currentAddress, tempBuf, count, ref numBytesRead);
            if (numBytesRead != UIntPtr.Zero) {
                Seek(numBytesRead.ToUInt32(), SeekOrigin.Current);
                Array.Copy(tempBuf, 0, buffer, offset, numBytesRead.ToUInt32());
                return (int)numBytesRead.ToUInt32();
            } else {
                return 0;
            }
#elif ISLINUX
            // TODO: Do we need to worry about alignment here?
            // According to the man page, that only matters for
            // PTRACE_{PEEK,POKE}USER, whereas here we're using DATA...
            // If it blows up for someone using ARM, we'll know why!
            c_ptr wordSize = sizeof(c_long);
            c_ptr numWords = (c_ptr)count / wordSize;
            c_ptr numLeftOverBytes = (c_ptr)count % wordSize;
            c_ptr numWordsRead;
            if (numLeftOverBytes > 0)
                numWords++;
            c_long[] tempBuf = new c_long[numWords];

            int errno = -(int)ptrace(PTRACE_ATTACH, process.Id, 0, 0);
            if (errno == 1)
                throw new UnauthorizedAccessException("Can't attach to process – you probably need to run 'sudo setcap cap_sys_ptrace=eip /path/to/Unity' (note that this will theoretically allow any Unity game you run to access/modify arbitrary processes' memory! You might want to unset cap_sys_ptrace before running untrusted games.)");
            else if (errno == 3)
                throw new FileNotFoundException("Process Not Found – it may have exited");
            else if (errno != 0)
                // Something bad happened, but we don't know what...
                return 0;

            int status;
            errno = waitpid(process.Id, out status, 0);
            if ((status & 0x7f) == 0) 
                throw new EndOfStreamException("Process has exited");

            for(numWordsRead = 0; numWordsRead < numWords; numWordsRead++) {
                tempBuf[numWordsRead] = ptrace(PTRACE_PEEKDATA,
                        process.Id,
                        (c_ptr)currentAddress + numWordsRead*wordSize,
                        0);
                errno = Marshal.GetLastWin32Error();
                if (errno != 0) {
                    // Tidy up.
                    ptrace(PTRACE_DETACH, process.Id, 0, 0);
                    if (errno == 5)
                        throw new IOException("Tried to read from invalid/misaligned location in process memory");
                    else if (errno == 14)
                        throw new AccessViolationException("Tried to read from invalid location in process memory");
                    else
                        // Something bad happened, but we don't know what...
                        break;
                }
            }

            ptrace(PTRACE_DETACH, process.Id, 0, 0);

            if (numWordsRead != 0) {
                int numBytesRead = (int)(numWordsRead * wordSize);
                if (numBytesRead > count)
                    numBytesRead = count;
                Seek(numBytesRead, SeekOrigin.Current);
                Buffer.BlockCopy(tempBuf, 0, buffer, offset, numBytesRead);
                return numBytesRead;
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
#if ISWINDOWS
            UIntPtr numBytesWritten = UIntPtr.Zero;
            byte[] tempBuf = new byte[count];
            Array.Copy(buffer, offset, tempBuf, 0, count);
            bool success = WriteProcessMemory(processHandle, (int)currentAddress, tempBuf, count, ref numBytesWritten);
            if (numBytesWritten != UIntPtr.Zero) {
                Seek(numBytesWritten.ToUInt32(), SeekOrigin.Current);
            }
#elif ISLINUX
            // TODO: Do we need to worry about alignment here?
            // According to the man page, that only matters for
            // PTRACE_{PEEK,POKE}USER, whereas here we're using DATA...
            // If it blows up for someone using ARM, we'll know why!
            c_ptr wordSize = sizeof(c_long);
            c_ptr numWords = (c_ptr)count / wordSize;
            c_ptr numLeftoverBytes = (c_ptr)count % wordSize;
            c_ptr numWordsWritten;
            c_long[] tempBuf = new c_long[(numLeftoverBytes > 0) ? numWords+1 : numWords];
            
            int errno = -(int)ptrace(PTRACE_ATTACH, process.Id, 0, 0);
            if (errno == 1)
                throw new UnauthorizedAccessException("Can't attach to process – you probably need to run 'sudo setcap cap_sys_ptrace=eip /path/to/Unity' (note that this will theoretically allow any Unity game you run to access/modify arbitrary processes' memory! You might want to unset cap_sys_ptrace before running untrusted games.)");
            else if (errno == 3)
                throw new FileNotFoundException("Process Not Found – it may have exited");
            else if (errno != 0)
                // Something bad happened, but we don't know what...
                return;

            int status;
            errno = waitpid(process.Id, out status, 0);
            if ((status & 0x7f) == 0) 
                throw new EndOfStreamException("Process has exited");

            if (numLeftoverBytes > 0) {
                // Get the last word from memory so we only overwrite the bytes we care about!
                tempBuf[numWords] = ptrace(PTRACE_PEEKDATA,
                        process.Id,
                        (c_ptr)currentAddress + numWords*wordSize,
                        0);
                errno = Marshal.GetLastWin32Error();
                if (errno != 0) {
                    // Tidy up.
                    ptrace(PTRACE_DETACH, process.Id, 0, 0);
                    if (errno == 5)
                        throw new IOException("Tried to read from invalid/misaligned location in process memory");
                    else if (errno == 14)
                        throw new AccessViolationException("Tried to read from invalid location in process memory");
                    else
                        // Something bad happened, but we don't know what...
                        return;
                }

                numWords++;
            }

            Buffer.BlockCopy(buffer, offset, tempBuf, 0, count);

            for(numWordsWritten = 0; numWordsWritten < numWords; numWordsWritten++) {
                ptrace(PTRACE_POKEDATA,
                        process.Id,
                        (c_ptr)currentAddress + numWordsWritten*wordSize,
                        (c_ptr)tempBuf[numWordsWritten]);
                errno = Marshal.GetLastWin32Error();
                if (errno != 0) {
                    // Tidy up.
                    ptrace(PTRACE_DETACH, process.Id, 0, 0);
                    if (errno == 5)
                        throw new IOException("Tried to write from invalid/misaligned location in process memory");
                    else if (errno == 14)
                        throw new AccessViolationException("Tried to write from invalid location in process memory");
                    else
                        // Something bad happened, but we don't know what...
                        break;
                }
            }

            ptrace(PTRACE_DETACH, process.Id, 0, 0);

            if (numWordsWritten != 0) {
                int numBytesWritten = (int)(numWordsWritten * wordSize);
                if (numBytesWritten > count)
                    numBytesWritten = count;
                Seek(numBytesWritten, SeekOrigin.Current);
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
#endif
#if (ISWINDOWS || ISLINUX)
            if (process != null) {
#if ISLINUX
                // Just in case...
                ptrace(PTRACE_DETACH, process.Id, 0, 0);
#endif
                process.Dispose();
                process = null;
            }
#endif
        }
    }
}
