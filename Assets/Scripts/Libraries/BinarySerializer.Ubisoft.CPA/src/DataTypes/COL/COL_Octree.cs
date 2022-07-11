namespace BinarySerializer.Ubisoft.CPA {
	public class COL_Octree : BinarySerializable {
		public COL_OctreeNode Root { get; set; }
		public ushort FacesCount { get; set; }
		public Pointer ElementBasesTable { get; set; }
		public GEO_ParallelBox BoundingVolume { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Root = s.SerializeObject<COL_OctreeNode>(Root, name: nameof(Root));
			FacesCount = s.Serialize<ushort>(FacesCount, name: nameof(FacesCount));
			s.Align(4, Offset);
			ElementBasesTable = s.SerializePointer(ElementBasesTable, name: nameof(ElementBasesTable));
			BoundingVolume = s.SerializeObject<GEO_ParallelBox>(BoundingVolume, name: nameof(BoundingVolume));

			throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");
		}
	}
}
