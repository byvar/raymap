﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public enum U64_StructType {
		Unknown,
		FixData,
		LevelsNameList, // Size: 0x40 * num_levels (0x46 or 70 in Rayman 2)
		Level, // Size: 0x38
		NbOfLanguages,
		LanguageString,
		TextStringList,
		TextString,
		StringChunk,
		StringLength,
		StringLengthList,
		LanguageStringList,
		OldVertexIndex,
		AnimSpeedList,
		DscMiscInfo,
		DscLvl,
		Mem,
		LevelDescription,
		MechanicalEnvironment,
		VisualEnvironment,
		BitmapInfo,
		Texture,
		BitmapCI4,
		BitmapCI8,
		BitmapRGBA16,
		PaletteRGBA16,
		TextureList,
		Way,
		LST_NodeIndex_and_NoCtrlTextureList,
		VisualMaterial,
		U64VertexList,
		U64GraphicsList,
		CompressedDSGraphicsList,
		GraphicsList3DS,
		GeometricElementU64IndexedTriangles,
		GeometricElementSprites,
		GeometricObject,
		Vertex,
		Edge,
		AnimListTable,
		GeometricElementCollideList,
		GeometricElementU64List,
		PhysicalObject,
		PhysicalCollSet__Zoo,
		AllVector3D,
		AllTripledIndex,
		ArrayOfStrings,
		StringList,
		SoundEventList,
		ObjectsTableList,
		ObjectsTable,
		Family,
		GeometricElementCollideTrianglesData__ColFacesPnt,
		GeometricElementCollideIndexedTriangles,
		GeometricElementSpheres,
		LST_GeometricElementSpheres,
		GeometricElementAlignedBoxes,
		LST_GeometricElementAlignedBoxes,
		LST_GeometricElementPoints,
		LST_GeometricElementCones,
		GeometricElementPoints,
		GeometricElementCones,
		Altimap,
		AltimapSquare,
		AltimapFace,
		AltimapUV,
		AltimapVertex,
		CharacterDyn,
		GameMaterial,
		CollideMaterial,
		IdCardBase,
		IdCardCamera,
		MechanicalMaterial,
		SuperObject,
		Sector,
		SuperObjectChildList,
		SubLevel,
		SubLevelList,
		LevelEntry,
		LevelEntryList,
		Character,
		Character3dData,
		State,
		AnimInfo,
		CharacterStandardGame,
		CharacterStream,
		CharacterWorld,
		CharacterSound,
		StateRef,
		StateRefList,
		BoundingVolume,
		LipsSynchro,
		StateList,
		LST_SectorGraphic_and_LST_Character, // type is for LST_SectorGraphic, but Ubisoft reused it for LST_Character because they're both simply a ushort index
		LST_SectorGraphicParam,
		LST_SoundEvent,
		LST_SectorActivity,
		LST_SectorCollision,
		LST_SectorSound,
		LST_SectorSoundParam,
		LST_SectorSoundEvent,
		LST_SectorSoundEventParam,
		LST_SectorStaticLights,
		Light,
		CharacterBrain,
		AIModel,
		DsgVar__Declaration,
		Comport,
		ListOfComport,
		Rule,
		Reflex,
		ListOfRules,
		ScriptNodeArray__Node,
		Intelligence,
		Node_String,
		WayGraph,
		WayGraphNode,
		Waypoint,
		LST_Valuation,
		LST_WaypointIndex,
		WayList,
		LST_WayListIndex,
		LST_Capacity,
		CharacterCollSet,
		ArrayOfZdx,
		ArrayOfZoneSet,
		LST_ZoneSet,
		ActivationZone,
		LST_ZdxIndex,
		LST_GeometricObject,
		Node_Vector3D,
		Node_Long,
		Node_Float,
		InputAction,
		InputList,
		InputLinkList,
		ItemStringList,
		WayLink,
		WayLinkVector,
		LST_WayLinkIndex,
		AnimVector3D,
		AnimTripledIndex,
		DscInput,
		DscInputActionList,
		InputLink,
		DsgVarInfo__DeclarationVariable,
		ArrayEntry,
		Declaration_Long,
		Declaration_UnsignedLong,
		Declaration_Float,
		Declaration_Vector3D,
		DsgMem__Initialization,
		InitVariable,
		InitVariableIdList,
		TypeVariable,
		TypeVariableIdList,
		FixPreloadSection,
		FixMem,
		LevelMem,
		CharacterCineInfo,
		CharacterLight,
		CharacterMicro,
	}
}