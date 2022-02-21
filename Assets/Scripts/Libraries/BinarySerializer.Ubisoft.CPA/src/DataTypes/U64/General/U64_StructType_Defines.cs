﻿using System.Collections.Generic;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public static class U64_StructType_Defines {
		public static Dictionary<ushort, U64_StructType> TypesDS = new Dictionary<ushort, U64_StructType>() {
			{ 0, U64_StructType.FixData },
			{ 1, U64_StructType.Character },
			{ 2, U64_StructType.Comport },
			{ 3, U64_StructType.AIModel },
			{ 4, U64_StructType.Family },
			{ 5, U64_StructType.State },
			{ 6, U64_StructType.ObjectsTable },
			{ 7, U64_StructType.InputAction },
			{ 8, U64_StructType.AnimInfo },
			{ 9, U64_StructType.Character3dData },
			{ 10, U64_StructType.ArrayOfZoneSet },
			{ 11, U64_StructType.ActivationZone },
			{ 12, U64_StructType.Waypoint },
			{ 13, U64_StructType.WayGraph },
			{ 14, U64_StructType.WayGraphNode },
			{ 15, U64_StructType.ArrayOfZdx },
			{ 16, U64_StructType.Light },
			{ 17, U64_StructType.Texture }, // size: 0x2
			{ 18, U64_StructType.BitmapCI4 },
			{ 19, U64_StructType.BitmapCI8 },
			{ 20, U64_StructType.BitmapRGBA16 },
			{ 21, U64_StructType.PaletteRGBA16 },
			{ 22, U64_StructType.U64VertexList },
			{ 23, U64_StructType.U64GraphicsList },
			{ 24, U64_StructType.GeometricElementU64IndexedTriangles },
			{ 25, U64_StructType.GeometricElementSprites },
			{ 26, U64_StructType.GeometricElementSpheres },
			{ 27, U64_StructType.GeometricElementAlignedBoxes },
			{ 28, U64_StructType.GeometricElementPoints },
			{ 29, U64_StructType.GeometricElementCones },
			{ 30, U64_StructType.Altimap },
			{ 31, U64_StructType.GeometricObject },
			{ 32, U64_StructType.GameMaterial },
			{ 33, U64_StructType.CollideMaterial },
			{ 34, U64_StructType.VisualMaterial },
			{ 35, U64_StructType.IdCardBase },
			{ 36, U64_StructType.IdCardCamera },
			{ 37, U64_StructType.MechanicalMaterial },
			{ 38, U64_StructType.PhysicalCollSet__Zoo },
			{ 39, U64_StructType.PhysicalObject },
			{ 40, U64_StructType.Sector },
			{ 41, U64_StructType.SuperObject },
			{ 42, U64_StructType.GeometricElementCollideTrianglesData__ColFacesPnt },
			{ 43, U64_StructType.LevelsNameList },
			{ 44, U64_StructType.Level },
			{ 45, U64_StructType.SubLevel },
			{ 46, U64_StructType.SubLevelList },
			{ 47, U64_StructType.LevelEntry },
			{ 48, U64_StructType.LevelEntryList },
			{ 49, U64_StructType.SuperObjectChildList },
			{ 50, U64_StructType.Vertex },
			{ 51, U64_StructType.Edge },
			{ 52, U64_StructType.AnimListTable },
			{ 53, U64_StructType.LST_SectorGraphic_and_PersoArray },
			{ 54, U64_StructType.LST_SectorActivity },
			{ 55, U64_StructType.LST_SectorCollision },
			{ 56, U64_StructType.LST_SectorStaticLights },
			{ 57, U64_StructType.LST_SectorSound },
			{ 58, U64_StructType.LST_SectorSoundParam },
			{ 59, U64_StructType.LST_SectorSoundEvent },
			{ 60, U64_StructType.LST_SectorSoundEventParam },
			{ 61, U64_StructType.MechanicalEnvironment },
			{ 62, U64_StructType.VisualEnvironment },
			{ 63, U64_StructType.BitmapInfo }, // size: 14
            { 64, U64_StructType.DsgVar__Declaration },
			{ 65, U64_StructType.StateList },
			{ 66, U64_StructType.CharacterBrain },
			{ 67, U64_StructType.CharacterCineInfo },
			{ 68, U64_StructType.CharacterCollSet },
			{ 69, U64_StructType.CharacterLight },
			{ 70, U64_StructType.CharacterMicro },
			{ 71, U64_StructType.CharacterStandardGame },
			{ 72, U64_StructType.CharacterStream },
			{ 73, U64_StructType.CharacterWorld },
			{ 74, U64_StructType.CharacterSound },
			{ 75, U64_StructType.ObjectsTableList },
			{ 76, U64_StructType.DsgVarInfo__DeclarationVariable },
			{ 77, U64_StructType.Intelligence },
			{ 78, U64_StructType.ListOfComport },
			{ 79, U64_StructType.ListOfRules },
			{ 80, U64_StructType.Rule },
			{ 81, U64_StructType.Reflex },
			{ 82, U64_StructType.ScriptNodeArray__Node },
			{ 83, U64_StructType.LST_GeometricObject },
			{ 84, U64_StructType.LST_GeometricElementAlignedBoxes },
			{ 85, U64_StructType.LST_GeometricElementSpheres },
			{ 86, U64_StructType.LST_GeometricElementCones },
			{ 87, U64_StructType.LST_GeometricElementPoints },
			{ 88, U64_StructType.LST_ZoneSet },
			{ 89, U64_StructType.LST_ZdxIndex },
			{ 90, U64_StructType.Way },
			{ 91, U64_StructType.LST_NodeIndex_and_NoCtrlTextureList },
			{ 92, U64_StructType.LST_Capacity },
			{ 93, U64_StructType.LST_Valuation },
			{ 94, U64_StructType.LST_WaypointIndex },
			{ 95, U64_StructType.WayList },
			{ 96, U64_StructType.LST_WayListIndex },
			{ 97, U64_StructType.LST_SectorGraphicParam },
			{ 98, U64_StructType.LST_SoundEvent },
			{ 99, U64_StructType.Node_Long },
			{ 100, U64_StructType.Node_Float },
			{ 101, U64_StructType.Node_Vector3D },
			{ 102, U64_StructType.Node_String },
			{ 103, U64_StructType.Declaration_Long },
			{ 104, U64_StructType.Declaration_UnsignedLong },
			{ 105, U64_StructType.Declaration_Float },
			{ 106, U64_StructType.Declaration_Vector3D },
			{ 107, U64_StructType.StateRef },
			{ 108, U64_StructType.StateRefList },
			{ 109, U64_StructType.BoundingVolume },
			{ 110, U64_StructType.LipsSynchro },
			{ 111, U64_StructType.DsgMem__Initialization },
			{ 112, U64_StructType.InitVariable },
			{ 113, U64_StructType.InitVariableIdList },
			{ 114, U64_StructType.TypeVariable },
			{ 115, U64_StructType.TypeVariableIdList },
			{ 116, U64_StructType.DscInput },
			{ 117, U64_StructType.DscInputActionList },
			{ 118, U64_StructType.InputLink },
			{ 119, U64_StructType.InputLinkList },
			{ 120, U64_StructType.InputList },
			{ 121, U64_StructType.ItemStringList },
			{ 122, U64_StructType.WayLink },
			{ 123, U64_StructType.WayLinkVector },
			{ 124, U64_StructType.LST_WayLinkIndex },
			{ 125, U64_StructType.AnimVector3D },
			{ 126, U64_StructType.AnimTripledIndex },
			{ 127, U64_StructType.AllVector3D },
			{ 128, U64_StructType.AllTripledIndex },
			{ 129, U64_StructType.ArrayOfStrings },
			{ 130, U64_StructType.StringList },
			{ 131, U64_StructType.SoundEventList },
			{ 132, U64_StructType.NbOfLanguages },
			{ 133, U64_StructType.TextString },
			{ 134, U64_StructType.LanguageString },
			{ 135, U64_StructType.TextStringList },
			{ 136, U64_StructType.StringLength },
			{ 137, U64_StructType.StringLengthList },
			{ 138, U64_StructType.LanguageStringList },
			{ 139, U64_StructType.OldVertexIndex },
			{ 140, U64_StructType.AnimSpeedList },
			{ 141, U64_StructType.DscMiscInfo },
			{ 142, U64_StructType.DscLvl },
			{ 143, U64_StructType.Mem },
			{ 144, U64_StructType.LevelDescription },
			{ 145, U64_StructType.FixPreloadSection },
			{ 146, U64_StructType.FixMem },
			{ 147, U64_StructType.LevelMem },
			{ 148, U64_StructType.GeometricElementU64List },
			{ 149, U64_StructType.GeometricElementCollideIndexedTriangles },
			{ 150, U64_StructType.GeometricElementCollideList },
			{ 151, U64_StructType.AltimapSquare },
			{ 152, U64_StructType.AltimapFace },
			{ 153, U64_StructType.AltimapUV },
			{ 154, U64_StructType.AltimapVertex },
			{ 155, U64_StructType.CharacterDyn },
			{ 156, U64_StructType.TextureList },
			{ 157, U64_StructType.StringChunk },
			{ 158, U64_StructType.ArrayEntry },
			// TOOD: Add other entries
		};

		public static Dictionary<ushort, U64_StructType> TypesN64 = new Dictionary<ushort, U64_StructType>() {
			{ 0, U64_StructType.FixData },
			{ 1, U64_StructType.Character },
			{ 2, U64_StructType.Comport },
			{ 3, U64_StructType.AIModel },
			{ 4, U64_StructType.Family },
			{ 5, U64_StructType.State },
			{ 6, U64_StructType.ObjectsTable },
			{ 7, U64_StructType.InputAction },
			{ 8, U64_StructType.AnimInfo },
			{ 9, U64_StructType.Character3dData },
			{ 10, U64_StructType.ArrayOfZoneSet },
			{ 11, U64_StructType.ActivationZone },
			{ 12, U64_StructType.Waypoint },
			{ 13, U64_StructType.WayGraph },
			{ 14, U64_StructType.WayGraphNode },
			{ 15, U64_StructType.ArrayOfZdx },
			{ 16, U64_StructType.Light },
			{ 17, U64_StructType.Texture }, // size: 0x2
			{ 21, U64_StructType.Palette },
			{ 22, U64_StructType.U64VertexList },
			{ 23, U64_StructType.U64GraphicsList },
			{ 24, U64_StructType.GeometricElementU64IndexedTriangles },
			{ 25, U64_StructType.GeometricElementSprites },
			{ 26, U64_StructType.GeometricElementSpheres },
			{ 27, U64_StructType.GeometricElementAlignedBoxes },
			{ 31, U64_StructType.GeometricObject },
			{ 32, U64_StructType.GameMaterial },
			{ 33, U64_StructType.CollideMaterial },
			{ 34, U64_StructType.VisualMaterial },
			{ 37, U64_StructType.MechanicalMaterial },
			{ 38, U64_StructType.PhysicalCollSet__Zoo },
			{ 39, U64_StructType.PhysicalObject },
			{ 40, U64_StructType.Sector },
			{ 41, U64_StructType.SuperObject },
			{ 42, U64_StructType.GeometricElementCollideTrianglesData__ColFacesPnt },
			{ 43, U64_StructType.LevelsNameList },
			{ 44, U64_StructType.Level },
			{ 45, U64_StructType.SubLevel },
			{ 46, U64_StructType.SubLevelList },
			{ 47, U64_StructType.LevelEntry },
			{ 48, U64_StructType.LevelEntryList },
			{ 49, U64_StructType.SuperObjectChildList },
			{ 50, U64_StructType.Vertex },
			{ 53, U64_StructType.LST_SectorGraphic_and_PersoArray },
			{ 54, U64_StructType.LST_SectorActivity },
			{ 55, U64_StructType.LST_SectorCollision },
			{ 56, U64_StructType.LST_SectorStaticLights },
			{ 57, U64_StructType.LST_SectorSound },
			{ 58, U64_StructType.LST_SectorSoundParam },
			{ 59, U64_StructType.LST_SectorSoundEvent },
			{ 60, U64_StructType.LST_SectorSoundEventParam },
			{ 63, U64_StructType.BitmapInfo }, // size: 12
            { 64, U64_StructType.DsgVar__Declaration },
			{ 65, U64_StructType.StateList },
			{ 66, U64_StructType.CharacterBrain },
			{ 67, U64_StructType.CharacterCineInfo },
			{ 68, U64_StructType.CharacterCollSet },
			{ 70, U64_StructType.CharacterMicro },
			{ 71, U64_StructType.CharacterStandardGame },
			{ 75, U64_StructType.ObjectsTableList },
			{ 76, U64_StructType.DsgVarInfo__DeclarationVariable },
			{ 77, U64_StructType.Intelligence },
			{ 78, U64_StructType.ListOfComport },
			{ 79, U64_StructType.ListOfRules },
			{ 80, U64_StructType.Rule },
			{ 81, U64_StructType.Reflex },
			{ 82, U64_StructType.ScriptNodeArray__Node },
			{ 83, U64_StructType.LST_GeometricObject },
			{ 84, U64_StructType.LST_GeometricElementAlignedBoxes },
			{ 85, U64_StructType.LST_GeometricElementSpheres },
			{ 88, U64_StructType.LST_ZoneSet },
			{ 89, U64_StructType.LST_ZdxIndex },
			{ 91, U64_StructType.LST_NodeIndex_and_NoCtrlTextureList },
			{ 92, U64_StructType.LST_Capacity },
			{ 93, U64_StructType.LST_Valuation },
			{ 97, U64_StructType.LST_SectorGraphicParam },
			{ 99, U64_StructType.Node_Long },
			{ 100, U64_StructType.Node_Float },
			{ 101, U64_StructType.Node_Vector3D },
			{ 102, U64_StructType.Node_String },
			{ 103, U64_StructType.Declaration_Long },
			{ 104, U64_StructType.Declaration_UnsignedLong },
			{ 105, U64_StructType.Declaration_Float },
			{ 106, U64_StructType.Declaration_Vector3D },
			{ 107, U64_StructType.StateRef },
			{ 108, U64_StructType.StateRefList },
			{ 111, U64_StructType.DsgMem__Initialization },
			{ 112, U64_StructType.InitVariable },
			{ 113, U64_StructType.InitVariableIdList },
			{ 118, U64_StructType.InputLink },
			{ 119, U64_StructType.InputLinkList },
			{ 120, U64_StructType.InputList },
			{ 127, U64_StructType.AllVector3D },
			{ 128, U64_StructType.AllTripledIndex },
			{ 132, U64_StructType.NbOfLanguages },
			{ 133, U64_StructType.TextString },
			{ 134, U64_StructType.LanguageString },
			{ 135, U64_StructType.TextStringList },
			{ 136, U64_StructType.StringLength },
			{ 137, U64_StructType.StringLengthList },
			{ 145, U64_StructType.FixPreloadSection },
			{ 148, U64_StructType.GeometricElementU64List },
			{ 149, U64_StructType.GeometricElementCollideIndexedTriangles },
			{ 150, U64_StructType.GeometricElementCollideList },
			{ 156, U64_StructType.TextureList },
			{ 157, U64_StructType.StringChunk },
			{ 158, U64_StructType.ArrayEntry },
		};

		public static Dictionary<ushort, U64_StructType> Types3DS = new Dictionary<ushort, U64_StructType>() {
			{ 0, U64_StructType.FixData },
			{ 1, U64_StructType.Character },
			{ 2, U64_StructType.Comport },
			{ 3, U64_StructType.AIModel },
			{ 4, U64_StructType.Family },
			{ 5, U64_StructType.State },
			{ 6, U64_StructType.ObjectsTable },
			{ 7, U64_StructType.InputAction },
			{ 8, U64_StructType.AnimInfo },
			{ 9, U64_StructType.Character3dData },
			{ 10, U64_StructType.ArrayOfZoneSet },
			{ 11, U64_StructType.ActivationZone },
			{ 12, U64_StructType.Waypoint },
			{ 13, U64_StructType.WayGraph },
			{ 14, U64_StructType.WayGraphNode },
			{ 15, U64_StructType.ArrayOfZdx },
			{ 16, U64_StructType.Light },
			{ 17, U64_StructType.Texture }, // size: 0x2
			{ 18, U64_StructType.U64VertexList },
			{ 19, U64_StructType.U64GraphicsList },
			{ 20, U64_StructType.GeometricElementU64IndexedTriangles },
			{ 21, U64_StructType.GeometricElementSprites },
			{ 22, U64_StructType.GeometricElementSpheres },
			{ 23, U64_StructType.GeometricElementAlignedBoxes },
			{ 27, U64_StructType.GeometricObject },
			{ 28, U64_StructType.GameMaterial },
			{ 29, U64_StructType.CollideMaterial },
			{ 30, U64_StructType.VisualMaterial },
			{ 33, U64_StructType.MechanicalMaterial },
			{ 34, U64_StructType.PhysicalCollSet__Zoo },
			{ 35, U64_StructType.PhysicalObject },
			{ 36, U64_StructType.Sector },
			{ 37, U64_StructType.SuperObject },
			{ 38, U64_StructType.GeometricElementCollideTrianglesData__ColFacesPnt },
			{ 39, U64_StructType.LevelsNameList },
			{ 40, U64_StructType.Level },
			{ 41, U64_StructType.SubLevel },
			{ 42, U64_StructType.SubLevelList },
			{ 43, U64_StructType.LevelEntry },
			{ 44, U64_StructType.LevelEntryList },
			{ 45, U64_StructType.SuperObjectChildList },
			{ 46, U64_StructType.Vertex },
			{ 49, U64_StructType.LST_SectorGraphic_and_PersoArray },
			{ 50, U64_StructType.LST_SectorActivity },
			{ 51, U64_StructType.LST_SectorCollision },
			{ 52, U64_StructType.LST_SectorStaticLights },
			{ 53, U64_StructType.LST_SectorSound },
			{ 54, U64_StructType.LST_SectorSoundParam },
			{ 55, U64_StructType.LST_SectorSoundEvent },
			{ 56, U64_StructType.LST_SectorSoundEventParam },
			{ 59, U64_StructType.BitmapInfo }, // size: 0x100D2. contains the actual texture data too!
            { 60, U64_StructType.DsgVar__Declaration },
			{ 61, U64_StructType.StateList },
			{ 62, U64_StructType.CharacterBrain },
			{ 63, U64_StructType.CharacterCineInfo },
			{ 64, U64_StructType.CharacterCollSet },
			{ 66, U64_StructType.CharacterMicro },
			{ 67, U64_StructType.CharacterStandardGame },
			{ 71, U64_StructType.ObjectsTableList },
			{ 72, U64_StructType.DsgVarInfo__DeclarationVariable },
			{ 73, U64_StructType.Intelligence },
			{ 74, U64_StructType.ListOfComport },
			{ 75, U64_StructType.ListOfRules },
			{ 76, U64_StructType.Rule },
			{ 77, U64_StructType.Reflex },
			{ 78, U64_StructType.ScriptNodeArray__Node },
			{ 79, U64_StructType.LST_GeometricObject },
			{ 80, U64_StructType.LST_GeometricElementAlignedBoxes },
			{ 81, U64_StructType.LST_GeometricElementSpheres },
			{ 84, U64_StructType.LST_ZoneSet },
			{ 85, U64_StructType.LST_ZdxIndex },
			{ 87, U64_StructType.LST_NodeIndex_and_NoCtrlTextureList },
			{ 88, U64_StructType.LST_Capacity },
			{ 89, U64_StructType.LST_Valuation },
			{ 93, U64_StructType.LST_SectorGraphicParam },
			{ 95, U64_StructType.Node_Long },
			{ 96, U64_StructType.Node_Float },
			{ 97, U64_StructType.Node_Vector3D },
			{ 98, U64_StructType.Node_String },
			{ 99, U64_StructType.Declaration_Long },
			{ 100, U64_StructType.Declaration_UnsignedLong },
			{ 101, U64_StructType.Declaration_Float },
			{ 102, U64_StructType.Declaration_Vector3D },
			{ 103, U64_StructType.StateRef },
			{ 104, U64_StructType.StateRefList },
			{ 107, U64_StructType.DsgMem__Initialization },
			{ 108, U64_StructType.InitVariable },
			{ 109, U64_StructType.InitVariableIdList },
			{ 114, U64_StructType.InputLink },
			{ 115, U64_StructType.InputLinkList },
			{ 116, U64_StructType.InputList },
			{ 123, U64_StructType.AllVector3D },
			{ 124, U64_StructType.AllTripledIndex },
			{ 128, U64_StructType.NbOfLanguages },
			{ 129, U64_StructType.TextString },
			{ 130, U64_StructType.LanguageString },
			{ 131, U64_StructType.TextStringList },
			{ 132, U64_StructType.StringLength },
			{ 133, U64_StructType.StringLengthList },
			{ 141, U64_StructType.FixPreloadSection },
			{ 144, U64_StructType.GeometricElementU64List },
			{ 145, U64_StructType.GeometricElementCollideIndexedTriangles },
			{ 146, U64_StructType.GeometricElementCollideList },
			{ 152, U64_StructType.TextureList },
			{ 153, U64_StructType.StringChunk },
			{ 154, U64_StructType.ArrayEntry },
		};

		public static readonly Dictionary<System.Type, U64_StructType> TypeMapping = new Dictionary<System.Type, U64_StructType>() {
			[typeof(U64_Fix)] = U64_StructType.FixData,
			// TODO
		};

		public static U64_StructType? GetType(Context c, ushort index) {
			Dictionary<ushort, U64_StructType> dict = c.GetCPASettings().Platform switch {
				Platform.N64 => TypesN64,
				Platform.DS => TypesDS,
				Platform._3DS => Types3DS,
				_ => throw new System.Exception($"Platform {c.GetCPASettings().Platform} is not a valid ROM platform")
			};
			if (dict.ContainsKey(index)) {
				return dict[index];
			} else {
				return null;
			}
		}
	}
}
