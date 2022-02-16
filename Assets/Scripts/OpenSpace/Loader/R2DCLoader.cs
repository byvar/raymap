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
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using BinarySerializer.Unity;

namespace OpenSpace.Loader {
    public class R2DCLoader : MapLoader {
		protected override async UniTask Load() {
            try {
                if (gameDataBinFolder == null || gameDataBinFolder.Trim().Equals("")) throw new Exception("GAMEDATABIN folder doesn't exist");
                if (lvlName == null || lvlName.Trim() == "") throw new Exception("No level name specified!");
                globals = new Globals();
				gameDataBinFolder += "/";
				await FileSystem.CheckDirectory(gameDataBinFolder);
				if (!FileSystem.DirectoryExists(gameDataBinFolder)) throw new Exception("GAMEDATABIN folder doesn't exist");
                loadingState = "Initializing files";
				await MapLoader.WaitIfNecessary();
				await CreateCNT();

                // FIX
                string fixDATPath = gameDataBinFolder + "FIX.DAT";
                texPaths[0] = gameDataBinFolder + "FIX.TEX";
                await PrepareFile(fixDATPath);
                await PrepareFile(texPaths[0]);
                DCDAT fixDAT = new DCDAT("fix", fixDATPath, 0);

                // LEVEL
                string lvlDATPath = gameDataBinFolder + lvlName + "/" + lvlName + ".DAT";
                texPaths[1] = gameDataBinFolder + lvlName + "/" + lvlName + ".TEX";
                await PrepareFile(lvlDATPath);
                await PrepareFile(texPaths[1]);
                DCDAT lvlDAT = new DCDAT(lvlName, lvlDATPath, 1);

                files_array[0] = fixDAT;
                files_array[1] = lvlDAT;

                await LoadDreamcast();

				if (Legacy_Settings.s.game == Legacy_Settings.Game.R2) {
					string logPathTexFix = gameDataBinFolder + "TEXTURE_FIX.LOG";
					string logPathTexLvl = gameDataBinFolder + lvlName + "/TEXTURE_" + lvlName + ".LOG";
					string logPathInfo = gameDataBinFolder + lvlName + "/INFO.LOG";
					/*yield return controller.StartCoroutine(PrepareFile(logPathTexFix));
					yield return controller.StartCoroutine(PrepareFile(logPathTexLvl));*/
					await PrepareFile(logPathInfo);
					if (FileSystem.FileExists(logPathInfo)) {
						ReadLog(FileSystem.GetFileReadStream(logPathInfo));
						await WaitIfNecessary();
					}
					/*if (FileSystem.FileExists(logPathTexFix)) {
						ReadLog(logPathTexFix);
						await WaitIfNecessary();
					}
					if (FileSystem.FileExists(logPathTexLvl)) {
						ReadLog(logPathTexLvl);
						await WaitIfNecessary();
					}*/
				} else if (Legacy_Settings.s.game == Legacy_Settings.Game.DD) {
					string backgroundPath = gameDataBinFolder + ConvertCase(lvlName, Legacy_Settings.CapsType.LevelFolder) + "/FOND.PVR";
					await PrepareFile(backgroundPath);
					if (FileSystem.FileExists(backgroundPath)) {
						TEX backgroundTexFile = new TEX(backgroundPath, compressed: false);
						globals.backgroundGameMaterial = new GameMaterial(null) {
							visualMaterial = new VisualMaterial(null) {
								textures = new List<VisualMaterialTexture>() {
									new VisualMaterialTexture() {
										texture = new TextureInfo(null) {
											width = (ushort)backgroundTexFile.textures[0].width,
											height = (ushort)backgroundTexFile.textures[0].height,
											Texture = backgroundTexFile.textures[0]
										}
									}
								},
								ambientCoef = new Vector4(1,1,1,1),
								diffuseCoef = new Vector4(1,1,1,1),
								specularCoef = new Vector4(1,1,1,1),
								num_textures = 1,
								receivedHints = VisualMaterial.Hint.Billboard
							}
						};
					}
				}

				fixDAT.Dispose();
                lvlDAT.Dispose();
            } finally {
                for (int i = 0; i < files_array.Length; i++) {
                    if (files_array[i] != null) {
                        files_array[i].Dispose();
                    }
                }
                if (cnt != null) cnt.Dispose();
			}
			await MapLoader.WaitIfNecessary();
			InitModdables();
        }

