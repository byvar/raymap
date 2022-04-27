namespace BinarySerializer.Nintendo.DS {
	public class DS3D_Command_Begin_Vtxs : DS3D_CommandData {
		public DS3D_PrimitiveType Type { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			s.DoBits<uint>(b => {
				Type = b.SerializeBits<DS3D_PrimitiveType>(Type, 2, name: nameof(Type));
			});
		}
		public override bool UseShortLog => true;
		public override string ToString() => $"{GetType()}({Type})";
	}
}
