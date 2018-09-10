using UnityEngine;
using UnityEditor;
using System;

namespace OpenSpace.Input {
    public class KeyWord { // IPT_tdstEntryElement
        public Pointer offset;

        public ushort indexOrKeyCode;
        public byte field_2;
        public byte field_3;
        
        public KeyWord[] subkeywords;
        public bool isFunction = false;

        public InputFunctions.FunctionType FunctionType {
            get {
                if (isFunction && Index <= Enum.GetNames(typeof(InputFunctions.FunctionType)).Length) {
                    return InputFunctions.GetFunctionType(Index);
                } else return InputFunctions.FunctionType.Unknown;
            }
        }

        public byte Index {
            get { return (byte)(indexOrKeyCode & 0xFF); }
        }

        public KeyWord(Pointer offset) {
            this.offset = offset;
        }

        public static KeyWord Read(Reader reader, Pointer offset, bool isFunction=true) {
            KeyWord keyWord = new KeyWord(offset);

            // Read 20 in total for R2iOS
            if (Settings.s.hasExtraInputData) {
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();
            }
            
            keyWord.indexOrKeyCode = reader.ReadUInt16();
            keyWord.field_2 = reader.ReadByte();
            keyWord.field_3 = reader.ReadByte();
            reader.ReadInt32();
            if (Settings.s.engineVersion == Settings.EngineVersion.R3) reader.ReadInt32();

            if (isFunction && Settings.s.game != Settings.Game.TTSE) {
                keyWord.isFunction = true;
                switch (keyWord.FunctionType) {
                    case InputFunctions.FunctionType.Not:
                        keyWord.subkeywords = new KeyWord[1];
                        keyWord.subkeywords[0] = KeyWord.Read(reader, Pointer.Current(reader));
                        break;
                    case InputFunctions.FunctionType.And:
                    case InputFunctions.FunctionType.Or:
                        keyWord.subkeywords = new KeyWord[2];
                        keyWord.subkeywords[0] = KeyWord.Read(reader, Pointer.Current(reader));
                        keyWord.subkeywords[1] = KeyWord.Read(reader, Pointer.Current(reader));
                        break;
                    case InputFunctions.FunctionType.KeyPressed:
                    case InputFunctions.FunctionType.KeyReleased:
                    case InputFunctions.FunctionType.KeyJustPressed:
                    case InputFunctions.FunctionType.KeyJustReleased:
                        keyWord.subkeywords = new KeyWord[1];
                        keyWord.subkeywords[0] = KeyWord.Read(reader, Pointer.Current(reader), isFunction: false);
                        break;
                    case InputFunctions.FunctionType.Sequence:
                        keyWord.subkeywords = new KeyWord[1];
                        keyWord.subkeywords[0] = KeyWord.Read(reader, Pointer.Current(reader), isFunction: false);
                        int sequenceLength = keyWord.subkeywords[0].indexOrKeyCode;
                        if (sequenceLength > 0) {
                            Array.Resize(ref keyWord.subkeywords, sequenceLength*2 + 2);
                            keyWord.subkeywords[1] = KeyWord.Read(reader, Pointer.Current(reader));
                            for (int i = 0; i < sequenceLength; i++) {
                                keyWord.subkeywords[2+i*2] = KeyWord.Read(reader, Pointer.Current(reader), isFunction: false); // Keycode
                                keyWord.subkeywords[3+i*2] = KeyWord.Read(reader, Pointer.Current(reader), isFunction: true); // SequenceKey
                            }
                        }
                        break;
                }
            } else {
            }

            return keyWord;
        }

        public int FillInSubKeywords(LinkedList<KeyWord> keywords, int thisIndex) {
            isFunction = true;
            int keywordsRead = 1;
            switch (FunctionType) {
                case InputFunctions.FunctionType.Not:
                    subkeywords = new KeyWord[1];
                    subkeywords[0] = keywords[thisIndex + keywordsRead];
                    keywordsRead += subkeywords[0].FillInSubKeywords(keywords, thisIndex + keywordsRead);
                    break;
                case InputFunctions.FunctionType.And:
                case InputFunctions.FunctionType.Or:
                    subkeywords = new KeyWord[2];
                    subkeywords[0] = keywords[thisIndex + keywordsRead];
                    keywordsRead += subkeywords[0].FillInSubKeywords(keywords, thisIndex + keywordsRead);
                    subkeywords[1] = keywords[thisIndex + keywordsRead];
                    keywordsRead += subkeywords[1].FillInSubKeywords(keywords, thisIndex + keywordsRead);
                    break;
                case InputFunctions.FunctionType.KeyPressed:
                case InputFunctions.FunctionType.KeyReleased:
                case InputFunctions.FunctionType.KeyJustPressed:
                case InputFunctions.FunctionType.KeyJustReleased:
                    subkeywords = new KeyWord[1];
                    subkeywords[0] = keywords[thisIndex + keywordsRead];
                    keywordsRead += 1;
                    break;
                case InputFunctions.FunctionType.Sequence:
                    subkeywords = new KeyWord[1];
                    subkeywords[0] = keywords[thisIndex + keywordsRead];
                    keywordsRead += 1;
                    int sequenceLength = subkeywords[0].indexOrKeyCode;
                    if (sequenceLength > 0) {
                        Array.Resize(ref subkeywords, sequenceLength * 2 + 2);
                        subkeywords[1] = keywords[thisIndex + keywordsRead];
                        keywordsRead += subkeywords[1].FillInSubKeywords(keywords, thisIndex + keywordsRead);
                        for (int i = 0; i < sequenceLength; i++) {
                            subkeywords[2 + i * 2] = keywords[thisIndex + keywordsRead]; // Keycode
                            keywordsRead += 1;
                            if (i < sequenceLength - 1) {
                                subkeywords[3 + i * 2] = keywords[thisIndex + keywordsRead]; // SequenceKey
                                keywordsRead += subkeywords[3 + i * 2].FillInSubKeywords(keywords, thisIndex + keywordsRead);
                            }
                        }
                    }
                    break;
            }
            return keywordsRead;
        }

        public override string ToString() {
            if (isFunction) {
                switch (FunctionType) {
                    case InputFunctions.FunctionType.Not:
                        return "!(" + subkeywords[0] + ")";
                    case InputFunctions.FunctionType.And:
                        return "(" + subkeywords[0] + " && " + subkeywords[1] + ")";
                    case InputFunctions.FunctionType.Or:
                        return "(" + subkeywords[0] + " || " + subkeywords[1] + ")";
                    case InputFunctions.FunctionType.KeyPressed:
                    case InputFunctions.FunctionType.KeyReleased:
                    case InputFunctions.FunctionType.KeyJustPressed:
                    case InputFunctions.FunctionType.KeyJustReleased:
                        return FunctionType + "(" + Enum.GetName(typeof(KeyCode), subkeywords[0].indexOrKeyCode) + ")";
                    case InputFunctions.FunctionType.Sequence:
                        string sequence = "";
                        // Skip 1 at the end (first sequenceKey), then do -2 to skip over every other sequenceKey
                        // Then stop because first two keywords (last two processed here) are length and sequenceEnd
                        for (int i = subkeywords.Length - 2; i > 1; i-=2) {
                            sequence += Enum.GetName(typeof(KeyCode), subkeywords[i].indexOrKeyCode);
                        }
                        return "Sequence: " + sequence;
                    default:
                        return FunctionType.ToString();
                }
            } else {
                return "[" + indexOrKeyCode + "]<" + Enum.GetName(typeof(KeyCode), indexOrKeyCode) + ">";
            }
        }
    }
}
