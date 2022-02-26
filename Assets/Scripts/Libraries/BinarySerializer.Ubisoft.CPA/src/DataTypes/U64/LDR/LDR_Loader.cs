
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class LDR_Loader {
		public Context Context { get; set; }
		public bool IsLoadingFix => !LevelIndex.HasValue;
		public LinkedList<StructReference> LoadQueue = new LinkedList<StructReference>();
		public bool IsProcessingLoadQueue { get; set; }

		public int? LevelIndex { get; set; }

		private Pointer DataPointer { get; set; }
		public LDR_FatFile Fat { get; set; }

		//public Dictionary<CPA_ROM_StructType, Dictionary<ushort, CPA_ROM_Struct>> Cache = new Dictionary<CPA_ROM_StructType, Dictionary<ushort, CPA_ROM_Struct>>();


		// TODO: Move to globals?
		public U64_Reference<GAM_Fix> Fix { get; set; }
		public U64_Reference<GAM_FixPreloadSection> FixPreloadSection { get; set; }


		public LDR_Loader(Context c, Pointer dataPointer) {
			Context = c;
			c.StoreObject<LDR_Loader>(ContextKey, this);
			DataPointer = dataPointer;
		}


		public void LoadLoop(SerializerObject s) {
			if(IsProcessingLoadQueue) return;
			IsProcessingLoadQueue = true;
			while (LoadQueue.First?.Value != null) {
				StructReference currentRef = LoadQueue.First?.Value;
				LoadQueue.RemoveFirst();

				var off_struct = GetStructPointer(currentRef.Type, currentRef.Index, global: currentRef.IsGlobal);
				Pointer off_current = s.CurrentPointer;
				s.Goto(off_struct);

				//s.Log("LDR: Resolving struct: {0}", currentRef.Name);
				if (currentRef.ArrayCount.HasValue) {
					currentRef.ArrayLoadCallback(s, (f,arrayIndex) => {
						f.CPA_Index = currentRef.Index;
						f.CPA_ArrayIndex = arrayIndex;
					});
				} else {
					currentRef.LoadCallback(s, (f) => {
						f.CPA_Index = currentRef.Index;
					});
				}

				s.Goto(off_current);
			}
			IsProcessingLoadQueue = false;
		}
		public class StructReference {
			public string Name { get; set; }
			public ushort Index { get; set; }
			public U64_StructType Type { get; set; }
			public bool IsGlobal { get; set; }

			public ResolveAction LoadCallback { get; set; }
			public ArrayResolveAction ArrayLoadCallback { get; set; }

			public uint? ArrayCount { get; set; }
		}
		public delegate void ResolveAction(SerializerObject s, Action<U64_Struct> configureAction);
		public delegate void ArrayResolveAction(SerializerObject s, Action<U64_Struct, int> configureAction);
		public delegate void ResolvedAction(U64_Struct f);

		public void RequestFile(SerializerObject s, U64_StructType type, ushort index,
			ResolveAction loadCallback, ArrayResolveAction arrayLoadCallback,
			bool isGlobal = false,
			uint? arrayCount = null,
			string name = "") {
			var fileRef = new StructReference() {
				Name = name,
				Index = index,
				Type = type,
				ArrayCount = arrayCount,
				LoadCallback = loadCallback,
				ArrayLoadCallback = arrayLoadCallback,
				IsGlobal = isGlobal,
			};
			LoadQueue.AddLast(fileRef);
			if(!IsProcessingLoadQueue) LoadLoop(s);
		}

		#region Pointer calculation
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
			var mappedType = U64_StructType_Defines.GetType(type);
			return GetStructPointer(mappedType.Value, index, global: global);
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
		#endregion

		#region Animations
		public A3D_AnimationsFile AnimationsFile { get; set; }
		private HashSet<ushort> AnimationsToLoad { get; set; } = new HashSet<ushort>();

		public void LoadInterpolatedAnimation(ushort animIndex) {
			// Index in cutTable
			AnimationsToLoad.Add(animIndex);
		}
		#endregion

		public static string ContextKey => nameof(LDR_Loader);
	}
}
