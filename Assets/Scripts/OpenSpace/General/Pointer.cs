using OpenSpace.FileFormat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenSpace {
    public class Pointer : IEquatable<Pointer> {
        public uint offset;
        public FileWithPointers file;
        public Pointer(uint offset, FileWithPointers file) {
            this.offset = offset;
            this.file = file;
        }

        public uint FileOffset {
            get {
                if (file != null) {
                    return (uint)(offset + file.baseOffset);
                } else return offset;
            }
        }

        public override bool Equals(System.Object obj) {
            return obj is Pointer && this == (Pointer)obj;
        }
        public override int GetHashCode() {
            return offset.GetHashCode() ^ file.GetHashCode();
        }

        public bool Equals(Pointer other) {
            return this == (Pointer)other;
        }

        public static bool operator ==(Pointer x, Pointer y) {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return x.offset == y.offset && x.file == y.file;
        }
        public static bool operator !=(Pointer x, Pointer y) {
            return !(x == y);
        }
        public static Pointer operator +(Pointer x, Decimal y) {
            return new Pointer((uint)((Decimal)x.offset + y), x.file);
        }
        public static Pointer operator -(Pointer x, Decimal y) {
            return new Pointer((uint)((Decimal)x.offset - y), x.file);
        }
        public override string ToString() {
            if (file != null && file.baseOffset != 0) {
                return file.name + "|" + String.Format("0x{0:X8}", offset) + "[" + String.Format("0x{0:X8}", offset + file.baseOffset) + "]";
            } else {
                return file.name + "|" + String.Format("0x{0:X8}", offset);
            }
        }

        public static Pointer GetPointerAtOffset(Pointer pointer) {
            MapLoader l = MapLoader.Loader;
            Pointer ptr = null;
            if (pointer.file.pointers.ContainsKey(pointer.offset)) {
                ptr = pointer.file.pointers[pointer.offset];
                if (ptr.offset == 0) return null;
                return ptr;
            } else if (pointer.file.allowUnsafePointers) {
                Reader reader = pointer.file.reader;
                Pointer.DoAt(ref reader, pointer, () => {
                    uint current_off = (uint)(reader.BaseStream.Position);
                    uint value = reader.ReadUInt32();
                    FileWithPointers file = pointer.file;
                    uint fileOff = (uint)(current_off - file.baseOffset);

                    if (file.pointers.ContainsKey(fileOff)) {
                        ptr = file.pointers[fileOff];
                    } else {
                        if (value == 0 || value == 0xFFFFFFFF) {
                            ptr = null;
                        } else {
                            ptr = file.GetUnsafePointer(value);
                        }
                    }
                });
                return ptr;
            }
            return null;
        }

        public static Pointer Read(Reader reader, bool allowMinusOne = false) {
            MapLoader l = MapLoader.Loader;
            uint current_off = (uint)(reader.BaseStream.Position);
            uint value = reader.ReadUInt32();
            FileWithPointers file = l.GetFileByReader(reader);
            if (file == null) throw new PointerException("Reader wasn't recognized.", "Pointer.Read");
            uint fileOff = (uint)(current_off - file.baseOffset);
            if (!file.pointers.ContainsKey(fileOff)) {
                if (value == 0 || (allowMinusOne && value == 0xFFFFFFFF)) return null;
                if (!l.allowDeadPointers && !file.allowUnsafePointers) {
                    throw new PointerException("Not a valid pointer at " + (Pointer.Current(reader) - 4) + ": " + value, "Pointer.Read");
                }
                if (file.allowUnsafePointers) {
                    Pointer ptr = file.GetUnsafePointer(value);
                    if (ptr == null) {
                        throw new PointerException("Not a valid pointer at " + (Pointer.Current(reader) - 4) + ": " + value, "Pointer.Read");
                    }
                    return ptr;
                }
                return null;
            }
            // Hack for R3GC US
            if (l.allowDeadPointers && file.name == "test" && file.pointers[fileOff].file.name == "fix") return null;
            return file.pointers[fileOff];
        }

        public static void Write(Writer writer, Pointer pointer) {
            MapLoader l = MapLoader.Loader;
            uint current_off = (uint)(writer.BaseStream.Position);
            FileWithPointers file = l.GetFileByWriter(writer);
            if (file == null) throw new FormatException("Writer wasn't recognized.");
            file.WritePointer(pointer);
        }

        public void Write(Writer writer) {
            Pointer.Write(writer, this);
        }

        // For readers
        public Pointer Goto(ref Reader reader) {
            Pointer oldPos = Current(reader);
            reader = file.reader;
            reader.BaseStream.Seek(offset + file.baseOffset, SeekOrigin.Begin);
            return oldPos;
        }

        public static Pointer Goto(ref Reader reader, Pointer newPos) {
            if (newPos != null) return newPos.Goto(ref reader);
            return null;
        }

        public static Pointer Current(Reader reader) {
            MapLoader l = MapLoader.Loader;
            uint curPos = (uint)reader.BaseStream.Position;
            FileWithPointers curFile = l.GetFileByReader(reader);
            return new Pointer((uint)(curPos - curFile.baseOffset), curFile);
        }

        public void DoAt(ref Reader reader, Action action) {
            Pointer off_current = Goto(ref reader, this);
            action();
            Goto(ref reader, off_current);
        }

        public static void DoAt(ref Reader reader, Pointer newPos, Action action) {
            if (newPos != null) newPos.DoAt(ref reader, action);
        }

        // For writers
        public Pointer Goto(ref Writer writer) {
            Pointer oldPos = Current(writer);
            writer = file.writer;
            writer.BaseStream.Seek(offset + file.baseOffset, SeekOrigin.Begin);
            return oldPos;
        }

        public static Pointer Goto(ref Writer writer, Pointer newPos) {
            if (newPos != null) return newPos.Goto(ref writer);
            return null;
        }

        public static Pointer Current(Writer writer) {
            MapLoader l = MapLoader.Loader;
            uint curPos = (uint)writer.BaseStream.Position;
            FileWithPointers curFile = l.GetFileByWriter(writer);
            return new Pointer((uint)(curPos - curFile.baseOffset), curFile);
        }

        public void DoAt(ref Writer writer, Action action)
        {
            Pointer off_current = Goto(ref writer, this);
            action();
            Goto(ref writer, off_current);
        }

        public static void DoAt(ref Writer writer, Pointer newPos, Action action)
        {
            if (newPos != null) newPos.DoAt(ref writer, action);
        }
    }

    public class Pointer<T> where T : OpenSpaceStruct, new() {
        public Pointer pointer;
        public T Value { get; set; }

        public Pointer(Reader reader, bool resolve = false, Action<T> onPreRead = null) {
            pointer = Pointer.Read(reader);
            if (resolve) {
                Resolve(reader, onPreRead: onPreRead);
            }
        }

        public Pointer(Pointer pointer, Reader reader = null, bool resolve = false, Action<T> onPreRead = null) {
            this.pointer = pointer;
            if (resolve) {
                Resolve(reader, onPreRead: onPreRead);
            }
        }

        public Pointer(Pointer pointer, T value) {
            this.pointer = pointer;
            this.Value = value;
        }
        public Pointer() {
            this.pointer = null;
            this.Value = null;
        }

        public Pointer<T> Resolve(Reader reader, Action<T> onPreRead = null) {
            MapLoader l = MapLoader.Loader;
            Value = l.FromOffsetOrRead<T>(reader, pointer, onPreRead: onPreRead);
            return this;
        }

        public static implicit operator T(Pointer<T> a) {
            return a.Value;
        }
        public static implicit operator Pointer<T>(T t) {
            if (t == null) {
                return new Pointer<T>(null, null);
            } else {
                return new Pointer<T>(t.Offset, t);
            }
        }
    }
}
