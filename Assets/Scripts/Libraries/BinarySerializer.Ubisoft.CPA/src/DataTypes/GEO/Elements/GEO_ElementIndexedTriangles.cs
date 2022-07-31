namespace BinarySerializer.Ubisoft.CPA {
	public class GEO_ElementIndexedTriangles : GEO_Element {
		public Pointer<GMT_GameMaterial> GameMaterial { get; set; }
		public Pointer<GLI_Material> VisualMaterial { get; set; }

		public ushort FacesCount { get; set; }
		public ushort ElementUVsCount { get; set; }
		public ushort UVStagesCount { get; set; } = 1;
		public short LightmapIndex { get; set; }
		
		// Faces
		public Pointer<GEO_TripledIndex[]> FacesPoints { get; set; } // Indices for GeometricObject.Points
		public Pointer UnknownR3GC { get; set; }
		public Pointer<GEO_TripledIndex[]> FacesUVs { get; set; } // Indices for ElementUVs
		public Pointer<MTH3D_Vector[]> FacesNormals { get; set; } // 1 normal for each face

		public Pointer<GEO_UV[]> ElementUVs { get; set; }

		public Pointer<GEO_TripledIndex[]> FacesEdges { get; set; }
		public Pointer<GEO_TripledIndex[]> FacesAdjacent { get; set; }

		// Largo
		public Pointer<ushort[]> LightmapUVPoints { get; set; } // Every index 
		public ushort LightmapUVsCount { get; set; }

		public Pointer UnknownMontreal { get; set; }

		public Pointer<ushort[]> UsedIndices { get; set; } // Vertex indices used by this element
		public ushort UsedIndicesCount { get; set; }
		public short ParallelBoxIndex { get; set; }

		public Pointer<GAM_Placeholder> PS2DisplayList { get; set; }
		public Pointer SpecialValue { get; set; }
		public bool IsVisibleInPortal { get; set; }
		public uint[] VertexArrays { get; set; }

		// CPA_3 Optimized mesh
		public ushort OptimizedPointsCount { get; set; }
		public Pointer<ushort[]> OptimizedPointsIndices { get; set; }
		public Pointer<ushort[]> OptimizedPointsUV { get; set; }
		public ushort OptimizedPointsInStripCount { get; set; }
		public ushort OptimizedIsolatedTrianglesCount { get; set; }
		public Pointer<ushort[]> OptimizedPointsInStrip { get; set; }
		public Pointer<GEO_TripledIndex[]> OptimizedIsolatedTriangles { get; set; }
		public string ElementName { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)
				|| s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2Revolution)) {
				VisualMaterial = s.SerializePointer<GLI_Material>(VisualMaterial, name: nameof(VisualMaterial))?.ResolveObject(s);
			} else {
				GameMaterial = s.SerializePointer<GMT_GameMaterial>(GameMaterial, name: nameof(GameMaterial))?.ResolveObject(s);
			}
			
			FacesCount = s.Serialize<ushort>(FacesCount, name: nameof(FacesCount));
			if (s.GetCPASettings().EngineVersion != EngineVersion.Rayman2Revolution) {
				ElementUVsCount = s.Serialize<ushort>(ElementUVsCount, name: nameof(ElementUVsCount));
				if(s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)
					|| s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.LargoWinch))
					UVStagesCount = s.Serialize<ushort>(UVStagesCount, name: nameof(UVStagesCount));
			}
			if(s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2Revolution))
				LightmapIndex = s.Serialize<short>(LightmapIndex, name: nameof(LightmapIndex));

			s.Align(4, Offset);

			// Faces
			FacesPoints = s.SerializePointer<GEO_TripledIndex[]>(FacesPoints, name: nameof(FacesPoints))?.ResolveObjectArray(s, FacesCount);

			if (s.GetCPASettings().EngineVersion == EngineVersion.Rayman2Revolution)
				throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");

			if (s.GetCPASettings().EngineVersion == EngineVersion.Rayman3 && s.GetCPASettings().Platform == Platform.GC)
				UnknownR3GC = s.SerializePointer(UnknownR3GC, name: nameof(UnknownR3GC));

			FacesUVs = s.SerializePointer<GEO_TripledIndex[]>(FacesUVs, name: nameof(FacesUVs))?.ResolveObjectArray(s, FacesCount * UVStagesCount);
			FacesNormals = s.SerializePointer<MTH3D_Vector[]>(FacesNormals, name: nameof(FacesNormals))?.ResolveObjectArray(s, FacesCount);

			// UVs
			ElementUVs = s.SerializePointer<GEO_UV[]>(ElementUVs, name: nameof(ElementUVs))?.ResolveObjectArray(s, ElementUVsCount);

			if(s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_Montreal))
				UnknownMontreal = s.SerializePointer(UnknownMontreal, name: nameof(UnknownMontreal));

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				FacesEdges = s.SerializePointer<GEO_TripledIndex[]>(FacesEdges, name: nameof(FacesEdges))?.ResolveObjectArray(s, FacesCount);
				FacesAdjacent = s.SerializePointer<GEO_TripledIndex[]>(FacesAdjacent, name: nameof(FacesAdjacent))?.ResolveObjectArray(s, FacesCount);
			} else if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.LargoWinch)) {
				LightmapUVPoints = s.SerializePointer<ushort[]>(LightmapUVPoints, name: nameof(LightmapUVPoints));
				LightmapUVsCount = s.Serialize<ushort>(LightmapUVsCount, name: nameof(LightmapUVsCount));

				LightmapUVPoints?.ResolveArray(s, LightmapUVsCount);
			}

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.TonicTrouble)) {
				UsedIndices = s.SerializePointer<ushort[]>(UsedIndices, name: nameof(UsedIndices));
				UsedIndicesCount = s.Serialize<ushort>(UsedIndicesCount, name: nameof(UsedIndicesCount));
				UsedIndices?.ResolveArray(s, UsedIndicesCount);

				ParallelBoxIndex = s.Serialize<short>(ParallelBoxIndex, name: nameof(ParallelBoxIndex));
			}

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				if(s.GetCPASettings().Platform == Platform.PS2)
					PS2DisplayList = s.SerializePointer<GAM_Placeholder>(PS2DisplayList, name: nameof(PS2DisplayList))?.ResolveObject(s);
				SpecialValue = s.SerializePointer(SpecialValue, name: nameof(SpecialValue));
				IsVisibleInPortal = s.Serialize<bool>(IsVisibleInPortal, name: nameof(IsVisibleInPortal));
				if (s.GetCPASettings().Platform == Platform.PS2) {
					s.Align(4, Offset);
					VertexArrays = s.SerializeArray<uint>(VertexArrays, 4, name: nameof(VertexArrays));
				} else {
					s.Align(2, Offset);
					OptimizedPointsCount = s.Serialize<ushort>(OptimizedPointsCount, name: nameof(OptimizedPointsCount));
					OptimizedPointsIndices = s.SerializePointer<ushort[]>(OptimizedPointsIndices, name: nameof(OptimizedPointsIndices))?.ResolveArray(s, OptimizedPointsCount);
					OptimizedPointsUV = s.SerializePointer<ushort[]>(OptimizedPointsUV, name: nameof(OptimizedPointsUV))?.ResolveArray(s, OptimizedPointsCount * UVStagesCount);
					OptimizedPointsInStripCount = s.Serialize<ushort>(OptimizedPointsInStripCount, name: nameof(OptimizedPointsInStripCount));
					OptimizedIsolatedTrianglesCount = s.Serialize<ushort>(OptimizedIsolatedTrianglesCount, name: nameof(OptimizedIsolatedTrianglesCount));
					OptimizedPointsInStrip = s.SerializePointer<ushort[]>(OptimizedPointsInStrip, name: nameof(OptimizedPointsInStrip))?.ResolveArray(s, OptimizedPointsInStripCount);
					OptimizedIsolatedTriangles = s.SerializePointer<GEO_TripledIndex[]>(OptimizedIsolatedTriangles, name: nameof(OptimizedIsolatedTriangles))?.ResolveObjectArray(s, OptimizedIsolatedTrianglesCount);
					if(s.GetCPASettings().HasNames)
						ElementName = s.SerializeString(ElementName, length: 52, name: nameof(ElementName));
				}

				throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");
			}
		}
	}
}
