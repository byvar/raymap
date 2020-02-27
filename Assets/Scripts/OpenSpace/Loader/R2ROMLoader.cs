using OpenSpace.FileFormat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections;
using OpenSpace.ROM;
using OpenSpace.FileFormat.Texture;
using OpenSpace.FileFormat.Texture.DS;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenSpace.Loader {
	public class R2ROMLoader : MapLoader {
		public ROMBIN data;
		public ROMBIN fat;
		public FATTable[] fatTables;
		public int CurrentLevel { get; private set; } = -1;
		public ROMAnimation[] romAnims;
		public ROMShAnimation[] shAnims;
		public ROMAnimationCutTable cutTable;
		public List<ObjectsTable> objectsTables = new List<ObjectsTable>();
		public List<Graph> graphsROM = new List<Graph>();
		public List<WayPoint> waypointsROM = new List<WayPoint>();
		public LevelHeader level;
		public Localization localizationROM = null;

		public Pointer[] texturesTable;
		public Pointer[] palettesTable;
		public uint ind_textureTable_i4;
		public uint ind_textureTable_i8;
		public uint ind_textureTable_rgba;
		public bool[] texturesTableSeen;
		public bool[] palettesTableSeen;
		
		public Dictionary<FATEntry.Type, Dictionary<ushort, ROMStruct>> romStructs = new Dictionary<FATEntry.Type, Dictionary<ushort, ROMStruct>>();

		public string[] LoadLevelList() {
			if (gameDataBinFolder == null || gameDataBinFolder.Trim().Equals("")) return null;
			gameDataBinFolder += "/";
			if (!File.Exists(gameDataBinFolder + "data.bin")) return null;
			if (!File.Exists(gameDataBinFolder + "fat.bin")) return null;
			files_array[SMem.Data] = new ROMBIN("data.bin", gameDataBinFolder + "data.bin", SMem.Data);
			files_array[SMem.Fat] = new ROMBIN("fat.bin", gameDataBinFolder + "fat.bin", SMem.Fat);
			
			// Load fat
			Reader reader = files_array[SMem.Fat].reader;
			uint num_tables = reader.ReadUInt32();
			if (fatTables == null) {
				fatTables = new FATTable[num_tables + 2];
			}
			for (uint i = 0; i < num_tables + 2; i++) {
				fatTables[i] = FATTable.Read(reader, Pointer.Current(reader), readEntries: i < 2);
			}
			string[] levels = new string[num_tables];
			Pointer off_levelList = GetStructPtr(FATEntry.Type.LevelList, (ushort)(0 | FATEntry.Flag.Fix), false);
			for (int i = 0; i < levels.Length; i++) {
				Pointer.DoAt(ref reader, off_levelList + 64*i, () => {
					reader.ReadUInt16();
					levels[i] = reader.ReadNullDelimitedString();
				});
			}
			/*LevelList levelList = GetOrRead<LevelList>(reader, 0x8000, l => l.num_levels = (ushort)num_tables);
			levels = levelList.levels.Select(l => l.name).ToArray();*/
			return levels;
		}


		protected override async Task Load() {
			try {
				if (gameDataBinFolder == null || gameDataBinFolder.Trim().Equals("")) throw new Exception("GAMEDATABIN folder doesn't exist");
				if (lvlName == null || lvlName.Trim() == "") throw new Exception("No level name specified!");
				globals = new Globals();
				gameDataBinFolder += "/";
				await FileSystem.CheckDirectory(gameDataBinFolder);
				if (!FileSystem.DirectoryExists(gameDataBinFolder)) throw new Exception("GAMEDATABIN folder doesn't exist");
				loadingState = "Initializing files";
				await PrepareFile(gameDataBinFolder + "data.bin");
				await PrepareFile(gameDataBinFolder + "fat.bin");
				await PrepareFile(gameDataBinFolder + "anims.bin");
				await PrepareFile(gameDataBinFolder + "shAnims.bin");
				await PrepareFile(gameDataBinFolder + "cuttable.bin");

				files_array[SMem.Data] = new ROMBIN("data.bin", gameDataBinFolder + "data.bin", SMem.Data);
				files_array[SMem.Fat] = new ROMBIN("fat.bin", gameDataBinFolder + "fat.bin", SMem.Fat);
				files_array[SMem.Anims] = new ROMBIN("anims.bin", gameDataBinFolder + "anims.bin", SMem.Anims);
				files_array[SMem.ShAnims] = new ROMBIN("shAnims.bin", gameDataBinFolder + "shAnims.bin", SMem.ShAnims);
				files_array[SMem.CutTable] = new ROMBIN("cuttable.bin", gameDataBinFolder + "cuttable.bin", SMem.CutTable);

				await LoadAnims();

				await LoadFat();
				// Determine level index
				await LoadFix();
				if (CurrentLevel == -1) {
					throw new Exception("Level list does not contain this level");
				}


				await LoadFatLevel(loadAll: exportTextures);
				
				await LoadData();

				if (exportTextures) {
					Debug.LogError("Textures saved. Due to how N64 loading works, levels can only be loaded without Save Textures.");
					return;
				}
				if (Settings.s.game == Settings.Game.R2
					&& (Settings.s.platform == Settings.Platform.N64 || Settings.s.platform == Settings.Platform.DS)) {
                    string objectNamesFileName = "objectNames_" + lvlName.ToLower() + ".json";
                    string objectNamesPath = gameDataBinFolder + objectNamesFileName;
					await PrepareFile(objectNamesPath);

					if (!FileSystem.FileExists(objectNamesPath)) {
                        objectNamesPath = Application.streamingAssetsPath + "/R2_N64_DS/" + objectNamesFileName; // Offline, the json doesn't exist, so grab it from StreamingAssets
                    }

					using (Stream stream = FileSystem.GetFileReadStream(objectNamesPath)) {
						if (stream != null) {
							ReadAndFillObjectNames(stream);
						}
					}
                }

                /*List<DSFATEntry> entries = new List<DSFATEntry>();
				for (int i = 0; i < fatTables.Length; i++) {
					for (int j = 0; j < fatTables[i].entries.Length; j++) {
						entries.Add(fatTables[i].entries[j]);
					}
				}
				entries.Sort((a,b) => (a.off_data.CompareTo(b.off_data)));*/
                /*IEnumerable<KeyValuePair<DSFATEntry, int>> groups = entries.GroupBy(e => e.type).Select(g => new KeyValuePair<DSFATEntry, int>(g.First(e1 => e1.unk1 == g.Max(e2 => e2.unk1)), g.Count()));
				foreach (KeyValuePair<DSFATEntry, int> g in groups) {
					print("Type: " + g.Key.type + " - Unk1: " + g.Key.unk1 + " - Amount: " + g.Value);
				}*/
                /*for (int i = 0; i < entries.Count; i++) {
					DSFATEntry entry = entries[i];
					uint nextOffset;
					if (i < entries.Count - 1) {
						nextOffset = entries[i + 1].off_data;
					} else {
						nextOffset = (uint)data.reader.BaseStream.Length;
					}
					uint size = nextOffset - entry.off_data;
					if (entry.EntryType == DSFATEntry.Type.Text) {
						Pointer off = new Pointer(entry.off_data, data);
						Pointer.DoAt(ref reader, off, () => {
							string bytes = reader.ReadNullDelimitedString();*/
                //print(entry.unk1 + " - " + bytes);
                /*string name = gameDataBinFolder + "ext/" + "t" + entry.tableIndex + " _e" + entry.entryIndex + "_ " + String.Format("0x{0:X8}", entry.off_data) + ".bin";
                Util.ByteArrayToFile(name, bytes);*/
                /*		});
					}
				}*/
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

        public void ReadAndFillObjectNames(Stream stream)
        {
            string dataAsJson = new StreamReader(stream).ReadToEnd();
            var objectNames = JsonConvert.DeserializeObject<Exporter.MapExporter.NameInfoContainer>(dataAsJson);

            Dictionary<Vector3, int> coordinateCount = new Dictionary<Vector3, int>();

            var positions = controller.romPersos.Where(p=>p.perso.p3dData.Value.objectsTable.Value!=null).Select(p => p.gameObject.transform.position);

            float minX = positions.Select(p => p.x).Min();
            float minY = positions.Select(p => p.y).Min();
            float minZ = positions.Select(p => p.z).Min();

            float maxX = positions.Select(p => p.x).Max();
            float maxY = positions.Select(p => p.y).Max();
            float maxZ = positions.Select(p => p.z).Max();

            Vector3 centerVector = new Vector3(minX + maxX, minY + maxY, minZ + maxZ) * 0.5f;

            foreach (ROMPersoBehaviour perso in controller.romPersos) {
                Vector3 pos = perso.gameObject.transform.position;

                if (coordinateCount.ContainsKey(pos)) {
                    coordinateCount[pos] += 1;
                } else {
                    coordinateCount[pos] = 1;
                }

            }

            var mostCommonCoordinate = coordinateCount.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            //Vector3 difference = Vector3.zero - mostCommonCoordinate; // Most Common Coordinate is Vector3.zero most of the time
            Vector3 difference = new Vector3(objectNames.centerX, objectNames.centerY, objectNames.centerZ) - centerVector; // Most Common Coordinate is Vector3.zero most of the time

            //Debug.Log("Difference with 0,0,0: " + difference);

            foreach (ROMPersoBehaviour perso in controller.romPersos) {

                Vector3 pos = perso.gameObject.transform.position + difference;
                Vector3 roundedPos = new Vector3((float)Math.Round(pos.x, 1), (float)Math.Round(pos.y, 1), (float)Math.Round(pos.z, 1));
                int hashCode = roundedPos.GetHashCode();

                if (objectNames.nameInfos.ContainsKey(hashCode)) {
                    var infos = objectNames.nameInfos[hashCode];
					Exporter.MapExporter.NameInfo correctInfo = null;

                    if (infos.Count == 1) {
                        correctInfo = infos[0];
                    } else {
                        /*foreach (var i in infos) {
                            if (i.customBits == perso.perso.stdGame.Value.customBits) { TODO: match on custom bits
                                correctInfo = i;
                                break;
                            }
                        }*/

                        foreach (var i in infos) {
                            if (i.numRules == perso.perso.brain.Value?.aiModel.Value?.comportsIntelligence.Value?.num_comports && 
                                i.numReflexes == perso.perso.brain.Value?.aiModel.Value?.comportsReflex.Value?.num_comports) {
                                correctInfo = i;
                                break;
                            }
                        }
                    }

                    if (correctInfo != null) {
                        perso.gameObject.name = "[" + correctInfo.familyName + "] " + correctInfo.modelName + " | " + correctInfo.instanceName;
                    }
                }

            }
        }

        public async Task LoadFat() {
			data = files_array[SMem.Data] as ROMBIN;
			fat = files_array[SMem.Fat] as ROMBIN;
			Reader reader = files_array[SMem.Fat].reader;

			loadingState = "Loading struct tables";
			await WaitIfNecessary();
			uint num_tables = reader.ReadUInt32();
			if (fatTables == null) {
				fatTables = new FATTable[num_tables+2];
			}
			for (uint i = 0; i < num_tables + 2; i++) {
				if (i < 2) {
					loadingState = "Loading struct table " + (i + 1) + "/" + (num_tables + 2);
					await WaitIfNecessary();
				}
				fatTables[i] = FATTable.Read(reader, Pointer.Current(reader), readEntries: i < 2);
			}
			await WaitIfNecessary();
		}

		public async Task LoadFatLevel(bool loadAll = false) {
			Reader reader = files_array[SMem.Fat].reader;
			for (uint i = 2; i < fatTables.Length; i++) {
				bool loadCurrent = loadAll || (i == CurrentLevel+2);
				if (loadCurrent) {
					loadingState = "Loading struct table " + (i + 1) + "/" + (fatTables.Length);
					await WaitIfNecessary();
					fatTables[i].ReadEntries(reader);
				}
			}
		}

		public async Task LoadFix() {
			data = files_array[SMem.Data] as ROMBIN;
			Reader reader = files_array[SMem.Data].reader;

			reader.ReadUInt16();
			reader.ReadUInt16();
			ushort num_textureTables = reader.ReadUInt16(); // for texture data. not referenced in fat.bin
			ushort flags = reader.ReadUInt16();
			ushort num_levels = reader.ReadUInt16();
			reader.ReadUInt16();

			if (Settings.s.platform == Settings.Platform.DS || Settings.s.platform == Settings.Platform.N64) {
				loadingState = "Loading texture tables";
				await WaitIfNecessary();
				for (int i = 0; i < 18; i++) {
					Pointer off_list = Pointer.Read(reader);
					uint num_list = reader.ReadUInt32();
				}
				Pointer off_table_i4 = Pointer.Read(reader);
				uint sz_table_i4 = reader.ReadUInt32() >> 2;
				Pointer off_table_i8 = Pointer.Read(reader);
				uint sz_table_i8 = reader.ReadUInt32() >> 2;
				Pointer off_table_rgba = Pointer.Read(reader);
				uint sz_table_rgba = reader.ReadUInt32() >> 2;
				ind_textureTable_i4 = 0;
				ind_textureTable_i8 = ind_textureTable_i4 + (sz_table_i4);
				ind_textureTable_rgba = ind_textureTable_i8 + (sz_table_i8);
				uint totalSz = ind_textureTable_rgba + (sz_table_rgba);
				texturesTable = new Pointer[totalSz];
				Pointer.DoAt(ref reader, off_table_i4, () => {
					for (int i = 0; i < sz_table_i4; i++) {
						texturesTable[ind_textureTable_i4 + i] = Pointer.Read(reader);
					}
				});
				Pointer.DoAt(ref reader, off_table_i8, () => {
					for (int i = 0; i < sz_table_i8; i++) {
						texturesTable[ind_textureTable_i8 + i] = Pointer.Read(reader);
					}
				});
				Pointer.DoAt(ref reader, off_table_rgba, () => {
					for (int i = 0; i < sz_table_rgba; i++) {
						texturesTable[ind_textureTable_rgba + i] = Pointer.Read(reader);
					}
				});
				Pointer off_palettesTable = Pointer.Read(reader);
				if (Settings.s.platform == Settings.Platform.DS) {
					uint sz_palettesTable = reader.ReadUInt32() >> 2;
					palettesTable = new Pointer[sz_palettesTable];
					print(texturesTable.Length + " - " + palettesTable.Length);
					Pointer.DoAt(ref reader, off_palettesTable, () => {
						for (int i = 0; i < sz_palettesTable; i++) {
							palettesTable[i] = Pointer.Read(reader);
						}
					});
				} else {
					palettesTable = new Pointer[0];
				}
			}

			// Read fix texture list
			loadingState = "Loading engine structure";
			await WaitIfNecessary();
			EngineStruct engineStruct = GetOrRead<EngineStruct>(reader, (ushort)(0 | FATEntry.Flag.Fix));

			// Read languages table
			localizationROM = new Localization();
			await localizationROM.Read(reader);

			// Load level list
			loadingState = "Loading level list";
			await WaitIfNecessary();
			LevelList levelList = GetOrRead<LevelList>(reader, (ushort)(0 | FATEntry.Flag.Fix), l => l.num_levels = num_levels);
			for (int i = 0; i < num_levels; i++) {
				if (levelList.levels[i].name.ToLower() == lvlName.ToLower()) {
					CurrentLevel = i;
					break;
				}
			}
		}

		public async Task LoadData() {
			Reader reader = files_array[SMem.Data].reader;
			if (exportTextures) {
				string state = loadingState;
				loadingState = "Exporting textures";
				await WaitIfNecessary();
				ExportTextures(reader);
				loadingState = state;
				await WaitIfNecessary();
				return;
			}

			// Load current level data
			loadingState = "Loading level data";
			await WaitIfNecessary();
			LevelHeader lh = GetOrRead<LevelHeader>(reader, (ushort)(CurrentLevel | (ushort)FATEntry.Flag.Fix));
			level = lh;
			loadingState = "Loading additional object lists";
			await WaitIfNecessary();
			for (ushort i = 0; i < 0x7FFF; i++) {
				// Only do it a few times because we're trying to load way more than there is,
				// so it takes really long if we yield for everything
				if (i % 4096 == 0) {
					loadingState = "Loading additional object lists: " + (i + 1);
					await WaitIfNecessary();
				}
				ObjectsTable ot = GetOrRead<ObjectsTable>(reader, (ushort)(i | (ushort)FATEntry.Flag.Fix));
				if (ot != null) objectsTables.Add(ot);
			}
			for (ushort i = 0; i < 0x8000; i++) {
				// Only do it a few times because we're trying to load way more than there is,
				// so it takes really long if we yield for everything
				if (i % 4096 == 0) {
					loadingState = "Loading additional object lists: " + (i + 1);
					await WaitIfNecessary();
				}
				ObjectsTable ot = GetOrRead<ObjectsTable>(reader, i);
				if (ot != null) objectsTables.Add(ot);
			}

			for (ushort i = 0; i < 0x7FFF; i++) {
				// Only do it a few times because we're trying to load way more than there is,
				// so it takes really long if we yield for everything
				if (i % 4096 == 0) {
					loadingState = "Loading waypoints: " + (i + 1);
					await WaitIfNecessary();
				}
				WayPoint wp = GetOrRead<WayPoint>(reader, (ushort)(i | (ushort)FATEntry.Flag.Fix));
			}
			for (ushort i = 0; i < 0x8000; i++) {
				// Only do it a few times because we're trying to load way more than there is,
				// so it takes really long if we yield for everything
				if (i % 4096 == 0) {
					loadingState = "Loading waypoints: " + (i + 1);
					await WaitIfNecessary();
				}
				WayPoint wp = GetOrRead<WayPoint>(reader, i);
			}
			for (ushort i = 0; i < 0x7FFF; i++) {
				// Only do it a few times because we're trying to load way more than there is,
				// so it takes really long if we yield for everything
				if (i % 4096 == 0) {
					loadingState = "Loading graphs: " + (i + 1);
					await WaitIfNecessary();
				}
				Graph g = GetOrRead<Graph>(reader, (ushort)(i | (ushort)FATEntry.Flag.Fix));
			}
			for (ushort i = 0; i < 0x8000; i++) {
				// Only do it a few times because we're trying to load way more than there is,
				// so it takes really long if we yield for everything
				if (i % 4096 == 0) {
					loadingState = "Loading graphs: " + (i + 1);
					await WaitIfNecessary();
				}
				Graph g = GetOrRead<Graph>(reader, i);
			}
			loadingState = "Initializing hierarchy";
			await WaitIfNecessary();
			if (lh != null) {
				if (lh.hierarchyRoot.Value != null) {
					lh.hierarchyRoot.Value.GetGameObject();
				}
			}
		}

		public async Task LoadAnims() {
			// Read anims.bin
			Reader reader = files_array[SMem.Anims].reader;
			loadingState = "Loading animations";
			await WaitIfNecessary();
			uint num_anims = reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt32();
			Pointer eof = null;
			romAnims = new ROMAnimation[num_anims];
			for (uint i = 0; i < num_anims; i++) {
				uint offset = reader.ReadUInt32();
				romAnims[i] = new ROMAnimation() {
					compressed = (offset & 0x80000000) == 0x80000000,
					index = i
				};
				romAnims[i].Init(new Pointer(offset & 0x7FFFFFFF, files_array[SMem.Anims]));
				if (i > 0) {
					romAnims[i - 1].compressedSize = romAnims[i].Offset.offset - romAnims[i - 1].Offset.offset;
				}
			}
			eof = new Pointer(reader.ReadUInt32(), files_array[SMem.Anims]); // EOF
			if (num_anims > 0) {
				romAnims[num_anims - 1].compressedSize = eof.offset - romAnims[num_anims - 1].Offset.offset;
			}
			for (uint i = 0; i < num_anims; i++) {
				romAnims[i].Read(reader);
			}

			// Read shAnims.bin
			reader = files_array[SMem.ShAnims].reader;
			List<ROMShAnimation> shAnimsList = new List<ROMShAnimation>();
			while (reader.BaseStream.Position < reader.BaseStream.Length) {
				ROMShAnimation shAnim = new ROMShAnimation();
				shAnim.Init(Pointer.Current(reader));
				shAnim.Read(reader, true);
				shAnimsList.Add(shAnim);
			}
			shAnims = shAnimsList.ToArray();

			// Read cuttable.bin
			reader = files_array[SMem.CutTable].reader;
			cutTable = new ROMAnimationCutTable();
			cutTable.Init(Pointer.Current(reader));
			cutTable.length = (ushort)num_anims;
			cutTable.Read(reader, true);
		}

		public void ExportTextures(Reader reader) {
			// Textures in data.bin
			texturesTableSeen = new bool[texturesTable.Length];
			palettesTableSeen = new bool[palettesTable.Length];
			for (int i = 0; i < fatTables.Length; i++) {
				for (int j = 0; j < fatTables[i].entries.Length; j++) {
					if (fatTables[i].entries[j].EntryType != FATEntry.Type.TextureInfo) continue;
					Pointer ptr = new Pointer(fatTables[i].entries[j].off_data, files_array[SMem.Data]);
					TextureInfo texInfo = new TextureInfo();
					texInfo.Init(ptr, fatTables[i].entries[j].index);
					texInfo.Read(reader);
				}
			}
			for (int i = 0; i < texturesTable.Length; i++) {
				if (!texturesTableSeen[i]) {
					print("Unused Texture: " + i + " - " + texturesTable[i] + ". Est. length: " + (texturesTable[i + 1].offset - texturesTable[i].offset));
					uint size = (texturesTable[i + 1].offset - texturesTable[i].offset);
					float logSize = Mathf.Log(size, 2);
					if (i < ind_textureTable_i8) {
						// I4
						for (int w = 3; w < 15; w++) {
							for (int h = 3; h < 15; h++) {
								if (w + h == (int)logSize + 1) {
									GF64 gf = new GF64(reader, texturesTable[i], 1 << w, 1 << h, GF64.Format.I4, null, 16);
									Util.ByteArrayToFile(gameDataBinFolder + "/textures/unused/" + GF64.Format.I4 + "_T" + (i-ind_textureTable_i4) + "_" + gf.texture.width + "x" + gf.texture.height + ".png", gf.texture.EncodeToPNG());
								}
							}
						}
					} else if (i < ind_textureTable_rgba) {
						// I8
						for (int w = 3; w < 15; w++) {
							for (int h = 3; h < 15; h++) {
								if (w + h == (int)logSize) {
									GF64 gf = new GF64(reader, texturesTable[i], 1 << w, 1 << h, GF64.Format.I8, null, 32);
									Util.ByteArrayToFile(gameDataBinFolder + "/textures/unused/" + GF64.Format.I8 + "_T" + (i-ind_textureTable_i8) + "_" + gf.texture.width + "x" + gf.texture.height + ".png", gf.texture.EncodeToPNG());
								}
							}
						}
					} else {
						// RGBA16
						for (int w = 3; w < 15; w++) {
							for (int h = 3; h < 15; h++) {
								if (w + h == (int)logSize-1) {
									GF64 gf = new GF64(reader, texturesTable[i], 1 << w, 1 << h, GF64.Format.RGBA, null, 32);
									Util.ByteArrayToFile(gameDataBinFolder + "/textures/unused/" + GF64.Format.RGBA + "_T" + (i-ind_textureTable_rgba) + "_" + gf.texture.width + "x" + gf.texture.height + ".png", gf.texture.EncodeToPNG());
								}
							}
						}
					}
				}
			}
			/*for (int i = 0; i < palettesTable.Length; i++) {
				if (!palettesTableSeen[i]) {
					print("Unused Palette: " + i + " - " + palettesTable[i] + ". Est. num colors: " + (palettesTable[i+1].offset-palettesTable[i].offset)/2);

					if (!File.Exists(gameDataBinFolder + "/textures/unused/palettes/" + i + ".pal")) {
						Pointer.DoAt(ref reader, palettesTable[i], () => {
							byte[] bytes = reader.ReadBytes((int)(palettesTable[i + 1].offset - palettesTable[i].offset));
							Util.ByteArrayToFile(gameDataBinFolder + "/textures/unused/palettes/" + i + ".bin", bytes);
						});
					}
				}
			}*/
			//print("Unused textures: " + texturesTableSeen.Where(t => !t).Count() + " - Unused palettes: " + palettesTableSeen.Where(p => !p).Count());
			if (Settings.s.platform == Settings.Platform.DS) {
				// R2 DS
				/*PAL palette = new PAL(gameDataBinFolder + "hud/objpal.bin");
				ExportNBFC("hud/sprlums1.nbfc", 4, 4, palette.palette);
				ExportNBFC("hud/sprcage1.nbfc", 4, 4, palette.palette);
				ExportNBFC("hud/sprraym1.nbfc", 4, 4, palette.palette);
				ExportNBFC("hud/sprraym2.nbfc", 4, 4, palette.palette);
				ExportNBFC("hud/sprenmy1.nbfc", 4, 4, palette.palette);
				ExportNBFC("hud/sprnumb0.nbfc", 2, 2, palette.palette);
				ExportNBFC("hud/sprnumb1.nbfc", 2, 2, palette.palette);
				ExportNBFC("hud/sprnumb2.nbfc", 2, 2, palette.palette);
				ExportNBFC("hud/sprnumb3.nbfc", 2, 2, palette.palette);
				ExportNBFC("hud/sprnumb4.nbfc", 2, 2, palette.palette);
				ExportNBFC("hud/sprnumb5.nbfc", 2, 2, palette.palette);
				ExportNBFC("hud/sprnumb6.nbfc", 2, 2, palette.palette);
				ExportNBFC("hud/sprnumb7.nbfc", 2, 2, palette.palette);
				ExportNBFC("hud/sprnumb8.nbfc", 2, 2, palette.palette);
				ExportNBFC("hud/sprnumb9.nbfc", 2, 2, palette.palette);
				ExportNBFC("hud/sprslash.nbfc", 2, 2, palette.palette);
				ExportNBFC("hud/spbars.nbfc", 1, 9, palette.palette);
				ExportNBFC("hud/stick.nbfc", 4, 4, palette.palette);
				ExportNBFC("hud/stickbase.nbfc", 8, 8, palette.palette);
				ExportNBFC("hud/sprspark.nbfc", 1, 1, palette.palette);
				ExportNBFC("hud/ok.nbfc", 4, 4, palette.palette);
				ExportNBFC("hud/smaller.nbfc", 4, 4, palette.palette);
				ExportNBFC("hud/bigger.nbfc", 4, 4, palette.palette);
				ExportNBFC("hud/starhi.nbfc", 2, 2, palette.palette);
				ExportNBFC("hud/starmed.nbfc", 2, 2, palette.palette);
				ExportNBFC("hud/starlow.nbfc", 2, 2, palette.palette);
				ExportNBFC("hud/slider.nbfc", 4, 4, palette.palette);
				ExportNBFC("hud/slider.nbfc", 4, 4, palette.palette);
				ExportGFX("hud/bgcalib.gfx", "hud/bgcalib.map", "hud/bgcalib.pal", 32, 32); // tiles: 768
				ExportGFX("hud/mainbg.gfx", "hud/mainbg.map", "hud/mainbg.pal", 32, 32); // tiles: 864*/

				// RRR DS
				/*foreach (string bg in rrrMapBackgrounds) {
					ExportGFX("hud/" + bg + ".bgc", "hud/" + bg + ".scr", "hud/" + bg + ".pal", 32, 32);
				}
				PAL[] palettes = rrrPalettes.Select(p => new PAL(gameDataBinFolder + "hud/" + p)).ToArray();
				foreach (RRRPalettedTextureReference texRef in rrrTexRefs) {
					ExportNBFC("hud/" + texRef.name, texRef.width / 8, texRef.height / 8, palettes[texRef.palette].palette, i4: true);
				}
				ExportNBFC("hud/Cage_Icone.bgc", 4, 4, new PAL(gameDataBinFolder + "hud/Cage_Icone.pal").palette, i4: true);
				ExportNBFC("hud/Etoile_Icone.bgc", 4, 4, new PAL(gameDataBinFolder + "hud/Etoile_Icone.pal").palette, i4: true);
				ExportNBFC("hud/hud_map_corner.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/hud_Map_Ind.pal").palette, i4: true);
				ExportNBFC("hud/ICO_Wind_NZ.bgc", 4, 4, new PAL(gameDataBinFolder + "hud/ICO_Wind_NZ.pal").palette, i4: true);
				ExportNBFC("hud/Jauge_Block.bgc", 1, 1, new PAL(gameDataBinFolder + "hud/Jauge_Block.pal").palette, i4: true);
				ExportNBFC("hud/Jauge_Fond.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Jauge_Block.pal").palette, i4: true);
				ExportNBFC("hud/Jauge_Quart.bgc", 1, 1, new PAL(gameDataBinFolder + "hud/Jauge_Block.pal").palette, i4: true);
				ExportNBFC("hud/lums_jaune.bgc", 4, 4, new PAL(gameDataBinFolder + "hud/lums_jaune.pal").palette, i4: true);
				ExportNBFC("hud/Map2d_blocage.bgc", 1, 2, new PAL(gameDataBinFolder + "hud/Map2d_blocage.pal").palette, i4: true);
				ExportNBFC("hud/Map2d_Cage.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Map2d_blocage.pal").palette, i4: true);
				ExportNBFC("hud/Map2d_Crystal.bgc", 2, 4, new PAL(gameDataBinFolder + "hud/Map2d_blocage.pal").palette, i4: true);
				ExportNBFC("hud/Map2d_Gard_Pris.bgc", 4, 4, new PAL(gameDataBinFolder + "hud/Map2d_blocage.pal").palette, i4: true);
				ExportNBFC("hud/Map2d_Mont_Or.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Map2d_blocage.pal").palette, i4: true);
				ExportNBFC("hud/Map2d_Teleport.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Map2d_blocage.pal").palette, i4: true);
				ExportNBFC("hud/Panneau1.bgc", 4, 4, new PAL(gameDataBinFolder + "hud/Panneau1.pal").palette, i4: true);
				ExportNBFC("hud/Panneau2.bgc", 4, 4, new PAL(gameDataBinFolder + "hud/Panneau2.pal").palette, i4: true);
				ExportNBFC("hud/Panneau3.bgc", 4, 4, new PAL(gameDataBinFolder + "hud/Panneau3.pal").palette, i4: true);
				ExportNBFC("hud/Ray_Fnt_0.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Ray_Fnt_0.pal").palette, i4: true);
				ExportNBFC("hud/Ray_Fnt_1.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Ray_Fnt_0.pal").palette, i4: true);
				ExportNBFC("hud/Ray_Fnt_2.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Ray_Fnt_0.pal").palette, i4: true);
				ExportNBFC("hud/Ray_Fnt_3.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Ray_Fnt_0.pal").palette, i4: true);
				ExportNBFC("hud/Ray_Fnt_4.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Ray_Fnt_0.pal").palette, i4: true);
				ExportNBFC("hud/Ray_Fnt_5.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Ray_Fnt_0.pal").palette, i4: true);
				ExportNBFC("hud/Ray_Fnt_6.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Ray_Fnt_0.pal").palette, i4: true);
				ExportNBFC("hud/Ray_Fnt_7.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Ray_Fnt_0.pal").palette, i4: true);
				ExportNBFC("hud/Ray_Fnt_8.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Ray_Fnt_0.pal").palette, i4: true);
				ExportNBFC("hud/Ray_Fnt_9.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Ray_Fnt_0.pal").palette, i4: true);
				ExportNBFC("hud/Ray_Fnt_slash.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/Ray_Fnt_0.pal").palette, i4: true);
				ExportNBFC("hud/trophee.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/trophee.pal").palette, i4: true);
				ExportNBFC("hud/wifi_level_0.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/wifi_level_0.pal").palette, i4: true);
				ExportNBFC("hud/wifi_level_1.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/wifi_level_0.pal").palette, i4: true);
				ExportNBFC("hud/wifi_level_2.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/wifi_level_0.pal").palette, i4: true);
				ExportNBFC("hud/wifi_level_3.bgc", 2, 2, new PAL(gameDataBinFolder + "hud/wifi_level_0.pal").palette, i4: true);*/
			}
			if (Settings.s.platform == Settings.Platform._3DS) {
				// Stored separately
				for (int i = 1; i < 25; i++) {
					ExportEtcFile("LoadingAnimation/Course_" + i.ToString("D2") + ".etc", 64, 64, false);
				}
				ExportEtcFile("LoadingAnimation/home.etc", 64, 64, true);
				foreach (string file in Directory.EnumerateFiles(gameDataBinFolder + "/vignette")) {
					string fileName = file.Substring((gameDataBinFolder + "/vignette\\").Length);
					ExportEtcFile("vignette/" + fileName, 512, 256, false);
				}
			}
		}

		public T Get<T>(ushort index) where T : ROMStruct {
			FATEntry.Type type = FATEntry.types[typeof(T)];
			// Special treatment for EntryActions
			if (type == FATEntry.Type.EntryAction
				&& ((index & (ushort)FATEntry.Flag.Fix) != (ushort)FATEntry.Flag.Fix)) {
				ushort ind = (ushort)(index | (ushort)FATEntry.Flag.Fix);
				if (romStructs.ContainsKey(type) && romStructs[type].ContainsKey(ind)) {
					return romStructs[type][ind] as T;
				}
			}
			if (!romStructs.ContainsKey(type) || !romStructs[type].ContainsKey(index)) return null;
			return romStructs[type][index] as T;
		}

		public T GetOrRead<T>(Reader reader, ushort index, Action<T> onPreRead = null) where T : ROMStruct, new() {
			T rs = Get<T>(index);
			if (rs == null) {
				if (index != 0xFFFF) {
					FATEntry.Type type = FATEntry.types[typeof(T)];
					Pointer offset = null;
					// For some reason, this type receives special treatment
					if (type == FATEntry.Type.EntryAction
						&& ((index & (ushort)FATEntry.Flag.Fix) != (ushort)FATEntry.Flag.Fix)) {
						ushort fixIndex = (ushort)(index | (ushort)FATEntry.Flag.Fix);
						offset = GetStructPtr(type, fixIndex);
						if (offset != null) {
							index = fixIndex;
						}
					}
					if (offset == null) {
						offset = GetStructPtr(type, index);
					}
					if (offset != null) {
						if (!romStructs.ContainsKey(type)) {
							romStructs[type] = new Dictionary<ushort, ROMStruct>();
						}
						if (!romStructs[type].ContainsKey(index)) {
							rs = new T();
							rs.Init(offset, index);
							romStructs[type][index] = rs;
							onPreRead?.Invoke(rs);
							rs.Read(reader);
						} else {
							Debug.LogWarning("Duplicate index " + index + " for type " + type);
						}
					}
				}
			}
			return rs;
		}

		public void ExportEtcFile(string name, int w, int h, bool hasAlpha) {
			if (Settings.s.platform == Settings.Platform._3DS) {
				using (Reader reader = new Reader(FileSystem.GetFileReadStream(gameDataBinFolder + name))) {
					byte[] textureBytes = reader.ReadBytes((int)reader.BaseStream.Length);
					if (!File.Exists(gameDataBinFolder + "/textures/" + Path.GetDirectoryName(name) + "/" + Path.GetFileNameWithoutExtension(name) + ".png")) {
						Texture2D tex = new ETC(textureBytes, w, h, hasAlpha).texture;
						Util.ByteArrayToFile(gameDataBinFolder + "/textures/" + Path.GetDirectoryName(name) + "/" + Path.GetFileNameWithoutExtension(name) + ".png", tex.EncodeToPNG());
					}
				}
			}
		}
		public void ExportNBFC(string name, int w, int h, Color[] palette, bool i4 = false) {
			if (Settings.s.platform == Settings.Platform.DS) {
				if(FileSystem.FileExists(gameDataBinFolder + name)) {
					if (!File.Exists(gameDataBinFolder + "/textures/" + Path.GetDirectoryName(name) + "/" + Path.GetFileNameWithoutExtension(name) + ".png")) {
						Texture2D tex = new NBFC(gameDataBinFolder + name, w, h, palette, i4).texture;
						Util.ByteArrayToFile(gameDataBinFolder + "/textures/" + Path.GetDirectoryName(name) + "/" + Path.GetFileNameWithoutExtension(name) + ".png", tex.EncodeToPNG());
					}
				}
			}
		}
		public void ExportGFX(string name, string mapName, string palName, int w, int h) {
			if (Settings.s.platform == Settings.Platform.DS) {
				if (FileSystem.FileExists(gameDataBinFolder + name)) {
					//if (!File.Exists(gameDataBinFolder + "/textures/" + Path.GetDirectoryName(name) + "/" + Path.GetFileNameWithoutExtension(name) + ".png")) {
						Texture2D tex = new GFX(gameDataBinFolder + name, gameDataBinFolder + mapName, gameDataBinFolder + palName, w, h).texture;
						Util.ByteArrayToFile(gameDataBinFolder + "/textures/" + Path.GetDirectoryName(name) + "/" + Path.GetFileNameWithoutExtension(name) + ".png", tex.EncodeToPNG());
					//}
				}
			}
		}

		public FATEntry GetEntry(ushort type, ushort index, bool global = false) {
			type = (ushort)(type & 0x7FFF);
			index = (ushort)(index & 0x7FFF);

			FATEntry levelEntry = fatTables[CurrentLevel + 2].entries.FirstOrDefault(e => e.type == type && e.index == index);
			if (levelEntry != null) return levelEntry;

			FATEntry fix2Entry = fatTables[1].entries.FirstOrDefault(e => e.type == type && e.index == index);
			if (fix2Entry != null) return fix2Entry;

			FATEntry fixEntry = fatTables[0].entries.FirstOrDefault(e => e.type == type && e.index == index);
			if (fixEntry != null) return fixEntry;
			
			if (global) {
				for (int i = 2; i < fatTables.Length; i++) {
					if (i == CurrentLevel + 2) continue;
					FATEntry entry = fatTables[i].entries.FirstOrDefault(e => e.type == type && e.index == index);
					if (entry != null) return entry;
				}
			}

			return null;
		}

		public FATEntry GetEntry(FATEntry.Type type, ushort index, bool global = false) {
			bool isFix = (index & (ushort)FATEntry.Flag.Fix) == (ushort)FATEntry.Flag.Fix;
			ushort ind = (ushort)(index & 0x7FFF);
			if (!global) {
				if (!isFix) {
					FATEntry levelEntry = fatTables[CurrentLevel + 2].GetEntry(type, ind);
					if (levelEntry != null) return levelEntry;

					FATEntry fix2Entry = fatTables[1].GetEntry(type, ind);
					if (fix2Entry != null) return fix2Entry;
				} else {
					FATEntry fixEntry = fatTables[0].GetEntry(type, ind);
					if (fixEntry != null) return fixEntry;
				}
			} else {
				FATEntry levelEntry = fatTables[CurrentLevel + 2].GetEntry(type, ind);
				if (levelEntry != null) return levelEntry;

				FATEntry fix2Entry = fatTables[1].GetEntry(type, ind);
				if (fix2Entry != null) return fix2Entry;

				FATEntry fixEntry = fatTables[0].GetEntry(type, ind);
				if (fixEntry != null) return fixEntry;

				for (int i = 2; i < fatTables.Length; i++) {
					if (i == CurrentLevel + 2) continue;
					FATEntry entry = fatTables[i].GetEntry(type, ind);
					if (entry != null) return entry;
				}
			}

			return null;
		}

		public Pointer GetStructPtr(ushort type, ushort index, bool global = false) {
			FATEntry entry = GetEntry(type, index, global: global);
			if (entry != null) {
				return new Pointer(entry.off_data, files_array[SMem.Data]);
			} else {
				return null;
			}
		}

		public Pointer GetStructPtr(FATEntry.Type type, ushort index, bool global = false) {
			FATEntry entry = GetEntry(type, index, global: global);
			if (entry != null) {
				return new Pointer(entry.off_data, files_array[SMem.Data]);
			} else {
				return null;
			}
		}

		string[] rrrMapBackgrounds = new string[] {
			"Background_MM_Default",
			"Background_MM_R1",
			"Background_MM_R2",
			"Background_MM_A1",
			"Background_MM_A2",
			"Background_MM_A3",
			"Background_MM_B1",
			"Background_MM_B2",
			"Background_MM_B3",
			"Background_MM_C1",
			"Background_MM_C2",
			"Background_MM_C3",
			"Background_MM_D1",
			"Background_MM_D2",
			"bg_sfx_0A",
			"bg_sfx_0B",
			"bg_sfx_1A",
			"bg_sfx_1B",
			"bg_sfx_2A",
			"bg_sfx_2B",
			"bg_sfx_3A",
			"bg_sfx_3B",
			"Interface_Ray",
			"bg_MapE_2_2",
			"bg_MapE_1_1",
			"bg_loading0",
			"bg_loading1",
			"bg_loading2",
			"bg_loading3",
			"bg_loading4",
			"Interface",
			"Interface_Ray",
			"Interface_Gar",
			"Interface_RayGear",
			"bg_com",
			"bg_rayman",
			"bg_gardien",
			"bg_MM_Default",
		};

		string[] rrrPalettes = new string[] {
			"0.pal",
			"Jauge_2.pal",
			"Jauge_Plt_1.pal",
			"Circle_Icone.pal",
			"RM_Icone.pal",
			"XP_Icone.pal",
			"Icone_Missile.pal",
			"GT_Icone.pal",
			"GF_Icone.pal",
			"GV_Icone.pal",
			"GG_Icone.pal",
			"lums_01.pal",
			"Cage_Ico1.pal",
			"Wind_Icon1.pal",
			"Serpent_Icon.pal",
			"DG_Icone.pal",
			"lap1.pal",
			"lap2.pal",
			"MapTermine.pal",
			"Cage_Ico.pal",
			"Icone_Ok.pal",
			"MM_pastillejaune.pal",
			"one.pal",
			"trophe_chiffre.pal",
			"Icone_Ok_1.pal",
			"MM_cadna.pal",
		};

		RRRPalettedTextureReference[] rrrTexRefs = new RRRPalettedTextureReference[] {
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 1, "Jauge_8.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 1, "Jauge_2.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 1, "Jauge_R.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 1, "Jauge_Fond_8.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 1, "Jauge_Fond_R.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 1, "Jauge_Fond_L.bgc"),
			new RRRPalettedTextureReference(1, 8, 8, 0, 2, "Jauge_Plt_1.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 0, "slash.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 0, "0.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 0, "1.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 0, "2.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 0, "3.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 0, "4.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 0, "5.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 0, "6.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 0, "7.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 0, "8.bgc"),
			new RRRPalettedTextureReference(1, 8, 0x10, 0x8000, 0, "9.bgc"),
			new RRRPalettedTextureReference(6, 0x20, 0x20, 0x80000000, 4, "RM_Icone.bgc"),
			new RRRPalettedTextureReference(2, 0x20, 0x20, 0x80000000, 5, "XP_Icone.bgc"),
			new RRRPalettedTextureReference(1, 0x20, 0x20, 0x80000000, 6, "Icone_Missile.bgc"),
			new RRRPalettedTextureReference(1, 0x20, 0x20, 0x80000000, 4, "MissBB_Icone.bgc"),
			new RRRPalettedTextureReference(1, 0x40, 0x40, 0xC0000000, 3, "Circle_Icone.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 7, "GT_Icone.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 8, "GF_Icone.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 9, "GV_Icone.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 0xA, "GG_Icone.bgc"),
			new RRRPalettedTextureReference(1, 0x20, 0x20, 0x80000000, 0xB, "lums_01.bgc"),
			new RRRPalettedTextureReference(1, 0x20, 0x20, 0x80000000, 0xB, "lums_02.bgc"),
			new RRRPalettedTextureReference(1, 0x20, 0x20, 0x80000000, 0xB, "lums_03.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 0xC, "Cage_Ico.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 0xC, "Cage_Ico1.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 0xC, "Cage_Ico2.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 0xC, "Cage_Ico3.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 0xD, "Wind_Icon1.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 0xD, "Wind_Icon2.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 0xD, "Wind_Icon3.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 0xE, "Serpent_Icon.bgc"),
			new RRRPalettedTextureReference(1, 8, 8, 0, 3, "hud_Map_Ind.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 4, "RM_Icone_Map.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 0xC, "Cage_Ico_Map.bgc"),
			new RRRPalettedTextureReference(4, 0x20, 0x20, 0x80000000, 0xF, "DG_Icone.bgc"),
			new RRRPalettedTextureReference(2, 0x20, 0x40, 0xC0008000, 0x10, "lap1.bgc"),
			new RRRPalettedTextureReference(2, 0x20, 0x40, 0xC0008000, 0x11, "lap2.bgc"),
			new RRRPalettedTextureReference(8, 0x20, 0x20, 0x80000000, 0x12, "MapTermine.bgc"),
			new RRRPalettedTextureReference(8, 0x20, 0x20, 0x80000000, 0x13, "Cage_Ico.bgc"),
			new RRRPalettedTextureReference(8, 0x20, 0x20, 0x80000000, 0x14, "Icone_Ok.bgc"),
			new RRRPalettedTextureReference(8, 0x10, 0x10, 0x40000000, 0x15, "MM_pastillejaune.bgc"),
			new RRRPalettedTextureReference(8, 0x20, 0x20, 0x80000000, 0x16, "one.bgc"),
			new RRRPalettedTextureReference(8, 0x20, 0x20, 0x80000000, 0x16, "seven.bgc"),
			new RRRPalettedTextureReference(8, 0x20, 0x20, 0x80000000, 0x16, "twintyfive.bgc"),
			new RRRPalettedTextureReference(8, 0x20, 0x20, 0x80000000, 0x16, "fourtysix.bgc"),
			new RRRPalettedTextureReference(8, 0x20, 0x20, 0x80000000, 0x16, "nintyn.bgc"),
			new RRRPalettedTextureReference(8, 0x40, 0x40, 0xC0000000, 0x17, "trophe_chiffre.bgc"),
			new RRRPalettedTextureReference(8, 0x20, 0x20, 0x80000000, 0x18, "Icone_Ok_1.bgc"),
			new RRRPalettedTextureReference(8, 0x20, 0x20, 0x80000000, 4, "RM_Icone.bgc"),
			new RRRPalettedTextureReference(8, 0x20, 0x20, 0x80000000, 0x19, "MM_cadna.bgc")
		};

		private struct RRRPalettedTextureReference {
			public string name;
			public byte width;
			public byte height;
			public byte palette;
			public RRRPalettedTextureReference(uint unk, byte width, byte height, uint unk2, byte palette, string name) {
				this.name = name;
				this.width = width;
				this.height = height;
				this.palette = palette;
			}
		}
	}

	public static class SMem {
		public const int Data = 0;
		public const int Fat = 1;
		public const int Anims = 2;
		public const int ShAnims = 3;
		public const int A3dHead = 4;
		public const int CutTable = 5;
	}
}
