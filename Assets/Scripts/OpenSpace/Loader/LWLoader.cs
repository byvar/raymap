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
using BinarySerializer.Unity;

namespace OpenSpace.Loader {
	public class LWLoader : MapLoader {
		public PBT[] pbt = new PBT[2];
		public LMS lms;

		public string[] languages_voice;
		public string[] languages_voice_loc;

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
					if (!Legacy_Settings.s.hasMemorySupport) throw new Exception("This game does not have memory support.");
					Legacy_Settings.s.loadFromMemory = true;
					MemoryFile mem = new MemoryFile(lvlName);
					files_array[0] = mem;
					await WaitIfNecessary();
					await LoadMemory();
				} else {
					// Prepare folder names
					string fixFolder = gameDataBinFolder + ConvertCase("Fix/", Legacy_Settings.CapsType.LevelFolder);
					string lvlFolder = gameDataBinFolder + ConvertCase(lvlName + "/", Legacy_Settings.CapsType.LevelFolder);

					// Prepare paths
					paths["fix.lvl"] = fixFolder + ConvertCase("Fix.lvl", Legacy_Settings.CapsType.LevelFile);
					paths["fix.ptr"] = fixFolder + ConvertCase("Fix.ptr", Legacy_Settings.CapsType.LevelFile);
					paths["fix.pbt"] = fixFolder + ConvertCase("Fix.pbt", Legacy_Settings.CapsType.LevelFile);
					paths["lvl.lvl"] = lvlFolder + ConvertCase(lvlName + ".lvl", Legacy_Settings.CapsType.LevelFile);
					paths["lvl.ptr"] = lvlFolder + ConvertCase(lvlName + ".ptr", Legacy_Settings.CapsType.LevelFile);
					paths["lvl.pbt"] = lvlFolder + ConvertCase(lvlName + ".pbt", Legacy_Settings.CapsType.LevelFile);
					paths["lvl.lms"] = lvlFolder + ConvertCase(lvlName + ".lms", Legacy_Settings.CapsType.LMFile);

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
					ReadLargoLVL(Mem.Fix, fixFolder + ConvertCase("Fix.dmp", Legacy_Settings.CapsType.LevelFile));
					ReadLargoLVL(Mem.Lvl, lvlFolder + ConvertCase(lvlName + ".dmp", Legacy_Settings.CapsType.LevelFile));
					
					pbt[Mem.Fix] = ReadPBT(paths["fix.pbt"], fixFolder + ConvertCase("Fix_PBT.dmp", Legacy_Settings.CapsType.LevelFile));
					pbt[Mem.Lvl] = ReadPBT(paths["lvl.pbt"], lvlFolder + ConvertCase(lvlName + "_PBT.dmp", Legacy_Settings.CapsType.LevelFile));
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
				using (Reader reader = new Reader(FileSystem.GetFileReadStream(path), Legacy_Settings.s.IsLittleEndian)) {
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
					string lvlFolder = gameDataBinFolder + ConvertCase(lvlName + "/", Legacy_Settings.CapsType.LevelFolder);
					for (int i = 0; i < lms.Count; i++) {
						Util.ByteArrayToFile(lvlFolder + "textures/" + ConvertCase(lvlName + "_" + i + ".png", Legacy_Settings.CapsType.LevelFile), lms.textures[i].EncodeToPNG());
					}
				}
				return lms;
			}
			return null;
		}



		public void ReadLanguagesVoice(Reader reader, LegacyPointer off_languages, uint num_languages) {
			languages_voice = new string[num_languages];
			languages_voice_loc = new string[num_languages];
			for (uint i = 0; i < num_languages; i++) {
				languages_voice[i] = reader.ReadString(0x14);
				languages_voice_loc[i] = reader.ReadString(0x14);
				//print(languages[i] + " - " + languages_loc[i]);
			}
		}

		#region FIX
		LegacyPointer off_animBankFix;
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
			ReadLevelNames(reader, LegacyPointer.Current(reader), num_lvlNames);
			if (Legacy_Settings.s.platform == Legacy_Settings.Platform.PC) {
				reader.ReadChars(0x1E);
				reader.ReadChars(0x1E); // two zero entries
			}
			string firstMapName = new string(reader.ReadChars(0x1E));
			byte num_languages_subtitles = reader.ReadByte();
			byte num_languages_voice = reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			print(LegacyPointer.Current(reader));
			LegacyPointer off_languages_subtitles = LegacyPointer.Read(reader);
			LegacyPointer off_languages_voice = LegacyPointer.Read(reader);
			LegacyPointer.DoAt(ref reader, off_languages_subtitles, () => {
				ReadLanguages(reader, off_languages_subtitles, num_languages_subtitles);
			});
			LegacyPointer.DoAt(ref reader, off_languages_voice, () => {
				ReadLanguagesVoice(reader, off_languages_voice, num_languages_voice);
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
			inputStruct = InputStructure.Read(reader, LegacyPointer.Current(reader));
			foreach (EntryAction ea in inputStruct.entryActions) {
				print(ea.ToString());
			}
			reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt32();
			ushort num_unk2 = reader.ReadUInt16();
			reader.ReadUInt16();
			LegacyPointer off_unk2 = LegacyPointer.Read(reader);
			LegacyPointer off_entryActions = LegacyPointer.Read(reader);
			LegacyPointer[] unkMatrices = new LegacyPointer[2];
			for (int i = 0; i < 2; i++) {
				unkMatrices[i] = LegacyPointer.Read(reader);
			}
			fonts = FromOffsetOrRead<FontStructure>(reader, LegacyPointer.Current(reader), inline: true);

			LegacyPointer off_matrices = LegacyPointer.Read(reader);
			LegacyPointer off_specialEntryAction = LegacyPointer.Read(reader);
			LegacyPointer off_identityMatrix = LegacyPointer.Read(reader);
			LegacyPointer off_unk = LegacyPointer.Read(reader);
			reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadBytes(0xc8);
			reader.ReadUInt16();
			reader.ReadUInt16();
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			LegacyPointer.Read(reader);
			LegacyPointer off_haloTexture = LegacyPointer.Read(reader);
			LegacyPointer off_material1 = LegacyPointer.Read(reader);
			LegacyPointer off_material2 = LegacyPointer.Read(reader);
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


			LegacyPointer.Read(reader);
			LegacyPointer.Read(reader);

			reader.ReadString(0x1E);
			reader.ReadString(0x1E);

			//Pointer off_animBankLvl = null;
			loadingState = "Loading globals";
			await WaitIfNecessary();
			globals.off_transitDynamicWorld = null;
			globals.off_actualWorld = LegacyPointer.Read(reader);
			globals.off_dynamicWorld = LegacyPointer.Read(reader);
			globals.off_fatherSector = LegacyPointer.Read(reader); // It is I, Father Sector.
			globals.off_firstSubMapPosition = LegacyPointer.Read(reader);

			globals.num_always = reader.ReadUInt32();
			globals.spawnablePersos = LinkedList<Perso>.ReadHeader(reader, LegacyPointer.Current(reader), LinkedList.Type.Double);
			LegacyPointer.Read(reader);
			globals.off_always_reusableSO = LegacyPointer.Read(reader); // There are (num_always) empty SuperObjects starting with this one.
			globals.off_always_reusableUnknown1 = LegacyPointer.Read(reader); // (num_always) * 0x2c blocks
			globals.off_always_reusableUnknown2 = LegacyPointer.Read(reader); // (num_always) * 0x4 blocks

			// Settings for perso in fix? Lights?
			LegacyPointer.Read(reader);
			LegacyPointer.Read(reader);
			LegacyPointer.Read(reader);

			LegacyPointer.Read(reader); // perso
			LegacyPointer.Read(reader);
			LegacyPointer off_unknown_first = LegacyPointer.Read(reader);
			LegacyPointer off_unknown_last = LegacyPointer.Read(reader);
			uint num_unknown = reader.ReadUInt32();

			families = LinkedList<Family>.ReadHeader(reader, LegacyPointer.Current(reader), type: LinkedList.Type.Double);

			LegacyPointer off_alwaysActiveCharacters_first = LegacyPointer.Read(reader);
			LegacyPointer off_alwaysActiveCharacters_last = LegacyPointer.Read(reader);
			uint num_alwaysActiveChars = reader.ReadUInt32();

			LegacyPointer.Read(reader);
			reader.ReadUInt32();
			globals.off_camera = LegacyPointer.Read(reader);
			reader.ReadUInt32();
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			//print(Pointer.Current(reader));

			LegacyPointer.Read(reader);
			LegacyPointer off_unk0_first = LegacyPointer.Read(reader);
			LegacyPointer off_unk0_last = LegacyPointer.Read(reader);
			uint num_unk = reader.ReadUInt32();
			LegacyPointer off_unk = LegacyPointer.Read(reader);

			loadingState = "Loading level textures";
			await ReadTexturesLvl(reader, LegacyPointer.Current(reader));

			LegacyPointer.Read(reader); // maybe perso in fix
			reader.ReadUInt32();
			LegacyPointer.Read(reader);
			uint num_soundMaterials = reader.ReadUInt32();
			LegacyPointer off_soundMaterials = LegacyPointer.Read(reader);
			LegacyPointer off_unkBlocks = LegacyPointer.Read(reader); // 3 blocks of 0xb4
			uint num_unkBlocks = reader.ReadUInt32();
			LegacyPointer.Read(reader);
			reader.ReadUInt32();
			BoundingVolume.Read(reader, LegacyPointer.Current(reader), BoundingVolume.Type.Box);
			reader.ReadUInt16();
			reader.ReadUInt16();
			LegacyPointer.Read(reader);
			reader.ReadUInt32();
			uint num_ipo = reader.ReadUInt32(); // Entries with an IPO SO pointer and a mesh pointer, then a pointer to an empty offset. RLI table?
			LegacyPointer off_ipo = LegacyPointer.Read(reader);
			reader.ReadBytes(0x30);
			uint num_unkPtrs = reader.ReadUInt32();
			for (int i = 0; i < num_unkPtrs; i++) {
				LegacyPointer.Read(reader);
			}
			reader.ReadBytes(0x10d8); // that's a lot of null bytes

			uint num_shadowDQ = reader.ReadUInt32();
			for (int i = 0; i < 21; i++) {
				LegacyPointer.Read(reader);
			}
			uint num_shadowHQ = reader.ReadUInt32();
			for (int i = 0; i < 21; i++) {
				LegacyPointer.Read(reader);
			}
			localization = FromOffsetOrRead<LocalizationStructure>(reader, LegacyPointer.Current(reader), inline: true);
			//print("Yay " + Pointer.Current(reader));
			reader.ReadUInt16();
			reader.ReadUInt16();
			reader.ReadUInt32();
			uint num_lightmaps = reader.ReadUInt32();
			LegacyPointer off_lightmapUVs = LegacyPointer.Read(reader);
			LegacyPointer.DoAt(ref reader, off_lightmapUVs, () => {
				off_lightmapUV = new LegacyPointer[num_lightmaps];
				for (int i = 0; i < num_lightmaps; i++) {
					reader.ReadUInt32();
					reader.ReadByte();
					reader.ReadBytes(3);
					off_lightmapUV[i] = LegacyPointer.Read(reader);
				}
			});
			reader.ReadByte();
			reader.ReadBytes(3);
			reader.ReadUInt32();
			reader.ReadUInt32();
			LegacyPointer.Read(reader);
			LegacyPointer.Read(reader);
			LegacyPointer.Read(reader);

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