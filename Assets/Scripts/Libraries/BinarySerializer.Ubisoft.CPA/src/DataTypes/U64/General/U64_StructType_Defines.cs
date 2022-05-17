using System;
using System.Collections.Generic;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public static class U64_StructType_Defines {
		private static Dictionary<ushort, U64_StructType> InitTypesDictionary(Context c) {
			var s = c.GetCPASettings();
			List<U64_StructType> Types = new List<U64_StructType>();
			Types.AddRange(new U64_StructType[] {
				U64_StructType.FixData,
				U64_StructType.Character,
				U64_StructType.Comport,
				U64_StructType.AIModel,
				U64_StructType.Family,
				U64_StructType.State,
				U64_StructType.ObjectsTable,
				U64_StructType.InputAction,
				U64_StructType.AnimInfo,
				U64_StructType.Character3dData,
				U64_StructType.ArrayOfZoneSet,
				U64_StructType.ActivationZone,
				U64_StructType.Waypoint,
				U64_StructType.WayGraph,
				U64_StructType.WayGraphNode,
				U64_StructType.ArrayOfZdx,
				U64_StructType.Light,
				U64_StructType.Texture, // size: 0x2
			});
			if(s.Platform != Platform._3DS)
				Types.AddRange(new U64_StructType[] {
					U64_StructType.BitmapCI4,
					U64_StructType.BitmapCI8,
					U64_StructType.BitmapRGBA16,
					U64_StructType.PaletteRGBA16,
				});
			Types.Add(U64_StructType.U64VertexList);
			if (s.Platform == Platform._3DS) {
				Types.Add(U64_StructType.GraphicsList3DS);
			} else if (s.EngineVersionTree.HasParent(EngineVersion.RaymanRavingRabbids)) {
				Types.Add(U64_StructType.CompressedDSGraphicsList);
			} else {
				Types.Add(U64_StructType.U64GraphicsList);
			}
			Types.AddRange(new U64_StructType[] {
				U64_StructType.GeometricElementU64IndexedTriangles,
				U64_StructType.GeometricElementSprites,
				U64_StructType.GeometricElementSpheres,
				U64_StructType.GeometricElementAlignedBoxes,
				U64_StructType.GeometricElementPoints,
				U64_StructType.GeometricElementCones,
				U64_StructType.Altimap,
				U64_StructType.GeometricObject,
				U64_StructType.GameMaterial,
				U64_StructType.CollideMaterial,
				U64_StructType.VisualMaterial,
				U64_StructType.IdCardBase,
				U64_StructType.IdCardCamera,
				U64_StructType.MechanicalMaterial,
				U64_StructType.PhysicalCollSet__Zoo,
				U64_StructType.PhysicalObject,
				U64_StructType.Sector,
				U64_StructType.SuperObject,
				U64_StructType.GeometricElementCollideTrianglesData__ColFacesPnt,
				U64_StructType.LevelsNameList,
				U64_StructType.Level,
				U64_StructType.SubLevel,
				U64_StructType.SubLevelList,
				U64_StructType.LevelEntry,
				U64_StructType.LevelEntryList,
				U64_StructType.SuperObjectChildList,
				U64_StructType.Vertex,
				U64_StructType.Edge,
				U64_StructType.AnimListTable,
				U64_StructType.LST_SectorGraphic_and_LST_Character,
				U64_StructType.LST_SectorActivity,
				U64_StructType.LST_SectorCollision,
				U64_StructType.LST_SectorStaticLights,
				U64_StructType.LST_SectorSound,
				U64_StructType.LST_SectorSoundParam,
				U64_StructType.LST_SectorSoundEvent,
				U64_StructType.LST_SectorSoundEventParam,
				U64_StructType.MechanicalEnvironment,
				U64_StructType.VisualEnvironment,
				U64_StructType.BitmapInfo, // size: 14
				U64_StructType.DsgVar__Declaration,
				U64_StructType.StateList,
				U64_StructType.CharacterBrain,
				U64_StructType.CharacterCineInfo,
				U64_StructType.CharacterCollSet,
				U64_StructType.CharacterLight,
				U64_StructType.CharacterMicro,
				U64_StructType.CharacterStandardGame,
				U64_StructType.CharacterStream,
				U64_StructType.CharacterWorld,
				U64_StructType.CharacterSound,
				U64_StructType.ObjectsTableList,
				U64_StructType.DsgVarInfo__DeclarationVariable,
				U64_StructType.Intelligence,
				U64_StructType.ListOfComport,
				U64_StructType.ListOfRules,
				U64_StructType.Rule,
				U64_StructType.Reflex,
				U64_StructType.ScriptNodeArray__Node,
				U64_StructType.LST_GeometricObject,
				U64_StructType.LST_GeometricElementAlignedBoxes,
				U64_StructType.LST_GeometricElementSpheres,
				U64_StructType.LST_GeometricElementCones,
				U64_StructType.LST_GeometricElementPoints,
				U64_StructType.LST_ZoneSet,
				U64_StructType.LST_ZdxIndex,
				U64_StructType.Way,
				U64_StructType.LST_NodeIndex_and_NoCtrlTextureList,
				U64_StructType.LST_Capacity,
				U64_StructType.LST_Valuation,
				U64_StructType.LST_WaypointIndex,
				U64_StructType.WayList,
				U64_StructType.LST_WayListIndex,
				U64_StructType.LST_SectorGraphicParam,
				U64_StructType.LST_SoundEvent,
				U64_StructType.Node_Long,
				U64_StructType.Node_Float,
				U64_StructType.Node_Vector3D,
				U64_StructType.Node_String,
				U64_StructType.Declaration_Long,
				U64_StructType.Declaration_UnsignedLong,
				U64_StructType.Declaration_Float,
				U64_StructType.Declaration_Vector3D,
				U64_StructType.StateRef,
				U64_StructType.StateRefList,
				U64_StructType.BoundingVolume,
				U64_StructType.LipsSynchro,
				U64_StructType.DsgMem__Initialization,
				U64_StructType.InitVariable,
				U64_StructType.InitVariableIdList,
				U64_StructType.TypeVariable,
				U64_StructType.TypeVariableIdList,
				U64_StructType.DscInput,
				U64_StructType.DscInputActionList,
				U64_StructType.InputLink,
				U64_StructType.InputLinkList,
				U64_StructType.InputList,
				U64_StructType.ItemStringList,
				U64_StructType.WayLink,
				U64_StructType.WayLinkVector,
				U64_StructType.LST_WayLinkIndex,
				U64_StructType.AnimVector3D,
				U64_StructType.AnimTripledIndex,
				U64_StructType.AllVector3D,
				U64_StructType.AllTripledIndex,
				U64_StructType.ArrayOfStrings,
				U64_StructType.StringList,
				U64_StructType.SoundEventList,
				U64_StructType.NbOfLanguages,
				U64_StructType.TextString,
				U64_StructType.LanguageString,
				U64_StructType.TextStringList,
				U64_StructType.StringLength,
				U64_StructType.StringLengthList,
				U64_StructType.LanguageStringList,
				U64_StructType.OldVertexIndex,
				U64_StructType.AnimSpeedList,
				U64_StructType.DscMiscInfo,
				U64_StructType.DscLvl,
				U64_StructType.Mem,
				U64_StructType.LevelDescription,
				U64_StructType.FixPreloadSection,
				U64_StructType.FixMem,
				U64_StructType.LevelMem,
				U64_StructType.GeometricElementU64List,
				U64_StructType.GeometricElementCollideIndexedTriangles,
				U64_StructType.GeometricElementCollideList,
				U64_StructType.AltimapSquare,
				U64_StructType.AltimapFace,
				U64_StructType.AltimapUV,
				U64_StructType.AltimapVertex,
				U64_StructType.CharacterDyn,
				U64_StructType.TextureList,
				U64_StructType.StringChunk,
				U64_StructType.ArrayEntry,
				U64_StructType.BackgroundCI8,
				U64_StructType.BackgroundPaletteList,
				U64_StructType.BackgroundInfo,
				U64_StructType.NODFile,
				U64_StructType.VignetteCount,
				U64_StructType.CPakFont,
				// TOOD: Add other entries? DontSaveStructStart starts here
			});
			Dictionary<ushort, U64_StructType> dict = new Dictionary<ushort, U64_StructType>();
			for(ushort i = 0; i < Types.Count; i++) {
				dict[i] = Types[i];
			}
			return c.StoreObject<Dictionary<ushort, U64_StructType>>(ContextKey, dict);
		}

		private static readonly Dictionary<Type, U64_StructType> TypeMapping = new Dictionary<Type, U64_StructType>() {
			// General
			[typeof(MTH3D_Vector)] = U64_StructType.AllVector3D,
			[typeof(MTH3D_ShortVector)] = U64_StructType.Vertex,
			[typeof(U64_TripledIndex)] = U64_StructType.AllTripledIndex,
			[typeof(U64_BoundingVolume)] = U64_StructType.BoundingVolume,
			[typeof(U64_StringChunk)] = U64_StructType.StringChunk,

			// GAM
			[typeof(GAM_Fix)] = U64_StructType.FixData,
			[typeof(GAM_LevelsNameList)] = U64_StructType.LevelsNameList,
			[typeof(GAM_FixPreloadSection)] = U64_StructType.FixPreloadSection,
			[typeof(GAM_Character)] = U64_StructType.Character,
			[typeof(GAM_Character3dData)] = U64_StructType.Character3dData,
			[typeof(GAM_CharacterStandardGame)] = U64_StructType.CharacterStandardGame,
			[typeof(GAM_CharacterBrain)] = U64_StructType.CharacterBrain,
			[typeof(GAM_CharacterCollSet)] = U64_StructType.CharacterCollSet,
			[typeof(GAM_CharacterMicro)] = U64_StructType.CharacterMicro,
			[typeof(GAM_CharacterDynamics)] = U64_StructType.CharacterDyn,
			[typeof(GAM_CharacterCineInfo)] = U64_StructType.CharacterCineInfo,
			[typeof(GAM_CharacterLight)] = U64_StructType.CharacterLight,
			[typeof(GAM_CharacterSound)] = U64_StructType.CharacterSound,
			[typeof(GAM_Family)] = U64_StructType.Family,
			[typeof(GAM_State)] = U64_StructType.State,
			[typeof(GAM_StateRef)] = U64_StructType.StateRef,
			[typeof(GAM_AnimInfo)] = U64_StructType.AnimInfo,
			[typeof(GAM_StateTransition)] = U64_StructType.StateList,
			[typeof(GAM_ObjectsTable)] = U64_StructType.ObjectsTable,
			[typeof(GAM_ObjectsTableEntry)] = U64_StructType.ObjectsTableList,
			[typeof(GAM_ZdxArray)] = U64_StructType.ArrayOfZdx,
			[typeof(GAM_ZdxIndex)] = U64_StructType.LST_ZdxIndex,
			[typeof(GAM_ZoneSetArray)] = U64_StructType.ArrayOfZoneSet,
			[typeof(GAM_ZoneSet)] = U64_StructType.LST_ZoneSet,
			[typeof(GAM_ActivationZone)] = U64_StructType.ActivationZone,
			[typeof(LST_ReferenceElement<GAM_State>)] = U64_StructType.StateRefList,
			[typeof(LST_ReferenceElement<GAM_Character>)] = U64_StructType.LST_SectorGraphic_and_LST_Character,

			// HIE
			[typeof(HIE_SuperObject)] = U64_StructType.SuperObject,
			[typeof(LST_ReferenceElement<HIE_SuperObject>)] = U64_StructType.SuperObjectChildList,

			// PO
			[typeof(PO_PhysicalObject)] = U64_StructType.PhysicalObject,
			[typeof(PO_Zoo)] = U64_StructType.PhysicalCollSet__Zoo,

			// GEO
			[typeof(GEO_GeometricObject)] = U64_StructType.GeometricObject,
			[typeof(GEO_VisualElementListEntry)] = U64_StructType.GeometricElementU64List,
			[typeof(GEO_ElementVisualIndexedTriangles)] = U64_StructType.GeometricElementU64IndexedTriangles,
			[typeof(GEO_ElementSprites)] = U64_StructType.GeometricElementSprites,
			[typeof(GEO_CollisionElementListEntry)] = U64_StructType.GeometricElementCollideList,
			[typeof(GEO_ElementCollisionIndexedTriangles)] = U64_StructType.GeometricElementCollideIndexedTriangles,
			[typeof(GEO_ElementSpheres)] = U64_StructType.GeometricElementSpheres,
			[typeof(GEO_Sphere)] = U64_StructType.LST_GeometricElementSpheres,
			[typeof(GEO_TripledIndex)] = U64_StructType.GeometricElementCollideTrianglesData__ColFacesPnt,
			[typeof(GEO_VerticesList)] = U64_StructType.U64VertexList,
			[typeof(GEO_GraphicsList3DS)] = U64_StructType.GraphicsList3DS,
			[typeof(GEO_GraphicsList)] = U64_StructType.U64GraphicsList,
			[typeof(GEO_CompressedGraphicsListDS)] = U64_StructType.CompressedDSGraphicsList,
			[typeof(LST_ReferenceElement<GEO_GeometricObject>)] = U64_StructType.LST_GeometricObject,

			// GLI
			[typeof(GLI_VisualMaterial)] = U64_StructType.VisualMaterial,
			[typeof(GLI_TextureListEntry)] = U64_StructType.TextureList,
			[typeof(GLI_Texture)] = U64_StructType.Texture,
			[typeof(GLI_BitmapInfo)] = U64_StructType.BitmapInfo,

			[typeof(GLI_BitmapCI4)] = U64_StructType.BitmapCI4,
			[typeof(GLI_BitmapCI8)] = U64_StructType.BitmapCI8,
			[typeof(GLI_BitmapRGBA16)] = U64_StructType.BitmapRGBA16,
			[typeof(GLI_PaletteRGBA16)] = U64_StructType.PaletteRGBA16,

			// GMT
			[typeof(GMT_GameMaterial)] = U64_StructType.GameMaterial,
			[typeof(GMT_CollideMaterial)] = U64_StructType.CollideMaterial,


			// MEC
			[typeof(MEC_IdCardBase)] = U64_StructType.IdCardBase,
			[typeof(MEC_IdCardCamera)] = U64_StructType.IdCardCamera,

			// AI
			[typeof(AI_AIModel)] = U64_StructType.AIModel,
			[typeof(AI_Intelligence)] = U64_StructType.Intelligence,
			[typeof(AI_Comport)] = U64_StructType.Comport,
			[typeof(AI_Rule)] = U64_StructType.Rule,
			[typeof(AI_Node)] = U64_StructType.ScriptNodeArray__Node,
			[typeof(AI_NodeInterpretFull)] = U64_StructType.NODFile,

			[typeof(AI_Node_Long)] = U64_StructType.Node_Long,
			[typeof(AI_Node_Float)] = U64_StructType.Node_Float,
			[typeof(AI_Node_Vector3D)] = U64_StructType.Node_Vector3D,
			[typeof(AI_Node_String)] = U64_StructType.Node_String,
			[typeof(AI_Declaration_UnsignedLong)] = U64_StructType.Declaration_UnsignedLong,

			[typeof(LST_ReferenceElement<AI_Comport>)] = U64_StructType.ListOfComport,
			[typeof(LST_ReferenceElement<AI_Rule>)] = U64_StructType.ListOfRules,

			// WAY
			[typeof(WAY_Graph)] = U64_StructType.WayGraph,
			[typeof(WAY_GraphNode)] = U64_StructType.WayGraphNode,
			[typeof(WAY_Capacity)] = U64_StructType.LST_Capacity,
			[typeof(WAY_Valuation)] = U64_StructType.LST_Valuation,
			[typeof(WAY_WayPoint)] = U64_StructType.Waypoint,
			[typeof(LST_ReferenceElement<WAY_WayPoint>)] = U64_StructType.LST_WaypointIndex,
			[typeof(LST_ReferenceElement<WAY_GraphNode>)] = U64_StructType.LST_NodeIndex_and_NoCtrlTextureList,

			// IPT
			[typeof(IPT_InputAction)] = U64_StructType.InputAction,
			[typeof(IPT_InputElement)] = U64_StructType.InputList,

			// TODO
		};

		public static Dictionary<ushort, U64_StructType> GetTypeDictionary(Context c) {
			Dictionary<ushort, U64_StructType> dict =
				c.GetStoredObject<Dictionary<ushort, U64_StructType>>(ContextKey) ?? InitTypesDictionary(c);
			return dict;
		}

		public static U64_StructType? GetType(Context c, ushort index) {
			var dict = GetTypeDictionary(c);

			if (dict.ContainsKey(index)) {
				return dict[index];
			} else {
				return null;
			}
		}

		public static U64_StructType? GetType(Type type, bool throwException = true) {
			var dict = TypeMapping;
			if (dict.ContainsKey(type)) {
				return dict[type];
			} else {
				if(throwException)
					throw new NotImplementedException($"Type {type} does not have a corresponding ROM StructType");
				return null;
			}
		}

		private const string ContextKey = "U64_StructTypes_Dictionary";
	}
}
