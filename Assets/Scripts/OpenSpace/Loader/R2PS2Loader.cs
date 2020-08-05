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
using OpenSpace.FileFormat.RenderWare;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace OpenSpace.Loader {
    public class R2PS2Loader : MapLoader {
		public List<TextureDictionary> txds = new List<TextureDictionary>();
		public MeshFile ato;
		public Pointer[] off_materials;
		public uint meshesRead = 0;
		public VisualMaterial lightCookieMaterial;
		public Color[] lightCookieColors;

		protected override async UniTask Load() {
            try {
                if (gameDataBinFolder == null || gameDataBinFolder.Trim().Equals("")) throw new Exception("GAMEDATABIN folder doesn't exist");
                if (lvlName == null || lvlName.Trim() == "") throw new Exception("No level name specified!");
                globals = new Globals();
				gameDataBinFolder += "/";
				await FileSystem.CheckDirectory(gameDataBinFolder);
				if (!FileSystem.DirectoryExists(gameDataBinFolder)) throw new Exception("GAMEDATABIN folder doesn't exist");
                loadingState = "Initializing files";
                await CreateCNT();


				// Prepare folder names
				string fixFolder = gameDataBinFolder + ConvertCase("Fix/", Settings.CapsType.LevelFolder);
				string lvlFolder = gameDataBinFolder + ConvertCase(lvlName + "/", Settings.CapsType.LevelFolder);
				
				// Prepare paths
				paths["fix.lv2"] = fixFolder + ConvertCase("Fix.lv2", Settings.CapsType.LevelFile);
				paths["fix.pt2"] = fixFolder + ConvertCase("Fix.pt2", Settings.CapsType.LevelFile);
				paths["lvl.lv2"] = lvlFolder + ConvertCase(lvlName + ".lv2", Settings.CapsType.LevelFile);
				paths["lvl.pt2"] = lvlFolder + ConvertCase(lvlName + ".pt2", Settings.CapsType.LevelFile);
				paths["lvl.ato.0"] = lvlFolder + ConvertCase(lvlName + ".ato.0", Settings.CapsType.LevelFile);
				paths["lvl.rw3.0"] = lvlFolder + ConvertCase(lvlName + ".rw3.0", Settings.CapsType.LevelFile);
				paths["lvl.lm3.0"] = lvlFolder + ConvertCase(lvlName + ".lm3.0", Settings.CapsType.LevelFile);
				paths["lvl.lm3.1"] = lvlFolder + ConvertCase(lvlName + ".lm3.1", Settings.CapsType.LevelFile);
				paths["lvl.lm3.2"] = lvlFolder + ConvertCase(lvlName + ".lm3.2", Settings.CapsType.LevelFile);
				paths["lvl.dmo"] = lvlFolder + ConvertCase(lvlName + ".dmo", Settings.CapsType.LevelFile);

				// Download files
				foreach (KeyValuePair<string, string> path in paths) {
					if (path.Value != null) await PrepareFile(path.Value);
				}

				loadingState = "Loading textures";
				await WaitIfNecessary();
				txds.Add(new TextureDictionary(paths["lvl.rw3.0"]));
				loadingState = "Loading lightmaps";
				await WaitIfNecessary();
				if (FileSystem.FileExists(paths["lvl.lm3.0"])) {
					txds.Add(new TextureDictionary(paths["lvl.lm3.0"]));
				}
				if (FileSystem.FileExists(paths["lvl.lm3.1"])) {
					txds.Add(new TextureDictionary(paths["lvl.lm3.1"]));
				}
				loadingState = "Loading geometry";
				await WaitIfNecessary();
				ato = new MeshFile(paths["lvl.ato.0"]);

				loadingState = "Loading level files";
				await WaitIfNecessary();
				LVL fix = new LVL("fix", paths["fix.lv2"], 0);
				LVL lvl = new LVL(lvlName, paths["lvl.lv2"], 1);
				files_array[0] = fix;
				files_array[1] = lvl;
				fix.ReadPTR(paths["fix.pt2"]);
				lvl.ReadPTR(paths["lvl.pt2"]);
				await LoadPS2();
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

        #region PS2
        async UniTask LoadPS2() {
			await WaitIfNecessary();
            textures = new TextureInfo[0];

            loadingState = "Loading fixed memory";
            await WaitIfNecessary();
            files_array[Mem.Fix].GotoHeader();
            Reader reader = files_array[Mem.Fix].reader;
            Pointer off_base_fix = Pointer.Current(reader);

			loadingState = "Loading input struct";
			await WaitIfNecessary();
			for (int i = 0; i < Settings.s.numEntryActions; i++) {
				Pointer.Read(reader); // 3DOS_EntryActions
			}

			inputStruct = InputStructure.Read(reader, Pointer.Current(reader));
			foreach (EntryAction ea in inputStruct.entryActions) {
				print(ea.ToString());
			}
			localization = FromOffsetOrRead<LocalizationStructure>(reader, Pointer.Current(reader), inline: true);

			/*
            Pointer off_inputStructure = Pointer.Read(reader);
            Pointer.DoAt(ref reader, off_inputStructure, () => {
                inputStruct = InputStructure.Read(reader, off_inputStructure);
				foreach (EntryAction ea in inputStruct.entryActions) {
					print(ea.ToString());
				}
			});*/


			/*uint base_language = reader.ReadUInt32(); //Pointer off_language = Pointer.Read(reader);
            reader.ReadUInt32();
            uint num_text_language = reader.ReadUInt32();
            reader.ReadUInt16();
            reader.ReadUInt16();
            reader.ReadUInt32(); // base
            Pointer off_text_general = Pointer.Read(reader);
            Pointer.DoAt(ref reader, off_text_general, () => {
                fontStruct = FontStructure.Read(reader, off_text_general);
            });
            Pointer off_inputStructure = Pointer.Read(reader);
            Pointer.DoAt(ref reader, off_inputStructure, () => {
                inputStruct = InputStructure.Read(reader, off_inputStructure);
				foreach (EntryAction ea in inputStruct.entryActions) {
					print(ea.ToString());
				}
			});

            await WaitIfNecessary();
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
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
            Pointer.Read(reader);
            Pointer off_levelNames = Pointer.Read(reader);
            Pointer off_languages = Pointer.Read(reader);
            uint num_levelNames = reader.ReadUInt32();
            uint num_languages = reader.ReadUInt32();
            reader.ReadUInt32(); // same as num_levelNames
            Pointer.DoAt(ref reader, off_levelNames, () => {
                lvlNames = new string[num_levelNames];
                for (uint i = 0; i < num_levelNames; i++) {
                    lvlNames[i] = reader.ReadString(0x1E);
                }
            });
            Pointer.DoAt(ref reader, off_languages, () => {
                ReadLanguages(reader, off_languages, num_languages);
            });
            if (languages != null && fontStruct != null) {
                for (int i = 0; i < num_languages; i++) {
                    loadingState = "Loading text files: " + (i+1) + "/" + num_languages;
                    string langFilePath = gameDataBinFolder + "TEXTS/" + languages[i].ToUpper() + ".LNG";
                    await PrepareFile(langFilePath));
                    files_array[2] = new DCDAT(languages[i], langFilePath, 2);
                    ((DCDAT)files_array[2]).SetHeaderOffset(base_language);
                    files_array[2].GotoHeader();
                    fontStruct.ReadLanguageTableDreamcast(files_array[2].reader, i, (ushort)num_text_language);
                    files_array[2].Dispose();
                }
            }
        
            loadingState = "Loading fixed textures";
            await WaitIfNecessary();
            Pointer off_events_fix = Pointer.Read(reader);
            uint num_events_fix = reader.ReadUInt32();
            uint num_textures_fix = reader.ReadUInt32();
            Pointer off_textures_fix = Pointer.Read(reader);
            Pointer.DoAt(ref reader, off_textures_fix, () => {
                Array.Resize(ref textures, (int)num_textures_fix);
                for (uint i = 0; i < num_textures_fix; i++) {
                    Pointer off_texture = Pointer.Read(reader);
                    textures[i] = null;
                    Pointer.DoAt(ref reader, off_texture, () => {
                        textures[i] = TextureInfo.Read(reader, off_texture);
                    });
                }
                TEX tex = new TEX(tplPaths[0]);
                for (uint i = 0; i < num_textures_fix; i++) {
                    if (textures[i] != null && tex.Count > i) {
                        textures[i].Texture = tex.textures[i];
                    }
                }
            });*/
			loadingState = "Loading level memory";
            await WaitIfNecessary();
            files_array[Mem.Lvl].GotoHeader();
            reader = files_array[Mem.Lvl].reader;
			string build = reader.ReadString(0x20);
			reader.ReadUInt32();
			reader.ReadUInt32(); // 0xc
			reader.ReadUInt32(); // 0
			Pointer.Read(reader);
			Pointer.Read(reader);

            // Globals
            globals.off_actualWorld = Pointer.Read(reader);
            globals.off_dynamicWorld = Pointer.Read(reader);
            globals.off_fatherSector = Pointer.Read(reader);
            globals.num_always = reader.ReadUInt32();
            globals.spawnablePersos = LinkedList<Perso>.ReadHeader(reader, Pointer.Current(reader), LinkedList.Type.Double);
			//globals.spawnablePersos.FillPointers(reader, globals.spawnablePersos.off_tail, globals.spawnablePersos.offset);
			Pointer.Read(reader); // format: (0x4 number, number * 0x4: null)
			globals.off_always_reusableSO = Pointer.Read(reader); // There are (num_always) empty SuperObjects starting with this one.
			Pointer.Read(reader);
			Pointer.Read(reader);
			Pointer.Read(reader);
			Pointer.Read(reader);
			Pointer.Read(reader);

			LinkedList<Perso> cameras = LinkedList<Perso>.ReadHeader(reader, Pointer.Current(reader), LinkedList.Type.Double);
			families = LinkedList<Family>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Double);
			LinkedList<Perso> mainChars = LinkedList<Perso>.ReadHeader(reader, Pointer.Current(reader), LinkedList.Type.Double);
            Pointer.Read(reader); // Rayman
            reader.ReadUInt32();
            globals.off_camera = Pointer.Read(reader); // Camera
			Pointer.Read(reader);
			Pointer.Read(reader);
			Pointer.Read(reader);

			uint numMeshes = (uint)files_array[Mem.Lvl].extraData["numMeshes"];
			uint numMaterials = (uint)files_array[Mem.Lvl].extraData["numMaterials"];
			uint numTextures = (uint)files_array[Mem.Lvl].extraData["numTextures"];
			uint numLightmappedObjects = (uint)files_array[Mem.Lvl].extraData["numLightmappedObjects"];
			//print("numTextures " + numTextures + " - " + txds[0].Count + " - " + numMeshes + " - " + ato.numAtomics + " - " + numLightmappedObjects);


			textures = new TextureInfo[numTextures];
			Pointer[] off_meshes = new Pointer[numMeshes];
			off_materials = new Pointer[numMaterials];
			for (int i = 0; i < numMeshes; i++) {
				off_meshes[i] = Pointer.Read(reader);
			}
			for (int i = 0; i < numMaterials; i++) {
				off_materials[i] = Pointer.Read(reader);
			}
			for (int i = 0; i < numTextures; i++) {
				Pointer off_textureInfo = Pointer.Read(reader);
				int texture_index = reader.ReadInt32();
				Pointer.DoAt(ref reader, off_textureInfo, () => {
					textures[i] = TextureInfo.Read(reader, off_textureInfo);
					textures[i].Texture = txds[0].Lookup(texture_index.ToString("D3"));
					//textures[i].Texture = txds[0].textures[txds[0].Count - 1 - texture_index];
				});
			}
			Pointer.Read(reader);
			reader.ReadUInt32();
			reader.ReadUInt32();
			Pointer.Read(reader);
			uint num_unk = reader.ReadUInt32();
			for (int i = 0; i < num_unk; i++) {
				Pointer.Read(reader);
			}
			uint num_unk2 = reader.ReadUInt32();
			for (int i = 0; i < num_unk2; i++) {
				Pointer.Read(reader);
			}
			Pointer.Read(reader);
			reader.ReadSingle(); // a bounding volume most likely
			reader.ReadSingle();
			reader.ReadSingle();
			reader.ReadSingle();
			reader.ReadSingle();
			reader.ReadSingle();
			Pointer.Read(reader);
			reader.ReadUInt32(); // 2?
			uint num_poTable = reader.ReadUInt32();
			Pointer off_poTable = Pointer.Read(reader);
			reader.ReadUInt32(); // 1. 10x 0
			reader.ReadUInt32(); // 2
			reader.ReadUInt32(); // 3
			reader.ReadUInt32(); // 4
			reader.ReadUInt32(); // 5
			reader.ReadUInt32(); // 6
			reader.ReadUInt32(); // 7
			reader.ReadUInt32(); // 8
			reader.ReadUInt32(); // 9
			reader.ReadUInt32(); // 10
			uint num_lightCookies = reader.ReadUInt32();
			lightCookieColors = new Color[num_lightCookies];
			for (int i = 0; i < num_lightCookies; i++) {
				reader.ReadUInt32();
				reader.ReadUInt32();
				reader.ReadUInt32();
				reader.ReadUInt32();
				byte b = reader.ReadByte();
				byte g = reader.ReadByte();
				byte r = reader.ReadByte();
				reader.ReadByte();
				lightCookieColors[i] = new Color(r / 255f, g / 255f, b / 255f, 1f);
				reader.ReadUInt32();
				reader.ReadInt32();
				reader.ReadUInt32();
			}
			for (int i = 0; i < num_lightCookies; i++) {
				reader.ReadByte();
			}
			reader.Align(0x4);
			Pointer off_lightCookieMaterial = Pointer.Read(reader);
			lightCookieMaterial = VisualMaterial.FromOffsetOrRead(off_lightCookieMaterial, reader);
			off_lightmapUV = new Pointer[numLightmappedObjects];
			for (int i = 0; i < numLightmappedObjects; i++) {
				reader.ReadUInt32();
				reader.ReadUInt32();
				reader.ReadUInt32();
				off_lightmapUV[i] = Pointer.Read(reader);
			}



			for (int i = 0; i < numMaterials; i++) {
				VisualMaterial.FromOffsetOrRead(off_materials[i], reader);
			}
			for (int i = 0; i < numMeshes; i++) {
				Pointer.DoAt(ref reader, off_meshes[i], () => {
					GeometricObject mesh = GeometricObject.Read(reader, off_meshes[i]);
					meshObjects.Add(mesh);
					//print("Mesh " + i + ": " + mesh.num_vertices + " - " + mesh.subblock_types[0] + " - " + mesh.num_subblocks);
				});
			}

			loadingState = "Loading families";
            await WaitIfNecessary();
            ReadFamilies(reader);
			//print("Families: " + families.Count);
            loadingState = "Loading superobject hierarchy";
            await WaitIfNecessary();
            ReadSuperObjects(reader);
            loadingState = "Loading always structure";
            await WaitIfNecessary();
            ReadAlways(reader);
            loadingState = "Filling in cross-references";
            await WaitIfNecessary();
            ReadCrossReferences(reader);
			await WaitIfNecessary();
        }
		#endregion

		public Texture2D GetLightmap(string name) {
			for (int i = 1; i < txds.Count; i++) {
				Texture2D tex = txds[i].Lookup(name);
				if (tex != null) {
					tex.filterMode = FilterMode.Bilinear;
					tex.wrapMode = TextureWrapMode.Clamp;
					return tex;
				}
			}
			return null;
		}
	}
}
