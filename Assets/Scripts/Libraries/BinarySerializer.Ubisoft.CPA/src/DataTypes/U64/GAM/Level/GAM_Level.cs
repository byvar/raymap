using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_Level : U64_Struct {
		public U64_Reference<U64_Placeholder> EntireLevel { get; set; }
		public U64_ArrayReference<U64_Placeholder> SubLevelList { get; set; }
		public U64_ArrayReference<LST_ReferenceElement<GAM_LevelEntry>> AlwaysEntryList { get; set; }
		public U64_Reference<GAM_LevelDescription> LevelDescription { get; set; }
		public U64_Reference<GAM_GenericMemory> MemoryDescription { get; set; }
		
		//public U64_Reference<GLI_VisualMaterial> SkyVisualMaterial { get; set; }
		public ExtendedFarClippingFlags ExtendedFarClipping { get; set; }
		public U64_ArrayReference<LST_ReferenceElement<U64_Placeholder>> RRRDSUnknown { get; set; } // StructType: RRRDS_Unknown1List. Count is always 0 in RRR DS and Rayman 3D though
		public ushort SubLevelListCount { get; set; }
		public ushort AlwaysEntryListCount { get; set; }
		public ushort RRRDSUnknownCount { get; set; }

		//public ushort LevelGameSaveLength { get; set; }
		public ushort[] TextureCacheSize { get; set; } // Unused on DS

		public ushort MorphTasksCount { get; set; }
		public ushort MorphElementsCount { get; set; }
		public ushort MorphPointsCount { get; set; }
		public ushort MorphQuality { get; set; }

		public ushort GraphicsSize { get; set; }
		public ushort GraphicsTransSize { get; set; }
		public ushort MatrixSize { get; set; }
		public ushort LightSize { get; set; }
		public ushort FifoSizeInKB { get; set; }

		public U64_ArrayReference<MTH3D_Vector> Vector3D { get; set; }
		public U64_ArrayReference<U64_TripledIndex> TripledIndex { get; set; }
		public ushort Vector3DCount { get; set; }
		public ushort TripledIndexCount { get; set; }

		public ushort CoordinateScale { get; set; }

		// Donald Duck only
		public U64_Reference<GLI_VisualMaterial> Background_TopLeft { get; set; }
		public U64_Reference<GLI_VisualMaterial> Background_TopRight { get; set; }
		public U64_Reference<GLI_VisualMaterial> Background_BottomLeft { get; set; }
		public U64_Reference<GLI_VisualMaterial> Background_BottomRight { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			EntireLevel = s.SerializeObject<U64_Reference<U64_Placeholder>>(EntireLevel, name: nameof(EntireLevel))?.Resolve(s);
			SubLevelList = s.SerializeObject<U64_ArrayReference<U64_Placeholder>>(SubLevelList, name: nameof(SubLevelList));
			AlwaysEntryList = s.SerializeObject<U64_ArrayReference<LST_ReferenceElement<GAM_LevelEntry>>>(AlwaysEntryList, name: nameof(AlwaysEntryList));
			LevelDescription = s.SerializeObject<U64_Reference<GAM_LevelDescription>>(LevelDescription, name: nameof(LevelDescription))?.Resolve(s);
			MemoryDescription = s.SerializeObject<U64_Reference<GAM_GenericMemory>>(MemoryDescription, name: nameof(MemoryDescription))?.Resolve(s);

			//SkyVisualMaterial = s.SerializeObject<U64_Reference<GLI_VisualMaterial>>(SkyVisualMaterial, name: nameof(SkyVisualMaterial))?.Resolve(s);
			ExtendedFarClipping = s.Serialize<ExtendedFarClippingFlags>(ExtendedFarClipping, name: nameof(ExtendedFarClipping));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.RaymanRavingRabbids))
				RRRDSUnknown = s.SerializeObject<U64_ArrayReference<LST_ReferenceElement<U64_Placeholder>>>(RRRDSUnknown, name: nameof(RRRDSUnknown));

			SubLevelListCount = s.Serialize<ushort>(SubLevelListCount, name: nameof(SubLevelListCount));
			AlwaysEntryListCount = s.Serialize<ushort>(AlwaysEntryListCount, name: nameof(AlwaysEntryListCount));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.RaymanRavingRabbids)) 
				RRRDSUnknownCount = s.Serialize<ushort>(RRRDSUnknownCount, name: nameof(RRRDSUnknownCount));

			SubLevelList?.Resolve(s, SubLevelListCount);
			AlwaysEntryList?.Resolve(s, AlwaysEntryListCount);
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.RaymanRavingRabbids))
				RRRDSUnknown?.Resolve(s, RRRDSUnknownCount);

			//LevelGameSaveLength = s.Serialize<ushort>(LevelGameSaveLength, name: nameof(LevelGameSaveLength));
			TextureCacheSize = s.SerializeArray<ushort>(TextureCacheSize, 6, name: nameof(TextureCacheSize));

			MorphTasksCount = s.Serialize<ushort>(MorphTasksCount, name: nameof(MorphTasksCount));
			MorphElementsCount = s.Serialize<ushort>(MorphElementsCount, name: nameof(MorphElementsCount));
			MorphPointsCount = s.Serialize<ushort>(MorphPointsCount, name: nameof(MorphPointsCount));
			MorphQuality = s.Serialize<ushort>(MorphQuality, name: nameof(MorphQuality));

			GraphicsSize = s.Serialize<ushort>(GraphicsSize, name: nameof(GraphicsSize));
			GraphicsTransSize = s.Serialize<ushort>(GraphicsTransSize, name: nameof(GraphicsTransSize));
			MatrixSize = s.Serialize<ushort>(MatrixSize, name: nameof(MatrixSize));
			LightSize = s.Serialize<ushort>(LightSize, name: nameof(LightSize));
			FifoSizeInKB = s.Serialize<ushort>(FifoSizeInKB, name: nameof(FifoSizeInKB));

			Vector3D = s.SerializeObject<U64_ArrayReference<MTH3D_Vector>>(Vector3D, name: nameof(Vector3D));
			TripledIndex = s.SerializeObject<U64_ArrayReference<U64_TripledIndex>>(TripledIndex, name: nameof(TripledIndex));
			Vector3DCount = s.Serialize<ushort>(Vector3DCount, name: nameof(Vector3DCount));
			TripledIndexCount = s.Serialize<ushort>(TripledIndexCount, name: nameof(TripledIndexCount));
			Vector3D?.Resolve(s, Vector3DCount);
			TripledIndex?.Resolve(s, TripledIndexCount);

			CoordinateScale = s.Serialize<ushort>(CoordinateScale, name: nameof(CoordinateScale));

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.DonaldDuckQuackAttack)) {
				Background_TopLeft = s.SerializeObject<U64_Reference<GLI_VisualMaterial>>(Background_TopLeft, name: nameof(Background_TopLeft))?.Resolve(s);
				Background_TopRight = s.SerializeObject<U64_Reference<GLI_VisualMaterial>>(Background_TopRight, name: nameof(Background_TopRight))?.Resolve(s);
				Background_BottomLeft = s.SerializeObject<U64_Reference<GLI_VisualMaterial>>(Background_BottomLeft, name: nameof(Background_BottomLeft))?.Resolve(s);
				Background_BottomRight = s.SerializeObject<U64_Reference<GLI_VisualMaterial>>(Background_BottomRight, name: nameof(Background_BottomRight))?.Resolve(s);
			}
		}

		// Index resolve actions
		public static MTH3D_Vector GetVector3DIndex(U64_Index<MTH3D_Vector> index) => index.Context.GetLoader().Level?.Value?.Vector3D?.Value[index.Index];

		[Flags]
		public enum ExtendedFarClippingFlags : ushort {
			NormalClipping   = (0 << 0), // Camera Near = 1, Far = 256
			ExtendedClipping = (1 << 0), // Camera Near = 2, Far = 1024
			EraseBackground  = (1 << 1)
		}
	}
}
