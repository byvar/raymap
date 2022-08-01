namespace BinarySerializer.Ubisoft.CPA {
	public class COL_OctreeFaceIndex : BinarySerializable {
		public COL_OctreeIndex ElementIndex { get; set; }
		public COL_OctreeIndex ElementDataIndex { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			ElementIndex = s.SerializeObject<COL_OctreeIndex>(ElementIndex, name: nameof(ElementIndex));
			ElementDataIndex = s.SerializeObject<COL_OctreeIndex>(ElementDataIndex, name: nameof(ElementDataIndex));
		}
	}
}
