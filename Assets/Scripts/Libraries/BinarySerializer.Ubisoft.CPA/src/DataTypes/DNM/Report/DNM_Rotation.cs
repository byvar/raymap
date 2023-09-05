namespace BinarySerializer.Ubisoft.CPA {
	public class DNM_Rotation : BinarySerializable {
		public float Angle { get; set; }
		public MTH3D_Vector Axis { get; set; } // Norm should be 1

		public override void SerializeImpl(SerializerObject s) {
			Angle = s.Serialize<float>(Angle, name: nameof(Angle));
			Axis = s.SerializeObject<MTH3D_Vector>(Axis, name: nameof(Axis));
		}
	}
}
