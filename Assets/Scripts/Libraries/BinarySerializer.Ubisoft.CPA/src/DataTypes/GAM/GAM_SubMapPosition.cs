namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_SubMapPosition : BinarySerializable {
		public int SubMap { get; set; }
		public Pointer<MAT_Transformation> Position { get; set; }
		public Pointer<GAM_SubMapPosition> NextPosition { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			SubMap = s.Serialize<int>(SubMap, name: nameof(SubMap));
			Position = s.SerializePointer<MAT_Transformation>(Position, name: nameof(Position))?.ResolveObject(s);
			NextPosition = s.SerializePointer<GAM_SubMapPosition>(NextPosition, name: nameof(NextPosition))?.ResolveObject(s);
		}
	}
}
