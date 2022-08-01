namespace BinarySerializer.Ubisoft.CPA {
	public class COL_OctreeNode : BinarySerializable {
		public MTH3D_Vector MinPart { get; set; }
		public MTH3D_Vector MaxPart { get; set; }
		public Pointer<Pointer<COL_OctreeNode>[]> Children { get; set; }
		public Pointer<COL_OctreeFaceIndices> FaceIndices { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			MinPart = s.SerializeObject<MTH3D_Vector>(MinPart, name: nameof(MinPart));
			MaxPart = s.SerializeObject<MTH3D_Vector>(MaxPart, name: nameof(MaxPart));
			Children = s.SerializePointer<Pointer<COL_OctreeNode>[]>(Children, name: nameof(Children))?.ResolvePointerArray(s, 8);
			FaceIndices = s.SerializePointer<COL_OctreeFaceIndices>(FaceIndices, name: nameof(FaceIndices))?.ResolveObject(s);

			Children?.Value?.ResolveObject(s);
		}
	}
}
