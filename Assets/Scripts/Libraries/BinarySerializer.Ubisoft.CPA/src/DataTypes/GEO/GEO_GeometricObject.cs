using System;

namespace BinarySerializer.Ubisoft.CPA {
	public class GEO_GeometricObject : BinarySerializable {
		// Vertices
		public Pointer<MTH3D_Vector[]> Points { get; set; }
		public Pointer<MTH3D_Vector[]> PointsNormals { get; set; }
		public Pointer<Pointer<GMT_GameMaterial>[]> PointsMaterials { get; set; }
		public Pointer<Pointer<float[]>[]> VertexTransparency { get; set; }
		public Pointer<GLI_FloatColor_RGBA[]> PointsReceivedLightIntensity { get; set; }

		// Elements
		public Pointer<GEO_ElementType[]> ElementTypes { get; set; }
		public Pointer<Pointer<GEO_Element>[]> Elements { get; set; }

		// Octree
		public Pointer<COL_Octree> Octree { get; set; }

		// Edges
		public Pointer<GEO_DoubledIndex[]> EdgesDI { get; set; }
		public Pointer<Pointer<GMT_GameMaterial>[]> EdgesMaterials { get; set; }

		// Bounding volumes
		public Pointer<GEO_ParallelBox[]> ParallelBoxes { get; set; }

		// Type
		public GEO_LookAtMode Type { get; set; }

		// Count
		public ushort PointsCount { get; set; }
		public ushort ElementsCount { get; set; }
		public ushort EdgesCount { get; set; }
		public ushort ParallelBoxesCount { get; set; }

		// Bounding sphere
		public GEO_BoundingSphere BoundingSphere { get; set; }

		// CPA1
		public Pointer SpecialValue { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			// Points
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				PointsCount = s.Serialize<ushort>(PointsCount, name: nameof(PointsCount));
				s.Align(4, Offset);
			}
			Points = s.SerializePointer<MTH3D_Vector[]>(Points, name: nameof(Points));
			PointsNormals = s.SerializePointer<MTH3D_Vector[]>(PointsNormals, name: nameof(PointsNormals));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				VertexTransparency = s.SerializePointer<Pointer<float[]>[]>(VertexTransparency, name: nameof(VertexTransparency))
					?.ResolvePointerArray(s, 4);
			} else {
				PointsMaterials = s.SerializePointer<Pointer<GMT_GameMaterial>[]>(PointsMaterials, name: nameof(PointsMaterials));
			}
			if ((s.GetCPASettings().Platform != Platform.GC || s.GetCPASettings().EngineVersion == EngineVersion.Rayman3) // Only present in R3 GC, not in other GC games
				&& (s.GetCPASettings().Platform != Platform.PS2 || s.GetCPASettings().EngineVersion != EngineVersion.Rayman3) // Not present in R3 PS2
				&& s.GetCPASettings().EngineVersion != EngineVersion.RaymanM) {
				PointsReceivedLightIntensity = s.SerializePointer<GLI_FloatColor_RGBA[]>(PointsReceivedLightIntensity, name: nameof(PointsReceivedLightIntensity));
			}

			// Elements
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				ElementsCount = s.Serialize<ushort>(ElementsCount, name: nameof(ElementsCount));
				s.Align(4, Offset);
			}
			ElementTypes = s.SerializePointer<GEO_ElementType[]>(ElementTypes, name: nameof(ElementTypes));
			Elements = s.SerializePointer<Pointer<GEO_Element>[]>(Elements, name: nameof(Elements));

			if(s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2) && !s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3))
				Octree = s.SerializePointer<COL_Octree>(Octree, name: nameof(Octree));

			// Edges
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				EdgesCount = s.Serialize<ushort>(EdgesCount, name: nameof(EdgesCount));
				s.Align(4, Offset);
			}
			EdgesDI = s.SerializePointer<GEO_DoubledIndex[]>(EdgesDI, name: nameof(EdgesDI));
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3))
				EdgesMaterials = s.SerializePointer<Pointer<GMT_GameMaterial>[]>(EdgesMaterials, name: nameof(EdgesMaterials));

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				ParallelBoxes = s.SerializePointer<GEO_ParallelBox[]>(ParallelBoxes, name: nameof(ParallelBoxes));
				
				Type = s.Serialize<GEO_LookAtMode>(Type, name: nameof(Type));

				PointsCount = s.Serialize<ushort>(PointsCount, name: nameof(PointsCount));
				ElementsCount = s.Serialize<ushort>(ElementsCount, name: nameof(ElementsCount));
				EdgesCount = s.Serialize<ushort>(EdgesCount, name: nameof(EdgesCount));
				ParallelBoxesCount = s.Serialize<ushort>(ParallelBoxesCount, name: nameof(ParallelBoxesCount));

				BoundingSphere = s.SerializeObject<GEO_BoundingSphere>(BoundingSphere, name: nameof(BoundingSphere));

				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
					throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");
				}
			} else {
				SpecialValue = s.SerializePointer(SpecialValue, name: nameof(SpecialValue));
			}

			// Resolve

			// Resolve points
			Points?.ResolveObjectArray(s, PointsCount);
			PointsNormals?.ResolveObjectArray(s, PointsCount);
			PointsMaterials?.ResolvePointerArray(s, PointsCount);
			PointsMaterials?.Value?.ResolveObject(s);
			VertexTransparency?.Value?.ResolveArray(s, PointsCount);
			PointsReceivedLightIntensity?.ResolveObjectArray(s, PointsCount);

			// Resolve elements
			ElementTypes?.ResolveArray(s, ElementsCount);
			Elements?.ResolvePointerArray(s, ElementsCount);
			Elements?.Value?.ResolveValue(s, SerializeElement());

			// Resolve edges
			EdgesDI?.ResolveObjectArray(s, EdgesCount);
			EdgesMaterials?.ResolvePointerArray(s, EdgesCount);
			EdgesMaterials?.Value?.ResolveObject(s);

			// Resolve octree
			Octree?.ResolveObject(s, onPreSerialize: o => o.Pre_ElementsCount = ElementsCount);

			// Resolve parallel boxes
			ParallelBoxes?.ResolveObjectArray(s, ParallelBoxesCount);
		}

		private Func<int, PointerFunctions.SerializeFunction<GEO_Element>> SerializeElement(Action<GEO_Element> onPreSerialize = null) {
			return (index) => {
				return (s, value, name) => {
					T SerializeElement<T>() where T : GEO_Element, new() {
						return s.SerializeObject<T>((T)value, onPreSerialize: onPreSerialize, name: name);
					}

					var elTypes = ElementTypes?.Value;
					return elTypes[index] switch {
						GEO_ElementType.IndexedTriangles => SerializeElement<GEO_ElementIndexedTriangles>(),
						GEO_ElementType.Sprites => SerializeElement<GEO_ElementSprites>(),
						GEO_ElementType.Points => SerializeElement<GEO_ElementPoints>(),
						GEO_ElementType.Spheres => SerializeElement<GEO_ElementSpheres>(),
						GEO_ElementType.AlignedBoxes => SerializeElement<GEO_ElementAlignedBoxes>(),
						GEO_ElementType.Cones => SerializeElement<GEO_ElementCones>(),
						_ => throw new BinarySerializableException(this, $"Unsupported element type {elTypes[index]}")
					};
				};
			};
		}
	}
}
