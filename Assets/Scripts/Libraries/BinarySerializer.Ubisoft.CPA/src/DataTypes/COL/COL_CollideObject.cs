using System;

namespace BinarySerializer.Ubisoft.CPA {
	public class COL_CollideObject : BinarySerializable {
		// Vertices
		public Pointer<MTH3D_Vector[]> Points { get; set; }
		public Pointer<MTH3D_Vector[]> PointsNormals { get; set; }
		public Pointer<Pointer<GMT_GameMaterial>[]> PointsMaterials { get; set; }
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
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)
				|| s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2Revolution)) {
				PointsCount = s.Serialize<ushort>(PointsCount, name: nameof(PointsCount));
				ElementsCount = s.Serialize<ushort>(ElementsCount, name: nameof(ElementsCount));

				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3))
					ParallelBoxesCount = s.Serialize<ushort>(ParallelBoxesCount, name: nameof(ParallelBoxesCount));

				s.Align(4, Offset);

				ElementTypes = s.SerializePointer<GEO_ElementType[]>(ElementTypes, name: nameof(ElementTypes));
				Elements = s.SerializePointer<Pointer<GEO_Element>[]>(Elements, name: nameof(Elements));
				Octree = s.SerializePointer<COL_Octree>(Octree, name: nameof(Octree))?.ResolveObject(s);

				if(s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3))
					ParallelBoxes = s.SerializePointer<GEO_ParallelBox[]>(ParallelBoxes, name: nameof(ParallelBoxes));
			} else {
				if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
					PointsCount = s.Serialize<ushort>(PointsCount, name: nameof(PointsCount));
					s.Align(4, Offset);
				}
				Points = s.SerializePointer<MTH3D_Vector[]>(Points, name: nameof(Points));
				PointsNormals = s.SerializePointer<MTH3D_Vector[]>(PointsNormals, name: nameof(PointsNormals));
				PointsMaterials = s.SerializePointer<Pointer<GMT_GameMaterial>[]>(PointsMaterials, name: nameof(PointsMaterials));
				PointsReceivedLightIntensity = s.SerializePointer<GLI_FloatColor_RGBA[]>(PointsReceivedLightIntensity, name: nameof(PointsReceivedLightIntensity));

				// Elements
				if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
					ElementsCount = s.Serialize<ushort>(ElementsCount, name: nameof(ElementsCount));
					s.Align(4, Offset);
				}
				ElementTypes = s.SerializePointer<GEO_ElementType[]>(ElementTypes, name: nameof(ElementTypes));
				Elements = s.SerializePointer<Pointer<GEO_Element>[]>(Elements, name: nameof(Elements));

				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2))
					Octree = s.SerializePointer<COL_Octree>(Octree, name: nameof(Octree))?.ResolveObject(s);

				// Edges
				if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
					EdgesCount = s.Serialize<ushort>(EdgesCount, name: nameof(EdgesCount));
					s.Align(4, Offset);
				}
				EdgesDI = s.SerializePointer<GEO_DoubledIndex[]>(EdgesDI, name: nameof(EdgesDI));
				EdgesMaterials = s.SerializePointer<Pointer<GMT_GameMaterial>[]>(EdgesMaterials, name: nameof(EdgesMaterials));

				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
					ParallelBoxes = s.SerializePointer<GEO_ParallelBox[]>(ParallelBoxes, name: nameof(ParallelBoxes));

					Type = s.Serialize<GEO_LookAtMode>(Type, name: nameof(Type));

					PointsCount = s.Serialize<ushort>(PointsCount, name: nameof(PointsCount));
					ElementsCount = s.Serialize<ushort>(ElementsCount, name: nameof(ElementsCount));
					EdgesCount = s.Serialize<ushort>(EdgesCount, name: nameof(EdgesCount));
					ParallelBoxesCount = s.Serialize<ushort>(ParallelBoxesCount, name: nameof(ParallelBoxesCount));

					BoundingSphere = s.SerializeObject<GEO_BoundingSphere>(BoundingSphere, name: nameof(BoundingSphere));
				} else {
					SpecialValue = s.SerializePointer(SpecialValue, name: nameof(SpecialValue));
				}
			}

			// Resolve

			// Resolve points
			Points?.ResolveObjectArray(s, PointsCount);
			PointsNormals?.ResolveObjectArray(s, PointsCount);
			PointsMaterials?.ResolvePointerArray(s, PointsCount);
			PointsMaterials?.Value?.ResolveObject(s);
			PointsReceivedLightIntensity?.ResolveObjectArray(s, PointsCount);

			// Resolve elements
			ElementTypes?.ResolveArray(s, ElementsCount);
			Elements?.ResolvePointerArray(s, ElementsCount);
			Elements?.Value?.ResolveValue(s, SerializeElement());

			// Resolve edges
			EdgesDI?.ResolveObjectArray(s, EdgesCount);
			EdgesMaterials?.ResolvePointerArray(s, EdgesCount);
			EdgesMaterials?.Value?.ResolveObject(s);

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
						GEO_ElementType.IndexedTriangles => SerializeElement<COL_ElementIndexedTriangles>(),
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
