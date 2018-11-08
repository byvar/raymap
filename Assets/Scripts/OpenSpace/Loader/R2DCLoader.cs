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

namespace OpenSpace.Loader {
    public class R2DCLoader : MapLoader {
        public override IEnumerator Load() {
            try {
                if (gameDataBinFolder == null || gameDataBinFolder.Trim().Equals("")) throw new Exception("GAMEDATABIN folder doesn't exist");
                if (lvlName == null || lvlName.Trim() == "") throw new Exception("No level name specified!");
                globals = new Globals();
				gameDataBinFolder += "/";
				yield return controller.StartCoroutine(FileSystem.CheckDirectory(gameDataBinFolder));
				if (!FileSystem.DirectoryExists(gameDataBinFolder)) throw new Exception("GAMEDATABIN folder doesn't exist");
                loadingState = "Initializing files";
                yield return controller.StartCoroutine(CreateCNT());

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

                yield return controller.StartCoroutine(LoadDreamcast());

                string logPathTexFix = gameDataBinFolder + "TEXTURE_FIX.LOG";
                string logPathTexLvl = gameDataBinFolder + lvlName + "/TEXTURE_" + lvlName + ".LOG";
                string logPathInfo = gameDataBinFolder + lvlName + "/INFO.LOG";
                /*yield return controller.StartCoroutine(PrepareFile(logPathTexFix));
                yield return controller.StartCoroutine(PrepareFile(logPathTexLvl));*/
                yield return controller.StartCoroutine(PrepareFile(logPathInfo));
                if (FileSystem.FileExists(logPathInfo)) {
                    ReadLog(FileSystem.GetFileReadStream(logPathInfo));
                    yield return null;
                }

                /*if (FileSystem.FileExists(logPathTexFix)) {
                    ReadLog(logPathTexFix);
                    yield return null;
                }
                if (FileSystem.FileExists(logPathTexLvl)) {
                    ReadLog(logPathTexLvl);
                    yield return null;
                }*/

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
            yield return null;
            InitModdables();
        }

        #region Dreamcast
        public IEnumerator LoadDreamcast() {
            textures = new TextureInfo[0];

            loadingState = "Loading fixed memory";
            yield return null;
            files_array[Mem.Fix].GotoHeader();
            Reader reader = files_array[Mem.Fix].reader;
            Pointer off_base_fix = Pointer.Current(reader);
            uint base_language = reader.ReadUInt32(); //Pointer off_language = Pointer.Read(reader);
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
            });

