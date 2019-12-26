using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.ROM {
	public class FATEntry {
		public Pointer offset;
		public uint off_data;
		public ushort type;
		public ushort index;

		// Calculated
		public uint tableIndex;
		public uint entryIndexWithinTable;
		public uint size;

		public static FATEntry Read(Reader reader, Pointer offset) {
			FATEntry entry = new FATEntry();
			entry.offset = offset;
			entry.off_data = reader.ReadUInt32();
			entry.type = reader.ReadUInt16();
			entry.index = reader.ReadUInt16();
			return entry;
		}

		public static Dictionary<ushort, Type> TypesDS = new Dictionary<ushort, Type>() {
			{ 0, Type.EngineStruct },
			{ 1, Type.Perso },
			{ 4, Type.Family },
			{ 5, Type.State },
			{ 6, Type.ObjectsTable },
			{ 8, Type.AnimationReference },
			{ 9, Type.Perso3dData },
			{ 17, Type.TextureInfoRef }, // size: 0x2
			{ 22, Type.VertexArray },
			{ 23, Type.GeometricElementTrianglesData },
			{ 24, Type.GeometricElementTriangles },
			{ 25, Type.GeometricElementSprites },
			{ 31, Type.GeometricObject },
			{ 32, Type.GameMaterial },
			{ 33, Type.CollideMaterial },
			{ 34, Type.VisualMaterial },
			{ 37, Type.MechanicsMaterial },
			{ 38, Type.CollSet },
			{ 39, Type.PhysicalObject },
			{ 40, Type.Sector },
			{ 41, Type.SuperObject },
			{ 42, Type.GeometricElementTrianglesCollideData },
			{ 43, Type.LevelList },
			{ 44, Type.LevelHeader },
			{ 45, Type.HierarchyRoot },
			{ 46, Type.ShortArray },
			{ 47, Type.SuperObjectDynamic },
			{ 48, Type.SuperObjectDynamicArray },
			{ 49, Type.SuperObjectArray },
			{ 50, Type.CompressedVector3Array },
			{ 63, Type.TextureInfo }, // size: 14
			{ 65, Type.StateTransitionArray },
			{ 71, Type.StdGame },
			{ 75, Type.ObjectsTableData },
			{ 91, Type.NoCtrlTextureList },
			{ 107, Type.StateArrayRef },
			{ 108, Type.StateArray },
			{ 127, Type.Vector3Array },
			{ 128, Type.Short3Array },
			{ 132, Type.NumLanguages },
			{ 133, Type.StringRef },
			{ 134, Type.LanguageTable },
			{ 135, Type.TextTable },
			{ 136, Type.Language_136 },
			{ 137, Type.Language_137 },
			{ 148, Type.GeometricElementListVisual },
			{ 149, Type.GeometricElementTrianglesCollide },
			{ 150, Type.GeometricElementListCollide },
			{ 156, Type.VisualMaterialTextures },
			{ 157, Type.String },
		};

		public static Dictionary<ushort, Type> TypesN64 = new Dictionary<ushort, Type>() {
			{ 0, Type.EngineStruct },
			{ 1, Type.Perso },
			{ 4, Type.Family },
			{ 5, Type.State },
			{ 6, Type.ObjectsTable },
			{ 8, Type.AnimationReference },
			{ 9, Type.Perso3dData },
			{ 17, Type.TextureInfoRef }, // size: 0x2
			{ 21, Type.Palette },
			{ 22, Type.VertexArray },
			{ 23, Type.GeometricElementTrianglesData },
			{ 24, Type.GeometricElementTriangles },
			{ 25, Type.GeometricElementSprites },
			{ 31, Type.GeometricObject },
			{ 32, Type.GameMaterial },
			{ 33, Type.CollideMaterial },
			{ 34, Type.VisualMaterial },
			{ 37, Type.MechanicsMaterial },
			{ 38, Type.CollSet },
			{ 39, Type.PhysicalObject },
			{ 40, Type.Sector },
			{ 41, Type.SuperObject },
			{ 42, Type.GeometricElementTrianglesCollideData },
			{ 43, Type.LevelList },
			{ 44, Type.LevelHeader },
			{ 45, Type.HierarchyRoot },
			{ 46, Type.ShortArray },
			{ 47, Type.SuperObjectDynamic },
			{ 48, Type.SuperObjectDynamicArray },
			{ 49, Type.SuperObjectArray },
			{ 50, Type.CompressedVector3Array },
			{ 63, Type.TextureInfo }, // size: 12
			{ 65, Type.StateTransitionArray },
			{ 71, Type.StdGame },
			{ 75, Type.ObjectsTableData },
			{ 91, Type.NoCtrlTextureList },
			{ 107, Type.StateArrayRef },
			{ 108, Type.StateArray },
			{ 127, Type.Vector3Array },
			{ 128, Type.Short3Array },
			{ 132, Type.NumLanguages },
			{ 133, Type.StringRef },
			{ 134, Type.LanguageTable },
			{ 135, Type.TextTable },
			{ 136, Type.Language_136 },
			{ 137, Type.Language_137 },
			{ 148, Type.GeometricElementListVisual },
			{ 149, Type.GeometricElementTrianglesCollide },
			{ 150, Type.GeometricElementListCollide },
			{ 156, Type.VisualMaterialTextures },
			{ 157, Type.String },
		};

		public static Dictionary<ushort, Type> Types3DS = new Dictionary<ushort, Type>() {
			{ 0, Type.EngineStruct },
			{ 1, Type.Perso },
			{ 4, Type.Family },
			{ 5, Type.State },
			{ 6, Type.ObjectsTable },
			{ 8, Type.AnimationReference },
			{ 9, Type.Perso3dData },
			{ 17, Type.TextureInfoRef }, // size: 0x2
			{ 18, Type.VertexArray },
			{ 19, Type.GeometricElementTrianglesData },
			{ 20, Type.GeometricElementTriangles },
			{ 21, Type.GeometricElementSprites },
			{ 27, Type.GeometricObject },
			{ 28, Type.GameMaterial },
			{ 29, Type.CollideMaterial },
			{ 30, Type.VisualMaterial },
			{ 33, Type.MechanicsMaterial },
			{ 34, Type.CollSet },
			{ 35, Type.PhysicalObject },
			{ 36, Type.Sector },
			{ 37, Type.SuperObject },
			{ 38, Type.GeometricElementTrianglesCollideData },
			{ 39, Type.LevelList },
			{ 40, Type.LevelHeader },
			{ 41, Type.HierarchyRoot },
			{ 42, Type.ShortArray },
			{ 43, Type.SuperObjectDynamic },
			{ 44, Type.SuperObjectDynamicArray },
			{ 45, Type.SuperObjectArray },
			{ 46, Type.CompressedVector3Array },
			{ 59, Type.TextureInfo }, // size: 0x100D2. contains the actual texture data too!
			{ 61, Type.StateTransitionArray },
			{ 67, Type.StdGame },
			{ 71, Type.ObjectsTableData },
			{ 87, Type.NoCtrlTextureList },
			{ 103, Type.StateArrayRef },
			{ 104, Type.StateArray },
			{ 123, Type.Vector3Array },
			{ 124, Type.Short3Array },
			{ 128, Type.NumLanguages },
			{ 129, Type.StringRef },
			{ 130, Type.LanguageTable },
			{ 131, Type.TextTable },
			{ 132, Type.Language_136 },
			{ 133, Type.Language_137 },
			{ 144, Type.GeometricElementListVisual },
			{ 145, Type.GeometricElementTrianglesCollide },
			{ 146, Type.GeometricElementListCollide },
			{ 152, Type.VisualMaterialTextures },
			{ 153, Type.String },
		};

		public enum Flag {
			Fix = 0x8000
		}

		public enum Type {
			Unknown,
			EngineStruct,
			LevelList, // Size: 0x40 * num_levels (0x46 or 70 in Rayman 2)
			LevelHeader, // Size: 0x38
			NumLanguages,
			LanguageTable,
			TextTable,
			StringRef,
			String,
			Language_136,
			Language_137,
			TextureInfo,
			TextureInfoRef,
			VisualMaterialTextures,
			NoCtrlTextureList,
			VisualMaterial,
			Palette,
			VertexArray,
			GeometricElementTrianglesData,
			GeometricElementTriangles,
			GeometricElementSprites,
			GeometricObject,
			CompressedVector3Array,
			GeometricElementListCollide,
			GeometricElementListVisual,
			PhysicalObject,
			CollSet,
			Vector3Array,
			Short3Array,
			ObjectsTableData,
			ObjectsTable,
			Family,
			GeometricElementTrianglesCollideData,
			GeometricElementTrianglesCollide,
			GameMaterial,
			CollideMaterial,
			MechanicsMaterial,
			SuperObject,
			Sector,
			SuperObjectArray,
			HierarchyRoot,
			ShortArray,
			SuperObjectDynamic,
			SuperObjectDynamicArray,
			Perso,
			Perso3dData,
			State,
			AnimationReference,
			StdGame,
			StateArrayRef,
			StateArray,
			StateTransitionArray,
		}

		public static Dictionary<System.Type, Type> types = new Dictionary<System.Type, Type>() {
			{ typeof(TextureInfo), Type.TextureInfo },
			{ typeof(TextureInfoRef), Type.TextureInfoRef },
			{ typeof(VisualMaterial), Type.VisualMaterial },
			{ typeof(VisualMaterialTextures), Type.VisualMaterialTextures },
			{ typeof(StringRef), Type.StringRef },
			{ typeof(String), Type.String },
			{ typeof(TextTable), Type.TextTable },
			{ typeof(LanguageTable), Type.LanguageTable },
			{ typeof(NumLanguages), Type.NumLanguages },
			{ typeof(NoCtrlTextureList), Type.NoCtrlTextureList },
			{ typeof(EngineStruct), Type.EngineStruct },
			{ typeof(GeometricElementSprites), Type.GeometricElementSprites },
			{ typeof(GeometricElementTriangles), Type.GeometricElementTriangles },
			{ typeof(GeometricElementTrianglesData), Type.GeometricElementTrianglesData },
			{ typeof(CompressedVector3Array), Type.CompressedVector3Array },
			{ typeof(GeometricElementListCollide), Type.GeometricElementListCollide },
			{ typeof(GeometricElementListVisual), Type.GeometricElementListVisual },
			{ typeof(GeometricObject), Type.GeometricObject },
			{ typeof(Vector3Array), Type.Vector3Array },
			{ typeof(Short3Array), Type.Short3Array },
			{ typeof(ObjectsTable), Type.ObjectsTable },
			{ typeof(ObjectsTableData), Type.ObjectsTableData },
			{ typeof(CollSet), Type.CollSet },
			{ typeof(PhysicalObject), Type.PhysicalObject },
			{ typeof(VertexArray), Type.VertexArray },
			{ typeof(GeometricElementTrianglesCollide), Type.GeometricElementTrianglesCollide },
			{ typeof(GeometricElementTrianglesCollideData), Type.GeometricElementTrianglesCollideData },
			{ typeof(GameMaterial), Type.GameMaterial },
			{ typeof(CollideMaterial), Type.CollideMaterial },
			{ typeof(MechanicsMaterial), Type.MechanicsMaterial },
			{ typeof(LevelList), Type.LevelList },
			{ typeof(SuperObject), Type.SuperObject },
			{ typeof(Sector), Type.Sector },
			{ typeof(SuperObjectArray), Type.SuperObjectArray },
			{ typeof(LevelHeader), Type.LevelHeader },
			{ typeof(HierarchyRoot), Type.HierarchyRoot },
			{ typeof(ShortArray), Type.ShortArray },
			{ typeof(SuperObjectDynamic), Type.SuperObjectDynamic },
			{ typeof(SuperObjectDynamicArray), Type.SuperObjectDynamicArray },
			{ typeof(Perso), Type.Perso },
			{ typeof(Perso3dData), Type.Perso3dData },
			{ typeof(Family), Type.Family },
			{ typeof(State), Type.State },
			{ typeof(AnimationReference), Type.AnimationReference },
			{ typeof(StdGame), Type.StdGame },
			{ typeof(StateArrayRef), Type.StateArrayRef },
			{ typeof(StateArray), Type.StateArray },
			{ typeof(StateTransitionArray), Type.StateTransitionArray },
		};

		public Type EntryType {
			get {
				return GetEntryType(type);
			}
		}

		public static Type GetEntryType(ushort type) {
			Dictionary<ushort, Type> dict = null;
			switch (Settings.s.platform) {
				case Settings.Platform._3DS: dict = Types3DS; break;
				case Settings.Platform.N64: dict = TypesN64; break;
				default: dict = TypesDS; break;
			}
			if (dict.ContainsKey(type)) {
				return dict[type];
			} else return Type.Unknown;
		}
	}
}
