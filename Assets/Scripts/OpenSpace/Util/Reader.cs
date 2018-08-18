using System;
using System.IO;

namespace OpenSpace {
    public class Reader : BinaryReader {
        public enum MaskingMode {
            Off,
            Number,
            Window
        }
        public delegate void ReadAction(Reader reader, Pointer offset);
        bool isLittleEndian = true;
        MaskingMode masking = MaskingMode.Off;
        uint mask = 0;
        uint currentMaskByte = 0;
        byte[] maskBytes;
        byte[] originalMaskBytes = new byte[] { 0x41, 0x59, 0xBE, 0xC7, 0x0D, 0x99, 0x1C, 0xA3, 0x75, 0x3F };
        public Reader(System.IO.Stream stream) : base(stream) { isLittleEndian = true; }
        public Reader(System.IO.Stream stream, bool isLittleEndian) : base(stream) { this.isLittleEndian = isLittleEndian; }

        public override int ReadInt32() {
            var data = ReadBytes(4);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToInt32(data, 0);
        }

        public override float ReadSingle() {
            var data = ReadBytes(4);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToSingle(data, 0);
        }

        public override Int16 ReadInt16() {
            var data = ReadBytes(2);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToInt16(data, 0);
        }

        public override UInt16 ReadUInt16() {
            var data = ReadBytes(2);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToUInt16(data, 0);
        }

        public override Int64 ReadInt64() {
            var data = ReadBytes(8);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToInt64(data, 0);
        }

        public override UInt32 ReadUInt32() {
            var data = ReadBytes(4);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToUInt32(data, 0);
        }

        public override byte[] ReadBytes(int count) {
            byte[] bytes = base.ReadBytes(count);
            if (masking != MaskingMode.Off) {
                if (masking == MaskingMode.Number) {
                    for (int i = 0; i < count; i++) {
                        bytes[i] = DecodeByte(bytes[i], mask);
                        mask = GetNextMask(mask);
                    }
                } else if (masking == MaskingMode.Window) {
                    for (int i = 0; i < count; i++) {
                        bytes[i] = DecodeByteWindow(bytes[i]);
                    }
                }
            }
            return bytes;
        }

        public override byte ReadByte() {
            byte result = base.ReadByte();
            if (masking != MaskingMode.Off) {
                if (masking == MaskingMode.Number) {
                    result = DecodeByte(result, mask);
                    mask = GetNextMask(mask);
                } else if (masking == MaskingMode.Window) {
                    result = DecodeByteWindow(result);
                }
            }
            return result;
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

        public string ReadString(int size) {
            return new string(ReadChars(size)).TrimEnd('\0');
        }

        // To make sure position is a multiple of alignBytes
        public void Align(int alignBytes) {
            if (BaseStream.Position % alignBytes != 0) {
                ReadBytes(alignBytes - (int)(BaseStream.Position % alignBytes));
            }
        }

        // To make sure position is a multiple of alignBytes after reading a block of blocksize, regardless of prior position
        public void Align(int blockSize, int alignBytes) {
            int rest = blockSize % alignBytes;
            if (rest > 0) {
                ReadBytes(alignBytes - rest);
            }
        }


        #region Masking (Rayman 2)
        public void ReadMask() {
            SetMask(ReadUInt32());
        }

        public void SetMask(uint mask) {
            this.mask = mask;
            masking = MaskingMode.Number;
        }

        public void InitWindowMask() {
            maskBytes = new byte[10];
            Array.Copy(originalMaskBytes, maskBytes, maskBytes.Length);
            masking = MaskingMode.Window;
            currentMaskByte = 0;
        }

        byte DecodeByte(byte toDecode, uint mask) {
            return (byte)(toDecode ^ ((mask >> 8) & 0xFF));
        }

        byte DecodeByteWindow(byte toDecode) {
            byte returnByte = (byte)(toDecode ^ (maskBytes[currentMaskByte]));
            maskBytes[currentMaskByte] = (byte)(originalMaskBytes[currentMaskByte] + toDecode);
            currentMaskByte = (uint)((currentMaskByte + 1) % maskBytes.Length);
            return returnByte;
        }

        uint GetNextMask(uint currentMask) {
            if (MapLoader.Loader.mode == MapLoader.Mode.Rayman2IOS) {
                return (uint)(16807 * ((currentMask ^ 0x75BD924u) % 0x1F31D) - 2836 * ((currentMask ^ 0x75BD924u) / 0x1F31D));
            } else {
                return (uint)(16807 * (currentMask ^ 0x75BD924) - 0x7FFFFFFF * ((currentMask ^ 0x75BD924) / 0x1F31D));
            }
        }

        public void SetInitialMask() {
            if (Settings.s.fixedInitialMask) {
                mask = 0x6AB5CC79;
            } else {
                uint currentMask = 0xFFFFFFFF;
                mask = (uint)(16807 * (currentMask ^ 0x75BD924) - (((currentMask ^ 0x75BD924) / -127773 << 31) - (currentMask ^ 0x75BD924) / 127773));
                if ((mask & 0x80000000) != 0) {
                    mask += 0x7FFFFFFF;
                    currentMask = mask;
                }
                MapLoader.Loader.print(mask);
            }
        }

        public void SetOrReadMask() {
            if (Settings.s.useInitialMask) {
                SetInitialMask();
            } else {
                ReadMask();
            }
        }

        // Turn off masking for this binary reader
        public void MaskingOff() {
            masking = MaskingMode.Off;
        }
        #endregion
    }
}