        #region Dreamcast
        async UniTask LoadDreamcast() {
            textures = new TextureInfo[0];

            loadingState = "Loading fixed memory";
            await WaitIfNecessary();
            files_array[Mem.Fix].GotoHeader();
            Reader reader = files_array[Mem.Fix].reader;
            LegacyPointer off_base_fix = LegacyPointer.Current(reader);
            uint base_language = reader.ReadUInt32(); //Pointer off_language = Pointer.Read(reader);
            reader.ReadUInt32();
            uint num_text_language = reader.ReadUInt32();
            reader.ReadUInt16();
            reader.ReadUInt16();
            reader.ReadUInt32(); // base
            LegacyPointer off_text_general = LegacyPointer.Read(reader);
            localization = FromOffsetOrRead<LocalizationStructure>(reader, off_text_general);
            LegacyPointer off_inputStructure = LegacyPointer.Read(reader);
            LegacyPointer.DoAt(ref reader, off_inputStructure, () => {
                inputStruct = InputStructure.Read(reader, off_inputStructure);
				foreach (EntryAction ea in inputStruct.entryActions) {
					print(ea.ToString());
				}
			});

            await WaitIfNecessary();
            LegacyPointer.Read(reader);
            LegacyPointer.Read(reader);
            LegacyPointer.Read(reader);
            LegacyPointer.Read(reader);
            LegacyPointer.Read(reader);
            LegacyPointer.Read(reader);
            LegacyPointer.Read(reader);
            LegacyPointer.Read(reader);
            LegacyPointer.Read(reader);
            LegacyPointer.Read(reader);
            LegacyPointer.Read(reader);
            LegacyPointer.Read(reader);
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            LegacyPointer.Read(reader);
            LegacyPointer off_levelNames = LegacyPointer.Read(reader);
            LegacyPointer off_languages = LegacyPointer.Read(reader);
            uint num_levelNames = reader.ReadUInt32();
            uint num_languages = reader.ReadUInt32();
            reader.ReadUInt32(); // same as num_levelNames
            LegacyPointer.DoAt(ref reader, off_levelNames, () => {
                lvlNames = new string[num_levelNames];
                for (uint i = 0; i < num_levelNames; i++) {
                    lvlNames[i] = reader.ReadString(0x1E);
                }
            });
            LegacyPointer.DoAt(ref reader, off_languages, () => {
                ReadLanguages(reader, off_languages, num_languages);
            });
            if (languages != null && localization != null) {
                for (int i = 0; i < num_languages; i++) {
                    loadingState = "Loading text files: " + (i+1) + "/" + num_languages;
                    string langFilePath = gameDataBinFolder + "TEXTS/" + languages[i].ToUpper() + ".LNG";
                    await PrepareFile(langFilePath);
                    files_array[2] = new DCDAT(languages[i], langFilePath, 2);
                    ((DCDAT)files_array[2]).SetHeaderOffset(base_language);
                    files_array[2].GotoHeader();
                    localization.ReadLanguageTableDreamcast(files_array[2].reader, i, (ushort)num_text_language);
                    files_array[2].Dispose();
                }
            }
        
            loadingState = "Loading fixed textures";
            await WaitIfNecessary();
            LegacyPointer off_events_fix = LegacyPointer.Read(reader);
            uint num_events_fix = reader.ReadUInt32();
            uint num_textures_fix = reader.ReadUInt32();
            LegacyPointer off_textures_fix = LegacyPointer.Read(reader);
            LegacyPointer.DoAt(ref reader, off_textures_fix, () => {
                Array.Resize(ref textures, (int)num_textures_fix);
                for (uint i = 0; i < num_textures_fix; i++) {
                    LegacyPointer off_texture = LegacyPointer.Read(reader);
                    textures[i] = null;
                    LegacyPointer.DoAt(ref reader, off_texture, () => {
                        textures[i] = TextureInfo.Read(reader, off_texture);
                    });
                }
                TEX tex = new TEX(texPaths[0]);
                for (uint i = 0; i < num_textures_fix; i++) {
                    if (textures[i] != null && tex.Count > i) {
                        textures[i].Texture = tex.textures[i];
                    }
                }
            });
            loadingState = "Loading level memory";
            await WaitIfNecessary();
            files_array[Mem.Lvl].GotoHeader();
            reader = files_array[Mem.Lvl].reader;

            // Animation stuff
            LegacyPointer off_animationBank = LegacyPointer.Current(reader);
            LegacyPointer.Read(reader);
            LegacyPointer.Read(reader);
            LegacyPointer.Read(reader);
            LegacyPointer.Read(reader);
            LegacyPointer.Read(reader);
            LegacyPointer.Read(reader);
            LegacyPointer.Read(reader);
            LegacyPointer.Read(reader);
            LegacyPointer.Read(reader);
            LegacyPointer.Read(reader);
            LegacyPointer.Read(reader);
            reader.ReadUInt32();
            LegacyPointer.Read(reader);

            // Globals
            globals.off_actualWorld = LegacyPointer.Read(reader);
            globals.off_dynamicWorld = LegacyPointer.Read(reader);
            globals.off_inactiveDynamicWorld = LegacyPointer.Read(reader);
            globals.off_fatherSector = LegacyPointer.Read(reader);
            reader.ReadUInt32();
            LegacyPointer off_always = LegacyPointer.Read(reader);
            LegacyPointer.DoAt(ref reader, off_always, () => {
                globals.num_always = reader.ReadUInt32();
                globals.spawnablePersos = LinkedList<Perso>.ReadHeader(reader, LegacyPointer.Current(reader), LinkedList.Type.Double);
                globals.spawnablePersos.FillPointers(reader, globals.spawnablePersos.off_tail, globals.spawnablePersos.offset);
                globals.off_always_reusableSO = LegacyPointer.Read(reader); // There are (num_always) empty SuperObjects starting with this one.
            });
            LegacyPointer.Read(reader);
            LegacyPointer off_objectTypes = LegacyPointer.Read(reader);
            LegacyPointer.DoAt(ref reader, off_objectTypes, () => {
                // Fill in pointers for the object type tables and read them
                objectTypes = new ObjectType[3][];
                for (uint i = 0; i < 3; i++) {
                    LegacyPointer off_names_header = LegacyPointer.Current(reader);
                    LegacyPointer off_names_first = LegacyPointer.Read(reader);
                    LegacyPointer off_names_last = LegacyPointer.Read(reader);
                    uint num_names = reader.ReadUInt32();

                    FillLinkedListPointers(reader, off_names_last, off_names_header);
                    ReadObjectNamesTable(reader, off_names_first, num_names, i);
                }
            });
            LegacyPointer.Read(reader);
            LegacyPointer off_mainChar = LegacyPointer.Read(reader);
            reader.ReadUInt32();
            uint num_persoInFixPointers = reader.ReadUInt32();
            LegacyPointer off_persoInFixPointers = LegacyPointer.Read(reader);

            //Pointer[] persoInFixPointers = new Pointer[num_persoInFixPointers];
            LegacyPointer.DoAt(ref reader, off_persoInFixPointers, () => {
                for (int i = 0; i < num_persoInFixPointers; i++) {
                    LegacyPointer off_perso = LegacyPointer.Read(reader);
                    LegacyPointer off_so = LegacyPointer.Read(reader);
                    byte[] unk = reader.ReadBytes(4);
                    LegacyPointer off_matrix = LegacyPointer.Current(reader); // It's better to change the pointer instead of the data as that is reused in some places
                    byte[] matrixData = reader.ReadBytes(0x68);
                    byte[] soFlags = reader.ReadBytes(4);
                    byte[] brothersAndParent = reader.ReadBytes(12);

                    LegacyPointer.DoAt(ref reader, off_perso, () => {
                        reader.ReadUInt32();
                        LegacyPointer off_stdGame = LegacyPointer.Read(reader);
                        if (off_stdGame != null && off_so != null) {
                            ((DCDAT)off_stdGame.file).OverwriteData(off_stdGame.FileOffset + 0xC, off_so.offset);
                        }
                    });
                    if (off_so != null) {
                        ((DCDAT)off_so.file).OverwriteData(off_so.FileOffset + 0x14, brothersAndParent);
                        ((DCDAT)off_so.file).OverwriteData(off_so.FileOffset + 0x20, off_matrix.offset);
                        ((DCDAT)off_so.file).OverwriteData(off_so.FileOffset + 0x30, soFlags);
                    }
                }
            });

            await WaitIfNecessary();
            LegacyPointer.Read(reader); // contains a pointer to the camera SO
            LegacyPointer off_cameras = LegacyPointer.Read(reader); // Double linkedlist of cameras
            LegacyPointer off_families = LegacyPointer.Read(reader);
            LegacyPointer.DoAt(ref reader, off_families, () => {
                families = LinkedList<Family>.ReadHeader(reader, LegacyPointer.Current(reader), type: LinkedList.Type.Double);
                families.FillPointers(reader, families.off_tail, families.off_head);
            });
            LegacyPointer.Read(reader); // At this pointer: a double linkedlist of fix perso's with headers (soptr, next, prev, hdr)
            LegacyPointer.Read(reader); // Rayman
            reader.ReadUInt32();
            LegacyPointer.Read(reader); // Camera
            reader.ReadUInt32();
            reader.ReadUInt32();

            loadingState = "Loading level textures";
            await WaitIfNecessary();
            uint num_textures_lvl = reader.ReadUInt32();
            uint num_textures_total = num_textures_fix + num_textures_lvl;
            LegacyPointer off_textures_lvl = LegacyPointer.Read(reader);
            LegacyPointer.DoAt(ref reader, off_textures_lvl, () => {
                Array.Resize(ref textures, (int)num_textures_total);
                for (uint i = num_textures_fix; i < num_textures_total; i++) {
                    LegacyPointer off_texture = LegacyPointer.Read(reader);
                    textures[i] = null;
                    LegacyPointer.DoAt(ref reader, off_texture, () => {
                        textures[i] = TextureInfo.Read(reader, off_texture);
                    });
                }
                TEX tex = new TEX(texPaths[1]);
                for (uint i = 0; i < num_textures_lvl; i++) {
                    if (textures[num_textures_fix + i] != null && tex.Count > i) {
                        textures[num_textures_fix + i].Texture = tex.textures[i];
                    }
                }
            });

            loadingState = "Loading families";
            await WaitIfNecessary();
            ReadFamilies(reader);
            loadingState = "Loading animation banks";
            await WaitIfNecessary();
            LegacyPointer.DoAt(ref reader, off_animationBank, () => {
                animationBanks = new AnimationBank[2];
                animationBanks[0] = AnimationBank.ReadDreamcast(reader, off_animationBank, off_events_fix, num_events_fix);
                animationBanks[1] = animationBanks[0];
            });
            loadingState = "Loading superobject hierarchy";
            await WaitIfNecessary();
            await ReadSuperObjects(reader);
            loadingState = "Loading always structure";
            await WaitIfNecessary();
            ReadAlways(reader);
            loadingState = "Filling in cross-references";
            await WaitIfNecessary();
            ReadCrossReferences(reader);
			loadingState = "Loading behavior copies";
			await WaitIfNecessary();
			ReadBehaviorCopies(reader);
			await WaitIfNecessary();
			/*print("Sectors: " + sectors.Count);
			for (int i = 0; i < sectors.Count; i++) {
				print("Sector " + i + "\t" + sectors[i].persos.Count + "\t" + sectors[i].staticLights.Count);
			}
			print("Persos: " + persos.Count);
			print("World: " + superObjects.Count(so => so.type == SuperObject.Type.World));
			print("IPOs: " + superObjects.Count(so => so.type == SuperObject.Type.IPO || so.type == SuperObject.Type.IPO_2));
			print("POs: " + superObjects.Count(so => so.type == SuperObject.Type.PO));
			print("SOs: " + superObjects.Count);
			print("Families: " + families.Count);
			print("Always: " + globals.num_always + " - Spawnables: " + globals.spawnablePersos.Count);*/

			// Parse transformation matrices and other settings for fix characters
			/*if (off_mainChar != null && off_matrix_mainChar != null) {
                SuperObject so = SuperObject.FromOffset(off_mainChar);
                Pointer.DoAt(ref reader, off_matrix_mainChar, () => {
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    Pointer off_matrix = Pointer.Current(reader);
                    Matrix mat = Matrix.Read(reader, off_matrix);
                    if (so != null) {
                        so.off_matrix = off_matrix;
                        so.matrix = mat;
                        if (so.Gao != null) {
                            so.Gao.transform.localPosition = mat.GetPosition(convertAxes: true);
                            so.Gao.transform.localRotation = mat.GetRotation(convertAxes: true);
                            so.Gao.transform.localScale = mat.GetScale(convertAxes: true);
                        }
                    }
                });
            }*/
		}
        #endregion

