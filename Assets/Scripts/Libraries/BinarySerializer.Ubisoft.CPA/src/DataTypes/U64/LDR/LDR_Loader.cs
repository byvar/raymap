
using System.Collections.Generic;
using System.Linq;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class LDR_Loader {
		public Context Context { get; set; }
		public bool IsLoadingFix => !LevelIndex.HasValue;

		public int? LevelIndex { get; set; }

		public Pointer DataPointer { get; set; }
		public LDR_FatFile Fat { get; set; }

		//public Dictionary<CPA_ROM_StructType, Dictionary<ushort, CPA_ROM_Struct>> Cache = new Dictionary<CPA_ROM_StructType, Dictionary<ushort, CPA_ROM_Struct>>();


		// TODO: Move to globals?
		public A3D_AnimationsFile AnimationsFile { get; set; }
		public U64_Reference<GAM_Fix> Fix { get; set; }
		public U64_Reference<GAM_FixPreloadSection> FixPreloadSection { get; set; }


		public LDR_Loader(Context c, Pointer dataPointer) {
			Context = c;
			c.StoreObject<LDR_Loader>(ContextKey, this);
			DataPointer = dataPointer;
		}


		public Pointer GetStructPointer(LDR_EntryRef fat) => DataPointer + fat.Address;

		public Pointer GetStructPointer(ushort type, ushort index, bool global = false) {
			var entry = GetEntry(type, index, global: global);
			if (entry != null) {
				return GetStructPointer(entry);
			} else {
				return null;
			}
		}

		public Pointer GetStructPointer(U64_StructType type, ushort index, bool global = false) {
			var entry = GetEntry(type, index, global: global);
			if (entry != null) {
				return GetStructPointer(entry);
			} else {
				return null;
			}
		}

		public Pointer GetStructPointer(System.Type type, ushort index, bool global = false) {
			var mapping = U64_StructType_Defines.TypeMapping;
			if (mapping.ContainsKey(type)) {
				return GetStructPointer(mapping[type], index, global: global);
			} else {
				throw new System.Exception($"Type {type} does not have a corresponding ROM StructType");
			}
		}

		public LDR_EntryRef GetEntry(ushort type, ushort index, bool global = false) {
			type = (ushort)BitHelpers.ExtractBits(type, 15, 0);
			index = (ushort)BitHelpers.ExtractBits(index, 15, 0);

			if (LevelIndex.HasValue) {
				var levelEntry = Fat.Levels[LevelIndex.Value].Fat.Value.GetEntry(type, index);
				if (levelEntry != null) return levelEntry;
			}

			var fix2Entry = Fat.FixLevels.Fat.Value.GetEntry(type, index);
			if (fix2Entry != null) return fix2Entry;

			var fixEntry = Fat.FixFix.Fat.Value.GetEntry(type, index);
			if (fixEntry != null) return fixEntry;

			if (global) {
				for (int i = 0; i < Fat.Levels.Length; i++) {
					if (LevelIndex.HasValue && i == LevelIndex.Value) continue;
					var entry = Fat.Levels[i].Fat.Value.GetEntry(type, index);
					if (entry != null) return entry;
				}
			}

			return null;
		}

		public LDR_EntryRef GetEntry(U64_StructType type, ushort index, bool global = false) {
			bool isFix = BitHelpers.ExtractBits(index, 1, 15) == 1;
			ushort ind = (ushort)BitHelpers.ExtractBits(index, 15, 0);
			if (!global) {
				if (!isFix) {
					if (LevelIndex.HasValue) {
						var levelEntry = Fat.Levels[LevelIndex.Value].Fat.Value.GetEntry(type, ind);
						if (levelEntry != null) return levelEntry;
					}

					var fix2Entry = Fat.FixLevels.Fat.Value.GetEntry(type, ind);
					if (fix2Entry != null) return fix2Entry;
				} else {
					var fixEntry = Fat.FixFix.Fat.Value.GetEntry(type, ind);
					if (fixEntry != null) return fixEntry;
				}
			} else {
				if (LevelIndex.HasValue) {
					var levelEntry = Fat.Levels[LevelIndex.Value].Fat.Value.GetEntry(type, ind);
					if (levelEntry != null) return levelEntry;
				}

				var fix2Entry = Fat.FixLevels.Fat.Value.GetEntry(type, ind);
				if (fix2Entry != null) return fix2Entry;

				var fixEntry = Fat.FixFix.Fat.Value.GetEntry(type, ind);
				if (fixEntry != null) return fixEntry;

				for (int i = 0; i < Fat.Levels.Length; i++) {
					if (LevelIndex.HasValue && i == LevelIndex.Value) continue;
					var entry = Fat.Levels[i].Fat.Value.GetEntry(type, ind);
					if (entry != null) return entry;
				}
			}

			return null;
		}

		public static string ContextKey => nameof(LDR_Loader);
	}
}
