using OpenSpace.AI;
using OpenSpace.Animation;
using OpenSpace.Collide;
using OpenSpace.Object;
using OpenSpace.FileFormat;
using OpenSpace.FileFormat.Texture;
using OpenSpace.Input;
using OpenSpace.Text;
using OpenSpace.Visual;
using OpenSpace.Waypoints;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using OpenSpace.Object.Properties;
using System.Collections;
using System.Text.RegularExpressions;
using lzo.net;
using System.IO.Compression;
using System.Threading.Tasks;
using OpenSpace.PS1;
using OpenSpace.PS1.GLI;
using System.Text;
using Cysharp.Threading.Tasks;

namespace OpenSpace.Loader {
    public class R2PS1Loader : MapLoader {
		public int CurrentLevel { get; private set; } = -1;
		public PS1VRAM vram = new PS1VRAM();
		public LevelHeader levelHeader;
		public PS1GameInfo game;
		public ushort[] maxScaleVector = new ushort[3] { 0, 0, 0 };
		public List<Tuple<PS1.Perso3dData, uint>>[] familyStates = new List<Tuple<PS1.Perso3dData, uint>>[3] {
			new List<Tuple<PS1.Perso3dData, uint>>(),
			new List<Tuple<PS1.Perso3dData, uint>>(),
			new List<Tuple<PS1.Perso3dData, uint>>()
		};
		public PS1Stream[] streams;
		PS1GameInfo.File.MemoryBlock mainMemoryBlock;

		// Actor
		public string actor1Name;
		public string actor2Name;
		public int Actor1Index { get; private set; } = 0;
		public int Actor2Index { get; private set; } = 1;
		private FileWithPointers actor1File;
		private FileWithPointers actor2File;
		public ActorFileHeader actor1Header;
		public ActorFileHeader actor2Header;

		// GameObjects
		public GameObject gao_fatherSector;
		public GameObject gao_dynamicWorld;
		public GameObject gao_inactiveDynamicWorld;
		public GameObject gao_always;

		public string[] LoadLevelList() {
			if (PS1GameInfo.Games.ContainsKey(Settings.s.mode)) {
				return PS1GameInfo.Games[Settings.s.mode].maps;
			}
			return new string[] { "<no map>" };
		}

		protected override async UniTask Load() {
            try {
                if (gameDataBinFolder == null || gameDataBinFolder.Trim().Equals("")) throw new Exception("GAMEDATABIN folder doesn't exist");
                if (lvlName == null || lvlName.Trim() == "") throw new Exception("No level name specified!");
                globals = new Globals();
				gameDataBinFolder += "/";
				await FileSystem.CheckDirectory(gameDataBinFolder);
				if (!FileSystem.DirectoryExists(gameDataBinFolder)) throw new Exception("GAMEDATABIN folder doesn't exist");
				if (!PS1GameInfo.Games.ContainsKey(Settings.s.mode)) throw new Exception("PS1 info wasn't defined for this mode");
				loadingState = "Initializing files";
				//RipRHRLoc();
				game = PS1GameInfo.Games[Settings.s.mode];
				CurrentLevel = Array.IndexOf(game.maps, lvlName);
				Actor1Index = Array.FindIndex(game.actors, a => a.Actor1 == actor1Name);
				Actor2Index = Array.FindIndex(game.actors, a => a.Actor2 == actor2Name);
				SetForcedActor();
				files_array = new FileWithPointers[0];
				await LoadAllDataFromDAT(game);

				if (UnitySettings.ExportPS1Files)
				{
					await ExtractAllFiles(game);
				}

				await InitAllFiles(game);
				await LoadLevel();
			} finally {
                for (int i = 0; i < files_array.Length; i++) {
                    if (files_array[i] != null) {
                        files_array[i].Dispose();
                    }
                }
                if (cnt != null) cnt.Dispose();
            }
            await WaitIfNecessary();
            InitModdables();
        }

		/*public void RipRHRLoc() {
			string offsetsFile = gameDataBinFolder + "en_string_offsets.bin";
			string locFile = gameDataBinFolder + "rhr_loc_from_memory.bin";
			List<ushort> offsets = new List<ushort>();
			string[] strings;
			using (Reader reader = new Reader(FileSystem.GetFileReadStream(offsetsFile))) {
				while (reader.BaseStream.Position < reader.BaseStream.Length) {
					offsets.Add(reader.ReadUInt16());
				}
			}
			strings = new string[offsets.Count];
			using (Reader reader = new Reader(FileSystem.GetFileReadStream(locFile))) {
				for (int i = 0; i < strings.Length; i++) {
					reader.BaseStream.Position = offsets[i];
					strings[i] = reader.ReadNullDelimitedString(encoding: Encoding.GetEncoding(1252));
				}
			}
			var output = strings;
			string json = Newtonsoft.Json.JsonConvert.SerializeObject(output, Newtonsoft.Json.Formatting.Indented);
			Util.ByteArrayToFile(gameDataBinFolder + "rhr.json", Encoding.UTF8.GetBytes(json));
		}*/

		public async UniTask InitAllFiles(PS1GameInfo game) {
			if (CurrentLevel < 0 || CurrentLevel >= game.maps.Length) return;
			PS1GameInfo.File mainFile = game.files.FirstOrDefault(f => f.fileID == 0);
			await InitFiles(game, mainFile, CurrentLevel);
			mainMemoryBlock = mainFile.memoryBlocks[CurrentLevel];
			if (mainFile.memoryBlocks[CurrentLevel].loadActor) {
				if (game.files.Any(f => f.bigfile == "ACTOR1")) {
					// Load Actor
					await InitFiles(game, game.files.FirstOrDefault(f => f.bigfile == "ACTOR1"), Actor1Index);
				}
				if (game.files.Any(f => f.bigfile == "ACTOR2")) {
					// Load Actor
					await InitFiles(game, game.files.FirstOrDefault(f => f.bigfile == "ACTOR2"), Actor2Index);
				}
			}
			loadingState = "Filling VRAM";
			await WaitIfNecessary();
			FillVRAM();
		}

