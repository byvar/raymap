namespace BinarySerializer.Ubisoft.CPA {
	public class MMG_HeaderBlockWithoutFree : BinarySerializable {
		public byte BoundedHeaderSize { get; set; }
		public byte Alignment { get; set; }
		public MMG_BlockMode Mode { get; set; }
		
		public override void SerializeImpl(SerializerObject s) {
			BoundedHeaderSize = s.Serialize<byte>(BoundedHeaderSize, name: nameof(BoundedHeaderSize));
			Alignment = s.Serialize<byte>(Alignment, name: nameof(Alignment));
			Mode = s.Serialize<MMG_BlockMode>(Mode, name: nameof(Mode));

			s.SerializePadding((int)(BoundedHeaderSize - (s.CurrentAbsoluteOffset - Offset.AbsoluteOffset)), logIfNotNull: true);
		}
	}
}
