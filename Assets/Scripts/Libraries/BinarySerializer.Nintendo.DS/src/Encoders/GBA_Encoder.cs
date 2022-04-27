using System;
using System.IO;

namespace BinarySerializer.Nintendo.DS {
	public abstract class GBA_Encoder : IStreamEncoder {
		protected virtual byte HeaderValue => 0;

		public abstract int ID { get; }
		public abstract string Name { get; }

		protected abstract void DecodeStream(Reader reader, Stream output, uint decompressedSize, byte headerValue);
		protected abstract void EncodeStream(Reader reader, Writer writer);

		public void DecodeStream(Stream input, Stream output) {
			using Reader reader = new Reader(input, isLittleEndian: true, leaveOpen: true);

			// Read the header
			byte header = reader.ReadByte();

			byte headerValue = (byte)BitHelpers.ExtractBits(header, 4, 0);
			int id = BitHelpers.ExtractBits(header, 4, 4);

			// Verify that the ID matches
			if (id != ID)
				throw new InvalidDataException($"The data is not compressed using {Name}");

			// Read the decompressed size
			uint decompressedSize = reader.ReadUInt24();

			if (decompressedSize == 0)
				decompressedSize = reader.ReadUInt32();

			// Decompress the data
			DecodeStream(reader, output, decompressedSize, headerValue);
		}

		public void EncodeStream(Stream input, Stream output) {
			if (input.Length > UInt24.MaxValue)
				throw new Exception($"Input length can not exceed {UInt24.MaxValue} bytes");

			using Reader reader = new Reader(input, isLittleEndian: true, leaveOpen: true);
			using Writer writer = new Writer(output, isLittleEndian: true, leaveOpen: true);

			// Write the header
			writer.Write((byte)((ID << 4) | HeaderValue));

			// Write the length
			writer.Write((UInt24)input.Length);

			// Compress the data
			EncodeStream(reader, writer);
		}
	}
}