		public async UniTask InitFiles(PS1GameInfo gameInfo, PS1GameInfo.File fileInfo, int index) {
			PS1GameInfo.File.MemoryBlock b = fileInfo.memoryBlocks[index];
			string levelDir = gameDataBinFolder + fileInfo.bigfile + "/";
			if (fileInfo.type == PS1GameInfo.File.Type.Map) {
				levelDir += (index < gameInfo.maps.Length ? gameInfo.maps[index] : (fileInfo.bigfile + "_" + index)) + "/";
			} else {
				levelDir += index + "/";
			}
			int curFile = files_array.Length;


			if (fileInfo.type == PS1GameInfo.File.Type.Map) {
				if (!b.inEngine) return;
				Array.Resize(ref files_array, files_array.Length + 2 + b.cutscenes.Length);
				files_array[curFile++] = new PS1Data(lvlName + ".sys", levelDir + "level.sys", curFile, 0);
				files_array[curFile++] = new PS1Data(lvlName + ".img", levelDir + "level.img", curFile, b.address);

				for (int i = 0; i < b.cutscenes.Length; i++) {
					string cutsceneFramesName = levelDir + "stream_frames_" + i + ".blk";
					files_array[curFile++] = new LinearFile("stream_frames_" + i + ".blk", cutsceneFramesName, curFile);
				}
			} else if(fileInfo.type == PS1GameInfo.File.Type.Actor) {
				Array.Resize(ref files_array, files_array.Length + 1);
				FileWithPointers f = null;
				PS1GameInfo.File mainFile = game.files.FirstOrDefault(fi => fi.fileID == 0);
				if (fileInfo.bigfile == "ACTOR1") {
					f = new PS1Data(fileInfo.bigfile + "_" + index + ".img", levelDir + "actor.img", curFile,
						GetActorAddress(0));
					actor1File = f;
				} else if (fileInfo.bigfile == "ACTOR2") {
					f = new PS1Data(fileInfo.bigfile + "_" + index + ".img", levelDir + "actor.img", curFile,
						GetActorAddress(1));
					actor2File = f;
				}
				files_array[curFile++] = f;
			}
			await UniTask.CompletedTask;
		}
		private uint GetActorAddress(int i) {
			PS1GameInfo.File mainFile = game.files.FirstOrDefault(fi => fi.fileID == 0);
			switch (i) {
				case 0:
					return mainFile.memoryBlocks[CurrentLevel].overrideActor1Address ?? game.actor1Address;
				case 1:
					if (Settings.s.game == Settings.Game.JungleBook) {
						return (mainFile.memoryBlocks[CurrentLevel].overrideActor1Address ?? game.actor1Address)
							+ (uint)actor1File.reader.BaseStream.Length;
					}
					return mainFile.memoryBlocks[CurrentLevel].overrideActor2Address ?? game.actor2Address;
				default: return 0;
			}
		}
		private void SetForcedActor() {
			if (Settings.s.game == Settings.Game.JungleBook) {
				if (CurrentLevel < 9) {
					// Story levels
					Actor1Index = CurrentLevel;
					Actor2Index = CurrentLevel;
					/* Base address of Actor2 = act1 base + size of actor 1. Size table:
					76FB0h, // 0
					7A000h, // 1
					7D690h, // 2
					67314h, // 3
					6AEF0h, // 4
					8AAE0h, // 5
					73F94h, // 6
					6DBCCh, // 7
					6B970h  // 8
					*/
				} else if (CurrentLevel == 0x13) {
					// Menu
					Actor1Index = 22; // "22_Kl"
					Actor2Index = 13; // "13_mow"
									  // Base address for Act2: 80149400

				} else {
					// Base address for Act2: 80149400
					// I guess anything goes between >= 9 && <= 26
				}
			} else if (Settings.s.game == Settings.Game.RRush) {
				if (CurrentLevel == 12) { // mainmenu
					Actor1Index = 8; // menubox1
					Actor2Index = 9; // menubox2
				}
			}
		}

		private void RelocateActorMemoryDonaldDuck() {
			Reader reader = files_array[Mem.Fix]?.reader;
			if (reader == null) throw new Exception("Level \"" + lvlName + "\" does not exist");
			Pointer.DoAt(ref reader, Pointer.Current(reader) + 0xe0, () => {
				Pointer off_geo_dynamic = Pointer.Read(reader);
				uint num_skinnableObjects = reader.ReadUInt32();
				int num_skins = 5;
				Pointer[] off_skins = new Pointer[num_skins];
				//skins = new SkinnableGeometricObjectList[num_skins];
				for (int i = 0; i < num_skins; i++) {
					off_skins[i] = Pointer.Read(reader);
					//skins[i] = Load.FromOffsetOrRead<SkinnableGeometricObjectList>(reader, off_skins[i], onPreRead: s => s.length = num_skinnableObjects);
				}
				Pointer off_defaultSkin = Pointer.Read(reader);
				Pointer.DoAt(ref reader, off_skins[Actor1Index], () => {
					Pointer off_entries = Pointer.Read(reader);
					Pointer off_memory = Pointer.Read(reader);
					uint sz_memory = reader.ReadUInt32();
					byte[] memory = null;
					Pointer.DoAt(ref reader, off_memory, () => {
						memory = reader.ReadBytes((int)sz_memory);
					});
					((off_defaultSkin.file) as PS1Data).OverwriteData(off_defaultSkin.FileOffset, memory);
				});
				reader.ReadUInt32();
				Pointer off_geo_static = Pointer.Read(reader);
				uint num_geo_dynamic = reader.ReadUInt32();
				/*if (num_skinnableObjects > 0) {
					// Write geometric objects into list
					uint start_geo_skin = num_geo_dynamic - num_skinnableObjects - 1;
					Pointer.DoAt(ref reader, off_geo_dynamic + (start_geo_skin * 8), () => {
						for (int i = 0; i < num_skinnableObjects; i++) {
							Pointer.Read(reader);
							Pointer off_cur = Pointer.Current(reader);
							(off_cur.file as PS1Data).OverwriteData(off_cur.FileOffset, (off_defaultSkin + i * 0x20).offset);
							Pointer.Read(reader);
						}
					});
				}*/
			});
		}

