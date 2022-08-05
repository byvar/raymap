namespace BinarySerializer.Ubisoft.CPA {
	public class COL_BoundingSphere : BinarySerializable, ICOL_BoundingVolume {
		public MTH3D_Vector Center { get; set; }
		public float Radius { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Center = s.SerializeObject<MTH3D_Vector>(Center, name: nameof(Center));
			Radius = s.Serialize<float>(Radius, name: nameof(Radius));
		}
	}
}
