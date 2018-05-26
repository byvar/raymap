using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LibR3 {
    public class R3Pointer : IEquatable<R3Pointer> {
        public uint offset;
        public int file;
        public R3Pointer(uint offset, int file) {
            this.offset = offset;
            this.file = file;
        }

        public override bool Equals(System.Object obj) {
            return obj is R3Pointer && this == (R3Pointer)obj;
        }
        public override int GetHashCode() {
            return offset.GetHashCode() ^ file.GetHashCode();
        }

        public bool Equals(R3Pointer other) {
            return this == (R3Pointer)other;
        }

        public static bool operator ==(R3Pointer x, R3Pointer y) {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return x.offset == y.offset && x.file == y.file;
        }
        public static bool operator !=(R3Pointer x, R3Pointer y) {
            return !(x == y);
        }
        public static R3Pointer operator +(R3Pointer x, Decimal y) {
            return new R3Pointer(x.offset + (uint)y, x.file);
        }
        public override string ToString() {
            return file + "|" + String.Format("0x{0:X8}", offset);
        }

        public static R3Pointer GetPointerAtOffset(R3Pointer pointer) {
            R3Loader l = R3Loader.Loader;
            if (l.ptrs_array[pointer.file].ContainsKey(pointer.offset)) {
                R3Pointer ptr = l.ptrs_array[pointer.file][pointer.offset];
                if (ptr.offset == 0) return null;
                return ptr;
            }
            return null;
        }

        public static R3Pointer Read(EndianBinaryReader reader) {
            R3Loader l = R3Loader.Loader;
            uint current_off = (uint)(reader.BaseStream.Position) - 4;
            uint value = reader.ReadUInt32();
            for (int i = 0; i < l.reader_array.Length; i++) {
                if (reader.Equals(l.reader_array[i])) {
                    if (!l.ptrs_array[i].ContainsKey(current_off)) {
                        if (value == 0) return null;
                        if (!l.allowDeadPointers) throw new FormatException("Not a valid pointer in file " + i + " at position " + (current_off + 4));
                        return null;
                    }
                    return l.ptrs_array[i][current_off];
                }
            }
            throw new FormatException("Reader wasn't recognized.");
        }

        // For readers
        public R3Pointer Goto(ref EndianBinaryReader reader) {
            R3Pointer oldPos = Current(reader);
            reader = R3Loader.Loader.reader_array[file];
            reader.BaseStream.Seek(offset + 4, SeekOrigin.Begin);
            return oldPos;
        }

        public static R3Pointer Goto(ref EndianBinaryReader reader, R3Pointer newPos) {
            if (newPos != null) return newPos.Goto(ref reader);
            return null;
        }

        public static R3Pointer Current(EndianBinaryReader reader) {
            R3Loader l = R3Loader.Loader;
            uint curPos = (uint)reader.BaseStream.Position;
            int curFile = 0;
            for (int i = 0; i < l.reader_array.Length; i++) {
                if (reader.Equals(l.reader_array[i])) {
                    curFile = i;
                    break;
                }
            }
            return new R3Pointer(curPos - 4, curFile);
        }

        // For writers
        public R3Pointer Goto(ref EndianBinaryWriter writer) {
            R3Pointer oldPos = Current(writer);
            writer = R3Loader.Loader.writer_array[file];
            writer.BaseStream.Seek(offset + 4, SeekOrigin.Begin);
            return oldPos;
        }

        public static R3Pointer Goto(ref EndianBinaryWriter writer, R3Pointer newPos) {
            if (newPos != null) return newPos.Goto(ref writer);
            return null;
        }

        public static R3Pointer Current(EndianBinaryWriter writer) {
            R3Loader l = R3Loader.Loader;
            uint curPos = (uint)writer.BaseStream.Position;
            int curFile = 0;
            for (int i = 0; i < l.writer_array.Length; i++) {
                if (writer.Equals(l.writer_array[i])) {
                    curFile = i;
                    break;
                }
            }
            return new R3Pointer(curPos - 4, curFile);
        }
    }
}