		private void RelocateActorFile(int index) {
			PS1GameInfo.File mainFile = game.files.FirstOrDefault(f => f.fileID == 0);
			if (!mainFile.memoryBlocks[CurrentLevel].relocateActor) return;

			Reader reader = files_array[Mem.Fix]?.reader;
			if (reader == null) throw new Exception("Level \"" + lvlName + "\" does not exist");
			FileWithPointers file = null;
			if (index == 0) {
				file = actor1File;
			} else {
				file = actor2File;
			}
			if (file != null) {
				Pointer soPointer = null;
				byte[] perso = null, unk = null, unk2 = null;
				if (Settings.s.game == Settings.Game.RRush) {
					Pointer.DoAt(ref reader, new Pointer((uint)file.headerOffset, file), () => {
						soPointer = Pointer.Read(reader);
						perso = reader.ReadBytes(0x18);
						unk = reader.ReadBytes(4);
						unk2 = reader.ReadBytes(2);
					});
					Pointer.DoAt(ref reader, Pointer.Current(reader) + 0x68, () => {
						Pointer off_persos = Pointer.Read(reader);
						Pointer.DoAt(ref reader, off_persos + (index * 0x18), () => {
							(off_persos.file as PS1Data).OverwriteData(Pointer.Current(reader).FileOffset, perso);
						});
					});
					Pointer.DoAt(ref reader, Pointer.Current(reader) + 0x40, () => {
						Pointer off_dynamicWorld = Pointer.Read(reader);
						Pointer.DoAt(ref reader, off_dynamicWorld, () => {
							reader.ReadBytes(0x8);
							Pointer firstChild = Pointer.Read(reader);
							Pointer lastChild = Pointer.Read(reader);
							uint numChild = reader.ReadUInt32();
							if (numChild == 0) {
								firstChild = soPointer;
								lastChild = soPointer;
							} else {
								(lastChild.file as PS1Data).OverwriteData(lastChild.FileOffset + 0x14, soPointer.offset);
								(soPointer.file as PS1Data).OverwriteData(soPointer.FileOffset + 0x18, lastChild.offset);
								lastChild = soPointer;
							}
							numChild++;
							(off_dynamicWorld.file as PS1Data).OverwriteData(off_dynamicWorld.FileOffset + 0x8, firstChild.offset);
							(off_dynamicWorld.file as PS1Data).OverwriteData(off_dynamicWorld.FileOffset + 0xC, lastChild.offset);
							(off_dynamicWorld.file as PS1Data).OverwriteData(off_dynamicWorld.FileOffset + 0x10, numChild);
						});
					});
				} else if (Settings.s.game == Settings.Game.JungleBook) {
					Pointer.DoAt(ref reader, new Pointer((uint)file.headerOffset, file) + 0x98, () => {
						//soPointer = Pointer.Read(reader);
						perso = reader.ReadBytes(0x18);
						/*unk = reader.ReadBytes(4);
						unk2 = reader.ReadBytes(2);*/
					});
					Pointer.DoAt(ref reader, Pointer.Current(reader) + 0x114, () => {
						Pointer off_persos = Pointer.Read(reader);
						Pointer.DoAt(ref reader, off_persos + (index * 0x18), () => {
							(off_persos.file as PS1Data).OverwriteData(Pointer.Current(reader).FileOffset, perso);
							// Start reading the data we've just overwritten
							reader.ReadUInt32();
							Pointer soPointerPointer = Pointer.Read(reader);
							Pointer.DoAt(ref reader, soPointerPointer, () => {
								soPointer = Pointer.Read(reader);
							});
						});
					});
					Pointer.DoAt(ref reader, Pointer.Current(reader) + 0xEC, () => {
						Pointer off_dynamicWorld = Pointer.Read(reader);
						Pointer.DoAt(ref reader, off_dynamicWorld, () => {
							reader.ReadBytes(0x8);
							Pointer firstChild = Pointer.Read(reader);
							Pointer lastChild = Pointer.Read(reader);
							uint numChild = reader.ReadUInt32();
							if (numChild == 0) {
								firstChild = soPointer;
								lastChild = soPointer;
							} else {
								(lastChild.file as PS1Data).OverwriteData(lastChild.FileOffset + 0x14, soPointer.offset);
								(soPointer.file as PS1Data).OverwriteData(soPointer.FileOffset + 0x18, lastChild.offset);
								lastChild = soPointer;
							}
							numChild++;
							(off_dynamicWorld.file as PS1Data).OverwriteData(off_dynamicWorld.FileOffset + 0x8, firstChild.offset);
							(off_dynamicWorld.file as PS1Data).OverwriteData(off_dynamicWorld.FileOffset + 0xC, lastChild.offset);
							(off_dynamicWorld.file as PS1Data).OverwriteData(off_dynamicWorld.FileOffset + 0x10, numChild);
						});
					});
				}
				Pointer.DoAt(ref reader, soPointer, () => {
					reader.ReadBytes(0x20);
					Pointer off_matrix = Pointer.Read(reader);
					Pointer.DoAt(ref reader, off_matrix, () => {
						(off_matrix.file as PS1Data).OverwriteData(off_matrix.FileOffset, new byte[0x14]);
						(off_matrix.file as PS1Data).OverwriteData(off_matrix.FileOffset + 0x14, (uint)(index * 0x100));
						(off_matrix.file as PS1Data).OverwriteData(off_matrix.FileOffset + 0x18, (uint)(0 * 0x100));
						(off_matrix.file as PS1Data).OverwriteData(off_matrix.FileOffset + 0x1C, (uint)(2 * 0x100));
					});
				});
			}
		}


