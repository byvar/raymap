using UnityEngine;
using UnityEditor;

namespace OpenSpace.Text {
    public class FontStructure {
        public struct TextTable {
            public Pointer off_textTable;
            public ushort num_entries_max; // Reserved memory for this table
            public ushort num_entries; // Used memory for this table
            public string[] entries;
        }

        public Pointer offset;
        public uint field0;
        public uint field4;
        public ushort num_languages;
        public ushort fieldA;
        public Pointer off_text_languages; //0xC
        public Pointer off_text_misc; //0x10

        public TextTable[] languages;
        public TextTable misc;

        public FontStructure(Pointer offset) {
            this.offset = offset;
        }

        public static FontStructure Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            FontStructure f = new FontStructure(offset);

            f.field0 = reader.ReadUInt32();
			if (Settings.s.game == Settings.Game.R2Revolution) {
				f.num_languages = reader.ReadUInt16();
				f.languages = new TextTable[f.num_languages];
				f.languages[0].num_entries = reader.ReadUInt16();
				f.languages[0].entries = new string[f.languages[0].num_entries];
				reader.ReadUInt32();
				f.off_text_misc = Pointer.Read(reader);
				f.off_text_languages = Pointer.Read(reader);

				// Read misc table, kinda hacky since there is no amount of entries listed
				Pointer.DoAt(ref reader, f.off_text_misc, () => {
					Pointer off_entry1 = Pointer.Read(reader);
					f.misc.num_entries = (ushort)((off_entry1.offset - f.off_text_misc.offset) / 4);
					Pointer.Goto(ref reader, f.off_text_misc);
					f.misc.entries = new string[f.misc.num_entries];
					for (int j = 0; j < f.misc.num_entries; j++) {
						Pointer off_text = Pointer.Read(reader);
						Pointer.DoAt(ref reader, off_text, () => {
							f.misc.entries[j] = reader.ReadNullDelimitedString();
						});
					}
				});
				// Read only first language.
				// Interestingly, other languages are stored in the menu level file.
				// When you select another language, it replaces the fix table by the one in the menu file
				// This saves fix memory since not all languages have to remain loaded throughout the whole game.
				Pointer.DoAt(ref reader, f.off_text_languages, () => {
					for (int j = 0; j < f.languages[0].num_entries; j++) {
						Pointer off_text = Pointer.Read(reader);
						Pointer.DoAt(ref reader, off_text, () => {
							f.languages[0].entries[j] = reader.ReadNullDelimitedString();
						});
					}
				});
			} else {
				if (Settings.s.platform != Settings.Platform.DC && Settings.s.game != Settings.Game.LargoWinch) f.field4 = reader.ReadUInt32();
				f.num_languages = reader.ReadUInt16();
				reader.ReadUInt16();
				f.off_text_languages = Pointer.Read(reader);
				f.off_text_misc = Pointer.Read(reader);

				// Read language table
				f.languages = new TextTable[f.num_languages];
				Pointer.DoAt(ref reader, f.off_text_languages, () => {
					//f.languages = new TextTable[f.num_languages];
					for (int i = 0; i < f.num_languages; i++) {
						f.languages[i].off_textTable = Pointer.Read(reader);
						f.languages[i].num_entries_max = reader.ReadUInt16();
						f.languages[i].num_entries = reader.ReadUInt16();
						f.languages[i].entries = new string[f.languages[i].num_entries];
						Pointer.DoAt(ref reader, f.languages[i].off_textTable, () => {
							for (int j = 0; j < f.languages[i].num_entries; j++) {
								Pointer off_text = Pointer.Read(reader);
								Pointer.DoAt(ref reader, off_text, () => {
									f.languages[i].entries[j] = reader.ReadNullDelimitedString();
								});
							}
						});
					}
				});

				// Read misc table
				Pointer.DoAt(ref reader, f.off_text_misc, () => {
					f.misc.off_textTable = Pointer.Read(reader);
					f.misc.num_entries_max = reader.ReadUInt16();
					f.misc.num_entries = reader.ReadUInt16();
					f.misc.entries = new string[f.misc.num_entries];

					Pointer.DoAt(ref reader, f.misc.off_textTable, () => {
						for (int j = 0; j < f.misc.num_entries; j++) {
							Pointer off_text = Pointer.Read(reader);
							Pointer.DoAt(ref reader, off_text, () => {
								f.misc.entries[j] = reader.ReadNullDelimitedString();
							});
						}
					});
				});
			}

            return f;
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