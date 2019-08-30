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
			{ 0, Type.EngineStructure },
			{ 5, Type.ObjectListEntry }, // ?
			{ 17, Type.TextureInfoRef }, // size: 0x2
			{ 22, Type.Vertex },
			{ 23, Type.Triangle },
			{ 34, Type.VisualMaterial },
			{ 43, Type.LevelList },
			{ 44, Type.LevelHeader },
			{ 63, Type.TextureInfo }, // size: 14
			{ 91, Type.NoCtrlTextureList },
			{ 132, Type.NumLanguages },
			{ 133, Type.StringRef },
			{ 134, Type.LanguageTable },
			{ 135, Type.TextTable },
			{ 136, Type.Language_136 },
			{ 137, Type.Language_137 },
			{ 156, Type.VisualMaterialTextures },
			{ 157, Type.String },
		};

		public static Dictionary<ushort, Type> TypesN64 = new Dictionary<ushort, Type>() {
			{ 0, Type.EngineStructure },
			{ 5, Type.ObjectListEntry }, // ?
			{ 17, Type.TextureInfoRef }, // size: 0x2
			{ 21, Type.Palette },
			{ 22, Type.Vertex },
			{ 23, Type.Triangle },
			{ 34, Type.VisualMaterial },
			{ 43, Type.LevelList },
			{ 44, Type.LevelHeader },
			{ 63, Type.TextureInfo }, // size: 12
			{ 91, Type.NoCtrlTextureList },
			{ 132, Type.NumLanguages },
			{ 133, Type.StringRef },
			{ 134, Type.LanguageTable },
			{ 135, Type.TextTable },
			{ 136, Type.Language_136 },
			{ 137, Type.Language_137 },
			{ 156, Type.VisualMaterialTextures },
			{ 157, Type.String },
		};

		public static Dictionary<ushort, Type> Types3DS = new Dictionary<ushort, Type>() {
			{ 0, Type.EngineStructure },
			{ 16, Type.Vertex },
			{ 17, Type.TextureInfoRef }, // size: 0x2
			{ 22, Type.Vertex },
			{ 23, Type.Triangle },
			{ 30, Type.VisualMaterial },
			{ 39, Type.LevelList },
			{ 40, Type.LevelHeader },
			{ 59, Type.TextureInfo }, // size: 0x100D2. contains the actual texture data too!
			{ 87, Type.NoCtrlTextureList },
			{ 128, Type.NumLanguages },
			{ 129, Type.StringRef },
			{ 130, Type.LanguageTable },
			{ 131, Type.TextTable },
			{ 132, Type.Language_136 },
			{ 133, Type.Language_137 },
			{ 152, Type.VisualMaterialTextures },
			{ 153, Type.String },
		};

		public enum Type {
			Unknown,
			EngineStructure,
			ObjectListEntry, // Size: 0x14
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
			Vertex,
			Triangle
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
		};

		public Type EntryType {
			get {
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
}