		public PointerList<PS1.State> GetStates(PS1.Perso3dData p3dData) {
			LevelHeader h = levelHeader;
			ActorFileHeader a1h = actor1Header;
			ActorFileHeader a2h = actor2Header;
			uint fileIndex = p3dData.GetFileIndex();
			switch (fileIndex) {
				case 1:
					if (a1h.states != null) { 
						return a1h.states;
					}
					break;
				case 2:
					if (a2h.states != null) {
						return a2h.states;
					}
					break;
			}
			return h.states;
		}
		public void CalculateNumberOfStatesPerFamily() {
			// Sort familyStates array. So we can determine which states belong to which family
			for (uint i = 0; i < 3; i++) {
				familyStates[i].Sort((t1, t2) => t1.Item2.CompareTo(t2.Item2));
			}
			HashSet<PS1.Family> completedFams = new HashSet<PS1.Family>();
			LevelHeader h = levelHeader;
			ActorFileHeader a1h = actor1Header;
			ActorFileHeader a2h = actor2Header;

			// First pass
			for (uint fi = 0; fi < 3; fi++) {
				for (int fs = 0; fs < familyStates[fi].Count; fs++) {
					var t = familyStates[fi][fs];
					var p3dData = t.Item1;
					int ind = (int)p3dData.stateIndex;
					PS1.Family fam = p3dData.family;
					if (fam == null) continue;

					PointerList<PS1.State> statePtrs = GetStates(p3dData);
					int numStates = statePtrs.Count;



					if (completedFams.Contains(p3dData.family)) {
						if (ind < numStates) {
							if (p3dData.family.startState > ind) p3dData.family.startState = (uint)ind;
							if (p3dData.family.endState < ind) p3dData.family.endState = (uint)ind;
						}
						continue;
					}
					completedFams.Add(fam);
					int startInd = 0; // inclusive
					int endInd = numStates; // exclusive
					if (ind >= numStates) {
						ind = 0;
					}
					int stateInd = ind;
					int famIndStart = fs;
					int famIndEnd = familyStates[fi].FindLastIndex(tup => tup.Item1.family == fam);
					if (famIndStart > 0) {
						startInd = (int)Math.Min(familyStates[fi][famIndStart - 1].Item2 + 1, ind);
					}
					if (famIndEnd < familyStates[fi].Count - 1) {
						endInd = (int)Math.Min(familyStates[fi][famIndEnd + 1].Item2, numStates);
					}
					for (int i = ind; i >= 0; i--) {
						if (i + 1 == startInd) break;
						if (statePtrs[i].anim != null && statePtrs[i].anim.index >= fam.animations.Length) {
							startInd = i + 1;
							break;
						}
					}
					for (int i = ind; i < endInd; i++) {
						if (statePtrs[i].anim != null && statePtrs[i].anim.index >= fam.animations.Length) {
							endInd = i;
							break;
						}
					}
					if (startInd > endInd) {
						startInd = endInd;
					}
					fam.startState = (uint)startInd;
					fam.endState = (uint)endInd;
				}
			}
			// Second pass
			for (uint fi = 0; fi < 3; fi++) {
				for (int fs = 0; fs < familyStates[fi].Count; fs++) {
					var t = familyStates[fi][fs];
					var p3dData = t.Item1;
					PS1.Family fam = p3dData.family;
					if (fam == null || fam.states != null) continue;

					PointerList<PS1.State> statePtrs = GetStates(p3dData);
					// Set states
					fam.states = new PS1.State[fam.endState - fam.startState];
					for (int i = 0; i < fam.states.Length; i++) {
						fam.states[i] = statePtrs[(int)fam.startState + i];
					}
				}
			}
		}
		public void ReadCollSetsR2(Reader reader) {
			if (Settings.s.game != Settings.Game.R2) return;
			Dictionary<PS1.CollSet, HashSet<PS1.Family>> collSets = new Dictionary<PS1.CollSet, HashSet<PS1.Family>>();
			Dictionary<PS1.Family, HashSet<PS1.CollSet>> fcollSets = new Dictionary<PS1.Family, HashSet<PS1.CollSet>>();
			foreach (PS1.Perso p in levelHeader.persos) {
				if (p.collSet == null || p.p3dData?.family == null) continue;
				if (CurrentLevel == Array.IndexOf(game.maps, "vulca_15")) { // Hack
					if (p.name == "OLD_Main_PF_I2") continue;
					//if (p.collSet.Offset.offset == 0x8013DF84) print(p.p3dData.stateIndex + " - " + levelHeader.states[(int)p.p3dData.stateIndex].Offset + " - " + p.name);
				}
				if (!collSets.ContainsKey(p.collSet)) collSets[p.collSet] = new HashSet<PS1.Family>();
				if (!fcollSets.ContainsKey(p.p3dData?.family)) fcollSets[p.p3dData.family] = new HashSet<PS1.CollSet>();
				collSets[p.collSet].Add(p.p3dData.family);
				fcollSets[p.p3dData.family].Add(p.collSet);
			}
			foreach (PS1.CollSet c in collSets.Keys) {
				/*foreach (PS1.Family f in collSets[c]) {
					print(fcollSets[f].Count);
					foreach (PS1.CollSet c2 in fcollSets[f]) {
						print(c2.Offset);
					}
				}*/
				PS1.State[] states = collSets[c].SelectMany(f => f.states).ToArray();
				//Array.Sort(states, (s1, s2) => s1.Offset.offset.CompareTo(s2.Offset.offset));
				c.ReadZdxListDependingOnStates(reader, states);
			}
		}

		public async UniTask LoadLevel() {
			Reader reader = files_array[Mem.Fix]?.reader;
			if (reader == null) throw new Exception("Level \"" + lvlName + "\" does not exist");
			if (game.actors != null && game.actors.Length > 0) {
				RelocateActorFile(0);
				RelocateActorFile(1);
			}
			if (Settings.s.game == Settings.Game.DD) {
				RelocateActorMemoryDonaldDuck();
			}

			// TODO: Load header here
			vram.Export(gameDataBinFolder + "vram.png");

			loadingState = "Loading level header";
			await WaitIfNecessary();
			levelHeader = FromOffsetOrRead<LevelHeader>(reader, Pointer.Current(reader));

			loadingState = "Loading superobject hierarchy";
			await WaitIfNecessary();
			levelHeader.fatherSector = FromOffsetOrRead<PS1.SuperObject>(reader, levelHeader.off_fatherSector, onPreRead: s => s.isDynamic = false);
			levelHeader.dynamicWorld = FromOffsetOrRead<PS1.SuperObject>(reader, levelHeader.off_dynamicWorld, onPreRead: s => s.isDynamic = true);
			levelHeader.inactiveDynamicWorld = FromOffsetOrRead<PS1.SuperObject>(reader, levelHeader.off_inactiveDynamicWorld, onPreRead: s => s.isDynamic = true);

			if (Settings.s.game == Settings.Game.RRush || Settings.s.game == Settings.Game.JungleBook) {
				if (actor1File != null) actor1Header = FromOffsetOrRead<ActorFileHeader>(reader, new Pointer((uint)actor1File.headerOffset, actor1File), onPreRead: h => h.file_index = 1);
				if (actor2File != null) actor2Header = FromOffsetOrRead<ActorFileHeader>(reader, new Pointer((uint)actor2File.headerOffset, actor2File), onPreRead: h => h.file_index = 2);
			}
			CalculateNumberOfStatesPerFamily();
			ReadCollSetsR2(reader);


			// Done reading here
			if (mainMemoryBlock.overlay_cine.size > 0) {
				PS1GameInfo.File file = game.files.FirstOrDefault(f => f.fileID == 0);
				string levelDir = gameDataBinFolder + file.bigfile + "/";
				levelDir += (CurrentLevel < game.maps.Length ? game.maps[CurrentLevel] : (file.bigfile + "_" + CurrentLevel)) + "/";

				//string levelDir = gameDataBinFolder + lvlName + "/";
				uint cineDataBaseAddress = levelHeader.off_animPositions.offset;
				Array.Resize(ref files_array, files_array.Length + 1);
				//print(cineDataBaseAddress);
				files_array[files_array.Length-1] = new PS1Data("overlay_cine.img", levelDir + "overlay_cine.img", files_array.Length - 1,
								cineDataBaseAddress + 0x1f800u + (uint)mainMemoryBlock.cineOffset * 0xc00);
			}
			if (levelHeader.num_geometricObjectsDynamic_cine != 0) {
				levelHeader.geometricObjectsDynamic.ReadExtra(reader, levelHeader.num_geometricObjectsDynamic_cine);
			}

			loadingState = "Calculating texture bounds";
			await WaitIfNecessary();
			CalculateTextures();

			loadingState = "Creating gameobjects";
			await WaitIfNecessary();
			gao_fatherSector = levelHeader.fatherSector?.GetGameObject();
			gao_fatherSector.name = "Father Sector | " + gao_fatherSector.name;
			gao_dynamicWorld = levelHeader.dynamicWorld?.GetGameObject();
			gao_dynamicWorld.name = "Dynamic World | " + gao_dynamicWorld.name;
			gao_inactiveDynamicWorld = levelHeader.inactiveDynamicWorld?.GetGameObject();
			gao_inactiveDynamicWorld.name = "Inactive Dynamic World | " + gao_inactiveDynamicWorld.name;
			gao_always = new GameObject("Always");
			gao_always.transform.position = new Vector3(0, -1000, 0);
			int i = 0;
			foreach (AlwaysList alw in levelHeader.always) {
				GameObject alwGao = alw.GetGameObject();
				alwGao.transform.SetParent(gao_always.transform);
				alwGao.transform.localPosition = new Vector3(i++ * 10f, 0f, 0f);
				i++;
			}
			// Uncomment to show camera modifiers
			/*if (levelHeader.cameraModifiers != null) {
				for(int cm = 0; cm < levelHeader.num_cameraModifiers; cm++) {
					PS1.CameraModifier c = levelHeader.cameraModifiers[cm];
					c.GetGameObject(levelHeader.cameraModifierVolumes[cm]);
				}
			}*/
			/*if (levelHeader.meshCollision != null) {
				foreach (PS1.GeometricObjectCollide c in levelHeader.meshCollision) {
					c.GetGameObject();
				}
			}*/

			GameObject persoPartsParent = new GameObject("Perso parts");
			persoPartsParent.transform.localPosition = new Vector3(0, 1000, 0);
			i = 0;
			foreach (ObjectsTable.Entry e in levelHeader.geometricObjectsDynamic.entries) {
				GameObject g = e.GetGameObject(null, out _);
				g.name = $"[{i}] {e.off_0} - {g.name}";
				g.transform.parent = persoPartsParent.transform;
				g.transform.localPosition = new Vector3(i++ * 4, 0, 0);
			}


			PS1GameInfo.File fileInfo = game.files.FirstOrDefault(f => f.fileID == 0);
			PS1GameInfo.File.MemoryBlock b = fileInfo.memoryBlocks[CurrentLevel];
			streams = new PS1Stream[b.cutscenes.Length];
			for (int j = 0; j < b.cutscenes.Length; j++) {
				loadingState = $"Loading cinematic streams : {j+1}/{b.cutscenes.Length}";
				await WaitIfNecessary();
				reader = files_array[2 + j].reader;
				streams[j] = FromOffsetOrRead<PS1Stream>(reader, Pointer.Current(reader), inline: true);
			}
		}

