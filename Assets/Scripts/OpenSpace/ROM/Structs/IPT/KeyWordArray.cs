using OpenSpace.Loader;
using System;
using UnityEngine;
using FunctionType = OpenSpace.Input.InputFunctions.FunctionType;
using JoypadKeyCode = OpenSpace.Input.JoypadKeyCode;

namespace OpenSpace.ROM {
	public class KeyWordArray : ROMStruct {
		public KeyWord[] keywords;

		public ushort length;

        protected override void ReadInternal(Reader reader) {
			keywords = new KeyWord[length];
			for (ushort i = 0; i < length; i++) {
				keywords[i] = new KeyWord();
				keywords[i].indexOrKeyCode = reader.ReadUInt16();
				//Loader.print("kw: " + string.Format("{0:X4}",keywords[i].indexOrKeyCode) + " - " + keywords[i].FunctionType);
			}
			if (keywords.Length > 0) {
				keywords[0].FillInSubKeywords(reader, keywords, 0);
			}
        }

		public class KeyWord {
			public ushort indexOrKeyCode;

			// Custom
			public KeyWord[] subkeywords;
			public Reference<EntryAction> subAction;
			public bool isFunction = false;

			public FunctionType FunctionType {
				get {
					if (IsFunction && Index <= Enum.GetNames(typeof(FunctionType)).Length) {
						return GetFunctionType(Index);
					} else return FunctionType.Unknown;
				}
			}
			public byte Index {
				get { return (byte)(indexOrKeyCode & 0xFF); }
			}
			public bool IsFunction {
				get { return (indexOrKeyCode & 0xF000) == 0xF000; }
			}
			public static FunctionType GetFunctionType(uint index) {
				try {
					return functionTypes[index];
					//return (FunctionType)(index);
				} catch (Exception) {
					return FunctionType.Unknown;
				}
			}


