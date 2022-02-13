using BinarySerializer.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenSpace.FileFormat {
    public class DSB : FileWithPointers {
        public class DSBText {
            public struct DSBTextEntry {
                public string key;
                public string stringValue;
                public int intValue;
                public string type;
                public string command;
            }
            public string name;
            public List<DSBTextEntry> strings = new List<DSBTextEntry>();
        }

        byte[] data = null;
        public List<DSBText> textFiles = new List<DSBText>();
        
        // Bigfile paths
        public string bigfileTextures;
        public string bigfileVignettes;
        public string bigfileCredits;

        // Data paths
        public string dllDataPath;
        public string gameDataPath; // Not used by the game
        public string worldDataPath;
        public string levelsDataPath;
        public string soundDataPath;
        public string saveGameDataPath;
        public string vignettesDataPath;
        public string optionsDataPath;

        // R2 demo old data paths
        public string textureDataPath;
        public string textDataPath;
        public string familiesDataPath;
        public string animsDataPath;
        public string objectClassesDataPath;
        public string objectBanksDataPath;
        public string mechanicsLibrariesDataPath;
        public string visualsDataPath;
        public string materialsDataPath;
        public string environmentDataPath;
        public string extrasPath;
        public string lipSyncDataPath;
        public string effectsDataPath;

        // Vignette name
        public string vignetteName;
        public List<string> levels = new List<string>();
        public uint firstLevelIndex;

        public DSB(string name, string path) : this(name, FileSystem.GetFileReadStream(path)) { }

        public DSB(string name, Stream stream) {
            baseOffset = 0;
            headerOffset = 0;
            this.name = name;
            using (Reader encodedReader = new Reader(stream, CPA_Settings.s.IsLittleEndian)) {
                int maskBytes = encodedReader.InitMask();
                data = encodedReader.ReadBytes((int)stream.Length - maskBytes);
            }
            reader = new Reader(new MemoryStream(data), CPA_Settings.s.IsLittleEndian);
        }

        public override void CreateWriter() {
            return; // Don't need to write to this file
        }

        public void Save(string path) {
            Util.ByteArrayToFile(path, data);
        }

        public void ReadAllSections() {
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            if (CPA_Settings.s.engineVersion == CPA_Settings.EngineVersion.TT) {
                if (CPA_Settings.s.game == CPA_Settings.Game.TTSE) {
                    ReadAllAsText();
                } else {
                    ReadMemoryDesc();
                    reader.ReadUInt32(); // 1
                    ReadDirectoriesDesc();
                    ReadBigFileDesc();
                    vignetteName = ReadString(); // first vignette shown on startup, UbiSoft.bmp
                    ReadString(); // GameData\Options\IPT.bin
                    reader.ReadUInt32(); // 1000
                    ReadTextFiles();
                    ReadConfig();
                    ReadString(); // "3"
                    ReadString(); // "Totalski" = first level
                }
            } else if (CPA_Settings.s.engineVersion == CPA_Settings.EngineVersion.Montreal) {
                dllDataPath = ReadString();
                gameDataPath = ReadString();
                worldDataPath = ReadString();
                levelsDataPath = ReadString();
                soundDataPath = ReadString();
                saveGameDataPath = ReadString();
                textureDataPath = ReadString();
                textureDataPath = ReadString();
                vignettesDataPath = ReadString();
                optionsDataPath = ReadString();
                bigfileVignettes = ReadString();
                bigfileTextures = ReadString();
                reader.ReadUInt32(); // 10000
                reader.ReadUInt16(); // 0
                ReadString(); // Default.cfg
                ReadString(); // Current.cfg
                ReadString(); // "manoir" = first level
            } else {
                while (reader.BaseStream.Position < reader.BaseStream.Length) {
                    ReadSection();
                }
            }
        }

        public void ReadAllAsText() {
            string fileString = reader.ReadString((int)reader.BaseStream.Length);
            string[] lines = fileString.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i].Trim();

                string methodPattern = @"(?<method>.+?)\((?<value>.+?)\)";
                Match methodMatch = Regex.Match(line, methodPattern, RegexOptions.IgnoreCase);
                if (methodMatch.Success) {
                    string method = methodMatch.Groups["method"].Value.Trim();
                    string value = methodMatch.Groups["value"].Value;
                    Match stringValueMatch = Regex.Match(value, @"""(?<value>.+?)""", RegexOptions.IgnoreCase);
                    if (stringValueMatch.Success) {
                        value = stringValueMatch.Groups["value"].Value;
                        if (CPA_Settings.s.engineVersion <= CPA_Settings.EngineVersion.Montreal) value = value.Replace("GameData\\", "");
                        switch (method) {
                            case "DirectoryOfEngineDLL": dllDataPath = value; break;
                            case "DirectoryOfTexture": textureDataPath = value; break;
                            case "DirectoryOfFixTexture": break; // FixTex
                            case "DirectoryOfTexts": textDataPath = value; break;
                            case "DirectoryOfGameData": gameDataPath = value; break;
                            case "DirectoryOfWorld": worldDataPath = value; break;
                            case "DirectoryOfLevels": levelsDataPath = value; break;
                            case "DirectoryOfFamilies": familiesDataPath = value; break;
                            case "DirectoryOfCharacters": break; // Same as levels dir
                            case "DirectoryOfAnimations": animsDataPath = value; break;
                            case "DirectoryOfGraphicsClasses": objectClassesDataPath = value; break;
                            case "DirectoryOfGraphicsBanks": objectBanksDataPath = value; break;
                            case "DirectoryOfMechanics": mechanicsLibrariesDataPath = value; break;
                            case "DirectoryOfSound": soundDataPath = value; break;
                            case "DirectoryOfVisuals": visualsDataPath = value; break;
                            case "DirectoryOfMaterials": materialsDataPath = value; break;
                            case "DirectoryOfEnvironment": environmentDataPath = value; break;
                            case "DirectoryOfSaveGame": saveGameDataPath = value; break;
                            case "DirectoryOfExtras": extrasPath = value; break;
                            case "DirectoryOfVignettes": vignettesDataPath = value; break;
                            case "DirectoryOfOptions": optionsDataPath = value; break;
                            case "DirectoryOfZdx": break; // same as levels dir
                            case "DirectoryOfLipsSync": lipSyncDataPath = value; break;
                            case "DirectoryOfEffects": effectsDataPath = value; break;
                            case "DirectoryOfInventory": break;
                            case "Vignettes": bigfileVignettes = value; break;
                            case "Textures": bigfileTextures = value; break;
                            case "LoadVignette": vignetteName = value; break;
                            case "AddInputDeviceFile":
                            case "AddFontFile":
                            case "AddStringsFile":
                            case "InitRandomManager":
                            case "DefaultFile":
                            case "CurrentFile":
                                break;
                        }
                    } else {
                        // Value not in double quotes
                    }
                }
            }
        }

        public void ReadSection() {
            uint type = reader.ReadUInt32();
            switch (type) {
                case 0: ReadMemoryDesc(); break;
                case 30: ReadLevelName(); break;
                case 40: ReadDirectoriesDesc(); break;
                case 32: ReadRandomDesc(); break;
                case 70: ReadVignetteDesc(); break;
                case 64: ReadBigFileDesc(); break;
                case 90: ReadLevelDesc(); break;
                case 92: ReadLevelDescSoundBanks(); break;
                case 100: ReadDescGameOption(); break;
                case 110: reader.ReadUInt32(); break; // ??? input structure related
                case 120: ReadUnknown(); break;
            }
        }

        private void ReadMemoryDesc() {
            uint id = reader.ReadUInt32();
            uint memSize;
            while (id != 0xFFFF) {
                if (CPA_Settings.s.mode == CPA_Settings.Mode.TonicTroublePC) {
                    switch (id) {
                        // Memory descriptions
                        case 10: reader.ReadUInt32(); break;
                        case 11: reader.ReadUInt32(); break;
                        case 0: reader.ReadUInt32(); break;
                        case 6: reader.ReadUInt32(); break;
                        case 8: reader.ReadUInt32(); break;
                        case 4: reader.ReadUInt32(); break;
                        case 2: reader.ReadUInt32(); reader.ReadUInt32(); break;
                        case 3: reader.ReadUInt32(); reader.ReadUInt32(); break;
                        case 14: reader.ReadUInt32(); break; // position memory
                        case 13: reader.ReadUInt32(); reader.ReadUInt32(); break; // Inventory memory
                        case 16: reader.ReadUInt32(); break; // lipsync memory
                        case 15: reader.ReadUInt32(); reader.ReadUInt32(); break; // Script memory
                        case 7: reader.ReadUInt32(); break;
                        case 12: reader.ReadUInt32(); break;
                        case 1: reader.ReadUInt32(); break;
                        case 17: reader.ReadUInt32(); break;
                        case 18: reader.ReadUInt32(); break;
                    }
                } else {
                    switch (id) {
                        case 1: memSize = reader.ReadUInt32(); break; // GAM_fn_vSetGameMemoryInFix
                        case 2: memSize = reader.ReadUInt32(); break; // GEO Create & Select Memory Channel
                        case 3: memSize = reader.ReadUInt32(); break; // GEO Create Memory Channel
                        case 4: memSize = reader.ReadUInt32(); break; // AI Fix Memory
                        case 5: memSize = reader.ReadUInt32(); break; // TMP Fix Memory
                        case 6: memSize = reader.ReadUInt32(); break; // IPT Memory
                        case 7: memSize = reader.ReadUInt32(); break; // SAI Fix Memory
                        case 8: memSize = reader.ReadUInt32(); break; // FON Memory
                        case 9: memSize = reader.ReadUInt32(); break; // POS Memory
                        case 10: memSize = reader.ReadUInt32(); break; // ???
                        case 11: memSize = reader.ReadUInt32(); break; // Mmg Init Specific Block
                        case 12: memSize = reader.ReadUInt32(); break; // AI Level Memory
                        case 13: memSize = reader.ReadUInt32(); break; // GEO Create & Select Memory Channel
                        case 14: memSize = reader.ReadUInt32(); break; // SAI Level Memory
                        case 15: memSize = reader.ReadUInt32(); break; // TMP Level Memory
                        case 16: reader.ReadUInt32(); reader.ReadUInt32(); break;
                    }
                }
                id = reader.ReadUInt32();
            }
        }

        private void ReadLevelName() {
            uint levelIndex = 0;
            firstLevelIndex = reader.ReadUInt32();
            uint id = reader.ReadUInt32();
            while (id != 0xFFFF) {
                switch (id) {
                    case 31:
                        levels.Add(ReadString());
                        levelIndex++;
                        break;
                }
                id = reader.ReadUInt32();
            }
        }

        private void ReadDirectoriesDesc() {
            if (CPA_Settings.s.mode == CPA_Settings.Mode.TonicTroublePC) {
                dllDataPath = ReadString();
                ReadString(); // gamedata/menus
                ReadString(); // gamedata/menus/anims
                gameDataPath = ReadString(); // gamedata
                ReadString(); // GameData\Texts
                worldDataPath = ReadString(); // GameData\World
                levelsDataPath = ReadString(); // GameData\World\Levels
                familiesDataPath = ReadString(); // GameData\World\Levels\_Common\Families
                ReadString(); // GameData\World\Levels
                animsDataPath = ReadString(); // GameData\World\Graphics\Anims
                objectClassesDataPath = ReadString(); // GameData\World\Graphics\Objects\Classes
                objectBanksDataPath = ReadString(); // GameData\World\Graphics\Objects\Banks
                mechanicsLibrariesDataPath = ReadString(); // GameData\World\Libraries\Mechanics
                soundDataPath = ReadString(); // GameData\World\Sound
                visualsDataPath = ReadString(); // GameData\World\Graphics\Visuals
                environmentDataPath = ReadString(); // GameData\World\Libraries\Environment
                materialsDataPath = ReadString(); // GameData\World\Libraries\Materials
                ReadString(); // GameData\World\Libraries\Materials
                ReadString(); // GameData\World\Libraries\Materials
                ReadString(); // GameData\World\Libraries\Materials
                saveGameDataPath = ReadString(); // GameData\SaveGame
                extrasPath = ReadString(); // GameData\Extras
                textureDataPath = ReadString(); // GameData\World\Graphics\Textures
                ReadString(); // GameData\FixTex
                vignettesDataPath = ReadString(); // GameData\Vignette
                optionsDataPath = ReadString(); // GameData\Options
                lipSyncDataPath = ReadString(); // GameData\World\SyncLips
                ReadString(); // GameData\World\Levels
                effectsDataPath = ReadString(); // GameData\World\Effects
                ReadString(); // GameData\Inventor
            } else {
                uint id = reader.ReadUInt32();
                while (id != 0xFFFF) {
                    if (CPA_Settings.s.game == CPA_Settings.Game.R2Demo || CPA_Settings.s.game == CPA_Settings.Game.RedPlanet) {
                        switch (id) {
                            case 41: dllDataPath = ReadString(); break;
                            case 58: textureDataPath = ReadString(); break;
                            case 43: textDataPath = ReadString(); break;
                            case 44: worldDataPath = ReadString(); break;
                            case 45: levelsDataPath = ReadString(); break;
                            case 46: familiesDataPath = ReadString(); break;
                            case 47: levelsDataPath = ReadString(); break;
                            case 48: animsDataPath = ReadString(); break;
                            case 49: objectClassesDataPath = ReadString(); break;
                            case 50: objectBanksDataPath = ReadString(); break;
                            case 51: mechanicsLibrariesDataPath = ReadString(); break;
                            case 52: soundDataPath = ReadString(); break; // It also sets graphicsDataPath & adds soundDataPath/[language_id] as 2nd SNDdatapath
                            case 53: visualsDataPath = ReadString(); break;
                            case 54: environmentDataPath = ReadString(); break;
                            case 55: materialsDataPath = ReadString(); break;
                            case 56: saveGameDataPath = ReadString(); break;
                            case 57: extrasPath = ReadString(); break;
                            case 59: vignettesDataPath = ReadString(); break;
                            case 60: optionsDataPath = ReadString(); break;
                            case 61: lipSyncDataPath = ReadString(); break;
                            case 62: levelsDataPath = ReadString(); break;
                            case 63: effectsDataPath = ReadString(); break;
                        }
                    } else {
                        switch (id) {
                            case 41: dllDataPath = ReadString(); break;
                            case 42: gameDataPath = ReadString(); break;
                            case 43: worldDataPath = ReadString(); break;
                            case 44: levelsDataPath = ReadString(); break;
                            case 45: soundDataPath = ReadString(); break; // It also sets graphicsDataPath & adds soundDataPath/[language_id] as 2nd SNDdatapath
                            case 46: saveGameDataPath = ReadString(); break;
                            case 48: vignettesDataPath = ReadString(); break;
                            case 49: optionsDataPath = ReadString(); break;
                                // no 47!
                        }
                    }
                    id = reader.ReadUInt32();
                }
            }
        }

        private void ReadRandomDesc() {
            uint randomSize = reader.ReadUInt32();
            uint id = reader.ReadUInt32();
            while (id != 0xFFFF) {
                switch (id) {
                    case 33: break; // _RND_fn_vComputeRandomTable, _RND_fn_vRemapRandomTable
                    case 34:
                        for (uint i = 0; i < randomSize; i++) {
                            reader.ReadUInt32(); // read random table from file
                        }
                        break; // _RND_fn_vRemapRandomTable

                }
                id = reader.ReadUInt32();
            }
        }

        private void ReadVignetteDesc() {
            uint id = reader.ReadUInt32();
            while (id != 0xFFFF) {
                switch (id) {
                    case 71: vignetteName = ReadString(); break; // VIG_fn_vLoadVignetteInStructure
                    case 72: vignetteName = ReadString(); break; // VIG_fn_vFreeVignette && VIG_fn_vLoadVignetteInStructure
                    case 73: break; // _VIG_fn_vInitVignette
                    case 75: break; // _VIG_fn_vDisplayVignette
                    case 76: reader.ReadBytes(0x10); break;
                    case 77: reader.ReadBytes(0x10); break;
                    case 78: reader.ReadBytes(0x40); break;
                    case 79: reader.ReadUInt32(); reader.ReadUInt32(); reader.ReadUInt32(); reader.ReadUInt32(); break; // Some complex stuff
                    case 80: break; // _VIG_fn_vAddBarToVignette
                    case 81: reader.ReadUInt32(); break; // _SNA_fn_ulGetMaxVignetteValueForLevel
                }
                id = reader.ReadUInt32();
            }
        }

        private void ReadBigFileDesc() {
            uint id = reader.ReadUInt32();
            while (id != 0xFFFF) {
                if (CPA_Settings.s.mode == CPA_Settings.Mode.TonicTroublePC) {
                    switch (id) {
                        case 19: bigfileVignettes = ReadString(); break;
                        case 20: bigfileTextures = ReadString(); break;
                        case 21: bigfileCredits = ReadString(); break;
                    }
                } else {
                    switch (id) {
                        case 65: bigfileTextures = ReadString(); break;
                        case 66: bigfileVignettes = ReadString(); break;
                    }
                }
                id = reader.ReadUInt32();
            }
        }

        private void ReadLevelDesc() {
            uint id = reader.ReadUInt32();
            while (id != 0xFFFF) {
                switch (id) {
                    case 91: reader.ReadUInt32(); break; // g_stAlways
                    case 92: ReadLevelDescSoundBanks(); break;
                }
                id = reader.ReadUInt32();
            }
        }

        private void ReadLevelDescSoundBanks() {
            uint id = reader.ReadUInt32();
            while (id != 0xFFFF) {
                switch (id) {
                    case 93:
                        uint num_bankDescriptors = reader.ReadUInt32();
                        for (uint i = 0; i < num_bankDescriptors; i++) {
                            reader.ReadUInt32(); // InitBank
                        }
                        break;
                    case 94:
                        uint bankDescriptor = reader.ReadUInt32(); // InitBank
                        break;
                }
                id = reader.ReadUInt32();
            }
        }

        private void ReadDescGameOption() {
            string option;
            uint id = reader.ReadUInt32();
            while (id != 0xFFFF) {
                switch (id) {
                    case 101:
                        option = ReadString();
                        break;
                    case 102:
                        option = ReadString();
                        break;
                    case 103:
                        option = ReadString(); // GLD_vSetFrameSynchro argument 1
                        option = ReadString(); // GLD_vSetFrameSynchro argument 2
                        option = ReadString(); // GLD_vSetFrameSynchro argument 3
                        break;
                }
                id = reader.ReadUInt32();
            }
        }

        private void ReadUnknown() { // INO devices stuff
            uint id = reader.ReadUInt32();
            while (id != 0xFFFF) {
                switch (id) {
                    case 121: reader.ReadUInt32(); break;
                    case 122: reader.ReadUInt32(); break;
                    case 123: break;
                    case 124: break; // Mouse device
                }
                id = reader.ReadUInt32();
            }
        }

        private void ReadTextFiles() {
            DSBText t = new DSBText();
            t.name = ReadString();
            while (t.name != "ENDTXT") {
                bool readingTextFile = true;
                while (readingTextFile) {
                    DSBText.DSBTextEntry e = new DSBText.DSBTextEntry();
                    e.command = ReadString();
                    switch (e.command) {
                        case "NewStringLenght":
                            e.key = ReadString();
                            e.intValue = reader.ReadInt32();
                            e.type = ReadString();
                            t.strings.Add(e);
                            break;
                        case "NewString":
                            e.key = ReadString();
                            e.stringValue = ReadString();
                            e.type = ReadString();
                            t.strings.Add(e);
                            break;
                        case "END":
                            readingTextFile = false;
                            break;
                        default:
                            throw new FormatException("Unknown DSBTEXT command: " + e.command);
                    }
                }
                textFiles.Add(t);
                // Start new text file
                t = new DSBText();
                t.name = ReadString();
            }
        }

        private void ReadConfig() { // TT only
            uint id = reader.ReadUInt32();
            while (id != 0xFFFF) {
                switch (id) {
                    case 24: ReadString(); break; // Default.cfg
                    case 25: ReadString(); break; // Current.cfg
                    case 22: ReadString(); break; // outro
                }
                id = reader.ReadUInt32();
            }
        }

        private string ReadString() {
            ushort strSize;
            if (CPA_Settings.s.engineVersion == CPA_Settings.EngineVersion.Montreal) {
                strSize = reader.ReadByte();
            } else {
                strSize = reader.ReadUInt16();
            }
            string result = reader.ReadString(strSize);
            if (CPA_Settings.s.engineVersion <= CPA_Settings.EngineVersion.Montreal) result = result.Replace("GameData\\", "");
            return result;
        }

        public override void WritePointer(LegacyPointer pointer) {
            return; // No pointers in this file
        }
    }
}
