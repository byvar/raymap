using UnityEngine;
using UnityEditor;

namespace OpenSpace.Text {
    public class LocalizationStructure : OpenSpaceStruct {
        public struct TextTable {
            public Pointer off_textTable;
            public ushort num_entries_max; // Reserved memory for this table
            public ushort num_entries; // Used memory for this table
            public string[] entries;
        }

        public uint field0;
        public uint field4;
        public ushort num_languages;
        public ushort fieldA;
        public Pointer off_text_languages; //0xC
        public Pointer off_text_misc; //0x10

        public TextTable[] languages;
        public TextTable misc;

		protected override void ReadInternal(Reader reader) {
			MapLoader l = MapLoader.Loader;
			field0 = reader.ReadUInt32();
			if (Settings.s.game == Settings.Game.R2Revolution) {
				num_languages = reader.ReadUInt16();
				languages = new TextTable[num_languages];
				languages[0].num_entries = reader.ReadUInt16();
				languages[0].entries = new string[languages[0].num_entries];
				reader.ReadUInt32();
				off_text_misc = Pointer.Read(reader);
				off_text_languages = Pointer.Read(reader);

				// Read misc table, kinda hacky since there is no amount of entries listed
				Pointer.DoAt(ref reader, off_text_misc, () => {
					Pointer off_entry1 = Pointer.Read(reader);
					misc.num_entries = (ushort)((off_entry1.offset - off_text_misc.offset) / 4);
					Pointer.Goto(ref reader, off_text_misc);
					misc.entries = new string[misc.num_entries];
					for (int j = 0; j < misc.num_entries; j++) {
						Pointer off_text = Pointer.Read(reader);
						Pointer.DoAt(ref reader, off_text, () => {
							misc.entries[j] = reader.ReadNullDelimitedString();
						});
					}
				});
				// Read only first language.
				// Interestingly, other languages are stored in the menu level file.
				// When you select another language, it replaces the fix table by the one in the menu file
				// This saves fix memory since not all languages have to remain loaded throughout the whole game.
				Pointer.DoAt(ref reader, off_text_languages, () => {
					for (int j = 0; j < languages[0].num_entries; j++) {
						Pointer off_text = Pointer.Read(reader);
						Pointer.DoAt(ref reader, off_text, () => {
							languages[0].entries[j] = reader.ReadNullDelimitedString();
						});
					}
				});
			} else {
				if (Settings.s.platform != Settings.Platform.DC
					&& Settings.s.platform != Settings.Platform.PS2
					&& Settings.s.game != Settings.Game.LargoWinch) field4 = reader.ReadUInt32();
				num_languages = reader.ReadUInt16();
				reader.ReadUInt16();
				off_text_languages = Pointer.Read(reader);
				off_text_misc = Pointer.Read(reader);

				// Read language table
				languages = new TextTable[num_languages];
				Pointer.DoAt(ref reader, off_text_languages, () => {
					//f.languages = new TextTable[f.num_languages];
					for (int i = 0; i < num_languages; i++) {
						languages[i].off_textTable = Pointer.Read(reader);
						languages[i].num_entries_max = reader.ReadUInt16();
						languages[i].num_entries = reader.ReadUInt16();
						languages[i].entries = new string[languages[i].num_entries];
						Pointer.DoAt(ref reader, languages[i].off_textTable, () => {
							for (int j = 0; j < languages[i].num_entries; j++) {
								Pointer off_text = Pointer.Read(reader);
								Pointer.DoAt(ref reader, off_text, () => {
									languages[i].entries[j] = reader.ReadNullDelimitedString();
									//l.print(languages[i].entries[j]);
								});
							}
						});
					}
				});

				// Read misc table
				Pointer.DoAt(ref reader, off_text_misc, () => {
					misc.off_textTable = Pointer.Read(reader);
					misc.num_entries_max = reader.ReadUInt16();
					misc.num_entries = reader.ReadUInt16();
					misc.entries = new string[misc.num_entries];

					Pointer.DoAt(ref reader, misc.off_textTable, () => {
						for (int j = 0; j < misc.num_entries; j++) {
							Pointer off_text = Pointer.Read(reader);
							Pointer.DoAt(ref reader, off_text, () => {
								misc.entries[j] = reader.ReadNullDelimitedString();
							});
						}
					});
				});
			}

		}

        public void ReadLanguageTableDreamcast(Reader reader, int index, ushort num_entries) {
            languages[index].off_textTable = Pointer.Current(reader);
            languages[index].num_entries = num_entries;
            languages[index].num_entries_max = num_entries;
            languages[index].entries = new string[num_entries];
            for (int j = 0; j < languages[index].num_entries; j++) {
                Pointer off_text = Pointer.Read(reader);
                Pointer.DoAt(ref reader, off_text, () => {
                    languages[index].entries[j] = reader.ReadNullDelimitedString();
                });
            }
        }

		public void ReadLanguageTablePS2(Reader reader, int index) {
			languages[index].off_textTable = Pointer.Read(reader);
			languages[index].num_entries_max = reader.ReadUInt16();
			languages[index].num_entries = reader.ReadUInt16();
			languages[index].entries = new string[languages[index].num_entries];
			for (int j = 0; j < languages[index].num_entries; j++) {
				Pointer off_text = Pointer.Read(reader);
				Pointer.DoAt(ref reader, off_text, () => {
					languages[index].entries[j] = reader.ReadNullDelimitedString();
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
	}
}