		#region DAT Parsing

		public async UniTask LoadAllDataFromDAT(PS1GameInfo game) {
			PS1GameInfo.File mainFile = game.files.FirstOrDefault(f => f.fileID == 0);
			await LoadDataFromDAT(game, mainFile, CurrentLevel);
			if (mainFile.memoryBlocks[CurrentLevel].loadActor) {
				if (game.files.Any(f => f.bigfile == "ACTOR1")) {
					// Load Actor
					await LoadDataFromDAT(game, game.files.FirstOrDefault(f => f.bigfile == "ACTOR1"), Actor1Index);
				}
				if (game.files.Any(f => f.bigfile == "ACTOR2")) {
					// Load Actor
					await LoadDataFromDAT(game, game.files.FirstOrDefault(f => f.bigfile == "ACTOR2"), Actor2Index);
				}
			}
		}
		public async UniTask LoadDataFromDAT(PS1GameInfo gameInfo, PS1GameInfo.File fileInfo, int index) {
			PS1GameInfo.File.MemoryBlock b = fileInfo.memoryBlocks[index];
			if (b.loadActor
				&& (Actor1Index < 0 || Actor2Index < 0
				|| Actor1Index >= gameInfo.actors.Length || Actor2Index >= gameInfo.actors.Length)) {
				throw new Exception("Actor could not be found");
			}
			string bigFile = fileInfo.bigfile;
			string bigFilePath = gameDataBinFolder + bigFile + "." + fileInfo.extension;

			string levelDir = gameDataBinFolder + fileInfo.bigfile + "/";
			if (fileInfo.type == PS1GameInfo.File.Type.Map) {
				levelDir += (index < gameInfo.maps.Length ? gameInfo.maps[index] : (bigFile + "_" + index)) + "/";
			} else {
				levelDir += index + "/";
			}
			await PrepareBigFile(bigFilePath, 2048);
			loadingState = "Extracting data from bigfile(s)";
			await WaitIfNecessary();
			if (FileSystem.FileExists(bigFilePath)) {
				using (Reader reader = new Reader(FileSystem.GetFileReadStream(bigFilePath), isLittleEndian: Settings.s.IsLittleEndian)) {

					loadingState = "Extracting data from bigfile(s): main data";
					await WaitIfNecessary();
					List<byte[]> mainBlock = await ExtractPackedBlocks(reader, b.main_compressed, fileInfo.baseLBA);
					await WaitIfNecessary();
					int blockIndex = 0;
					if (fileInfo.type == PS1GameInfo.File.Type.Map) {
						if (CurrentLevel < 0 || CurrentLevel >= fileInfo.memoryBlocks.Length) return;
						if (!b.inEngine) return;
						if (Settings.s.game != Settings.Game.RRush && !b.exeOnly) FileSystem.AddVirtualFile(levelDir + "vignette.tim", mainBlock[blockIndex++]);
						if (!b.exeOnly && b.inEngine) {
							if (b.hasSoundEffects) {
								FileSystem.AddVirtualFile(levelDir + "sound.vb", mainBlock[blockIndex++]);
							}
							FileSystem.AddVirtualFile(levelDir + "vram.xtp", mainBlock[blockIndex++]);
							FileSystem.AddVirtualFile(levelDir + "level.sys", mainBlock[blockIndex++]);
						}
						// skip exe
						byte[] exe = mainBlock[blockIndex++];
						byte[] exeData = mainBlock[blockIndex++];
						/*byte[] newData = new byte[exeHeader.Length + exeData.Length];*/
					Util.AppendArrayAndMergeReferences(ref exe, ref exeData);
						FileSystem.AddVirtualFile(levelDir + "executable.pxe", exe);
						if (!b.exeOnly && b.inEngine) {
							FileSystem.AddVirtualFile(levelDir + "level.img", mainBlock[blockIndex++]);
						}
					} else if(fileInfo.type == PS1GameInfo.File.Type.Actor) {
						FileSystem.AddVirtualFile(levelDir + "vram.xtp", mainBlock[blockIndex++]);
						FileSystem.AddVirtualFile(levelDir + "actor.img", mainBlock[blockIndex++]);
					}
					if (blockIndex != mainBlock.Count) {
						Debug.LogWarning("Not all blocks were extracted!");
					}
					loadingState = "Extracting data from bigfile(s): cinematic overlay";
					await WaitIfNecessary();
					byte[] cineblock = await ExtractBlock(reader, b.overlay_cine, fileInfo.baseLBA);
					if (cineblock != null) {
						FileSystem.AddVirtualFile(levelDir + "overlay_cine.img", cineblock);
					}
					for (int j = 0; j < b.cutscenes.Length; j++) {
						loadingState = $"Extracting data from bigfile(s): streams ({j+1}/{b.cutscenes.Length})";
						await WaitIfNecessary();
						string cutsceneAudioName = levelDir + "stream_audio_" + j + ".blk";
						string cutsceneFramesName = levelDir + "stream_frames_" + j + ".blk";
						byte[] cutsceneAudioBlk = await ExtractBlock(reader, b.cutscenes[j], fileInfo.baseLBA);
						if (cutsceneAudioBlk != null) {
							byte[] cutsceneAudio;
							byte[] cutsceneFrames;
							SplitCutsceneStream(cutsceneAudioBlk, out cutsceneAudio, out cutsceneFrames);
							FileSystem.AddVirtualFile(cutsceneFramesName, cutsceneFrames);
						}
					}
				}
			}
		}

