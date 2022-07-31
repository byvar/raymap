namespace BinarySerializer.Ubisoft.CPA {
	public class COL_ElementIndexedTriangles : GEO_Element {
		public Pointer<GMT_GameMaterial> GameMaterial { get; set; }

		public ushort FacesCount { get; set; }
		public ushort ElementUVsCount { get; set; }
		
		// Faces
		public Pointer<GEO_TripledIndex[]> FacesPoints { get; set; } // Indices for GeometricObject.Points
		public Pointer<GEO_TripledIndex[]> FacesUVs { get; set; } // Indices for ElementUVs
		public Pointer<MTH3D_Vector[]> FacesNormals { get; set; } // 1 normal for each face
		public Pointer UnknownLargo { get; set; }

		public Pointer<GEO_UV[]> ElementUVs { get; set; }

		public Pointer UnknownMontreal { get; set; }

		public Pointer<ushort[]> UsedIndices { get; set; } // Vertex indices used by this element
		public ushort UsedIndicesCount { get; set; }
		public short ParallelBoxIndex { get; set; }

		// CPA_3
		public Pointer<GEO_ElementIndexedTriangles> ElementForVisualisation { get; set; }
		public Pointer<GEO_TripledIndex[]> FacesEdges { get; set; }
		public Pointer<MTH3D_Vector[]> EdgesNormals { get; set; }
		public Pointer<float[]> EdgesCoefficients { get; set; }
		public ushort EdgesCount { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			GameMaterial = s.SerializePointer<GMT_GameMaterial>(GameMaterial, name: nameof(GameMaterial))?.ResolveObject(s);

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				FacesPoints = s.SerializePointer<GEO_TripledIndex[]>(FacesPoints, name: nameof(FacesPoints));
				FacesNormals = s.SerializePointer<MTH3D_Vector[]>(FacesNormals, name: nameof(FacesNormals));
				FacesCount = s.Serialize<ushort>(FacesCount, name: nameof(FacesCount));
				ParallelBoxIndex = s.Serialize<short>(ParallelBoxIndex, name: nameof(ParallelBoxIndex));

				FacesPoints?.ResolveObjectArray(s, FacesCount);
				FacesNormals?.ResolveObjectArray(s, FacesCount);

				ElementForVisualisation = s.SerializePointer<GEO_ElementIndexedTriangles>(ElementForVisualisation, name: nameof(ElementForVisualisation))?.ResolveObject(s);

				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.RaymanM)) { // So no Dinosaur
					FacesEdges = s.SerializePointer<GEO_TripledIndex[]>(FacesEdges, name: nameof(FacesEdges))?.ResolveObjectArray(s, FacesCount);
					EdgesNormals = s.SerializePointer<MTH3D_Vector[]>(EdgesNormals, name: nameof(EdgesNormals));
					EdgesCoefficients = s.SerializePointer<float[]>(EdgesCoefficients, name: nameof(EdgesCoefficients));
					EdgesCount = s.Serialize<ushort>(EdgesCount, name: nameof(EdgesCount));
					s.Align(4, Offset);

					EdgesNormals?.ResolveObjectArray(s, EdgesCount);
					EdgesCoefficients?.ResolveArray(s, EdgesCount);
				}
			} else {
				FacesCount = s.Serialize<ushort>(FacesCount, name: nameof(FacesCount));
				ElementUVsCount = s.Serialize<ushort>(ElementUVsCount, name: nameof(ElementUVsCount));

				// Faces
				FacesPoints = s.SerializePointer<GEO_TripledIndex[]>(FacesPoints, name: nameof(FacesPoints))?.ResolveObjectArray(s, FacesCount);
				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2Revolution)) {
					if (s.GetCPASettings().EngineVersion == EngineVersion.LargoWinch) {
						FacesNormals = s.SerializePointer<MTH3D_Vector[]>(FacesNormals, name: nameof(FacesNormals))?.ResolveObjectArray(s, FacesCount);
						UnknownLargo = s.SerializePointer(UnknownLargo, name: nameof(UnknownLargo));
					}
				} else {
					FacesUVs = s.SerializePointer<GEO_TripledIndex[]>(FacesUVs, name: nameof(FacesUVs))?.ResolveObjectArray(s, FacesCount);
					FacesNormals = s.SerializePointer<MTH3D_Vector[]>(FacesNormals, name: nameof(FacesNormals))?.ResolveObjectArray(s, FacesCount);

					// UVs
					ElementUVs = s.SerializePointer<GEO_UV[]>(ElementUVs, name: nameof(ElementUVs))?.ResolveObjectArray(s, ElementUVsCount);

					if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_Montreal))
						UnknownMontreal = s.SerializePointer(UnknownMontreal, name: nameof(UnknownMontreal));

					if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.TonicTrouble)) {
						UsedIndices = s.SerializePointer<ushort[]>(UsedIndices, name: nameof(UsedIndices));
						UsedIndicesCount = s.Serialize<ushort>(UsedIndicesCount, name: nameof(UsedIndicesCount));
						UsedIndices?.ResolveArray(s, UsedIndicesCount);

						ParallelBoxIndex = s.Serialize<short>(ParallelBoxIndex, name: nameof(ParallelBoxIndex));
					}
				}
			}
		}
	}
}