            yield return null;
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
                    yield return controller.StartCoroutine(PrepareFile(langFilePath));
                    files_array[2] = new DCDAT(languages[i], langFilePath, 2);
                    ((DCDAT)files_array[2]).SetHeaderOffset(base_language);
                    files_array[2].GotoHeader();
                    fontStruct.ReadLanguageTableDreamcast(files_array[2].reader, i, (ushort)num_text_language);
                    files_array[2].Dispose();
                }
            }
        
            loadingState = "Loading fixed textures";
            yield return null;
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
            });
            loadingState = "Loading level memory";
            yield return null;
            files_array[Mem.Lvl].GotoHeader();
            reader = files_array[Mem.Lvl].reader;

            // Animation stuff
            Pointer off_animationBank = Pointer.Current(reader);
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
            Pointer.Read(reader);

            // Globals
            globals.off_actualWorld = Pointer.Read(reader);
            globals.off_dynamicWorld = Pointer.Read(reader);
            globals.off_inactiveDynamicWorld = Pointer.Read(reader);
            globals.off_fatherSector = Pointer.Read(reader);
            reader.ReadUInt32();
            Pointer off_always = Pointer.Read(reader);
            Pointer.DoAt(ref reader, off_always, () => {
                globals.num_always = reader.ReadUInt32();
                globals.spawnablePersos = LinkedList<Perso>.ReadHeader(reader, Pointer.Current(reader), LinkedList.Type.Double);
                globals.spawnablePersos.FillPointers(reader, globals.spawnablePersos.off_tail, globals.spawnablePersos.offset);
                globals.off_always_reusableSO = Pointer.Read(reader); // There are (num_always) empty SuperObjects starting with this one.
            });
            Pointer.Read(reader);
            Pointer off_objectTypes = Pointer.Read(reader);
            Pointer.DoAt(ref reader, off_objectTypes, () => {
                // Fill in pointers for the object type tables and read them
                objectTypes = new ObjectType[3][];
                for (uint i = 0; i < 3; i++) {
                    Pointer off_names_header = Pointer.Current(reader);
                    Pointer off_names_first = Pointer.Read(reader);
                    Pointer off_names_last = Pointer.Read(reader);
                    uint num_names = reader.ReadUInt32();

                    FillLinkedListPointers(reader, off_names_last, off_names_header);
                    ReadObjectNamesTable(reader, off_names_first, num_names, i);
                }
            });
            Pointer.Read(reader);
            Pointer off_mainChar = Pointer.Read(reader);
            reader.ReadUInt32();
            uint num_persoInFixPointers = reader.ReadUInt32();
            Pointer off_persoInFixPointers = Pointer.Read(reader);

            //Pointer[] persoInFixPointers = new Pointer[num_persoInFixPointers];
            Pointer.DoAt(ref reader, off_persoInFixPointers, () => {
                for (int i = 0; i < num_persoInFixPointers; i++) {
                    Pointer off_perso = Pointer.Read(reader);
                    Pointer off_so = Pointer.Read(reader);
                    byte[] unk = reader.ReadBytes(4);
                    Pointer off_matrix = Pointer.Current(reader); // It's better to change the pointer instead of the data as that is reused in some places
                    byte[] matrixData = reader.ReadBytes(0x68);
                    byte[] soFlags = reader.ReadBytes(4);
                    byte[] brothersAndParent = reader.ReadBytes(12);

                    Pointer.DoAt(ref reader, off_perso, () => {
                        reader.ReadUInt32();
                        Pointer off_stdGame = Pointer.Read(reader);
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

            yield return null;
            Pointer.Read(reader); // contains a pointer to the camera SO
            Pointer off_cameras = Pointer.Read(reader); // Double linkedlist of cameras
            Pointer off_families = Pointer.Read(reader);
            Pointer.DoAt(ref reader, off_families, () => {
                families = LinkedList<Family>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Double);
                families.FillPointers(reader, families.off_tail, families.off_head);
            });
            Pointer.Read(reader); // At this pointer: a double linkedlist of fix perso's with headers (soptr, next, prev, hdr)
            Pointer.Read(reader); // Rayman
            reader.ReadUInt32();
            Pointer.Read(reader); // Camera
            reader.ReadUInt32();
            reader.ReadUInt32();

            loadingState = "Loading level textures";
            yield return null;
            uint num_textures_lvl = reader.ReadUInt32();
            uint num_textures_total = num_textures_fix + num_textures_lvl;
            Pointer off_textures_lvl = Pointer.Read(reader);
            Pointer.DoAt(ref reader, off_textures_lvl, () => {
                Array.Resize(ref textures, (int)num_textures_total);
                for (uint i = num_textures_fix; i < num_textures_total; i++) {
                    Pointer off_texture = Pointer.Read(reader);
                    textures[i] = null;
                    Pointer.DoAt(ref reader, off_texture, () => {
                        textures[i] = TextureInfo.Read(reader, off_texture);
                    });
                }
                TEX tex = new TEX(tplPaths[1]);
                for (uint i = 0; i < num_textures_lvl; i++) {
                    if (textures[num_textures_fix + i] != null && tex.Count > i) {
                        textures[num_textures_fix + i].Texture = tex.textures[i];
                    }
                }
            });

            loadingState = "Loading families";
            yield return null;
            ReadFamilies(reader);
            loadingState = "Loading animation banks";
            yield return null;
            Pointer.DoAt(ref reader, off_animationBank, () => {
                animationBanks = new AnimationBank[2];
                animationBanks[0] = AnimationBank.ReadDreamcast(reader, off_animationBank, off_events_fix, num_events_fix);
                animationBanks[1] = animationBanks[0];
            });
            loadingState = "Loading superobject hierarchy";
            yield return null;
            ReadSuperObjects(reader);
            loadingState = "Loading always structure";
            yield return null;
            ReadAlways(reader);
            loadingState = "Filling in cross-references";
            yield return null;
            ReadCrossReferences(reader);

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
                        Pointer offset = file.GetUnsafePointer(offsetUint);
                        switch (type) {
                            case "eST_Comport":
                                Behavior b = Behavior.FromOffset(offset);
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
                                AnimationReference ar = AnimationReference.FromOffset(offset);
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
    }
}
