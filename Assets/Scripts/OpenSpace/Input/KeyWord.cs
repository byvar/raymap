using UnityEngine;
using UnityEditor;
using System;

namespace OpenSpace.Input {
    public class KeyWord { // IPT_tdstEntryElement
        public Pointer offset;

        public ushort indexOrKeyCode;
		public int valueAsInt;
		public Pointer valueAsPointer;
        
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
            KeyWord keyword = new KeyWord(offset);

            // Read 20 in total for R2iOS
            if (Settings.s.hasExtraInputData) {
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();
            }

			Pointer off_value = Pointer.Current(reader);
            keyword.indexOrKeyCode = reader.ReadUInt16();
			Pointer.Goto(ref reader, off_value);

            if (Settings.s == Settings.R2PC) {
                keyword.valueAsInt = reader.ReadInt16();
                reader.ReadInt16();
            } else {
                keyword.valueAsInt = reader.ReadInt32();
            }

			keyword.valueAsPointer = Pointer.GetPointerAtOffset(off_value);
			if (Settings.s.engineVersion == Settings.EngineVersion.R3 && Settings.s.game != Settings.Game.LargoWinch) reader.ReadInt32();
			if(Settings.s.game != Settings.Game.R2Revolution) reader.ReadInt32();

            /*if (isFunction && Settings.s.game != Settings.Game.TTSE) {
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
					case InputFunctions.FunctionType.JoystickPressed:
					case InputFunctions.FunctionType.JoystickReleased:
					case InputFunctions.FunctionType.JoystickJustPressed:
					case InputFunctions.FunctionType.JoystickJustReleased:
						keyWord.subkeywords = new KeyWord[2];
						keyWord.subkeywords[0] = KeyWord.Read(reader, Pointer.Current(reader));
						keyWord.subkeywords[1] = KeyWord.Read(reader, Pointer.Current(reader));
						break;
				}
            } else {
            }*/

            return keyword;
        }

