using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

namespace PeepsCompress
{
    public class YAY0 : SlidingWindowAlgorithm
    {
        public byte[] buildYAY0CompressedBlock(ref List<byte> layoutBits, ref List<byte> uncompressedData, ref List<int[]> offsetLengthPairs, int decompressedSize, int offset)
        {
            List<byte> finalYAY0Block = new List<byte>();
            List<byte> layoutBytes = new List<byte>();
            List<byte> compressedDataBytes = new List<byte>();
            List<byte> extendedLengthBytes = new List<byte>();

            int compressedOffset = 16 + offset; //header size
            int uncompressedOffset;

            //add Yay0 magic number
            finalYAY0Block.AddRange(Encoding.ASCII.GetBytes("Yay0"));

            //add decompressed data size
            byte[] decompressedSizeArray = BitConverter.GetBytes(decompressedSize);
            Array.Reverse(decompressedSizeArray);
            finalYAY0Block.AddRange(decompressedSizeArray);

            //assemble layout bytes
            while (layoutBits.Count > 0)
            {
                while (layoutBits.Count < 8)
                {
                    layoutBits.Add(0);
                }

                string layoutBitsString = layoutBits[0].ToString() + layoutBits[1].ToString() + layoutBits[2].ToString() + layoutBits[3].ToString()
                        + layoutBits[4].ToString() + layoutBits[5].ToString() + layoutBits[6].ToString() + layoutBits[7].ToString();

                byte[] layoutByteArray = new byte[1];
                layoutByteArray[0] = Convert.ToByte(layoutBitsString, 2);
                layoutBytes.Add(layoutByteArray[0]);
                layoutBits.RemoveRange(0, (layoutBits.Count < 8) ? layoutBits.Count : 8);

            }

            //assemble offsetLength shorts
            foreach (int[] offsetLengthPair in offsetLengthPairs)
            {
                //if < 18, set 4 bits -2 as matchLength
                //if >= 18, set matchLength == 0, write length to new byte - 0x12

                int adjustedOffset = offsetLengthPair[0];
                int adjustedLength = (offsetLengthPair[1] >= 18) ? 0 : offsetLengthPair[1] - 2; //vital, 4 bit range is 0-15. Number must be at least 3 (if 2, when -2 is done, it will think it is 3 byte format), -2 is how it can store up to 17 without an extra byte because +2 will be added on decompression

                int compressedInt = ((adjustedLength << 12) | adjustedOffset - 1);

                byte[] compressed2Byte = new byte[2];
                compressed2Byte[0] = (byte)(compressedInt & 0xFF);
                compressed2Byte[1] = (byte)((compressedInt >> 8) & 0xFF);

                compressedDataBytes.Add(compressed2Byte[1]);
                compressedDataBytes.Add(compressed2Byte[0]);

                if (adjustedLength == 0)
                {
                    extendedLengthBytes.Add((byte)(offsetLengthPair[1] - 18));
                }
            }

            //pad layout bits if needed
            while (layoutBytes.Count % 4 != 0)
            {
                layoutBytes.Add(0);
            }

            compressedOffset += layoutBytes.Count;

            //add final compressed offset
            byte[] compressedOffsetArray = BitConverter.GetBytes(compressedOffset);
            Array.Reverse(compressedOffsetArray);
            finalYAY0Block.AddRange(compressedOffsetArray);

            //add final uncompressed offset
            uncompressedOffset = compressedOffset + compressedDataBytes.Count;
            byte[] uncompressedOffsetArray = BitConverter.GetBytes(uncompressedOffset);
            Array.Reverse(uncompressedOffsetArray);
            finalYAY0Block.AddRange(uncompressedOffsetArray);

            //add layout bits
            foreach (byte layoutByte in layoutBytes)                 //add layout bytes to file
            {
                finalYAY0Block.Add(layoutByte);
            }

            //add compressed data
            foreach (byte compressedByte in compressedDataBytes)     //add compressed bytes to file
            {
                finalYAY0Block.Add(compressedByte);
            }

            //non-compressed/additional-length bytes
            {
                for (int i = 0; i < layoutBytes.Count; i++)
                {
                    BitArray arrayOfBits = new BitArray(new byte[1] { layoutBytes[i] });

                    for (int j = 7; ((j > -1) && ((uncompressedData.Count > 0) || (compressedDataBytes.Count > 0))) ; j--)
                    {
                        if (arrayOfBits[j] == true)
                        {
                            finalYAY0Block.Add(uncompressedData[0]);
                            uncompressedData.RemoveAt(0);
                        }
                        else
                        {
                            if (compressedDataBytes.Count > 0)
                            {
                                int length = compressedDataBytes[0] >> 4;
                                compressedDataBytes.RemoveRange(0, 2);

                                if (length == 0)
                                {
                                    finalYAY0Block.Add(extendedLengthBytes[0]);
                                    extendedLengthBytes.RemoveAt(0);
                                }

                                
                            }
                        }
                    }
                }
            }

            return finalYAY0Block.ToArray();
        }


