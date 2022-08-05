namespace BinarySerializer.Ubisoft.CPA {
	public class COL_ParallelBox : BinarySerializable, ICOL_BoundingVolume {
		public MTH3D_Vector Min { get; set; }
		public MTH3D_Vector Max { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Min = s.SerializeObject<MTH3D_Vector>(Min, name: nameof(Min));
			Max = s.SerializeObject<MTH3D_Vector>(Max, name: nameof(Max));
		}
	}
}
