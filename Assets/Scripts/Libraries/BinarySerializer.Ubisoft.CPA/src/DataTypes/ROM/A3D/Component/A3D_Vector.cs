namespace BinarySerializer.Ubisoft.CPA.ROM {
	public class A3D_Vector : BinarySerializable {
		public int IntX { get; set; } // Divide by 4096 to get float
		public int IntY { get; set; }
		public int IntZ { get; set; }

		public CPA_Vector Vector { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().Platform == Platform.N64) {
				Vector = s.SerializeObject<CPA_Vector>(Vector, name: nameof(Vector));
			} else {
				IntX = s.Serialize<int>(IntX, name: nameof(IntX));
				IntY = s.Serialize<int>(IntY, name: nameof(IntY));
				IntZ = s.Serialize<int>(IntZ, name: nameof(IntZ));
			}
		}
	}
}