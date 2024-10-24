using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using System;
using System.Text;

namespace OpenSpace.Text {
    public class LocalizationStructure : OpenSpaceStruct {
        public struct TextTable {
            public LegacyPointer off_textTable;
            public ushort num_entries_max; // Reserved memory for this table
            public ushort num_entries; // Used memory for this table
            public string[] entries;
        }

        public uint field0;
        public uint field4;
        public ushort num_languages;
        public ushort fieldA;
        public LegacyPointer off_text_languages; //0xC
        public LegacyPointer off_text_misc; //0x10

        public TextTable[] languages;
        public TextTable misc;

		protected override void ReadInternal(Reader reader) {
			Func<Reader, string> ReadString;
			ReadString = (Reader r) => r.ReadNullDelimitedString(encoding: Legacy_Settings.s.platform == Legacy_Settings.Platform.iOS ? Encoding.UTF8 : null);
			//ReadString = (Reader r) => BinarySerializer.Ubisoft.CPA.LanguageParser.ReadSpecialEncodedString(r, BinarySerializer.Ubisoft.CPA.CPA_GameMode.Rayman2DC);
			//ReadString = (Reader r) => BinarySerializer.Ubisoft.CPA.LanguageParser.ReadSpecialEncodedString(r, BinarySerializer.Ubisoft.CPA.CPA_GameMode.Rayman3PC, lang: BinarySerializer.Ubisoft.CPA.VersionLanguage.Hebrew);
			MapLoader l = MapLoader.Loader;
			field0 = reader.ReadUInt32();
			if (Legacy_Settings.s.game == Legacy_Settings.Game.R2Revolution) {
				num_languages = reader.ReadUInt16();
				languages = new TextTable[num_languages];
				languages[0].num_entries = reader.ReadUInt16();
				languages[0].entries = new string[languages[0].num_entries];
				reader.ReadUInt32();
				off_text_misc = LegacyPointer.Read(reader);
				off_text_languages = LegacyPointer.Read(reader);

				// Read misc table, kinda hacky since there is no amount of entries listed
				LegacyPointer.DoAt(ref reader, off_text_misc, () => {
					LegacyPointer off_entry1 = LegacyPointer.Read(reader);
					misc.num_entries = (ushort)((off_entry1.offset - off_text_misc.offset) / 4);
					LegacyPointer.Goto(ref reader, off_text_misc);
					misc.entries = new string[misc.num_entries];
					for (int j = 0; j < misc.num_entries; j++) {
						LegacyPointer off_text = LegacyPointer.Read(reader);
						LegacyPointer.DoAt(ref reader, off_text, () => {
							misc.entries[j] = ReadString(reader);
						});
					}
				});
				// Read only first language.
				// Interestingly, other languages are stored in the menu level file.
				// When you select another language, it replaces the fix table by the one in the menu file
				// This saves fix memory since not all languages have to remain loaded throughout the whole game.
				LegacyPointer.DoAt(ref reader, off_text_languages, () => {
					for (int j = 0; j < languages[0].num_entries; j++) {
						LegacyPointer off_text = LegacyPointer.Read(reader);
						LegacyPointer.DoAt(ref reader, off_text, () => {
							languages[0].entries[j] = ReadString(reader);
						});
					}
				});
			} else {
				if (Legacy_Settings.s.platform != Legacy_Settings.Platform.DC
					&& Legacy_Settings.s.platform != Legacy_Settings.Platform.PS2
					&& Legacy_Settings.s.game != Legacy_Settings.Game.LargoWinch) field4 = reader.ReadUInt32();
				num_languages = reader.ReadUInt16();
				reader.ReadUInt16();
				off_text_languages = LegacyPointer.Read(reader);
				if (Legacy_Settings.s.game != Legacy_Settings.Game.RedPlanet) {
					off_text_misc = LegacyPointer.Read(reader);
				} else {
					misc.entries = new string[0];
				}

				// Read language table
				languages = new TextTable[num_languages];
				LegacyPointer.DoAt(ref reader, off_text_languages, () => {
					//f.languages = new TextTable[f.num_languages];
					for (int i = 0; i < num_languages; i++) {
						languages[i].off_textTable = LegacyPointer.Read(reader);
						languages[i].num_entries_max = reader.ReadUInt16();
						languages[i].num_entries = reader.ReadUInt16();
						languages[i].entries = new string[languages[i].num_entries];
						LegacyPointer.DoAt(ref reader, languages[i].off_textTable, () => {
							for (int j = 0; j < languages[i].num_entries; j++) {
								LegacyPointer off_text = LegacyPointer.Read(reader);
								LegacyPointer.DoAt(ref reader, off_text, () => {
									languages[i].entries[j] = ReadString(reader);
									//l.print(languages[i].entries[j]);
								});
							}
						});
					}
				});

				// Read misc table
				LegacyPointer.DoAt(ref reader, off_text_misc, () => {
					misc.off_textTable = LegacyPointer.Read(reader);
					misc.num_entries_max = reader.ReadUInt16();
					misc.num_entries = reader.ReadUInt16();
					misc.entries = new string[misc.num_entries];

					LegacyPointer.DoAt(ref reader, misc.off_textTable, () => {
						for (int j = 0; j < misc.num_entries; j++) {
							LegacyPointer off_text = LegacyPointer.Read(reader);
							LegacyPointer.DoAt(ref reader, off_text, () => {
								misc.entries[j] = ReadString(reader);
							});
						}
					});
				});
			}

		}