			public int FillInSubKeywords(Reader reader, KeyWord[] keywords, int thisIndex) {
				isFunction = true;
				int keywordsRead = 1;
				switch (FunctionType) {
					case FunctionType.Not:
						subkeywords = new KeyWord[1];
						subkeywords[0] = keywords[thisIndex + keywordsRead];
						keywordsRead += subkeywords[0].FillInSubKeywords(reader, keywords, thisIndex + keywordsRead);
						break;
					case FunctionType.And:
					case FunctionType.Or:
						subkeywords = new KeyWord[2];
						subkeywords[0] = keywords[thisIndex + keywordsRead];
						keywordsRead += subkeywords[0].FillInSubKeywords(reader, keywords, thisIndex + keywordsRead);
						subkeywords[1] = keywords[thisIndex + keywordsRead];
						keywordsRead += subkeywords[1].FillInSubKeywords(reader, keywords, thisIndex + keywordsRead);
						break;
					case FunctionType.KeyPressed:
					case FunctionType.KeyReleased:
					case FunctionType.KeyJustPressed:
					case FunctionType.KeyJustReleased:
						subkeywords = new KeyWord[1];
						subkeywords[0] = keywords[thisIndex + keywordsRead];
						keywordsRead += 1;
						break;
					case FunctionType.Sequence:
						subkeywords = new KeyWord[1];
						subkeywords[0] = keywords[thisIndex + keywordsRead];
						keywordsRead += 1;
						int sequenceLength = subkeywords[0].indexOrKeyCode;
						if (sequenceLength > 0) {
							Array.Resize(ref subkeywords, sequenceLength * 2 + 2);
							subkeywords[1] = keywords[thisIndex + keywordsRead];
							keywordsRead += subkeywords[1].FillInSubKeywords(reader, keywords, thisIndex + keywordsRead);
							for (int i = 0; i < sequenceLength; i++) {
								subkeywords[2 + i * 2] = keywords[thisIndex + keywordsRead]; // Keycode
								keywordsRead += 1;
								if (i < sequenceLength - 1) {
									subkeywords[3 + i * 2] = keywords[thisIndex + keywordsRead]; // SequenceKey
									keywordsRead += subkeywords[3 + i * 2].FillInSubKeywords(reader, keywords, thisIndex + keywordsRead);
								}
							}
						}
						break;
					case FunctionType.JoystickPressed:
					case FunctionType.JoystickReleased:
					case FunctionType.JoystickOrPadPressed:
					case FunctionType.JoystickOrPadReleased:
						subkeywords = new KeyWord[3];
						subkeywords[0] = keywords[thisIndex + keywordsRead];
						keywordsRead += 1;
						subkeywords[1] = keywords[thisIndex + keywordsRead];
						keywordsRead += 1;
						subkeywords[2] = keywords[thisIndex + keywordsRead];
						keywordsRead += 1;
						break;
					case FunctionType.JoystickJustPressed:
					case FunctionType.JoystickJustReleased:
					case FunctionType.JoystickOrPadJustPressed:
					case FunctionType.JoystickOrPadJustReleased:
						subkeywords = new KeyWord[2];
						subkeywords[0] = keywords[thisIndex + keywordsRead];
						keywordsRead += 1;
						subkeywords[1] = keywords[thisIndex + keywordsRead];
						keywordsRead += 1;
						break;
					case FunctionType.JoystickAxeValue:
						subkeywords = new KeyWord[4];
						subkeywords[0] = keywords[thisIndex + keywordsRead];
						keywordsRead += 1;
						subkeywords[1] = keywords[thisIndex + keywordsRead];
						keywordsRead += 1;
						subkeywords[2] = keywords[thisIndex + keywordsRead];
						keywordsRead += 1;
						subkeywords[3] = keywords[thisIndex + keywordsRead];
						keywordsRead += 1;
						break;
					case FunctionType.ActionJustValidated:
					case FunctionType.ActionJustInvalidated:
						subkeywords = new KeyWord[1];
						subkeywords[0] = keywords[thisIndex + keywordsRead];
						subkeywords[0].subAction = new Reference<EntryAction>(subkeywords[0].indexOrKeyCode, reader, resolve: true);
						keywordsRead += 1;
						break;
					case FunctionType.ActionValidated:
					case FunctionType.ActionInvalidated:
						subkeywords = new KeyWord[2];
						subkeywords[0] = keywords[thisIndex + keywordsRead];
						subkeywords[0].subAction = new Reference<EntryAction>(subkeywords[0].indexOrKeyCode, reader, resolve: true);
						keywordsRead += 1;
						subkeywords[1] = keywords[thisIndex + keywordsRead];
						keywordsRead += 1;
						break;
				}
				return keywordsRead;
			}

