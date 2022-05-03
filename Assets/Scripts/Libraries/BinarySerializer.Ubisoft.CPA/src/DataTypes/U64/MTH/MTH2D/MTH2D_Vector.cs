namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class MTH2D_Vector : U64_Struct {
		public CPA.MTH2D_Vector Vector { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Vector = s.SerializeObject<CPA.MTH2D_Vector>(Vector, name: nameof(Vector));
		}
		public override string ShortLog => Vector.ShortLog;
		public override bool UseShortLog => true;
	}
}
