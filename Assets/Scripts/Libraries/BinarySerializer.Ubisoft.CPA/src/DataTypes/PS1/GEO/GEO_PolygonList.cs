using System;

namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class GEO_PolygonList : BinarySerializable
	{
		public PolygonListType Type { get; set; }
		public ushort Ushort_02 { get; set; }
		public Pointer PolygonsPointer { get; set; }

		// Serialized from pointers
		public uint PolygonsCount { get; set; }
		public GEO_Sprite[] Sprites { get; set; }
		public GEO_QuadLOD[] QuadLODs { get; set; }
		public GEO_TriangleNoTexture[] TriangleNoTextures { get; set; }
		public GEO_QuadNoTexture[] QuadNoTextures { get; set; }
		public GEO_Triangle[] Triangles { get; set; }
		public GEO_Quad[] Quads { get; set; }

		public GEO_IPS1Polygon[] Polygons {
			get {
				switch (Type) {
					case PolygonListType.Quad: return Quads;
					case PolygonListType.QuadLOD: return QuadLODs;
					case PolygonListType.QuadNoTexture: return QuadNoTextures;
					case PolygonListType.Triangle: return Triangles;
					case PolygonListType.TriangleNoTexture: return TriangleNoTextures;
					case PolygonListType.Sprite: return Sprites;
					default: throw new BinarySerializableException(this, $"Invalid polygon type {Type}");
				}
			}
		}

		public override void SerializeImpl(SerializerObject s)
		{
			Type = s.Serialize<PolygonListType>(Type, name: nameof(Type));
			Ushort_02 = s.Serialize<ushort>(Ushort_02, name: nameof(Ushort_02));
			PolygonsPointer = s.SerializePointer(PolygonsPointer, name: nameof(PolygonsPointer));

			// Serialize data from pointers
			s.DoAt(PolygonsPointer, () =>
			{
				PolygonsCount = s.Serialize<uint>(PolygonsCount, name: nameof(PolygonsCount));

				switch (Type)
				{
					case PolygonListType.Sprite:
						Sprites = s.SerializeObjectArray<GEO_Sprite>(Sprites, PolygonsCount, name: nameof(Sprites));
						break;
					
					case PolygonListType.QuadLOD:
						QuadLODs = s.SerializeObjectArray<GEO_QuadLOD>(QuadLODs, PolygonsCount, name: nameof(QuadLODs));
						break;
					
					case PolygonListType.TriangleNoTexture:
						TriangleNoTextures = s.SerializeObjectArray<GEO_TriangleNoTexture>(TriangleNoTextures, PolygonsCount, name: nameof(TriangleNoTextures));
						break;
					
					case PolygonListType.QuadNoTexture:
						QuadNoTextures = s.SerializeObjectArray<GEO_QuadNoTexture>(QuadNoTextures, PolygonsCount, name: nameof(QuadNoTextures));
						break;
					
					case PolygonListType.Triangle:
						Triangles = s.SerializeObjectArray<GEO_Triangle>(Triangles, PolygonsCount, name: nameof(Triangles));
						break;
					
					case PolygonListType.Quad:
						Quads = s.SerializeObjectArray<GEO_Quad>(Quads, PolygonsCount, name: nameof(Quads));
						break;

					default:
						throw new ArgumentOutOfRangeException(nameof(Type), Type, null);
				}
			});
		}

		public enum PolygonListType : ushort
		{
			Sprite = 0,
			QuadLOD = 1,

			TriangleNoTexture = 3,
			QuadNoTexture = 4,
			Triangle = 5,
			Quad = 6,
		}
	}
}