        public override byte[] decompress(BinaryReader br)
        {
			int offset = (int)(br.BaseStream.Position);
			Stream inputFile = br.BaseStream;
            List<byte> newFile = new List<byte>();

            string magicNumber = Encoding.ASCII.GetString(br.ReadBytes(4));

            if (magicNumber.ToLower() == "yay0")
            {
                int decompressedLength = br.ReadInt32();
                int compressedOffset = br.ReadInt32() + offset;
                int uncompressedOffset = br.ReadInt32() + offset;
                int currentOffset;

                try
                {
                    while (newFile.Count < decompressedLength)
                    {
                        byte bits = br.ReadByte(); //byte of layout bits
                        BitArray arrayOfBits = new BitArray(new byte[1] { bits });

                        for (int i = 7; i > -1 && (newFile.Count < decompressedLength); i--) //iterate through layout bits
                        {
                            if (arrayOfBits[i] == true)
                            {
                                //non-compressed
                                //add one byte from uncompressedOffset to newFile

                                currentOffset = (int)inputFile.Position;

                                inputFile.Seek(uncompressedOffset, SeekOrigin.Begin);

                                newFile.Add(br.ReadByte());
                                uncompressedOffset++;

                                inputFile.Seek(currentOffset, SeekOrigin.Begin);

                            }
                            else
                            {
                                //compressed
                                //read 2 bytes
                                //4 bits = length
                                //12 bits = offset

                                currentOffset = (int)inputFile.Position;
                                inputFile.Seek(compressedOffset, SeekOrigin.Begin);

                                byte byte1 = br.ReadByte();
                                byte byte2 = br.ReadByte();
                                compressedOffset += 2;

                                byte byte1Upper = (byte)((byte1 & 0x0F));
                                byte byte1Lower = (byte)((byte1 & 0xF0) >> 4);

                                int finalOffset = ((byte1Upper << 8) | byte2) + 1;
                                int finalLength;

                                if (byte1Lower == 0)
                                {
                                    inputFile.Seek(uncompressedOffset, SeekOrigin.Begin);
                                    finalLength = br.ReadByte() + 0x12;
                                    uncompressedOffset++;
                                }
                                else
                                {
                                    finalLength = byte1Lower + 2;
                                }

                                for (int j = 0; j < finalLength; j++) //add data for finalLength iterations
                                {
                                    newFile.Add(newFile[newFile.Count - finalOffset]); //add byte at offset (fileSize - finalOffset) to file
                                }

                                inputFile.Seek(currentOffset, SeekOrigin.Begin); //return to layout bits
                                
                            }
                        }
                    }
                
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                }

                inputFile.Close();

                return newFile.ToArray();
            }
            else
            {
				Debug.LogError("This is not a YAY0 file.");
                return null;
            }
        }


		public byte[] DecompressBytes(BinaryReader reader) {
			return decompress(reader);
		}
    }
}
