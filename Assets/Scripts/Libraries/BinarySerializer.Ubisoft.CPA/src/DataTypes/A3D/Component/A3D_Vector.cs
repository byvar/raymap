namespace BinarySerializer.Ubisoft.CPA {
	public class A3D_Vector : BinarySerializable {
		public MTH3D_Vector Vector { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Vector = s.SerializeObject<MTH3D_Vector>(Vector, name: nameof(Vector));
		}

		public override bool UseShortLog => base.UseShortLog;
		public override string ToString() => Vector.ToString();
	}
}