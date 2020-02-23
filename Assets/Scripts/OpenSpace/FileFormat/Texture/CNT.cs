// Adapted from Rayman2Lib by szymski
// https://github.com/szymski/Rayman2Lib/blob/master/csharp_tools/Rayman2Lib/CNTFile.cs

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.FileFormat.Texture {
    public class CNT : IDisposable {
        public class FileStruct {
            public string name;
            public string directory;
            public int pointer;
            public int size;
            public byte[] xorKey;
            public uint checksum;
            public int fileNum;

            public string FullName {
                get {
                    if (directory != null && directory != "") {
                        return directory + "\\" + name;
                    } else {
                        return name;
                    }
                }
            }

            public string TGAName {
                get {
                    string fullName = FullName;
                    return fullName.Replace(".gf", ".tga");
                }
            }
        }


        public string[][] directoryList = null;
        public List<FileStruct> fileList = new List<FileStruct>();
        int directoryCount = 0;
        int fileCount = 0;



        Reader[] readers = null;


        bool isLittleEndian = true;
        //uint count = 0;

        public uint Count {
            get { return (uint)fileCount; }
        }

        public Texture2D[] textures = null;

        public CNT(string path) : this(FileSystem.GetFileReadStream(path)) { }

        public CNT(string[] paths) {
            directoryList = new string[paths.Length][];
            readers = new Reader[paths.Length];
            for (int i = 0; i < paths.Length; i++) {
				readers[i] = new Reader(FileSystem.GetFileReadStream(paths[i]), isLittleEndian);
            }
        }

		public void SetCacheSize(int size) {
			for (int i = 0; i < readers.Length; i++) {
				if (readers[i] != null && (readers[i].BaseStream as PartialHttpStream) != null) {
					((PartialHttpStream)readers[i].BaseStream).SetCacheLength(size);
				}
			}
		}

        public CNT(Stream stream) {
            directoryList = new string[1][];
            readers = new Reader[1];
        }

		public async Task Init() {
			for (int i = 0; i < readers.Length; i++) {
				await Init(i, readers[i]);
			}
		}

        public async Task Init(int readerIndex, Reader reader) {
			PartialHttpStream httpStream = reader.BaseStream as PartialHttpStream;
			if(httpStream != null) await httpStream.FillCacheForRead(11);
            int localDirCount = reader.ReadInt32();
            int localFileCount = reader.ReadInt32();
            directoryCount += localDirCount;
            fileCount += localFileCount;
            directoryList[readerIndex] = new string[localDirCount];
            byte isXor = reader.ReadByte();
            byte isChecksum = reader.ReadByte();
            byte xorKey = reader.ReadByte();
            byte curChecksum = 0;
			// Load directories
			//Debug.Log("directories");
			if (httpStream != null) await httpStream.FillCacheForRead(300 * localDirCount);
			await MapLoader.WaitIfNecessary();
			for (int i = 0; i < localDirCount; i++) {
				//if (httpStream != null) yield return c.StartCoroutine(httpStream.FillCacheForRead(4));
				int strLen = reader.ReadInt32();
                string directory = "";

                if (isXor != 0 || isChecksum != 0) {
                    for (int j = 0; j < strLen; j++) {
                        byte b = reader.ReadByte();
                        if (isXor != 0) {
                            b = (byte)(xorKey ^ b);
                        }
                        if (isChecksum != 0) {
                            curChecksum = (byte)((curChecksum + b) % 256);
                        }
                        directory += (char)b;
                    }
                } else {
                    directory = reader.ReadString(strLen);
                }

                directoryList[readerIndex][i] = directory;
            }

            // Load and check version
            //if (httpStream != null) yield return c.StartCoroutine(httpStream.FillCacheForRead(1));
            byte directoryChecksum = reader.ReadByte();
            if (directoryChecksum != curChecksum) {
                Debug.LogWarning("CNT: Directory checksum failed");
            }

			// Read files
			//Debug.Log("files");
			if (httpStream != null) await httpStream.FillCacheForRead(300 * localFileCount);
			for (int i = 0; i < localFileCount; i++) {
				//if (httpStream != null) yield return c.StartCoroutine(httpStream.FillCacheForRead(8));
				int dirIndex = reader.ReadInt32();
                int strLen = reader.ReadInt32();

                string file = "";

                //if (httpStream != null) yield return c.StartCoroutine(httpStream.FillCacheForRead(size + 16));
                if (isXor != 0) {
                    for (int j = 0; j < strLen; j++) {
                        byte b = reader.ReadByte();
                        b = (byte)(xorKey ^ b);
                        file += (char)b;
                    }
                } else {
                    file = reader.ReadString(strLen);
                }

                byte[] fileXorKey = new byte[4];
                reader.Read(fileXorKey, 0, 4);

                uint fileChecksum = reader.ReadUInt32();

                int dataPointer = reader.ReadInt32();
                int fileSize = reader.ReadInt32();

                string dir = dirIndex != -1 ? directoryList[readerIndex][dirIndex] : "";
                
                fileList.Add(new FileStruct() {
                    directory = dir,
                    name = file,
                    pointer = dataPointer,
                    size = fileSize,
                    xorKey = fileXorKey,
                    checksum = fileChecksum,
                    fileNum = readerIndex
                });
			}
		}



        public byte[] GetFileBytes(FileStruct file) {
            if (file == null) return null;
			
            readers[file.fileNum].BaseStream.Seek(file.pointer, SeekOrigin.Begin);

            byte[] data = new byte[file.size];
            readers[file.fileNum].Read(data, 0, data.Length);

			for (int i = 0; i < file.size; i++) {
                if ((file.size % 4) + i < file.size)
                    data[i] = (byte)(data[i] ^ file.xorKey[i % 4]);
            }
            //MonoBehaviour.print(file.pointer + " - " + file.magic2 + " - " + file.name + " - " + data.Length);

            return data;
        }

        public GF GetGF(string filename) {
            FileStruct file = fileList.FirstOrDefault(f => f.FullName == filename);
			return GetGF(file);
        }

		public GF GetGF(FileStruct file) {
			if (file == null) return null;
			return new GF(GetFileBytes(file));
		}

        public GF GetGFByTGAName(string tgaName) {
            FileStruct file = fileList.FirstOrDefault(f => f.TGAName.ToLower().Replace('/', '\\').Equals(tgaName.ToLower().Replace('/','\\')));
            if (file == null) return null;
            byte[] bytes = GetFileBytes(file);
            //Util.ByteArrayToFile("textures/" + file.FullName, bytes);
            GF gf = new GF(bytes);
            return gf;
        }

		public async Task<GF> PrepareGFByTGAName(string tgaName) {
			FileStruct file = fileList.FirstOrDefault(f => f.TGAName.ToLower().Replace('/', '\\').Equals(tgaName.ToLower().Replace('/', '\\')));
			if (file == null) {
				return null;
			}
			Reader reader = readers[file.fileNum];
			PartialHttpStream httpStream = reader.BaseStream as PartialHttpStream;
			if (httpStream != null) {
				Controller c = MapLoader.Loader.controller;
				readers[file.fileNum].BaseStream.Seek(file.pointer, SeekOrigin.Begin);
				await httpStream.FillCacheForRead(file.size);
			}
			byte[] bytes = GetFileBytes(file);
			//Util.ByteArrayToFile("textures/" + file.FullName, bytes);
			return new GF(bytes);
		}

        public void Dispose() {
            for (int i = 0; i < readers.Length; i++) {
                readers[i].Close();
            }
        }

        public class SaveFileStruct {
            public string name;
            public string dir;
            public byte[] data;
        }

        static string XORString(string str, byte key) {
            string newStr = "";

            for (int i = 0; i < str.Length; i++) {
                newStr += (char)(str[i] ^ key);
            }

            return newStr;
        }
    }
}