			public override string ToString() {
				if (isFunction) {
					switch (FunctionType) {
						case FunctionType.Not:
							return "!(" + subkeywords[0] + ")";
						case FunctionType.And:
							return "(" + subkeywords[0] + " && " + subkeywords[1] + ")";
						case FunctionType.Or:
							return "(" + subkeywords[0] + " || " + subkeywords[1] + ")";
						case FunctionType.KeyPressed:
						case FunctionType.KeyReleased:
						case FunctionType.KeyJustPressed:
						case FunctionType.KeyJustReleased:
							return FunctionType + "(" + Enum.GetName(typeof(OpenSpace.Input.KeyCode), subkeywords[0].indexOrKeyCode) + ")";
						case FunctionType.Sequence:
							string sequence = "";
							// Skip 1 at the end (first sequenceKey), then do -2 to skip over every other sequenceKey
							// Then stop because first two keywords (last two processed here) are length and sequenceEnd
							for (int i = subkeywords.Length - 2; i > 1; i -= 2) {
								sequence += Enum.GetName(typeof(OpenSpace.Input.KeyCode), subkeywords[i].indexOrKeyCode);
							}
							return "Sequence(\"" + sequence + "\")";
						case FunctionType.JoystickPressed:
						case FunctionType.JoystickReleased:
						case FunctionType.JoystickOrPadPressed:
						case FunctionType.JoystickOrPadReleased:
							return FunctionType + "("
								+ ((Settings.s.platform == Settings.Platform._3DS) ? Enum.GetName(typeof(KeyCode3DS), subkeywords[1].indexOrKeyCode).ToString() : subkeywords[1].indexOrKeyCode.ToString())
								+ (subkeywords[0].indexOrKeyCode != 0 ? (", " + subkeywords[0].indexOrKeyCode) : "")
								+ (subkeywords[2].indexOrKeyCode != 0 ? (", " + subkeywords[2].indexOrKeyCode) : "") + ")";
						case FunctionType.JoystickJustPressed:
						case FunctionType.JoystickJustReleased:
						case FunctionType.JoystickOrPadJustPressed:
						case FunctionType.JoystickOrPadJustReleased:
							return FunctionType + "(" + Enum.GetName(typeof(KeyCode3DS), subkeywords[1].indexOrKeyCode) + (subkeywords[0].indexOrKeyCode != 0 ? (", " + subkeywords[0].indexOrKeyCode) : "") + ")";
						case FunctionType.JoystickAxeValue:
							return FunctionType + "("
								+ (subkeywords[1].indexOrKeyCode == 4 ? "X" : "Y")
								+ ", " + subkeywords[2].indexOrKeyCode
								+ ", " + subkeywords[3].indexOrKeyCode
								+ (subkeywords[0].indexOrKeyCode != 0 ? (", " + subkeywords[0].indexOrKeyCode) : "") + ")";
						case FunctionType.ActionValidated:
						case FunctionType.ActionInvalidated:
							EntryAction action = subkeywords[0]?.subAction.Value;
							return FunctionType + "("
								+ (action != null ? action.GetNameString() : "null")
								+ (subkeywords[1].indexOrKeyCode != 0 ? (", " + subkeywords[1].indexOrKeyCode) : "")
								+ ")";
						//return FunctionType + "{" + (action != null ? ((action.name != null && action.name.Trim() != "") ? ("\"" + action.name + "\"") : action.ToBasicString()) : "null") + "}";
						case FunctionType.ActionJustValidated:
						case FunctionType.ActionJustInvalidated:
							EntryAction action2 = subkeywords[0]?.subAction.Value;
							return FunctionType + "("
								+ (action2 != null ? action2.GetNameString() : "null")
								+ ")";
							//return FunctionType + "{" + (action != null ? ((action.name != null && action.name.Trim() != "") ? ("\"" + action.name + "\"") : action.ToBasicString()) : "null") + "}";
						default:
							return FunctionType.ToString() + "()";
					}
				} else {
					return "[" + indexOrKeyCode + "]<" + Enum.GetName(typeof(KeyCode), indexOrKeyCode) + ">";
				}
			}

			public static FunctionType[] functionTypes = new FunctionType[] {
				FunctionType.Unknown,
				FunctionType.And,
				FunctionType.Or,
				FunctionType.Not,
				FunctionType.KeyJustPressed,
				FunctionType.KeyJustReleased,
				FunctionType.KeyPressed,
				FunctionType.KeyReleased,
				FunctionType.ActionJustValidated,
				FunctionType.ActionJustInvalidated,
				FunctionType.ActionValidated,
				FunctionType.ActionInvalidated,
				FunctionType.PadJustPressed,
				FunctionType.PadJustReleased,
				FunctionType.PadPressed,
				FunctionType.PadReleased,
				FunctionType.JoystickAxeValue,
				FunctionType.JoystickJustPressed,
				FunctionType.JoystickJustReleased,
				FunctionType.JoystickPressed,
				FunctionType.JoystickReleased,
				FunctionType.JoystickOrPadJustPressed,
				FunctionType.JoystickOrPadJustReleased,
				FunctionType.JoystickOrPadPressed,
				FunctionType.JoystickOrPadReleased,
				FunctionType.Sequence,
				FunctionType.SequenceKey,
				FunctionType.SequenceKeyEnd,
				FunctionType.SequencePad,
				FunctionType.SequencePadEnd,
			};
			public enum KeyCode3DS {
				Dive = 0x10,
				Jump = 0x11,
				_18 = 0x12,
				_19 = 0x13,
				CamLeft = 0x14,
				CamRight = 0x15,
				_22 = 0x16,
				_23 = 0x17,
				LookMode = 0x18,
				Strafe = 0x19,
				_26 = 0x1A,
				Shoot = 0x1B,
			}
		}

	}
}
