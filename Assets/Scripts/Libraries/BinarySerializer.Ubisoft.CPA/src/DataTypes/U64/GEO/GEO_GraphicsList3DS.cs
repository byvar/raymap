using BinarySerializer.N64;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GEO_GraphicsList3DS : U64_Struct {
		public ushort Pre_VerticesCount { get; set; }
		public ushort Pre_FacesCount { get; set; }

		public Triangle[] Triangles { get; set; }
		public UV[] UVs { get; set; }
		public U64_ShortVector3D[] Vertices { get; set; }
		public U64_ShortVector3D[] Colors { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Triangles = s.SerializeObjectArray<Triangle>(Triangles, Pre_FacesCount, name: nameof(Triangles));
			UVs = s.SerializeObjectArray<UV>(UVs, Pre_VerticesCount, name: nameof(UVs));
			Vertices = s.SerializeObjectArray<U64_ShortVector3D>(Vertices, Pre_VerticesCount, name: nameof(Vertices));
			Colors = s.SerializeObjectArray<U64_ShortVector3D>(Colors, Pre_VerticesCount, name: nameof(Colors));
		}

		public class Triangle : BinarySerializable {
			public ushort Index0 { get; set; }
			public ushort Index1 { get; set; }
			public ushort Index2 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Index0 = s.Serialize<ushort>(Index0, name: nameof(Index0));
				Index1 = s.Serialize<ushort>(Index1, name: nameof(Index1));
				Index2 = s.Serialize<ushort>(Index2, name: nameof(Index2));
			}
		}

		public class UV : BinarySerializable {
			public float U { get; set; }
			public float V { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				U = s.Serialize<float>(U, name: nameof(U));
				V = s.Serialize<float>(V, name: nameof(V));
			}
		}
	}
}