        private void ReadLog(Stream logStream) {
            string logPattern = @"^0x(?<offset>[a-fA-F0-9]+?) : \((?<type>[^\)]+?)\)(?<name>.*?)    0x(?<offset2>[a-fA-F0-9]+?)$";
			string comportNamePattern = @"^(?<family>[^\\]+?)\\(?<model>[^\\]+?)\\(?<model2>[^\\]+?)\.(?<type>...?)\^.*$";
			DCDAT file = files_array[0] as DCDAT;
            using (StreamReader sr = new StreamReader(logStream)) {
                while (sr.Peek() >= 0) {
                    string line = sr.ReadLine().Trim();
                    Match logMatch = Regex.Match(line, logPattern, RegexOptions.IgnoreCase);
                    if (logMatch.Success) {
                        string offsetStr = logMatch.Groups["offset"].Value;
                        string type = logMatch.Groups["type"].Value;
                        string name = logMatch.Groups["name"].Value;
                        string offset2Str = logMatch.Groups["offset2"].Value;
                        if (offsetStr.Length < 8) offsetStr = new String('0', offsetStr.Length - 8) + offsetStr;
                        uint offsetUint = Convert.ToUInt32(offsetStr, 16);
                        LegacyPointer offset = file.GetUnsafePointer(offsetUint);
                        switch (type) {
                            case "eST_Comport":
                                Behavior b = FromOffset<Behavior>(offset);
								if (b == null) {
									Match comportMatch = Regex.Match(name, comportNamePattern, RegexOptions.IgnoreCase);
									if (comportMatch.Success) {
										string modelName = comportMatch.Groups["model"].Value;
										AIModel aiModel = aiModels.FirstOrDefault(ai => ai.name == modelName);
										Reader reader = files_array[Mem.Fix].reader;
										LegacyPointer.DoAt(ref reader, offset, () => {
											Behavior newB = FromOffsetOrRead<Behavior>(reader, offset);
											if (aiModel != null && newB != null) {
												switch (comportMatch.Groups["type"].Value) {
													case "rul":
														foreach (Behavior originalBehavior in aiModel.behaviors_normal) {
															if (newB.ContentEquals(originalBehavior)) {
																originalBehavior.copies.Add(newB.Offset);
																b = originalBehavior;
																break;
															}
														}
														break;
													case "rfx":
														foreach (Behavior originalBehavior in aiModel.behaviors_reflex) {
															if (newB.ContentEquals(originalBehavior)) {
																originalBehavior.copies.Add(newB.Offset);
																b = originalBehavior;
																break;
															}
														}
														break;
												}
											}
										});
									}
								}
                                if (b != null) b.name = name;
                                /*if (name.Contains("piranha\\MIC_PiranhaSauteurVisible\\MIC_PiranhaSauteurVisible")) {
                                    print("Offset: " + offset + " - " + b);
                                }*/
                                break;
                            case "eST_State":
                                State state = State.FromOffset(offset);
                                if (state != null) state.name = name;
                                break;
                            case "eST_Anim3d":
                                AnimationReference ar = FromOffset<AnimationReference>(offset);
                                if (ar != null) ar.name = name;
                                break;
                            case "eST_Graph":
                                Graph g = Graph.FromOffset(offset);
                                if (g != null) g.name = name;
                                break;
                            case "eST_Sector":
                                Sector s = Sector.FromOffset(offset);
                                if (s != null) {
                                    s.name = name;
                                    s.Gao.name = name;
                                }
                                break;
                        }
                    }
                }
            }
        }

