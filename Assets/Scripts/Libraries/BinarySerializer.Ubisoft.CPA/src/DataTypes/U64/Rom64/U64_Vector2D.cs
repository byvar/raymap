namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class U64_Vector2D : U64_Struct {
		public MTH2D_Vector Vector { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Vector = s.SerializeObject<MTH2D_Vector>(Vector, name: nameof(Vector));
		}
		public override string ShortLog => Vector.ShortLog;
		public override bool UseShortLog => true;
	}
}
