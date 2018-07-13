using UnityEngine;
using UnityEditor;
using System;

namespace OpenSpace.Input {
    public class KeyWord { // IPT_tdstEntryElement
        public Pointer offset;

        public byte indexOrKeyCode;
        public byte field_1;
        public byte field_2;
        public byte field_3;

        public InputFunctions.FunctionType functionType = InputFunctions.FunctionType._UNKNOWN;

        public KeyWord kw_not;
        public KeyWord kw_and1, kw_and2;
        public KeyWord kw_or1, kw_or2;
        public KeyWord kw_generic;

        public string name = "";

        public KeyWord(Pointer offset)
        {
            this.offset = offset;
        }

        public static KeyWord Read(EndianBinaryReader reader, Pointer offset)
        {
            KeyWord keyWord = new KeyWord(offset);

            // Read 20 in total for R2iOS
            if (Settings.s.hasExtraInputData) {
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();
            }

            if (Settings.s.IsLittleEndian) {
                keyWord.indexOrKeyCode = reader.ReadByte();
                keyWord.field_1 = reader.ReadByte();
            } else {
                keyWord.field_1 = reader.ReadByte();
                keyWord.indexOrKeyCode = reader.ReadByte();
            }
            keyWord.field_2 = reader.ReadByte();
            keyWord.field_3 = reader.ReadByte();

            if (keyWord.indexOrKeyCode <= Enum.GetNames(typeof(InputFunctions.FunctionType)).Length) {
                keyWord.functionType = InputFunctions.GetFunctionType(keyWord.indexOrKeyCode);
            }

            reader.ReadInt32(); // 8 bytes for R2PC

            // Read 12 in total for R3
            if (Settings.s.engineMode == Settings.EngineMode.R3) {
                reader.ReadInt32();
            }

            switch (keyWord.functionType) {
                case InputFunctions.FunctionType.Not:

                    keyWord.kw_not = KeyWord.Read(reader, Pointer.Current(reader));

                    keyWord.name += "(" + keyWord.kw_not + ")";

                break;
                case InputFunctions.FunctionType.And:

                    keyWord.kw_and1 = KeyWord.Read(reader, Pointer.Current(reader));
                    keyWord.kw_and2 = KeyWord.Read(reader, Pointer.Current(reader));

                    keyWord.name += "(" + keyWord.kw_and1 + " && " + keyWord.kw_and2 + ")";

                break;
                case InputFunctions.FunctionType.Or:

                    keyWord.kw_or1 = KeyWord.Read(reader, Pointer.Current(reader));
                    keyWord.kw_or2 = KeyWord.Read(reader, Pointer.Current(reader));

                    keyWord.name += "(" + keyWord.kw_or1 + " || " + keyWord.kw_or2 + ")";
                break;

                case InputFunctions.FunctionType.KeyPressed:
                case InputFunctions.FunctionType.KeyReleased:
                case InputFunctions.FunctionType.KeyJustPressed:
                case InputFunctions.FunctionType.KeyJustReleased:

                    keyWord.name = (keyWord.functionType != InputFunctions.FunctionType._UNKNOWN) ? keyWord.functionType.ToString() : "[" + keyWord.indexOrKeyCode + "]";

                    keyWord.kw_generic = KeyWord.Read(reader, Pointer.Current(reader));
                    string keyCodeName = Enum.GetName(typeof(KeyCode), keyWord.kw_generic.indexOrKeyCode);

                    keyWord.name += "<" + keyCodeName + ">";
                break;

                case InputFunctions.FunctionType.Sequence:

                    string sequence = "";

                    // Sequence length is the whole keyword after sequence
                    int sequenceLength = reader.ReadInt32();
                    reader.ReadInt32();

                    // SequenceEnd is placed at the beginning of a sequence for some reason, not used
                    KeyWord sequenceEnd = KeyWord.Read(reader, Pointer.Current(reader));

                    for (int i=0;i<sequenceLength;i++) {

                        // Read the KeyCode (actually a KeyWord, 8 bytes on r2pc), located 8 bytes before SequenceKey
                        int sequenceKeyCode = reader.ReadInt32();
                        reader.ReadInt32();

                        string sequenceKeyCodeName = Enum.GetName(typeof(KeyCode), sequenceKeyCode);
                        sequence = sequenceKeyCodeName + sequence;

                        // Read 12 for R3
                        if (Settings.s.engineMode == Settings.EngineMode.R3) {
                            reader.ReadInt32();
                        }

                        // Read 20 for R2iOS
                        if (Settings.s.platform == Settings.Platform.iOS) {
                            reader.ReadInt32();
                            reader.ReadInt32();
                            reader.ReadInt32();
                        }

                        // Read the KeyWord for "SequenceKey"
                        reader.ReadInt32();
                        reader.ReadInt32();

                        // Read 12 in total for R3
                        if (Settings.s.engineMode == Settings.EngineMode.R3) {
                            reader.ReadInt32();
                        }

                        // Read 20 in total for R2iOS
                        if (Settings.s.platform == Settings.Platform.iOS) {
                            reader.ReadInt32();
                            reader.ReadInt32();
                            reader.ReadInt32();
                        }
                    }

                    MapLoader.Loader.print("FOUND SEQUENCE " + sequence);

                    keyWord.name = "Sequence: "+sequence;

                break;

                default:
                    keyWord.name = (keyWord.functionType != InputFunctions.FunctionType._UNKNOWN) ? keyWord.functionType.ToString() : "[" + keyWord.indexOrKeyCode + "]";
                break;
            }

            return keyWord;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
