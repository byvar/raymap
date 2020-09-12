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
using OpenSpace.Cinematics;
using System.IO.Compression;
using lzo.net;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace OpenSpace.Loader {
	public class LWLoader : MapLoader {
		public PBT[] pbt = new PBT[2];
		public LMS lms;

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

				if (lvlName.EndsWith(".exe")) {
					if (!Settings.s.hasMemorySupport) throw new Exception("This game does not have memory support.");
					Settings.s.loadFromMemory = true;
					MemoryFile mem = new MemoryFile(lvlName);
					files_array[0] = mem;
					await WaitIfNecessary();
					await LoadMemory();
				} else {
					// Prepare folder names
					string fixFolder = gameDataBinFolder + ConvertCase("Fix/", Settings.CapsType.LevelFolder);
					string lvlFolder = gameDataBinFolder + ConvertCase(lvlName + "/", Settings.CapsType.LevelFolder);

					// Prepare paths
					paths["fix.lvl"] = fixFolder + ConvertCase("Fix.lvl", Settings.CapsType.LevelFile);
					paths["fix.ptr"] = fixFolder + ConvertCase("Fix.ptr", Settings.CapsType.LevelFile);
					paths["fix.pbt"] = fixFolder + ConvertCase("Fix.pbt", Settings.CapsType.LevelFile);
					paths["lvl.lvl"] = lvlFolder + ConvertCase(lvlName + ".lvl", Settings.CapsType.LevelFile);
					paths["lvl.ptr"] = lvlFolder + ConvertCase(lvlName + ".ptr", Settings.CapsType.LevelFile);
					paths["lvl.pbt"] = lvlFolder + ConvertCase(lvlName + ".pbt", Settings.CapsType.LevelFile);
					paths["lvl.lms"] = lvlFolder + ConvertCase(lvlName + ".lms", Settings.CapsType.LMFile);

					// Download files
					foreach (KeyValuePair<string, string> path in paths) {
						if (path.Value != null) await PrepareFile(path.Value);
					}

					lvlNames[Mem.Fix] = "fix";
					lvlPaths[Mem.Fix] = paths["fix.lvl"];
					ptrPaths[Mem.Fix] = paths["fix.ptr"];
					lvlNames[Mem.Lvl] = lvlName;
					lvlPaths[Mem.Lvl] = paths["lvl.lvl"];
					ptrPaths[Mem.Lvl] = paths["lvl.ptr"];

					for (int i = 0; i < lvlPaths.Length; i++) {
						if (lvlPaths[i] == null) continue;
						if (FileSystem.FileExists(lvlPaths[i])) {
							files_array[i] = new LVL(lvlNames[i], lvlPaths[i], i);
						}
					}
					ReadLargoLVL(Mem.Fix, fixFolder + ConvertCase("Fix.dmp", Settings.CapsType.LevelFile));
					ReadLargoLVL(Mem.Lvl, lvlFolder + ConvertCase(lvlName + ".dmp", Settings.CapsType.LevelFile));
					
					pbt[Mem.Fix] = ReadPBT(paths["fix.pbt"], fixFolder + ConvertCase("Fix_PBT.dmp", Settings.CapsType.LevelFile));
					pbt[Mem.Lvl] = ReadPBT(paths["lvl.pbt"], lvlFolder + ConvertCase(lvlName + "_PBT.dmp", Settings.CapsType.LevelFile));
					lms = ReadLMS(paths["lvl.lms"]);

					for (int i = 0; i < loadOrder.Length; i++) {
						int j = loadOrder[i];
						if (files_array[j] != null && FileSystem.FileExists(ptrPaths[j])) {
							((LVL)files_array[j]).ReadPTR(ptrPaths[j]);
						}
					}

					await LoadFIX();
					await LoadLVL();
				}
			} finally {
				for (int i = 0; i < files_array.Length; i++) {
					if (files_array[i] != null) {
						if (!(files_array[i] is MemoryFile)) files_array[i].Dispose();
					}
				}
				if (cnt != null) cnt.Dispose();
			}
			await WaitIfNecessary();
			InitModdables();
		}

		private void ReadLargoLVL(int index, string path) {
			files_array[index].GotoHeader();
			Reader reader = files_array[index].reader;
			reader.ReadUInt32();
			uint compressed = reader.ReadUInt32();
			uint decompressed = reader.ReadUInt32();
			string vignette = reader.ReadString(0x104);
			reader.ReadUInt32();
			byte[] decData = DecompressLargo(reader, compressed, decompressed);
			((LVL)files_array[index]).OverrideData(decData);
			if (FileSystem.mode != FileSystem.Mode.Web) {
				Util.ByteArrayToFile(path, decData);
			}
		}

		private PBT ReadPBT(string path, string dmpPath) {
			if (FileSystem.FileExists(path)) {
				using (Reader reader = new Reader(FileSystem.GetFileReadStream(path), Settings.s.IsLittleEndian)) {
					uint decompressed = reader.ReadUInt32();
					uint compressed = reader.ReadUInt32();
					byte[] decData = DecompressLargo(reader, compressed, decompressed);
					if (FileSystem.mode != FileSystem.Mode.Web) {
						Util.ByteArrayToFile(dmpPath, decData);
					}
					return new PBT(new MemoryStream(decData));
				}
			}
			return null;
		}
		private LMS ReadLMS(string path) {
			if (FileSystem.FileExists(path)) {
				LMS lms = new LMS(FileSystem.GetFileReadStream(path));
				if (lms != null && exportTextures) {
					string lvlFolder = gameDataBinFolder + ConvertCase(lvlName + "/", Settings.CapsType.LevelFolder);
					for (int i = 0; i < lms.Count; i++) {
						Util.ByteArrayToFile(lvlFolder + "textures/" + ConvertCase(lvlName + "_" + i + ".png", Settings.CapsType.LevelFile), lms.textures[i].EncodeToPNG());
					}
				}
				return lms;
			}
			return null;
		}

		#region FIX
		Pointer off_animBankFix;
		async UniTask LoadFIX() {
			textures = new TextureInfo[0];
			loadingState = "Loading fixed memory";
			await WaitIfNecessary();
			files_array[Mem.Fix].GotoHeader();
			Reader reader = files_array[Mem.Fix].reader;
			reader.ReadUInt32(); // Offset of languages
			byte num_lvlNames = reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			ReadLevelNames(reader, Pointer.Current(reader), num_lvlNames);
			if (Settings.s.platform == Settings.Platform.PC) {
				reader.ReadChars(0x1E);
				reader.ReadChars(0x1E); // two zero entries
			}
			string firstMapName = new string(reader.ReadChars(0x1E));
			byte num_languages_subtitles = reader.ReadByte();
			byte num_languages_voice = reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			print(Pointer.Current(reader));
			Pointer off_languages_subtitles = Pointer.Read(reader);
			Pointer off_languages_voice = Pointer.Read(reader);
			Pointer.DoAt(ref reader, off_languages_subtitles, () => {
				ReadLanguages(reader, off_languages_subtitles, num_languages_subtitles);
			});
			Pointer.DoAt(ref reader, off_languages_voice, () => {
				ReadLanguages(reader, off_languages_voice, num_languages_voice);
			});

			int sz_entryActions = 0xC0;

			reader.ReadBytes(sz_entryActions); // 3DOS_EntryActions
			reader.ReadUInt16();
			ushort num_matrices = reader.ReadUInt16();
			for (int i = 0; i < 4; i++) {
				reader.ReadBytes(0x101);
			}
			loadingState = "Loading input structure";
			await WaitIfNecessary();
			inputStruct = InputStructure.Read(reader, Pointer.Current(reader));
			foreach (EntryAction ea in inputStruct.entryActions) {
				print(ea.ToString());
			}
			reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt32();
			ushort num_unk2 = reader.ReadUInt16();
			reader.ReadUInt16();
			Pointer off_unk2 = Pointer.Read(reader);
			Pointer off_entryActions = Pointer.Read(reader);
			Pointer[] unkMatrices = new Pointer[2];
			for (int i = 0; i < 2; i++) {
				unkMatrices[i] = Pointer.Read(reader);
			}
			fonts = FromOffsetOrRead<FontStructure>(reader, Pointer.Current(reader), inline: true);

			Pointer off_matrices = Pointer.Read(reader);
			Pointer off_specialEntryAction = Pointer.Read(reader);
			Pointer off_identityMatrix = Pointer.Read(reader);
			Pointer off_unk = Pointer.Read(reader);
			reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadBytes(0xc8);
			reader.ReadUInt16();
			reader.ReadUInt16();
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			Pointer.Read(reader);
			Pointer off_haloTexture = Pointer.Read(reader);
			Pointer off_material1 = Pointer.Read(reader);
			Pointer off_material2 = Pointer.Read(reader);
			for (int i = 0; i < 10; i++) {
				reader.ReadBytes(0xcc);
			}
		}
		#endregion

		#region LVL
		async UniTask LoadLVL() {
			loadingState = "Loading level memory";
			await WaitIfNecessary();
			files_array[Mem.Lvl].GotoHeader();
			Reader reader = files_array[Mem.Lvl].reader;
			long totalSize = reader.BaseStream.Length;

			reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt32();


			Pointer.Read(reader);
			Pointer.Read(reader);

			reader.ReadString(0x1E);
			reader.ReadString(0x1E);

			//Pointer off_animBankLvl = null;
			loadingState = "Loading globals";
			await WaitIfNecessary();
			globals.off_transitDynamicWorld = null;
			globals.off_actualWorld = Pointer.Read(reader);
			globals.off_dynamicWorld = Pointer.Read(reader);
			globals.off_fatherSector = Pointer.Read(reader); // It is I, Father Sector.
			globals.off_firstSubMapPosition = Pointer.Read(reader);

			globals.num_always = reader.ReadUInt32();
			globals.spawnablePersos = LinkedList<Perso>.ReadHeader(reader, Pointer.Current(reader), LinkedList.Type.Double);
			Pointer.Read(reader);
			globals.off_always_reusableSO = Pointer.Read(reader); // There are (num_always) empty SuperObjects starting with this one.
			globals.off_always_reusableUnknown1 = Pointer.Read(reader); // (num_always) * 0x2c blocks
			globals.off_always_reusableUnknown2 = Pointer.Read(reader); // (num_always) * 0x4 blocks

			// Settings for perso in fix? Lights?
			Pointer.Read(reader);
			Pointer.Read(reader);
			Pointer.Read(reader);

			Pointer.Read(reader); // perso
			Pointer.Read(reader);
			Pointer off_unknown_first = Pointer.Read(reader);
			Pointer off_unknown_last = Pointer.Read(reader);
			uint num_unknown = reader.ReadUInt32();

			families = LinkedList<Family>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Double);

			Pointer off_alwaysActiveCharacters_first = Pointer.Read(reader);
			Pointer off_alwaysActiveCharacters_last = Pointer.Read(reader);
			uint num_alwaysActiveChars = reader.ReadUInt32();

			Pointer.Read(reader);
			reader.ReadUInt32();
			globals.off_camera = Pointer.Read(reader);
			reader.ReadUInt32();
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			//print(Pointer.Current(reader));

			Pointer.Read(reader);
			Pointer off_unk0_first = Pointer.Read(reader);
			Pointer off_unk0_last = Pointer.Read(reader);
			uint num_unk = reader.ReadUInt32();
			Pointer off_unk = Pointer.Read(reader);

			loadingState = "Loading level textures";
			await ReadTexturesLvl(reader, Pointer.Current(reader));

			Pointer.Read(reader); // maybe perso in fix
			reader.ReadUInt32();
			Pointer.Read(reader);
			uint num_soundMaterials = reader.ReadUInt32();
			Pointer off_soundMaterials = Pointer.Read(reader);
			Pointer off_unkBlocks = Pointer.Read(reader); // 3 blocks of 0xb4
			uint num_unkBlocks = reader.ReadUInt32();
			Pointer.Read(reader);
			reader.ReadUInt32();
			BoundingVolume.Read(reader, Pointer.Current(reader), BoundingVolume.Type.Box);
			reader.ReadUInt16();
			reader.ReadUInt16();
			Pointer.Read(reader);
			reader.ReadUInt32();
			uint num_ipo = reader.ReadUInt32(); // Entries with an IPO SO pointer and a mesh pointer, then a pointer to an empty offset. RLI table?
			Pointer off_ipo = Pointer.Read(reader);
			reader.ReadBytes(0x30);
			uint num_unkPtrs = reader.ReadUInt32();
			for (int i = 0; i < num_unkPtrs; i++) {
				Pointer.Read(reader);
			}
			reader.ReadBytes(0x10d8); // that's a lot of null bytes

			uint num_shadowDQ = reader.ReadUInt32();
			for (int i = 0; i < 21; i++) {
				Pointer.Read(reader);
			}
			uint num_shadowHQ = reader.ReadUInt32();
			for (int i = 0; i < 21; i++) {
				Pointer.Read(reader);
			}
			localization = FromOffsetOrRead<LocalizationStructure>(reader, Pointer.Current(reader), inline: true);
			//print("Yay " + Pointer.Current(reader));
			reader.ReadUInt16();
			reader.ReadUInt16();
			reader.ReadUInt32();
			uint num_lightmaps = reader.ReadUInt32();
			Pointer off_lightmapUVs = Pointer.Read(reader);
			Pointer.DoAt(ref reader, off_lightmapUVs, () => {
				off_lightmapUV = new Pointer[num_lightmaps];
				for (int i = 0; i < num_lightmaps; i++) {
					reader.ReadUInt32();
					reader.ReadByte();
					reader.ReadBytes(3);
					off_lightmapUV[i] = Pointer.Read(reader);
				}
			});
			reader.ReadByte();
			reader.ReadBytes(3);
			reader.ReadUInt32();
			reader.ReadUInt32();
			Pointer.Read(reader);
			Pointer.Read(reader);
			Pointer.Read(reader);

			// Parse actual world & always structure
			loadingState = "Loading families";
			await WaitIfNecessary();
			ReadFamilies(reader);
			loadingState = "Loading superobject hierarchy";
			await WaitIfNecessary();
			await ReadSuperObjects(reader);
			loadingState = "Loading always structure";
			await WaitIfNecessary();
			ReadAlways(reader);


			loadingState = "Filling in cross-references";
			await WaitIfNecessary();
			ReadCrossReferences(reader);
		}
		#endregion



		public byte[] DecompressLargo(Reader reader, uint compressed, uint decompressed) {
			byte[] decompressedData = new byte[decompressed];
			int dstByte = 0, srcByte = 1;
			byte zeroByte = reader.ReadByte();
			while (srcByte < compressed) {
				byte instruction = reader.ReadByte();
				srcByte++;
				int function = instruction >> 5;
				int arg = instruction & 0x3F;
				int numToCopy, offsetInBuf;
				//MapLoader.Loader.print(string.Format("{0:X8}", reader.BaseStream.Position-1) + " - Function: " + function + " or " + (instruction >> 3));
				switch (function) {
					case 0:
					case 1:
						//MapLoader.Loader.print("Copy");
						// Copy from src
						numToCopy = arg + 1;
						byte[] data = reader.ReadBytes(numToCopy);
						//Array.Resize(ref decompressedData, dstByte + numToCopy);
						Array.Copy(data, 0, decompressedData, dstByte, numToCopy);
						dstByte += numToCopy;
						srcByte += numToCopy;
						break;
					case 2:
					case 3:
						//MapLoader.Loader.print("Copy from DST");
						// Copy from dst
						numToCopy = (arg & 3) + 2;
						offsetInBuf = (arg >> 2) + 1;
						//Array.Resize(ref decompressedData, dstByte + numToCopy);
						for (int i = 0; i < numToCopy; i++) {
							decompressedData[dstByte] = decompressedData[dstByte - offsetInBuf];
							dstByte++;
						}
						break;
					case 4:
						//MapLoader.Loader.print("Zero byte");
						// Zero byte
						numToCopy = (arg & 0x1F) + 2;
						//Array.Resize(ref decompressedData, dstByte + numToCopy);
						for (int i = 0; i < numToCopy; i++) {
							decompressedData[dstByte] = zeroByte;
							dstByte++;
						}
						break;
					case 5:
						//MapLoader.Loader.print("Long copy");
						// Long copy from dst
						arg = (((int)(arg & 0x1F)) << 8) + reader.ReadByte();
						numToCopy = (arg & 0xF) + 3;
						offsetInBuf = (arg >> 4) + 1;
						//MapLoader.Loader.print(arg + " - " + numToCopy + " - " + offsetInBuf);
						//Util.ByteArrayToFile(MapLoader.Loader.gameDataBinFolder + "dec.bin1", decompressedData);
						//MapLoader.Loader.print(dstByte + " - " + offsetInBuf);
						//Array.Resize(ref decompressedData, dstByte + numToCopy);
						for (int i = 0; i < numToCopy; i++) {
							decompressedData[dstByte] = decompressedData[dstByte - offsetInBuf];
							dstByte++;
						}
						srcByte += 1;
						break;
					case 6:
						//MapLoader.Loader.print("Very Long Copy");
						// Very long copy from dst
						arg = (int)(arg & 0x1F) << 16;
						arg += reader.ReadByte() << 8;
						arg += reader.ReadByte();
						numToCopy = (arg & 0x7F) + 4;
						offsetInBuf = (arg >> 7) + 1;
						//MapLoader.Loader.print(dstByte + " - " + offsetInBuf);
						//Array.Resize(ref decompressedData, dstByte + numToCopy);
						for (int i = 0; i < numToCopy; i++) {
							decompressedData[dstByte] = decompressedData[dstByte - offsetInBuf];
							dstByte++;
						}
						srcByte += 2;
						break;
					case 7:
						//MapLoader.Loader.print("Longest Copy");
						// Longest copy from dst
						arg = (int)(arg & 0x1F) << 24;
						arg += reader.ReadByte() << 16;
						arg += reader.ReadByte() << 8;
						arg += reader.ReadByte();
						numToCopy = (arg & 0x1FF) + 5;
						offsetInBuf = (arg >> 9) + 1;
						//Array.Resize(ref decompressedData, dstByte + numToCopy);
						for (int i = 0; i < numToCopy; i++) {
							decompressedData[dstByte] = decompressedData[dstByte - offsetInBuf];
							dstByte++;
						}
						srcByte += 3;
						break;
					case 8: // All the following is just src bytes read
					case 9:
						//(*(_BYTE *)lz_src_curByte & 0x3F) + 2;
						break;
					case 10:
					case 11:
						// 1
						break;
					case 12:
						// 2
						break;
					case 13:
						// 3
						break;
					case 14:
						// 4
						break;
				}
			}
			return decompressedData;
		}

		public Texture2D GetLightmap(int index) {
			if (lms != null && lms.textures.Length > index) {
				Texture2D tex = lms.textures[index];
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