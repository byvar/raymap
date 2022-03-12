using System;
using System.Linq;

namespace BinarySerializer.Ubisoft.CPA.PS1
{
	/// <summary>
	/// A file packed in a big file which consists of multiple, often compressed, blocks
	/// </summary>
	public class PackedFile : BinarySerializable
	{
		public PackedFileBlock[] Blocks { get; set; }

		/// <summary>
		/// Combines the blocks and returns the full file data
		/// </summary>
		/// <returns>The full file data</returns>
		public byte[] GetFileBytes()
		{
			int fileSize = Blocks.Sum(x => x.DecompressedSize);

			if (fileSize <= 0)
				return null;

			byte[] fileData = new byte[fileSize];

			int offset = 0;

			foreach (PackedFileBlock block in Blocks.Where(x => x.Data != null))
			{
				Array.Copy(block.Data, 0, fileData, offset, block.DecompressedSize);
				offset += block.DecompressedSize;
			}

			return fileData;
		}

		public override void SerializeImpl(SerializerObject s)
		{
			Blocks = s.SerializeObjectArrayUntil<PackedFileBlock>(Blocks, x => x.Data == null, name: nameof(Blocks));
		}
	}
}