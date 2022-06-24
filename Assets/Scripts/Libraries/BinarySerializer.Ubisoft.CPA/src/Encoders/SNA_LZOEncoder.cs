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
			CompressedChecksum = DecompressedChecksum = 0; // TODO

			using Writer writer = new Writer(output, isLittleEndian: true, leaveOpen: true);
			using Reader reader = new Reader(input, isLittleEndian: true, leaveOpen: true);

			writer.Write(IsCompressed);
			writer.Write(CompressedSize);
			writer.Write(CompressedChecksum);
			writer.Write(DecompressedSize);
			writer.Write(DecompressedChecksum);

			writer.Write(reader.ReadBytes((int)CompressedSize));
		}

		public static SNA_LZOEncoder GetIfRequired(CPA_Settings s) {
			if (s.EngineVersionTree.HasParent(EngineVersion.PlaymobilHype)) {
				return new SNA_LZOEncoder();
			} else {
				return null;
			}
		}
	}
}