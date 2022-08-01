namespace BinarySerializer.Ubisoft.CPA {
	public class COL_Octree : BinarySerializable {
		public uint Pre_ElementsCount { get; set; }

		public Pointer<COL_OctreeNode> Root { get; set; }
		public ushort FacesCount { get; set; }
		public Pointer<ushort[]> ElementBasesTable { get; set; }
		public GEO_ParallelBox BoundingVolume { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Root = s.SerializePointer<COL_OctreeNode>(Root, name: nameof(Root))?.ResolveObject(s);

			FacesCount = s.Serialize<ushort>(FacesCount, name: nameof(FacesCount));
			s.Align(4, Offset);

			ElementBasesTable = s.SerializePointer<ushort[]>(ElementBasesTable, name: nameof(ElementBasesTable))
				?.ResolveArray(s, Pre_ElementsCount);

			BoundingVolume = s.SerializeObject<GEO_ParallelBox>(BoundingVolume, name: nameof(BoundingVolume));
		}
	}
}
