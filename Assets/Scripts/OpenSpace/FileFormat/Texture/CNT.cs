// Adapted from Rayman2Lib by szymski
// https://github.com/szymski/Rayman2Lib/blob/master/csharp_tools/Rayman2Lib/CNTFile.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace OpenSpace.FileFormat.Texture {
    class CNT : IDisposable {
        public enum CNTVersion {
            Rayman2,
            Rayman2Vignette
        }

        public class FileStruct {
            public string name;
            public string directory;
            public int pointer;
            public int size;
            public byte[] xorKey;
            public uint magic2;
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

        public CNTVersion version;


        public string[][] directoryList = null;
        public List<FileStruct> fileList = new List<FileStruct>();
        int directoryCount = 0;
        int fileCount = 0;



        Reader[] readers = null;


        bool isLittleEndian = true;
        uint count = 0;

        public uint Count {
            get { return (uint)fileCount; }
        }

        public Texture2D[] textures = null;

        public CNT(string path) : this(File.OpenRead(path)) { }

        public CNT(string[] paths) {
            directoryList = new string[paths.Length][];
            readers = new Reader[paths.Length];
            for (int i = 0; i < paths.Length; i++) {
                Init(i, File.OpenRead(paths[i]));
            }
        }

        public CNT(Stream stream) {
            directoryList = new string[1][];
            readers = new Reader[1];
            Init(0, stream);
        }

        void Init(int readerIndex, Stream stream) {
            readers[readerIndex] = new Reader(stream, isLittleEndian);
            Reader reader = readers[readerIndex];
            int localDirCount = reader.ReadInt32();
            int localFileCount = reader.ReadInt32();
            directoryCount += localDirCount;
            fileCount += localFileCount;
            directoryList[readerIndex] = new string[localDirCount];
            // Check signature
            if (reader.ReadInt16() != 257) {
                throw new FormatException("This is not a valid CNT archive!");
            }

            byte xorKey = reader.ReadByte();

            // Load directories
            for (int i = 0; i < localDirCount; i++) {
                int strLen = reader.ReadInt32();
                string directory = "";

                for (int j = 0; j < strLen; j++) {
                    directory += (char)(xorKey ^ reader.ReadByte());
                }

                directoryList[readerIndex][i] = directory;
            }

            // Load and check version
            byte verId = reader.ReadByte();

            switch (verId) {
                case 246:
                    version = CNTVersion.Rayman2; break;
                default:
                    version = CNTVersion.Rayman2Vignette; break;
            }

            // Read files
            for (int i = 0; i < localFileCount; i++) {
                int dirIndex = reader.ReadInt32();
                int size = reader.ReadInt32();

                string file = "";

                for (int j = 0; j < size; j++) {
                    file += (char)(xorKey ^ reader.ReadByte());
                }

                byte[] fileXorKey = new byte[4];
                reader.Read(fileXorKey, 0, 4);

                uint magic2 = reader.ReadUInt32();

                int dataPointer = reader.ReadInt32();
                int fileSize = reader.ReadInt32();

                string dir = dirIndex != -1 ? directoryList[readerIndex][dirIndex] : "";
                
                fileList.Add(new FileStruct() {
                    directory = dir,
                    name = file,
                    pointer = dataPointer,
                    size = fileSize,
                    xorKey = fileXorKey,
                    magic2 = magic2,
                    fileNum = readerIndex
                });
            }
        }



        public byte[] GetFileBytes(FileStruct file) {
            if (file == null) return null;
            readers[file.fileNum].BaseStream.Position = file.pointer;

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
            if (file == null) return null;
            return new GF(GetFileBytes(file));
        }

        public GF GetGFByTGAName(string tgaName) {
            FileStruct file = fileList.FirstOrDefault(f => f.TGAName.ToLower().Equals(tgaName.ToLower()));
            if (file == null) return null;
            return new GF(GetFileBytes(file));
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