		public async UniTask ExtractAllFiles(PS1GameInfo game) {
			foreach (PS1GameInfo.File f in game.files) {
				await ExtractDAT(game, f, relocatePointers: true);
			}
		}

		public async UniTask ExtractDAT(PS1GameInfo gameInfo, PS1GameInfo.File fileInfo, bool relocatePointers = false) {
			string bigFile = fileInfo.bigfile;
			string bigFilePath = gameDataBinFolder + bigFile + "." + fileInfo.extension;
			uint cineDataBaseAddress = 0;
			if (FileSystem.FileExists(bigFilePath)) {
				using (Reader reader = new Reader(FileSystem.GetFileReadStream(bigFilePath))) {
					PS1GameInfo.File.MemoryBlock[] memoryBlocks = fileInfo.memoryBlocks;
					for (int i = 0; i < memoryBlocks.Length; i++) {
						int gptIndex = 0, lvlIndex = 0;
						PS1GameInfo.File.MemoryBlock b = memoryBlocks[i];
						string levelDir = gameDataBinFolder + fileInfo.bigfile + "/";
						if (fileInfo.type == PS1GameInfo.File.Type.Map) {
							levelDir += (i < gameInfo.maps.Length ? gameInfo.maps[i] : (bigFile + "_" + i)) + "/";
						} else {
							levelDir += i + "/";
						}
						List<byte[]> mainBlock = await ExtractPackedBlocks(reader, b.main_compressed, fileInfo.baseLBA);
						int blockIndex = 0;
						if (fileInfo.type == PS1GameInfo.File.Type.Map) {
							if (Settings.s.game != Settings.Game.RRush && !b.exeOnly) Util.ByteArrayToFile(levelDir + "vignette.tim", mainBlock[blockIndex++]);
							if (!b.exeOnly && b.inEngine) {
								if (b.hasSoundEffects) {
									Util.ByteArrayToFile(levelDir + "sound.vb", mainBlock[blockIndex++]);
								}
								Util.ByteArrayToFile(levelDir + "vram.xtp", mainBlock[blockIndex++]);
								Util.ByteArrayToFile(levelDir + "level.sys", mainBlock[blockIndex++]);
								if (relocatePointers) {
									gptIndex = blockIndex - 1;
								}
							}
							byte[] exe = mainBlock[blockIndex++];
							byte[] exeData = mainBlock[blockIndex++];
							/*byte[] newData = new byte[exeHeader.Length + exeData.Length];*/
							Util.AppendArrayAndMergeReferences(ref exe, ref exeData);
							Util.ByteArrayToFile(levelDir + "executable.pxe", exe);
							if (!b.exeOnly && b.inEngine) {
								Util.ByteArrayToFile(levelDir + "level.img", mainBlock[blockIndex++]);
								if (relocatePointers) {
									lvlIndex = blockIndex - 1;
									uint length = (uint)mainBlock[lvlIndex].Length;
									byte[] data = mainBlock[gptIndex];
									for (int j = 0; j < data.Length; j++) {
										if (data[j] == 0x80) {
											int off = j - 3;
											uint ptr = BitConverter.ToUInt32(data, off);
											if (ptr >= b.address && ptr < b.address + length) {
												if (off == 0x14c) {
													cineDataBaseAddress = ptr;
												}
												ptr = (ptr - b.address) + 0xDD000000;
												byte[] newData = BitConverter.GetBytes(ptr);
												for (int y = 0; y < 4; y++) {
													data[off + 3 - y] = newData[y];
												}
												j += 3;
											}
										}
									}
									Util.ByteArrayToFile(levelDir + "level_relocated.sys", data);
									data = mainBlock[lvlIndex];
									for (int j = 0; j < data.Length; j++) {
										if (data[j] == 0x80) {
											int off = j - 3;
											uint ptr = BitConverter.ToUInt32(data, off);
											if (ptr >= b.address && ptr < b.address + length) {
												ptr = (ptr - b.address) + 0xDD000000;
												byte[] newData = BitConverter.GetBytes(ptr);
												for (int y = 0; y < 4; y++) {
													data[off + 3 - y] = newData[y];
												}
												j += 3;
											}
										}
									}
									Util.ByteArrayToFile(levelDir + "level_relocated.img", data);
								}
							}
						} else if (fileInfo.type == PS1GameInfo.File.Type.Actor) {
							Util.ByteArrayToFile(levelDir + "vram.xtp", mainBlock[blockIndex++]);
							Util.ByteArrayToFile(levelDir + "actor.img", mainBlock[blockIndex++]);
							if (bigFile == "ACTOR1") {
								byte[] data = mainBlock[blockIndex - 1];
								uint baseAddress = game.actor1Address;
								uint length = (uint)data.Length;
								for (int j = 0; j < data.Length; j++) {
									if (data[j] == 0x80) {
										int off = j - 3;
										uint ptr = BitConverter.ToUInt32(data, off);
										if (ptr >= baseAddress && ptr < baseAddress + length) {
											ptr = (ptr - baseAddress) + 0xDD000000;
											byte[] newData = BitConverter.GetBytes(ptr);
											for (int y = 0; y < 4; y++) {
												data[off + 3 - y] = newData[y];
											}
											j += 3;
										}
									}
								}
								Util.ByteArrayToFile(levelDir + "actor_relocated.img", data);
							}
						} else if (fileInfo.type == PS1GameInfo.File.Type.Sound) {
							Util.ByteArrayToFile(levelDir + "sound.vb", mainBlock[blockIndex++]);
						}
						if (blockIndex != mainBlock.Count) {
							Debug.LogWarning("Not all blocks were exported!");
						}


						Util.ByteArrayToFile(levelDir + "overlay_game.img", await ExtractBlock(reader, b.overlay_game, fileInfo.baseLBA));
						byte[] cineblock = await ExtractBlock(reader, b.overlay_cine, fileInfo.baseLBA);
						Util.ByteArrayToFile(levelDir + "overlay_cine.img", cineblock);
						if(cineblock != null) {
							byte[] data = cineblock;
							cineDataBaseAddress += 0x1f800 + 0x32 * 0xc00; // magic!
							for (int j = 0; j < data.Length; j++) {
								if (data[j] == 0x80) {
									int off = j - 3;
									uint ptr = BitConverter.ToUInt32(data, off);
									if (ptr >= b.address && ptr < cineDataBaseAddress) {
										ptr = (ptr - b.address) + 0xDD000000;
										byte[] newData = BitConverter.GetBytes(ptr);
										for (int y = 0; y < 4; y++) {
											data[off + 3 - y] = newData[y];
										}
										j += 3;
									}
									if (ptr >= cineDataBaseAddress && ptr < cineDataBaseAddress + data.Length) {
										ptr = (ptr - cineDataBaseAddress) + 0xCC000000;
										byte[] newData = BitConverter.GetBytes(ptr);
										for (int y = 0; y < 4; y++) {
											data[off + 3 - y] = newData[y];
										}
										j += 3;
									}
								}
							}
							Util.ByteArrayToFile(levelDir + "overlay_cine_relocated.img", data);
						}
						for (int j = 0; j < b.cutscenes.Length; j++) {
							string cutsceneAudioName = levelDir + "stream_audio_" + j + ".blk";
							string cutsceneFramesName = levelDir + "stream_frames_" + j + ".blk";
							byte[] cutsceneAudioBlk = await ExtractBlock(reader, b.cutscenes[j], fileInfo.baseLBA);
							if (cutsceneAudioBlk != null) {
								//Util.ByteArrayToFile(levelDir + "stream_full_" + j + ".blk", cutsceneAudioBlk);
								byte[] cutsceneAudio;
								byte[] cutsceneFrames;
								SplitCutsceneStream(cutsceneAudioBlk, out cutsceneAudio, out cutsceneFrames);
								Util.ByteArrayToFile(cutsceneAudioName, cutsceneAudio);
								Util.ByteArrayToFile(cutsceneFramesName, cutsceneFrames);
							}
						}
						//ParseMainBlock(mainBlock, b, i, gameDataBinFolder + fileInfo.bigfile + "/" + bigFile + "_" + i + "_main");
						await WaitIfNecessary();
					}
				}
			}
		}

