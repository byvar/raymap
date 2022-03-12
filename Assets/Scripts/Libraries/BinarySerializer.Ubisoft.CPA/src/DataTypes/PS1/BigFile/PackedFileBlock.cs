namespace BinarySerializer.Ubisoft.CPA.PS1
{
	/// <summary>
	/// A block for a packed file
	/// </summary>
	public class PackedFileBlock : BinarySerializable
	{
		public int DecompressedSize { get; set; }
		public int CompressedSize { get; set; }
		public byte[] Data { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			DecompressedSize = s.Serialize<int>(DecompressedSize, name: nameof(DecompressedSize));

			// If the size is 0 we've reached the end of the file. If it's -1 we're in the padding between sectors.
			if (DecompressedSize == 0 || DecompressedSize == -1)
				return;

			CompressedSize = s.Serialize<int>(CompressedSize, name: nameof(CompressedSize));

			if (CompressedSize != DecompressedSize)
			{
				s.DoEncoded(new LZOEncoder(DecompressedSize, CompressedSize),
					() => Data = s.SerializeArray<byte>(Data, DecompressedSize, name: nameof(Data)));
			}
			else
			{
				Data = s.SerializeArray<byte>(Data, DecompressedSize, name: nameof(Data));
			}
		}
	}
}