namespace BinarySerializer.Ubisoft.CPA.PS1
{
	/// <summary>
	/// A reference to a <see cref="PackedFileArchive"/>. This is used in Rayman 2 in the game executable.
	/// </summary>
	public class PackedFileArchiveReference : BinarySerializable
	{
		public byte[] Bytes_00 { get; set; }
		public uint Destination { get; set; }
		public uint Uint_0C { get; set; } // 1 if it has sound effects
		public BlockRef Main { get; set; }
		public BlockRef OverlayGame { get; set; }
		public BlockRef OverlayCine { get; set; }
		public BlockRef[] Cinematics { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			Bytes_00 = s.SerializeArray<byte>(Bytes_00, 8, name: nameof(Bytes_00));
			Destination = s.Serialize<uint>(Destination, name: nameof(Destination));
			Uint_0C = s.Serialize<uint>(Uint_0C, name: nameof(Uint_0C));
			Main = s.SerializeObject<BlockRef>(Main, name: nameof(Main));
			OverlayGame = s.SerializeObject<BlockRef>(OverlayGame, name: nameof(OverlayGame));
			OverlayCine = s.SerializeObject<BlockRef>(OverlayCine, name: nameof(OverlayCine));
			Cinematics = s.SerializeObjectArray<BlockRef>(Cinematics, 20, name: nameof(Cinematics));
		}

		public class BlockRef : BinarySerializable
		{
			public uint LBA { get; set; }
			public uint Length { get; set; }

			public override void SerializeImpl(SerializerObject s)
			{
				LBA = s.Serialize<uint>(LBA, name: nameof(LBA));
				Length = s.Serialize<uint>(Length, name: nameof(Length));
			}
		}
	}
}