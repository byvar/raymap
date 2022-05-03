namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class U64_BoundingVolume : U64_Struct {
		public CPA.MTH3D_Vector SphereCenter { get; set; }
		public float SphereRadius { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			SphereCenter = s.SerializeObject<CPA.MTH3D_Vector>(SphereCenter, name: nameof(SphereCenter));
			SphereRadius = s.Serialize<float>(SphereRadius, name: nameof(SphereRadius));
		}
	}
}