		public void ParseMainBlock(byte[] data, PS1GameInfo.File.MemoryBlock block, int index, string basename) {
			using (MemoryStream ms = new MemoryStream(data)) {
				using (Reader reader = new Reader(ms, Settings.s.IsLittleEndian)) {
					reader.ReadUInt32();
					reader.ReadUInt32();
					uint loadingImgSize = reader.ReadUInt32();
					reader.ReadUInt32();
					uint sz2 = reader.ReadUInt32();
					byte[] loadingImg = reader.ReadBytes((int)loadingImgSize - 14);
					Util.ByteArrayToFile(basename + "_loading.img", loadingImg);
					reader.ReadUInt32();
					if (index < 57) {
						if (block.hasSoundEffects) {
							uint i = 0;
							int size = reader.ReadInt32();
							while (size != 0) {
								byte[] playableData = reader.ReadBytes(size * 4 * 2);
								Util.ByteArrayToFile(basename + "_playableData_" + i + ".img", playableData);
								print(string.Format("{0:X8}",reader.BaseStream.Position));
								size = reader.ReadInt32();
								i++;
							}
							//byte[] some
						}
						byte[] textureMem = reader.ReadBytes(0xB0000);
						Util.ByteArrayToFile(basename + "_textures.img", textureMem);
						byte[] preExeData = reader.ReadBytes(0x200);
						Util.ByteArrayToFile(basename + "_preExeData.blk", preExeData);
					}
					byte[] exe = reader.ReadBytes((int)reader.BaseStream.Length - (int)reader.BaseStream.Position);
					Util.ByteArrayToFile(basename + ".pxe", exe);
				}
			}
		}

		public void SplitCutsceneStream(byte[] cutsceneData, out byte[] cutsceneAudio, out byte[] cutsceneFrames) {
			List<byte[]> cutsceneAudioList = new List<byte[]>();
			List<byte[]> cutsceneFramesList = new List<byte[]>();
			using (MemoryStream ms = new MemoryStream(cutsceneData)) {
				using (Reader reader = new Reader(ms, Settings.s.IsLittleEndian)) {
					uint size_frame_packet = 1;
					while (reader.BaseStream.Position < reader.BaseStream.Length && size_frame_packet > 0) {
						size_frame_packet = reader.ReadUInt32();
						//print("HDR " + string.Format("{0:X8}", hdrSize));
						if (size_frame_packet != 0xFFFFFFFF) {
							reader.BaseStream.Position -= 4;
							cutsceneFramesList.Add(reader.ReadBytes((int)size_frame_packet + 4));
							bool readParts = true;
							while (readParts && reader.BaseStream.Position < reader.BaseStream.Length) {
								uint size = reader.ReadUInt32();
								//print("SIZE " + string.Format("{0:X8}", size));
								if (size == 0xFFFFFFFE) {
									readParts = false;
									if (reader.BaseStream.Position % 0x800 > 0) {
										reader.BaseStream.Position = 0x800 * ((reader.BaseStream.Position / 0x800) + 1);
									}
								} else {
									bool isNull = (size & 0x80000000) != 0;
									size = size & 0x7FFFFFFF;
									if (isNull) {
										cutsceneAudioList.Add(Enumerable.Repeat((byte)0x0, (int)size).ToArray());
									} else {
										cutsceneAudioList.Add(reader.ReadBytes((int)size));
									}
								}
							}
						}
					}
				}
			}
			cutsceneAudio = cutsceneAudioList.SelectMany(i => i).ToArray();
			cutsceneFrames = cutsceneFramesList.SelectMany(i => i).ToArray();
		}

