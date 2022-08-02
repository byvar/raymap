namespace BinarySerializer.Ubisoft.CPA {
	public class GEO_Edge : BinarySerializable {
		public short[] Points { get; set; }
		public MTH3D_Vector[] VectorPoints { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Points = s.SerializeArray<short>(Points, 2, name: nameof(Points));
			VectorPoints = s.SerializeObjectArray<MTH3D_Vector>(VectorPoints, 2, name: nameof(VectorPoints));
		}
	}
}
