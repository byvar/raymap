using System;
using System.IO;

namespace LibR3 {
    public class EndianBinaryReader : BinaryReader {
        bool isLittleEndian = true;
        public EndianBinaryReader(System.IO.Stream stream) : base(stream) { isLittleEndian = true; }
        public EndianBinaryReader(System.IO.Stream stream, bool isLittleEndian) : base(stream) { this.isLittleEndian = isLittleEndian; }

        public override int ReadInt32() {
            var data = base.ReadBytes(4);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToInt32(data, 0);
        }

        public override float ReadSingle() {
            var data = base.ReadBytes(4);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToSingle(data, 0);
        }

        public override Int16 ReadInt16() {
            var data = base.ReadBytes(2);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToInt16(data, 0);
        }

        public override UInt16 ReadUInt16() {
            var data = base.ReadBytes(2);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToUInt16(data, 0);
        }

        public override Int64 ReadInt64() {
            var data = base.ReadBytes(8);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToInt64(data, 0);
        }

        public override UInt32 ReadUInt32() {
            var data = base.ReadBytes(4);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToUInt32(data, 0);
        }

        public string ReadNullDelimitedString() {
            string result = "";
            char c = ReadChar();
            while (c != 0x0) {
                result += c;
                c = ReadChar();
            }
            return result;
        }
    }
}