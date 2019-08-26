using System;
using System.IO;

namespace OpenSpace {
    public class Writer : BinaryWriter {
        bool isLittleEndian = true;
        //bool masking = false; // for Rayman 2
        //uint mask = 0;
        public Writer(System.IO.Stream stream) : base(stream) { isLittleEndian = true; }
        public Writer(System.IO.Stream stream, bool isLittleEndian) : base(stream) { this.isLittleEndian = isLittleEndian; }

        public override void Write(Int32 value) {
            var data = BitConverter.GetBytes(value);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            base.Write(data);
        }

        public override void Write(Int16 value) {
            var data = BitConverter.GetBytes(value);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            base.Write(data);
        }

        public override void Write(UInt32 value) {
            var data = BitConverter.GetBytes(value);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            base.Write(data);
        }

        public override void Write(UInt16 value) {
            var data = BitConverter.GetBytes(value);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            base.Write(data);
        }

        public override void Write(Int64 value) {
            var data = BitConverter.GetBytes(value);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            base.Write(data);
        }

        public override void Write(UInt64 value) {
            var data = BitConverter.GetBytes(value);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            base.Write(data);
        }

        public override void Write(Single value) {
            var data = BitConverter.GetBytes(value);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            base.Write(data);
        }

        public override void Write(Double value) {
            var data = BitConverter.GetBytes(value);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            base.Write(data);
        }

        public void WriteNullDelimitedString(string value) {
            var data = value.ToCharArray();
            base.Write(data);
        }

        // To make sure position is a multiple of alignBytes
        public void Align(int alignBytes) {
            if (BaseStream.Position % alignBytes != 0) {
                BaseStream.Seek(alignBytes - (int)(BaseStream.Position % alignBytes), SeekOrigin.Current);
            }
        }

        // To make sure position is a multiple of alignBytes after reading a block of blocksize, regardless of prior position
        public void Align(int blockSize, int alignBytes) {
            int rest = blockSize % alignBytes;
            if (rest > 0) {
                BaseStream.Seek(alignBytes - rest, SeekOrigin.Current);
            }
        }
    }
}