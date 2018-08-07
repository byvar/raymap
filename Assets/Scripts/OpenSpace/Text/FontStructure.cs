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
            f.field4 = reader.ReadUInt32();
            f.num_languages = reader.ReadUInt16();
            reader.ReadUInt16();
            f.off_text_languages = Pointer.Read(reader);
            f.off_text_misc = Pointer.Read(reader);

            // Read language table
            if (f.off_text_languages != null) {
                f.languages = new TextTable[f.num_languages];
                Pointer off_current = Pointer.Goto(ref reader, f.off_text_languages);
                for (int i = 0; i < f.num_languages; i++) {
                    f.languages[i].off_textTable = Pointer.Read(reader);
                    f.languages[i].num_entries_max = reader.ReadUInt16();
                    f.languages[i].num_entries = reader.ReadUInt16();
                    f.languages[i].entries = new string[f.languages[i].num_entries];
                    if (f.languages[i].off_textTable != null) {
                        Pointer off_current_table = Pointer.Goto(ref reader, f.languages[i].off_textTable);
                        for (int j = 0; j < f.languages[i].num_entries; j++) {
                            Pointer off_text = Pointer.Read(reader);
                            if (off_text != null) {
                                Pointer off_current_entry = Pointer.Goto(ref reader, off_text);
                                f.languages[i].entries[j] = reader.ReadNullDelimitedString();
                                Pointer.Goto(ref reader, off_current_entry);
                            }
                        }
                        Pointer.Goto(ref reader, off_current_table);
                    }
                }
                Pointer.Goto(ref reader, off_current);
            }

            // Read misc table
            if (f.off_text_misc != null) {
                Pointer off_current = Pointer.Goto(ref reader, f.off_text_misc);
                f.misc.off_textTable = Pointer.Read(reader);
                f.misc.num_entries_max = reader.ReadUInt16();
                f.misc.num_entries = reader.ReadUInt16();
                f.misc.entries = new string[f.misc.num_entries];
                if (f.misc.off_textTable != null) {
                    Pointer.Goto(ref reader, f.misc.off_textTable);
                    for (int j = 0; j < f.misc.num_entries; j++) {
                        Pointer off_text = Pointer.Read(reader);
                        if (off_text != null) {
                            Pointer off_current_entry = Pointer.Goto(ref reader, off_text);
                            f.misc.entries[j] = reader.ReadNullDelimitedString();
                            Pointer.Goto(ref reader, off_current_entry);
                        }
                    }
                }
                Pointer.Goto(ref reader, off_current);
            }

            return f;
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