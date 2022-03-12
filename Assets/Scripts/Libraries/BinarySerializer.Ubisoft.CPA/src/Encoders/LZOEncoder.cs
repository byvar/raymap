using System;
using System.IO;
using System.IO.Compression;
using lzo.net;

namespace BinarySerializer.Ubisoft.CPA
{
	public class LZOEncoder : IStreamEncoder
	{
		public LZOEncoder(long decompressedSize, long compressedSize)
		{
			DecompressedSize = decompressedSize;
			CompressedSize = compressedSize;
		}

		public long DecompressedSize { get; }
		public long CompressedSize { get; }

		public string Name => "LZO";

		public void DecodeStream(Stream input, Stream output)
		{
			// The LzoStream wraps the input stream into a buffered stream meaning it might read more data than needed. Because of
			// this we need to copy the compressed data into a memory stream to ensure the input stream position remains correct.

			byte[] compressedBuffer = new byte[CompressedSize];
			int read = input.Read(compressedBuffer, 0, compressedBuffer.Length);

			if (read != compressedBuffer.Length)
				throw new EndOfStreamException();

			using MemoryStream compressedStream = new MemoryStream(compressedBuffer);

			using LzoStream lzo = new LzoStream(compressedStream, CompressionMode.Decompress, leaveOpen: true);
			lzo.SetLength(DecompressedSize);
			lzo.CopyTo(output);
		}

		public void EncodeStream(Stream input, Stream output)
		{
			throw new NotImplementedException();
		}
	}
}