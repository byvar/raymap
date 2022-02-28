namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class U64_Vector3D : U64_Struct {
		public CPA_Vector3D Vector { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Vector = s.SerializeObject<CPA_Vector3D>(Vector, name: nameof(Vector));
		}
		public override string ShortLog => Vector.ShortLog;
		public override bool UseShortLog => true;
	}
}
