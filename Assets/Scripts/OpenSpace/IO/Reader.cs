using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OpenSpace {
    public class Reader : BinaryReader {
        public enum MaskingMode {
            Off,
            Number,
            Window,
            RedPlanet
        }
        public delegate void ReadAction(Reader reader, Pointer offset);
        Encoding wind1252 = Encoding.GetEncoding(1252);
        bool isLittleEndian = true;
        MaskingMode masking = MaskingMode.Off;
        uint mask = 0;
        uint currentMaskByte = 0;
        uint bytesReadSinceAlignStart = 0;
        bool autoAlignOn = false;
        byte[] maskBytes;
        byte[] originalMaskBytes = new byte[] { 0x41, 0x59, 0xBE, 0xC7, 0x0D, 0x99, 0x1C, 0xA3, 0x75, 0x3F };
        public Reader(System.IO.Stream stream) : base(stream) { isLittleEndian = true; }
        public Reader(System.IO.Stream stream, bool isLittleEndian) : base(stream) { this.isLittleEndian = isLittleEndian; }
        public bool AutoAligning {
            get { return autoAlignOn; }
            set { autoAlignOn = value; bytesReadSinceAlignStart = 0; }
        }

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

		public override UInt64 ReadUInt64() {
			var data = ReadBytes(8);
			if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
			return BitConverter.ToUInt64(data, 0);
		}

		public override byte[] ReadBytes(int count) {
            byte[] bytes = base.ReadBytes(count);
            if(autoAlignOn) bytesReadSinceAlignStart += (uint)bytes.Length;
            if (masking != MaskingMode.Off) {
                if (masking == MaskingMode.Number || masking == MaskingMode.RedPlanet) {
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
            if(autoAlignOn) bytesReadSinceAlignStart++;
            if (masking != MaskingMode.Off) {
                if (masking == MaskingMode.Number || masking == MaskingMode.RedPlanet) {
                    result = DecodeByte(result, mask);
                    mask = GetNextMask(mask);
                } else if (masking == MaskingMode.Window) {
                    result = DecodeByteWindow(result);
                }
            }
            return result;
        }

        public string ReadNullDelimitedString(Encoding encoding = null) {
            string result = "";
            if (encoding == null) {
                encoding = wind1252;
            }
            //if (encoding != null) {
                List<byte> bytes = new List<byte>();
                byte b = ReadByte();
                while (b != 0x0) {
                    bytes.Add(b);
                    b = ReadByte();
                }
                if (bytes.Count > 0) {
                    return encoding.GetString(bytes.ToArray());
                }
            /*} else {
                char c = Convert.ToChar(ReadByte());
                while (c != 0x0) {
                    result += c;
                    c = Convert.ToChar(ReadByte());
                }
            }*/
            return result;
        }

        public string ReadString(int size) {
			byte[] bytes = ReadBytes(size);
			int firstIndexOf = Array.IndexOf(bytes, (byte)0x0);
			if (firstIndexOf >= 0 && firstIndexOf < bytes.Length) {
				if(firstIndexOf == 0) return "";
				Array.Resize(ref bytes, firstIndexOf);
			}
			return wind1252.GetString(bytes);
			//return System.Text.Encoding.UTF8.GetString(ReadBytes(size)).TrimEnd('\0');
		}

        // To make sure position is a multiple of alignBytes
        public void Align(int alignBytes) {
            if (BaseStream.Position % alignBytes != 0) {
                ReadBytes(alignBytes - (int)(BaseStream.Position % alignBytes));
            }
        }
        public void AlignOffset(int alignBytes, int offset) {
            if ((BaseStream.Position - offset) % alignBytes != 0) {
                ReadBytes(alignBytes - (int)((BaseStream.Position - offset) % alignBytes));
            }
        }

        // To make sure position is a multiple of alignBytes after reading a block of blocksize, regardless of prior position
        public void Align(int blockSize, int alignBytes) {
            int rest = blockSize % alignBytes;
            if (rest > 0) {
                byte[] aligned = ReadBytes(alignBytes - rest);
                foreach (byte b in aligned) if (b != 0x0) throw new Exception("fuuuuuuuu");
            }
        }
        
        public void AutoAlign(int alignBytes) {
            if (bytesReadSinceAlignStart % alignBytes != 0) {
                ReadBytes(alignBytes - (int)(bytesReadSinceAlignStart % alignBytes));
            }
            bytesReadSinceAlignStart = 0;
        }


        #region Masking (Rayman 2)
        public void ReadMask() {
            SetMask(ReadUInt32());
        }

        public void SetMask(uint mask) {
            this.mask = mask;
            masking = MaskingMode.Number;
        }

        private void InitWindowMask() {
            maskBytes = new byte[10];
            Array.Copy(originalMaskBytes, maskBytes, maskBytes.Length);
            masking = MaskingMode.Window;
            currentMaskByte = 0;
        }

        byte DecodeByte(byte toDecode, uint mask) {
            if (masking == MaskingMode.RedPlanet) {
                //return (byte)((toDecode + (mask & 0xFF)) ^ ((mask >> 8) & 0xFF));
                //return (byte)(toDecode ^ (((mask & 0xFF) + ((mask >> 8) & 0xFF))));
                //return (byte)(((mask & 0xFF) + (toDecode ^ ((mask >> 8) & 0xFF))) & 0xFF);
                //return (byte)((mask + (toDecode ^ (mask >> 8))) & 0xFF);
                return (byte)((((mask >> 8) & 0xFF) ^ ((toDecode + 0x100) - (mask & 0xFF))) & 0xFF);
            } else {
                return (byte)(toDecode ^ ((mask >> 8) & 0xFF));
            }
        }

        byte DecodeByteWindow(byte toDecode) {
            byte returnByte = (byte)(toDecode ^ (maskBytes[currentMaskByte]));
            maskBytes[currentMaskByte] = (byte)(originalMaskBytes[currentMaskByte] + toDecode);
            currentMaskByte = (uint)((currentMaskByte + 1) % maskBytes.Length);
            return returnByte;
        }

        uint GetNextMask(uint currentMask) {
            if (masking == MaskingMode.RedPlanet) {
                byte mask0 = (byte)(((mask & 0xFF) + ((mask >> 16) & 0xFF)) & 0xFF);
                byte mask1 = (byte)((((mask >> 8) & 0xFF) + ((mask >> 24) & 0xFF)) & 0xFF);
                return (uint)((mask & 0xFFFF0000) + (mask1 << 8) + (mask0));
            } else {
                // 0x075BD924 = 123459876
                if (Settings.s.platform == Settings.Platform.iOS) {
                    return (uint)(16807 * ((currentMask ^ 0x75BD924u) % 0x1F31D) - 2836 * ((currentMask ^ 0x75BD924u) / 0x1F31D));
                } else {
                    return (uint)(16807 * (currentMask ^ 0x75BD924) - 0x7FFFFFFF * ((currentMask ^ 0x75BD924) / 0x1F31D));
                }
            }
        }

        public int InitMask() {
            switch (Settings.s.encryption) {
                case Settings.Encryption.ReadInit:
                    ReadMask(); return 4;
                case Settings.Encryption.Window:
                    InitWindowMask(); return 0;
                case Settings.Encryption.FixedInit:
                    mask = 0x6AB5CC79; return 0;
                case Settings.Encryption.RedPlanet:
                    ReadUInt32();
                    mask = 0x6AB5CC79;
                    masking = MaskingMode.RedPlanet;
                    return 4;
                case Settings.Encryption.CalculateInit:
                    uint currentMask = 0xFFFFFFFF;
					// 0x075BD924 = 123459876
					mask = (uint)(16807 * (currentMask ^ 0x75BD924) - (((currentMask ^ 0x75BD924) / -127773 << 31) - (currentMask ^ 0x75BD924) / 127773));
                    if ((mask & 0x80000000) != 0) {
                        mask += 0x7FFFFFFF;
                        currentMask = mask;
                    }
                    return 0;
                default:
                    return 0;
            }
        }

        // Turn off masking for this binary reader
        public void MaskingOff() {
            masking = MaskingMode.Off;
        }
        #endregion
    }
}