using System;
using System.Collections;
using System.IO;

namespace BinarySerializer.Ubisoft.CPA
{
    /// <summary>
    /// Encoder for CPA ROM64 Compressed Data
    /// </summary>
    public class YAY0Encoder : IStreamEncoder
    {
        public string Name => "YAY0";

        public void DecodeStream(Stream input, Stream output) {
            // Create a reader for the input
            using var reader = new Reader(input, isLittleEndian: true, leaveOpen: true);
            long inputOffset = reader.BaseStream.Position;

            string magic = reader.ReadString(4, System.Text.Encoding.ASCII);
            if (magic.ToUpper() != "YAY0") throw new Exception($"Stream was not a valid YAY0 stream.");
            long decompressedSize = reader.ReadInt32();
            long compressedOffset = inputOffset + reader.ReadInt32();
            long uncompressedOffset = inputOffset + reader.ReadInt32();
            long currentLayoutOffset = reader.BaseStream.Position;

            var decompressedBuffer = new byte[decompressedSize];
            using var decompressedStream = new MemoryStream(decompressedBuffer);

            // Keep track of the amount of data we have decompressed
            long currentlyDecompressedSize = 0;

            // Decompress until we've decompressed everything
            while (currentlyDecompressedSize < decompressedSize) {
                // Read layout bits
                byte layoutBits = reader.ReadByte();
                currentLayoutOffset = reader.BaseStream.Position;

                for (int i = 0; (i < 8) && (currentlyDecompressedSize < decompressedSize); i++) {
                    if (BitHelpers.ExtractBits(layoutBits, 1, 7 - i) == 1) {
                        //non-compressed
                        //add one byte from uncompressedOffset to newFile
                        reader.BaseStream.Position = uncompressedOffset;

                        // Copy byte
                        decompressedStream.WriteByte(reader.ReadByte());
                        currentlyDecompressedSize++;
                        uncompressedOffset++;

                        reader.BaseStream.Position = currentLayoutOffset;
                    } else {
                        //compressed
                        //read 2 bytes
                        //4 bits = length
                        //12 bits = offset
                        reader.BaseStream.Position = compressedOffset;

                        // Read bytes from compressed offset
                        ushort compressedHeader = reader.ReadUInt16();
                        ushort offset = (ushort)(BitHelpers.ExtractBits(compressedHeader, 12, 0) + 1);
                        ushort length = (ushort)BitHelpers.ExtractBits(compressedHeader, 4, 12);
                        compressedOffset += 2;

                        // Calculate length
                        if (length == 0) {
                            reader.BaseStream.Position = uncompressedOffset;
                            length = (ushort)(reader.ReadByte() + 0x10);
                            uncompressedOffset++;
                        }
                        length += 2;

                        // Write bytes
                        var p = decompressedStream.Position - offset;
                        for (ulong j = length; j > 0; --j)
                            decompressedStream.WriteByte(decompressedBuffer[p++]);
                        currentlyDecompressedSize += length;

                        reader.BaseStream.Position = currentLayoutOffset;
                    }
                }
            }

            input.Position = Math.Max(Math.Max(currentLayoutOffset, uncompressedOffset), compressedOffset);

            // Set position back to 0
            decompressedStream.Position = 0;

            // Copy the decompressed data
            decompressedStream.CopyTo(output);
        }

        public void EncodeStream(Stream input, Stream output) => throw new NotImplementedException();
    }
}