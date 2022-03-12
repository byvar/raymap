namespace BinarySerializer.Ubisoft.CPA.PS1
{
	/// <summary>
	/// An archive of multiple packed files. Usually the files for a level.
	/// </summary>
	public class PackedFileArchive : BinarySerializable
	{
		public long Pre_MaxLength { get; set; }

		public PackedFile[] Files { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			long endOffset = s.CurrentFileOffset + Pre_MaxLength;

			Files = s.SerializeObjectArrayUntil(Files, 
				x => x.Blocks[0].DecompressedSize == -1 || s.CurrentFileOffset >= endOffset, name: nameof(Files));
		}
	}
}