		public async UniTask<byte[]> ExtractBlock(Reader reader, PS1GameInfo.File.LBA lba, uint baseLBA) {
			byte[] data;
			if (lba.lba < baseLBA || lba.size <= 0) return null;
			reader.BaseStream.Seek((lba.lba - baseLBA) * 0x800, SeekOrigin.Begin);

			PartialHttpStream httpStream = reader.BaseStream as PartialHttpStream;
			if (httpStream != null) await httpStream.FillCacheForRead((int)lba.size);
			data = reader.ReadBytes((int)lba.size);
			return data;
		}
		public async UniTask<List<byte[]>> ExtractPackedBlocks(Reader reader, PS1GameInfo.File.LBA lba, uint baseLBA) {
			PartialHttpStream httpStream = reader.BaseStream as PartialHttpStream;
			List<byte[]> datas = new List<byte[]>();
			byte[] data;
			if (lba.lba < baseLBA || lba.size <= 0) return null;
			reader.BaseStream.Seek((lba.lba - baseLBA) * 0x800, SeekOrigin.Begin);

			data = new byte[0];
			uint end = (lba.lba + lba.size - baseLBA) * 0x800;
			bool previousWasZero = false;
			bool previousWasFF = false;
			while (reader.BaseStream.Position < end) {
				if (httpStream != null) await httpStream.FillCacheForRead(0x1004);
				uint decompressedSize = reader.ReadUInt32(); // 0x8000
				if (previousWasFF) {
					if (decompressedSize == 0xFFFFFFFF && reader.ReadUInt32() == 0) {
						reader.Align(0x800);
						previousWasFF = false;
					} else {
						reader.BaseStream.Position = 0x800 * (reader.BaseStream.Position / 0x800);
						byte[] uncompressedData = reader.ReadBytes(0x800);
						if (uncompressedData != null) {
							int originalDataLength = data.Length;
							Array.Resize(ref data, originalDataLength + uncompressedData.Length);
							Array.Copy(uncompressedData, 0, data, originalDataLength, uncompressedData.Length);
						}
					}
				} else {
					if (decompressedSize == 0) {
						if (previousWasZero) {
							reader.Align(0x800);
							previousWasZero = false;
							break;
						} else {
							previousWasZero = true;
						}
						previousWasFF = false;
						// If previous was zero, then padding to 0x800. If previous was not zero, then new file.
						//print(decompressedSize + " - " + String.Format("0x{0:X8}", reader.BaseStream.Position));
						datas.Add(data);
						data = new byte[0];
						continue;
					} else if (decompressedSize == 0xFFFFFFFF) {
						if (previousWasZero) {
							reader.Align(0x800);
							previousWasFF = true;
						}
						previousWasZero = false;
						continue;
					} else {
						previousWasZero = false;
						previousWasFF = false;
					}
					uint compressedSize = reader.ReadUInt32();
					if (httpStream != null) await httpStream.FillCacheForRead((int)compressedSize);
					//print(compressedSize + " - " + decompressedSize + " - " + String.Format("0x{0:X8}", reader.BaseStream.Position));
					byte[] uncompressedData = null;
					if (compressedSize == decompressedSize) {
						uncompressedData = reader.ReadBytes((int)decompressedSize);
					} else {
						byte[] compressedData = reader.ReadBytes((int)compressedSize);
						using (var compressedStream = new MemoryStream(compressedData))
						using (var lzo = new LzoStream(compressedStream, CompressionMode.Decompress))
						using (Reader lzoReader = new Reader(lzo, Settings.s.IsLittleEndian)) {
							lzo.SetLength(decompressedSize);
							uncompressedData = lzoReader.ReadBytes((int)decompressedSize);
						}
					}
					if (uncompressedData != null) {
						int originalDataLength = data.Length;
						Array.Resize(ref data, originalDataLength + uncompressedData.Length);
						Array.Copy(uncompressedData, 0, data, originalDataLength, uncompressedData.Length);
					}
				}
			}
			if (data.Length > 0) {
				datas.Add(data);
			}
			return datas;
		}
		#endregion

		#region Textures
		public void FillVRAM() {
			int startXPage = 5;
			if (Settings.s.game == Settings.Game.JungleBook) {
				startXPage = 8;
			}
			vram.currentXPage = startXPage;

			PS1GameInfo.File fileInfo = game.files.FirstOrDefault(f => f.fileID == 0);
			int index = CurrentLevel;
			string levelDir = gameDataBinFolder + fileInfo.bigfile + "/";
			if (fileInfo.type == PS1GameInfo.File.Type.Map) {
				levelDir += (index < game.maps.Length ? game.maps[index] : (fileInfo.bigfile + "_" + index)) + "/";
			} else {
				levelDir += index + "/";
			}

			using (Reader reader = new Reader(FileSystem.GetFileReadStream(levelDir + "vram.xtp"))) {
				byte[] data = reader.ReadBytes((int)reader.BaseStream.Length);
				int width = Mathf.CeilToInt(data.Length / (float)(PS1VRAM.page_height * 2));
				vram.AddData(data, width);
			}
			if (game.files.Any(f => f.bigfile == "ACTOR1")) {
				using (Reader reader = new Reader(FileSystem.GetFileReadStream(gameDataBinFolder + "ACTOR1/" + Actor1Index + "/vram.xtp"))) {
					byte[] data = reader.ReadBytes((int)reader.BaseStream.Length);
					vram.AddDataAt(startXPage, 0, 0, 0, data, PS1VRAM.page_width);
				}
			}
			if (game.files.Any(f => f.bigfile == "ACTOR2")) {
				using (Reader reader = new Reader(FileSystem.GetFileReadStream(gameDataBinFolder + "ACTOR2/" + Actor2Index + "/vram.xtp"))) {
					byte[] data = reader.ReadBytes((int)reader.BaseStream.Length);
					vram.AddDataAt(startXPage + 1, 0, 0, 0, data, PS1VRAM.page_width);
				}
			}
		}

		private List<TextureBounds> textureBounds = new List<TextureBounds>();

		public void RegisterTexture(ushort pageInfo, ushort palette, int xMin, int xMax, int yMin, int yMax) {
			TextureBounds b = new TextureBounds() {
				pageInfo = pageInfo,
				paletteInfo = palette,
				xMin = xMin,
				xMax = xMax,
				yMin = yMin,
				yMax = yMax
			};

            bool newTexture = true;
            foreach(TextureBounds u in textureBounds) {
                if (u.HasOverlap(b)) {
                    u.ExpandWithBounds(b);
                    newTexture = false;
                    break;
                }
            }

            if (newTexture) {
                textureBounds.Add(b);
            }
		}

        public void CalculateTextures() {
            int i = 0;
            foreach (TextureBounds b in textureBounds) {
                int w = b.xMax - b.xMin;
                int h = b.yMax - b.yMin;
				//print(w + " - " + h + " - " + b.xMin + " - " + b.yMin + " - " + b.pageInfo + " - " + b.paletteInfo);
                Texture2D tex = vram.GetTexture((ushort)w, (ushort)h, b.pageInfo, b.paletteInfo, b.xMin, b.yMin);
				tex.wrapMode = TextureWrapMode.Clamp;
                b.texture = tex;
                if (exportTextures) {
                    Util.ByteArrayToFile(gameDataBinFolder + "textures/main/" + lvlName + "/" + i++ + $"_{string.Format("{0:X4}",b.pageInfo)}_{b.xMin}_{b.yMin}_{w}_{h}" + ".png", tex.EncodeToPNG());
                }
            }
        }

        public TextureBounds GetTextureBounds(ushort pageInfo, ushort paletteInfo, int x, int y) {
			return textureBounds.FirstOrDefault(
				t => t.pageInfo == pageInfo && t.paletteInfo == paletteInfo &&
				x >= t.xMin && x < t.xMax &&
				y >= t.yMin && y < t.yMax);
		}
		#endregion
	}
}
