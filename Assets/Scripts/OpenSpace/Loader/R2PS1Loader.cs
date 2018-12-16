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

namespace OpenSpace.Loader {
    public class R2PS1Loader : MapLoader {
        public override IEnumerator Load() {
            try {
                if (gameDataBinFolder == null || gameDataBinFolder.Trim().Equals("")) throw new Exception("GAMEDATABIN folder doesn't exist");
                if (lvlName == null || lvlName.Trim() == "") throw new Exception("No level name specified!");
                globals = new Globals();
				gameDataBinFolder += "/";
				yield return controller.StartCoroutine(FileSystem.CheckDirectory(gameDataBinFolder));
				if (!FileSystem.DirectoryExists(gameDataBinFolder)) throw new Exception("GAMEDATABIN folder doesn't exist");
                loadingState = "Initializing files";
				byte[] data = new byte[0];
				/*using (Reader reader = new Reader(FileSystem.GetFileReadStream(gameDataBinFolder + lvlName))) {
					reader.BaseStream.Position = 0x64a2a0;
					while (reader.BaseStream.Position > 0x000E1804) {
						uint num_bytes = 0;
						while (reader.BaseStream.Position > 0x000E1804) {
							uint comprSize = 0;
							while (reader.BaseStream.Position > 0x000E1804) {
								reader.BaseStream.Position -= 5;
								num_bytes++;
								comprSize = reader.ReadUInt32();
								if (comprSize == num_bytes) break;
							}
							// Check if decomp size > compr size
							reader.BaseStream.Position -= 8;
							uint decomprSize = reader.ReadUInt32();
							if (decomprSize > comprSize && decomprSize < 0x01000000) {
								break;
							} else {
								reader.BaseStream.Position += 4;
							}
						}
						// Match
						reader.BaseStream.Position -= 4;
						print(num_bytes + " - " + String.Format("0x{0:X8}", reader.BaseStream.Position));
						reader.BaseStream.Position -= 4;
						if (reader.ReadUInt32() == 0) reader.BaseStream.Position -= 4;
						yield return null;
					}
				}*/
				/*using (Reader reader = new Reader(FileSystem.GetFileReadStream(gameDataBinFolder + lvlName))) {
					bool previousWasZero = false;
					bool previousWasFF = false;
					//reader.BaseStream.Position = 0x1199b7bd;
					while (reader.BaseStream.Position < reader.BaseStream.Length) {
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
								print(decompressedSize + " - " + String.Format("0x{0:X8}", reader.BaseStream.Position));
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
							print(decompressedSize + " - " + String.Format("0x{0:X8}", reader.BaseStream.Position));
							byte[] uncompressedData = null;
							byte[] compressedData = reader.ReadBytes((int)compressedSize);
							using (var compressedStream = new MemoryStream(compressedData))
							using (var lzo = new LzoStream(compressedStream, CompressionMode.Decompress))
							using (Reader lzoReader = new Reader(lzo, Settings.s.IsLittleEndian)) {
								lzo.SetLength(decompressedSize);
								uncompressedData = lzoReader.ReadBytes((int)decompressedSize);
							}
							if (uncompressedData != null) {
								int originalDataLength = data.Length;
								Array.Resize(ref data, originalDataLength + uncompressedData.Length);
								Array.Copy(uncompressedData, 0, data, originalDataLength, uncompressedData.Length);
							}
						}
						
					}
				}
				if (data.Length > 0) {
					Util.ByteArrayToFile(gameDataBinFolder + lvlName + ".dmp", data);
				}*/
					/*yield return controller.StartCoroutine(CreateCNT());

					// FIX
					string fixDATPath = gameDataBinFolder + "FIX.DAT";
					tplPaths[0] = gameDataBinFolder + "FIX.TEX";
					yield return controller.StartCoroutine(PrepareFile(fixDATPath));
					yield return controller.StartCoroutine(PrepareFile(tplPaths[0]));
					DCDAT fixDAT = new DCDAT("fix", fixDATPath, 0);

					// LEVEL
					string lvlDATPath = gameDataBinFolder + lvlName + "/" + lvlName + ".DAT";
					tplPaths[1] = gameDataBinFolder + lvlName + "/" + lvlName + ".TEX";
					yield return controller.StartCoroutine(PrepareFile(lvlDATPath));
					yield return controller.StartCoroutine(PrepareFile(tplPaths[1]));
					DCDAT lvlDAT = new DCDAT(lvlName, lvlDATPath, 1);

					files_array[0] = fixDAT;
					files_array[1] = lvlDAT;

					string logPathTexFix = gameDataBinFolder + "TEXTURE_FIX.LOG";
					string logPathTexLvl = gameDataBinFolder + lvlName + "/TEXTURE_" + lvlName + ".LOG";
					string logPathInfo = gameDataBinFolder + lvlName + "/INFO.LOG";
					/*yield return controller.StartCoroutine(PrepareFile(logPathTexFix));
					yield return controller.StartCoroutine(PrepareFile(logPathTexLvl));*/


					/*fixDAT.Dispose();
					lvlDAT.Dispose();*/
				} finally {
                for (int i = 0; i < files_array.Length; i++) {
                    if (files_array[i] != null) {
                        files_array[i].Dispose();
                    }
                }
                if (cnt != null) cnt.Dispose();
            }
            yield return null;
            InitModdables();
        }
    }
}