        public int FillInSubKeywords(ref Reader reader, LinkedList<KeyWord> keywords, int thisIndex) {
            isFunction = true;
            int keywordsRead = 1;
            switch (FunctionType) {
                case InputFunctions.FunctionType.Not:
                    subkeywords = new KeyWord[1];
                    subkeywords[0] = keywords[thisIndex + keywordsRead];
                    keywordsRead += subkeywords[0].FillInSubKeywords(ref reader, keywords, thisIndex + keywordsRead);
                    break;
                case InputFunctions.FunctionType.And:
                case InputFunctions.FunctionType.Or:
                    subkeywords = new KeyWord[2];
                    subkeywords[0] = keywords[thisIndex + keywordsRead];
                    keywordsRead += subkeywords[0].FillInSubKeywords(ref reader, keywords, thisIndex + keywordsRead);
                    subkeywords[1] = keywords[thisIndex + keywordsRead];
                    keywordsRead += subkeywords[1].FillInSubKeywords(ref reader, keywords, thisIndex + keywordsRead);
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
                        Array.Resize(ref subkeywords, sequenceLength+1);
                        for (int i = 0; i < sequenceLength; i++) {
                            subkeywords[1+i] = keywords[thisIndex + keywordsRead];
                            keywordsRead += subkeywords[1+i].FillInSubKeywords(ref reader, keywords, thisIndex + keywordsRead);
                        }
                    }
                    break;
                case InputFunctions.FunctionType.SequenceKey:
                case InputFunctions.FunctionType.SequenceKeyEnd:
                    subkeywords = new KeyWord[1];
                    subkeywords[0] = keywords[thisIndex + keywordsRead]; // Keycode
                    keywordsRead += 1;
                    break;
                case InputFunctions.FunctionType.SequencePad:
                case InputFunctions.FunctionType.SequencePadEnd:
                    if (Settings.s.game == Settings.Game.LargoWinch) {
                        subkeywords = new KeyWord[3];
                        subkeywords[0] = keywords[thisIndex + keywordsRead];
                        keywordsRead += 1;
                        subkeywords[1] = keywords[thisIndex + keywordsRead]; // 0
                        keywordsRead += 1;
                        subkeywords[2] = keywords[thisIndex + keywordsRead]; // Keycode
                        keywordsRead += 1;
                    } else {
                        subkeywords = new KeyWord[2];
                        subkeywords[0] = keywords[thisIndex + keywordsRead]; // 0
                        keywordsRead += 1;
                        subkeywords[1] = keywords[thisIndex + keywordsRead]; // Keycode
                        keywordsRead += 1;
                    }
                    break;
                case InputFunctions.FunctionType.MouseAxeValue:
                case InputFunctions.FunctionType.MouseAxePosition:
                    subkeywords = new KeyWord[3];
                    subkeywords[0] = keywords[thisIndex + keywordsRead];
                    keywordsRead += 1;
                    subkeywords[1] = keywords[thisIndex + keywordsRead];
                    keywordsRead += 1;
                    subkeywords[2] = keywords[thisIndex + keywordsRead];
                    keywordsRead += 1;
                    break;
                case InputFunctions.FunctionType.MousePressed:
                case InputFunctions.FunctionType.MouseJustPressed:
                case InputFunctions.FunctionType.MouseJustReleased:
                    subkeywords = new KeyWord[1];
                    subkeywords[0] = keywords[thisIndex + keywordsRead];
                    keywordsRead += 1;
                    break;
                case InputFunctions.FunctionType.PadPressed:
                case InputFunctions.FunctionType.PadReleased:
                case InputFunctions.FunctionType.PadJustPressed:
                case InputFunctions.FunctionType.PadJustReleased:
                case InputFunctions.FunctionType.JoystickPressed:
				case InputFunctions.FunctionType.JoystickReleased:
				case InputFunctions.FunctionType.JoystickJustPressed:
				case InputFunctions.FunctionType.JoystickJustReleased:
				case InputFunctions.FunctionType.JoystickOrPadPressed:
				case InputFunctions.FunctionType.JoystickOrPadReleased:
				case InputFunctions.FunctionType.JoystickOrPadJustPressed:
				case InputFunctions.FunctionType.JoystickOrPadJustReleased:
                    if (Settings.s.game == Settings.Game.LargoWinch) {
                        subkeywords = new KeyWord[3];
                        subkeywords[0] = keywords[thisIndex + keywordsRead];
                        keywordsRead += 1;
                        subkeywords[1] = keywords[thisIndex + keywordsRead];
                        keywordsRead += 1;
                        subkeywords[2] = keywords[thisIndex + keywordsRead];
                        keywordsRead += 1;
                    } else {
                        subkeywords = new KeyWord[2];
                        subkeywords[0] = keywords[thisIndex + keywordsRead];
                        keywordsRead += 1;
                        subkeywords[1] = keywords[thisIndex + keywordsRead];
                        keywordsRead += 1;
                    }
					break;
				case InputFunctions.FunctionType.JoystickAxeValue:
                    if (Settings.s.game == Settings.Game.LargoWinch) {
                        subkeywords = new KeyWord[5];
                        subkeywords[0] = keywords[thisIndex + keywordsRead];
                        keywordsRead += 1;
                        subkeywords[1] = keywords[thisIndex + keywordsRead];
                        keywordsRead += 1;
                        subkeywords[2] = keywords[thisIndex + keywordsRead];
                        keywordsRead += 1;
                        subkeywords[3] = keywords[thisIndex + keywordsRead];
                        keywordsRead += 1;
                        subkeywords[4] = keywords[thisIndex + keywordsRead];
                        keywordsRead += 1;
                    } else {
                        subkeywords = new KeyWord[4];
                        subkeywords[0] = keywords[thisIndex + keywordsRead];
                        keywordsRead += 1;
                        subkeywords[1] = keywords[thisIndex + keywordsRead];
                        keywordsRead += 1;
                        subkeywords[2] = keywords[thisIndex + keywordsRead];
                        keywordsRead += 1;
                        subkeywords[3] = keywords[thisIndex + keywordsRead];
                        keywordsRead += 1;
                    }
					break;
                case InputFunctions.FunctionType.JoystickAngularValue:
                case InputFunctions.FunctionType.JoystickTrueNormValue:
                case InputFunctions.FunctionType.JoystickCorrectedNormValue:
                    subkeywords = new KeyWord[5];
                    subkeywords[0] = keywords[thisIndex + keywordsRead];
                    keywordsRead += 1;
                    subkeywords[1] = keywords[thisIndex + keywordsRead];
                    keywordsRead += 1;
                    subkeywords[2] = keywords[thisIndex + keywordsRead];
                    keywordsRead += 1;
                    subkeywords[3] = keywords[thisIndex + keywordsRead];
                    keywordsRead += 1;
                    subkeywords[4] = keywords[thisIndex + keywordsRead];
                    keywordsRead += 1;
                    break;
                case InputFunctions.FunctionType.ActionValidated:
				case InputFunctions.FunctionType.ActionInvalidated:
				case InputFunctions.FunctionType.ActionJustValidated:
				case InputFunctions.FunctionType.ActionJustInvalidated:
					subkeywords = new KeyWord[1];
					subkeywords[0] = keywords[thisIndex + keywordsRead];
					if (subkeywords[0] != null && Settings.s.game == Settings.Game.TT) {
						EntryAction.FromOffsetOrRead(subkeywords[0].valueAsPointer, reader);
					}
					keywordsRead += 1;
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
                        for (int i = subkeywords.Length - 1; i > 0; i--) {
                            KeyWord w = subkeywords[i];
                            switch (w.FunctionType) {
                                case InputFunctions.FunctionType.SequenceKey:
                                case InputFunctions.FunctionType.SequenceKeyEnd:
                                    sequence += Enum.GetName(typeof(KeyCode), subkeywords[i].subkeywords[0].indexOrKeyCode);
                                    break;
                                case InputFunctions.FunctionType.SequencePad:
                                case InputFunctions.FunctionType.SequencePadEnd:
                                    sequence += GetJoyPadString(subkeywords[i].subkeywords);
                                    break;
                            }
                        }
                        return "Sequence(\"" + sequence + "\")";
                    case InputFunctions.FunctionType.PadPressed:
                    case InputFunctions.FunctionType.PadReleased:
                    case InputFunctions.FunctionType.PadJustPressed:
                    case InputFunctions.FunctionType.PadJustReleased:
                    case InputFunctions.FunctionType.JoystickPressed:
					case InputFunctions.FunctionType.JoystickReleased:
					case InputFunctions.FunctionType.JoystickJustPressed:
					case InputFunctions.FunctionType.JoystickJustReleased:
					case InputFunctions.FunctionType.JoystickOrPadPressed:
					case InputFunctions.FunctionType.JoystickOrPadReleased:
					case InputFunctions.FunctionType.JoystickOrPadJustPressed:
					case InputFunctions.FunctionType.JoystickOrPadJustReleased:
                        return FunctionType + GetJoyPadString(subkeywords);
					case InputFunctions.FunctionType.JoystickAxeValue:
                        if (Settings.s.game == Settings.Game.LargoWinch) {
                            return FunctionType + "("
                                + (subkeywords[2].indexOrKeyCode == 4 ? "X" : "Y")
                                + ", " + subkeywords[3].valueAsInt
                                + ", " + subkeywords[4].valueAsInt
                                + (subkeywords[1].indexOrKeyCode != 0 ? (", " + subkeywords[1].indexOrKeyCode) : "") + ")";
                        } else {
                            return FunctionType + "("
                                + (subkeywords[1].indexOrKeyCode == 4 ? "X" : "Y")
                                + ", " + subkeywords[2].valueAsInt
                                + ", " + subkeywords[3].valueAsInt
                                + (subkeywords[0].indexOrKeyCode != 0 ? (", " + subkeywords[0].indexOrKeyCode) : "") + ")";
                        }
                    case InputFunctions.FunctionType.ActionValidated:
					case InputFunctions.FunctionType.ActionInvalidated:
					case InputFunctions.FunctionType.ActionJustValidated:
					case InputFunctions.FunctionType.ActionJustInvalidated:
						Pointer off_action = subkeywords[0].valueAsPointer;
						if (Settings.s.engineVersion <= Settings.EngineVersion.Montreal) return FunctionType + "()";
						EntryAction action = EntryAction.FromOffset(off_action);
						return FunctionType + "{" + (action != null ? ((action.name != null && action.name.Trim() != "") ? ("\"" + action.name + "\"") : action.ToBasicString()) : "null") + "}";
					default:
                        return FunctionType.ToString() + "()";
                }
            } else {
                return "[" + indexOrKeyCode + "]<" + Enum.GetName(typeof(KeyCode), indexOrKeyCode) + ">";
            }
        }

        private string GetJoyPadString(KeyWord[] subkeywords) {
            int firstKW = 0;
            if (Settings.s.game == Settings.Game.LargoWinch) firstKW++;
            if (Settings.s.platform == Settings.Platform.GC) {
                return "(" + Enum.GetName(typeof(GameCubeKeyCode), subkeywords[firstKW + 1].indexOrKeyCode) + (subkeywords[firstKW + 0].indexOrKeyCode != 0 ? (", " + subkeywords[firstKW + 0].indexOrKeyCode) : "") + ")";
            } else if (Settings.s.platform == Settings.Platform.DC) {
                return "(" + Enum.GetName(typeof(DreamcastKeyCode), subkeywords[firstKW + 1].indexOrKeyCode) + (subkeywords[firstKW + 0].indexOrKeyCode != 0 ? (", " + subkeywords[firstKW + 0].indexOrKeyCode) : "") + ")";
            } else if (Settings.s.platform == Settings.Platform.PS2) {
                if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                    return "(" + Enum.GetName(typeof(RevolutionPS2KeyCode), subkeywords[firstKW + 1].indexOrKeyCode) + (subkeywords[firstKW + 0].indexOrKeyCode != 0 ? (", " + subkeywords[firstKW + 0].indexOrKeyCode) : "") + ")";
                } else {
                    return "(" + Enum.GetName(typeof(PS2KeyCode), subkeywords[firstKW + 1].indexOrKeyCode) + (subkeywords[firstKW + 0].indexOrKeyCode != 0 ? (", " + subkeywords[firstKW + 0].indexOrKeyCode) : "") + ")";
                }
            } else {
                return "(" + Enum.GetName(typeof(JoypadKeyCode), subkeywords[firstKW + 1].indexOrKeyCode) + (subkeywords[firstKW + 0].indexOrKeyCode != 0 ? (", " + subkeywords[firstKW + 0].indexOrKeyCode) : "") + ")";
            }
        }
    }
}
