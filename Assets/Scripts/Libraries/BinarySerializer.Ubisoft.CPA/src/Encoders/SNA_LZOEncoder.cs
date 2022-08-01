using System;
using System.IO;
using System.IO.Compression;
using lzo.net;

namespace BinarySerializer.Ubisoft.CPA
{
	public class SNA_LZOEncoder : IStreamEncoder
	{
		public uint IsCompressed { get; private set; }
		public uint CompressedSize { get; set; }
		public uint CompressedChecksum { get; set; }
		public uint DecompressedSize { get; set; }
		public uint DecompressedChecksum { get; set; }

		public string Name => "SNA_LZOEncoding";

		public void DecodeStream(Stream input, Stream output)
		{
			using Reader reader = new Reader(input, isLittleEndian: true, leaveOpen: true);

			IsCompressed = reader.ReadUInt32();
			CompressedSize = reader.ReadUInt32();
			CompressedChecksum = reader.ReadUInt32();
			DecompressedSize = reader.ReadUInt32();
			DecompressedChecksum = reader.ReadUInt32();

			byte[] compressedData = reader.ReadBytes((int)CompressedSize);

			if (IsCompressed != 0) {
				using MemoryStream compressedStream = new MemoryStream(compressedData);
				using LzoStream lzo = new LzoStream(compressedStream, CompressionMode.Decompress, leaveOpen: true);
				lzo.SetLength(DecompressedSize);
				lzo.CopyTo(output);
			} else {
				output.Write(compressedData, 0, compressedData.Length);
			}
		}

		public void EncodeStream(Stream input, Stream output)
		{
			// No compression :)

			IsCompressed = 0;
			CompressedSize = DecompressedSize = (uint)(input.Length - input.Position);

			using Writer writer = new Writer(output, isLittleEndian: true, leaveOpen: true);
			using Reader reader = new Reader(input, isLittleEndian: true, leaveOpen: true);
			byte[] data = reader.ReadBytes((int)CompressedSize);
			CompressedChecksum = DecompressedChecksum = (uint)CalculateChecksum(data);

			writer.Write(IsCompressed);
			writer.Write(CompressedSize);
			writer.Write(CompressedChecksum);
			writer.Write(DecompressedSize);
			writer.Write(DecompressedChecksum);

			writer.Write(data);
		}

		public static SNA_LZOEncoder GetIfRequired(CPA_Settings s, uint sizeOrCount) {
			if (sizeOrCount != 0 && s.EngineVersionTree.HasParent(EngineVersion.PlaymobilHype)) {
				return new SNA_LZOEncoder();
			} else {
				return null;
			}
		}
		long CalculateChecksum(byte[] data) {
			long sum = 1;
			long v5 = 0;
			long v39 = 0;

			if (data == null) {
				return 1;
			}

			uint offset = 0;
			for (uint i = (uint)data.Length; i != 0; v5 %= 0xFFF1u) {
				uint v8 = i;
				if (i >= 5552)
					v8 = 5552;
				for (i -= v8; v8 >= 16; v5 = sum + v39) {
					v8 -= 16;
					v39 = data[offset + 14]
						+ 2 * data[offset + 13]
						+ 3 * data[offset + 12]
						+ 4 * data[offset + 11]
						+ 5 * data[offset + 10]
						+ 6 * data[offset + 9]
						+ 7 * data[offset + 8]
						+ 8 * data[offset + 7]
						+ 9 * data[offset + 6]
						+ 10 * data[offset + 5]
						+ 11 * data[offset + 4]
						+ 12 * data[offset + 3]
						+ 13 * data[offset + 2]
						+ 14 * data[offset + 1]
						+ 15 * data[offset + 0]
						+ 15 * sum
						+ v5;
					sum = data[offset + 15]
						+ data[offset + 14]
						+ data[offset + 13]
						+ data[offset + 12]
						+ data[offset + 11]
						+ data[offset + 10]
						+ data[offset + 9]
						+ data[offset + 8]
						+ data[offset + 7]
						+ data[offset + 6]
						+ data[offset + 5]
						+ data[offset + 4]
						+ data[offset + 3]
						+ data[offset + 2]
						+ data[offset + 1]
						+ data[offset + 0]
						+ sum;

					offset += 16;
				}
				if (v8 != 0) {
					do {
						sum += data[offset++];
						v5 += sum;
						--v8;
					}
					while (v8 > 0);
				}
				sum %= 0xFFF1u;
			}
			return sum | (v5 << 16);
		}
	}
}