		#region Behavior copies
		private void ReadBehaviorCopies(Reader reader) {
			// DC has loose behaviors that are copies of existing behaviors
			foreach (AIModel ai in aiModels) {
				if (ai.behaviors_normal != null) {
					for (int i = 0; i < ai.behaviors_normal.Length; i++) {
						if (ai.behaviors_normal[i].scripts != null) {
							for (int j = 0; j < ai.behaviors_normal[i].scripts.Length; j++) {
								List<ScriptNode> nodes = ai.behaviors_normal[i].scripts[j].scriptNodes;
								foreach (ScriptNode node in nodes) {
									if (node.param_ptr != null && node.nodeType == ScriptNode.NodeType.ComportRef) {
										Behavior b = FromOffset<Behavior>(node.param_ptr);
										if (b == null) {
											LegacyPointer.DoAt(ref reader, node.param_ptr, () => {
												ReadBehaviorCopy(reader, node.param_ptr, ai);
											});
										}
									}
								}
							}
						}
					}
				}
				if (ai.behaviors_reflex != null) {
					for (int i = 0; i < ai.behaviors_reflex.Length; i++) {
						if (ai.behaviors_reflex[i].scripts != null) {
							for (int j = 0; j < ai.behaviors_reflex[i].scripts.Length; j++) {
								List<ScriptNode> nodes = ai.behaviors_reflex[i].scripts[j].scriptNodes;
								foreach (ScriptNode node in nodes) {
									if (node.param_ptr != null && node.nodeType == ScriptNode.NodeType.ComportRef) {
										Behavior b = FromOffset<Behavior>(node.param_ptr);
										if (b == null) {
											LegacyPointer.DoAt(ref reader, node.param_ptr, () => {
												ReadBehaviorCopy(reader, node.param_ptr, ai);
											});
										}
									}
								}
							}
						}
					}
				}
			}
		}
		private void ReadBehaviorCopy(Reader reader, LegacyPointer offset, AIModel ai) {
            Behavior b = FromOffsetOrRead<Behavior>(reader, offset);
			if (b != null && ai != null && ai.behaviors_normal != null) {
				foreach (Behavior originalBehavior in ai.behaviors_normal) {
					if (b.ContentEquals(originalBehavior)) {
						originalBehavior.copies.Add(b.Offset);
						b = null;
						break;
					}
				}
			}
			if (b != null && ai != null && ai.behaviors_reflex != null) {
				foreach (Behavior originalBehavior in ai.behaviors_reflex) {
					if (b.ContentEquals(originalBehavior)) {
						originalBehavior.copies.Add(b.Offset);
						b = null;
						break;
					}
				}
			}
			if (b != null && behaviors != null) {
				foreach (Behavior originalBehavior in behaviors) {
					if (b.ContentEquals(originalBehavior)) {
						originalBehavior.copies.Add(b.Offset);
						b = null;
						break;
					}
				}
			}
		}
		#endregion
	}
}
