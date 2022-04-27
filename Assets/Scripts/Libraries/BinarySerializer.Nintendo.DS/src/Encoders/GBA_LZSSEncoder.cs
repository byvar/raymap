using System;
using System.IO;

namespace BinarySerializer.Nintendo.DS {
	// Implemented from dsdecmp, todo: refactor code to follow project coding style
	public class GBA_LZSSEncoder : GBA_Encoder {
		public override int ID => 1;
		public override string Name => "GBA_LZSS";

		protected override void DecodeStream(Reader reader, Stream output, uint decompressedSize, byte headerValue) {
			/*  Data header (32bit)
                  Bit 0-3   Reserved
                  Bit 4-7   Compressed type (must be 1 for LZ77)
                  Bit 8-31  Size of decompressed data
                Repeat below. Each Flag Byte followed by eight Blocks.
                Flag data (8bit)
                  Bit 0-7   Type Flags for next 8 Blocks, MSB first
                Block Type 0 - Uncompressed - Copy 1 Byte from Source to Dest
                  Bit 0-7   One data byte to be copied to dest
                Block Type 1 - Compressed - Copy N+3 Bytes from Dest-Disp-1 to Dest
                  Bit 0-3   Disp MSBs
                  Bit 4-7   Number of bytes to copy (minus 3)
                  Bit 8-15  Disp LSBs
             */

			// the maximum 'DISP-1' is 0xFFF.
			const int bufferLength = 0x1000;
			byte[] buffer = new byte[bufferLength];
			int bufferOffset = 0;

			int currentOutSize = 0;
			byte flags = 0;
			int mask = 1;
			while (currentOutSize < decompressedSize) {
				// (throws when requested new flags byte is not available)
				#region Update the mask. If all flag bits have been read, get a new set.
				// the current mask is the mask used in the previous run. So if it masks the
				// last flag bit, get a new flags byte.
				if (mask == 1) {
					flags = reader.ReadByte();
					mask = 0x80;
				} else {
					mask >>= 1;
				}
				#endregion

				// bit = 1 <=> compressed.
				if ((flags & mask) > 0) {
					#region Get length and displacement('disp') values from next 2 bytes

					byte byte1 = reader.ReadByte();
					byte byte2 = reader.ReadByte();

					// the number of bytes to copy
					int length = byte1 >> 4;
					length += 3;

					// from where the bytes should be copied (relatively)
					int disp = ((byte1 & 0x0F) << 8) | byte2;
					disp += 1;

					if (disp > currentOutSize)
						throw new InvalidDataException($"Cannot go back more than already written. " +
													   $"DISP = 0x{disp:X}, #written bytes = 0x{currentOutSize:X} at " +
													   $"0x{reader.BaseStream.Position - 2:X}");

					#endregion

					int bufIdx = bufferOffset + bufferLength - disp;
					for (int i = 0; i < length; i++) {
						byte next = buffer[bufIdx % bufferLength];
						bufIdx++;
						output.WriteByte(next);
						buffer[bufferOffset] = next;
						bufferOffset = (bufferOffset + 1) % bufferLength;
					}
					currentOutSize += length;
				} else {
					byte next = reader.ReadByte();
					currentOutSize++;
					output.WriteByte(next);
					buffer[bufferOffset] = next;
					bufferOffset = (bufferOffset + 1) % bufferLength;
				}
			}
		}

		protected override void EncodeStream(Reader reader, Writer writer) {
			// save the input data in an array to prevent having to go back and forth in a file
			byte[] data = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));

			// we do need to buffer the output, as the first byte indicates which blocks are compressed.
			// this version does not use a look-ahead, so we do not need to buffer more than 8 blocks at a time.
			byte[] outbuffer = new byte[8 * 2 + 1];
			outbuffer[0] = 0;
			var bufferlength = 1;
			var bufferedBlocks = 0;
			int readBytes = 0;

			while (readBytes < data.Length) {
				#region If 8 blocks are bufferd, write them and reset the buffer
				// we can only buffer 8 blocks at a time.
				if (bufferedBlocks == 8) {
					writer.Write(outbuffer, 0, bufferlength);
					// reset the buffer
					outbuffer[0] = 0;
					bufferlength = 1;
					bufferedBlocks = 0;
				}
				#endregion

				// determine if we're dealing with a compressed or raw block.
				// it is a compressed block when the next 3 or more bytes can be copied from
				// somewhere in the set of already compressed bytes.
				int oldLength = Math.Min(readBytes, 0x1000);
				int length = GetOccurrenceLength(data, readBytes, (int)Math.Min(data.Length - readBytes, 0x12), readBytes - oldLength, oldLength, out int disp);

				// length not 3 or more? next byte is raw data
				if (length < 3) {
					outbuffer[bufferlength++] = data[readBytes++];
				} else {
					// 3 or more bytes can be copied? next (length) bytes will be compressed into 2 bytes
					readBytes += length;

					// mark the next block as compressed
					outbuffer[0] |= (byte)(1 << (7 - bufferedBlocks));

					outbuffer[bufferlength] = (byte)(((length - 3) << 4) & 0xF0);
					outbuffer[bufferlength] |= (byte)(((disp - 1) >> 8) & 0x0F);
					bufferlength++;
					outbuffer[bufferlength] = (byte)((disp - 1) & 0xFF);
					bufferlength++;
				}

				bufferedBlocks++;
			}

			// copy the remaining blocks to the output
			if (bufferedBlocks > 0)
				writer.Write(outbuffer, 0, bufferlength);
		}

		private static int GetOccurrenceLength(byte[] data, int newIndex, int newLength, int oldIndex, int oldLength, out int disp, int minDisp = 1) {
			disp = 0;

			if (newLength == 0)
				return 0;

			int maxLength = 0;

			// try every possible 'disp' value (disp = oldLength - i)
			for (int i = 0; i < oldLength - minDisp; i++) {
				// work from the start of the old data to the end, to mimic the original implementation's behaviour
				// (and going from start to end or from end to start does not influence the compression ratio anyway)
				int currentOldStart = oldIndex + i;
				int currentLength = 0;

				// determine the length we can copy if we go back (oldLength - i) bytes
				// always check the next 'newLength' bytes, and not just the available 'old' bytes,
				// as the copied data can also originate from what we're currently trying to compress.
				for (int j = 0; j < newLength; j++) {
					// stop when the bytes are no longer the same
					if (data[currentOldStart + j] != data[newIndex + j])
						break;

					currentLength++;
				}

				// update the optimal value
				if (currentLength > maxLength) {
					maxLength = currentLength;
					disp = oldLength - i;

					// if we cannot do better anyway, stop trying.
					if (maxLength == newLength)
						break;
				}
			}
			return maxLength;
		}
	}
}