        public void ReadLanguageTableDreamcast(Reader reader, int index, ushort num_entries) {
			Func<Reader, string> ReadString;
			ReadString = (Reader r) => r.ReadNullDelimitedString();
			//ReadString = (Reader r) => BinarySerializer.Ubisoft.CPA.LanguageParser.ReadJapaneseString(r, BinarySerializer.Ubisoft.CPA.CPA_GameMode.Rayman2DC);
			languages[index].off_textTable = LegacyPointer.Current(reader);
            languages[index].num_entries = num_entries;
            languages[index].num_entries_max = num_entries;
            languages[index].entries = new string[num_entries];
            for (int j = 0; j < languages[index].num_entries; j++) {
                LegacyPointer off_text = LegacyPointer.Read(reader);
                LegacyPointer.DoAt(ref reader, off_text, () => {
                    languages[index].entries[j] = ReadString(reader);
				});
            }
        }

		public void ReadLanguageTablePS2(Reader reader, int index) {
			Func<Reader, string> ReadString;
			ReadString = (Reader r) => r.ReadNullDelimitedString();
			//ReadString = (Reader r) => BinarySerializer.Ubisoft.CPA.LanguageParser.ReadJapaneseString(r, BinarySerializer.Ubisoft.CPA.CPA_GameMode.Rayman2PS2);
			languages[index].off_textTable = LegacyPointer.Read(reader);
			languages[index].num_entries_max = reader.ReadUInt16();
			languages[index].num_entries = reader.ReadUInt16();
			languages[index].entries = new string[languages[index].num_entries];
			for (int j = 0; j < languages[index].num_entries; j++) {
				LegacyPointer off_text = LegacyPointer.Read(reader);
				LegacyPointer.DoAt(ref reader, off_text, () => {
					languages[index].entries[j] = ReadString(reader);
				});
			}
		}

        public string GetTextForHandleAndLanguageID(int index, uint currentLanguageId) { // FON_fn_szGetTextPointerForHandle(index)
            if (index == -1) {
                return "";
            } else if (index >= 20000) { // *(*fontStructure_0x10 + 4 * a1 - 80000);
                if ((index - 20000) < misc.entries.Length) {
                    return misc.entries[index - 20000];
                } else {
                    return "null";
                }
            } else { // *(*(dialogStartOffset + 8 * currentLanguageID) + 4 * index);
                return languages[currentLanguageId].entries[index];
            }
        }

        public string GenerateReadableHandle(int index)
        {
            string text = GetTextForHandleAndLanguageID(index, 0);
            text = Regex.Replace(text, @"\/[^:]+:", " ").Trim();
            text = Regex.Replace(text, @"[^\sa-zA-Z0-9_]+", string.Empty);
			text = Regex.Replace(text, @"\s", "_");
            text = Regex.Replace(text, "__+", "_"); // Replace double/triple/etc. _
			return $"{index}_{text}";
        }
	}
}