namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class COL_GeometricObjectCollide : BinarySerializable
	{
		public ushort VerticesCount { get; set; }
		public ushort NormalsCount { get; set; }
		public ushort TrianglesCount { get; set; }
		public ushort QuadsCount { get; set; }
		public byte[] Bytes_10 { get; set; }
		public Pointer VerticesPointer { get; set; }
		public Pointer NormalsPointer { get; set; }
		public Pointer TrianglesPointer { get; set; }
		public Pointer QuadsPointer { get; set; }
		public uint Uint_38 { get; set; }

		// Serialized from pointers
		public COL_GeometricObjectCollideVector[] Vertices { get; set; }
		public COL_GeometricObjectCollideVector[] Normals { get; set; }
		public COL_GeometricObjectCollidePolygon[] Triangles { get; set; }
		public COL_GeometricObjectCollidePolygon[] Quads { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			VerticesCount = s.Serialize<ushort>(VerticesCount, name: nameof(VerticesCount));
			NormalsCount = s.Serialize<ushort>(NormalsCount, name: nameof(NormalsCount));
			TrianglesCount = s.Serialize<ushort>(TrianglesCount, name: nameof(TrianglesCount));
			QuadsCount = s.Serialize<ushort>(QuadsCount, name: nameof(QuadsCount));
			Bytes_10 = s.SerializeArray<byte>(Bytes_10, 0x20, name: nameof(Bytes_10));
			VerticesPointer = s.SerializePointer(VerticesPointer, name: nameof(VerticesPointer));
			NormalsPointer = s.SerializePointer(NormalsPointer, name: nameof(NormalsPointer));
			TrianglesPointer = s.SerializePointer(TrianglesPointer, name: nameof(TrianglesPointer));
			QuadsPointer = s.SerializePointer(QuadsPointer, name: nameof(QuadsPointer));
			Uint_38 = s.Serialize<uint>(Uint_38, name: nameof(Uint_38));

			// Serialize data from pointers
			s.DoAt(VerticesPointer, () => 
				Vertices = s.SerializeObjectArray<COL_GeometricObjectCollideVector>(Vertices, VerticesCount, name: nameof(Vertices)));
			s.DoAt(NormalsPointer, () =>
				Normals = s.SerializeObjectArray<COL_GeometricObjectCollideVector>(Normals, NormalsCount, onPreSerialize: v => v.Pre_CoordinateScale = (short.MaxValue / 8f), name: nameof(Normals)));
			s.DoAt(TrianglesPointer, () =>
				Triangles = s.SerializeObjectArray<COL_GeometricObjectCollidePolygon>(Triangles, TrianglesCount, x => x.Pre_IsQuad = false, name: nameof(Triangles)));
			s.DoAt(QuadsPointer, () =>
				Quads = s.SerializeObjectArray<COL_GeometricObjectCollidePolygon>(Quads, QuadsCount, x => x.Pre_IsQuad = true, name: nameof(Quads)));
		}
	}
}