using System;

namespace BinarySerializer.Ubisoft.CPA {
	public class COL_CollideObject : BinarySerializable {
		// Vertices
		public Pointer<MTH3D_Vector[]> Points { get; set; }

		// Elements
		public Pointer<GEO_ElementType[]> ElementTypes { get; set; }
		public Pointer<Pointer<GEO_Element>[]> Elements { get; set; }

		// Octree
		public Pointer<COL_Octree> Octree { get; set; }

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
		public float BoundingSphereRadius { get; set; }
		public MTH3D_Vector BoundingSphereCenter { get; set; }

		public GEO_GeometricObject GeometricObject { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			// Points
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)
				|| s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2Revolution)) {
				PointsCount = s.Serialize<ushort>(PointsCount, name: nameof(PointsCount));
				ElementsCount = s.Serialize<ushort>(ElementsCount, name: nameof(ElementsCount));

				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3))
					ParallelBoxesCount = s.Serialize<ushort>(ParallelBoxesCount, name: nameof(ParallelBoxesCount));

				s.Align(4, Offset);

				Points = s.SerializePointer<MTH3D_Vector[]>(Points, name: nameof(Points));
				ElementTypes = s.SerializePointer<GEO_ElementType[]>(ElementTypes, name: nameof(ElementTypes));
				Elements = s.SerializePointer<Pointer<GEO_Element>[]>(Elements, name: nameof(Elements));
				Octree = s.SerializePointer<COL_Octree>(Octree, name: nameof(Octree));

				if(s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3))
					ParallelBoxes = s.SerializePointer<GEO_ParallelBox[]>(ParallelBoxes, name: nameof(ParallelBoxes));

				BoundingSphereRadius = s.Serialize<float>(BoundingSphereRadius, name: nameof(BoundingSphereRadius));
				BoundingSphereCenter = s.SerializeObject<MTH3D_Vector>(BoundingSphereCenter, name: nameof(BoundingSphereCenter));
			} else {
				GeometricObject = s.SerializeObject<GEO_GeometricObject>(GeometricObject, name: nameof(GeometricObject));
			}

			// Resolve

			// Resolve points
			Points?.ResolveObjectArray(s, PointsCount);

			// Resolve elements
			ElementTypes?.ResolveArray(s, ElementsCount);
			Elements?.ResolvePointerArray(s, ElementsCount);
			Elements?.Value?.ResolveValue(s, SerializeElement());

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
