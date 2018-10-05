using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

namespace OpenSpace.Input {
    public class EntryAction : ILinkedListEntry { // IPT_tdstEntryElement
        public Pointer offset;

        public uint num_keywords;
        public Pointer off_firstKeyWord;
        public Pointer off_name;
        public Pointer off_name2;
        public byte active;

        public string name = null;
        public string name2 = null;
        public KeyWord firstKeyWord  = null;

        // Only in Tonic Trouble: Special Edition
        public Pointer off_entryAction_next = null;
        public Pointer off_entryAction_prev = null;
        public Pointer off_entryAction_hdr = null;

        public Pointer NextEntry {
            get { return off_entryAction_next; }
        }

        public Pointer PreviousEntry {
            get { return off_entryAction_prev; }
        }

        public EntryAction(Pointer offset) {
            this.offset = offset;
        }

        public static EntryAction Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            //MapLoader.Loader.print("Off: " + offset);
            EntryAction element = new EntryAction(offset);

            if (Settings.s.game == Settings.Game.TTSE) {
                element.off_entryAction_next = Pointer.Read(reader);
                element.off_entryAction_prev = Pointer.Read(reader);
                reader.ReadUInt32(); //element.off_entryAction_hdr = Pointer.Read(reader); // hdr pointer doesn't work here
                LinkedList<KeyWord> keywords = LinkedList<KeyWord>.Read(ref reader, Pointer.Current(reader),
                    (off_element) => {
                        return KeyWord.Read(reader, off_element);
                    },
                    flags: Settings.s.hasLinkedListHeaderPointers ?
                            LinkedList.Flags.HasHeaderPointers :
                            LinkedList.Flags.NoPreviousPointersForDouble,
                    type: LinkedList.Type.Default);
                element.off_name = Pointer.Read(reader);
                reader.ReadInt32(); // -2
                reader.ReadUInt32();
                reader.ReadByte();
                element.active = reader.ReadByte();
                reader.ReadBytes(2);

                if (keywords != null && keywords.Count > 0) {
                    element.firstKeyWord = keywords[0];
                    element.firstKeyWord.FillInSubKeywords(keywords, 0);
                }
            } else {
                if (Settings.s.hasExtraInputData) {
                    reader.ReadBytes(0x18);
                }
                element.num_keywords = reader.ReadUInt32();
                element.off_firstKeyWord = Pointer.Read(reader);
                if (Settings.s.engineVersion < Settings.EngineVersion.R2) reader.ReadUInt32(); // Offset of extra input data in tmp memory? It's different by 0x18 every time
                element.off_name = Pointer.Read(reader);
                if (Settings.s.hasExtraInputData || Settings.s.platform == Settings.Platform.DC || Settings.s.engineVersion == Settings.EngineVersion.R3) element.off_name2 = Pointer.Read(reader);
                reader.ReadInt32(); // -2
                reader.ReadUInt32();
                element.active = reader.ReadByte();
                reader.ReadBytes(3);

                Pointer.DoAt(ref reader, element.off_firstKeyWord, () => {
                    element.firstKeyWord = KeyWord.Read(reader, element.off_firstKeyWord);
                });
            }
            Pointer.DoAt(ref reader, element.off_name, () => {
                element.name = reader.ReadNullDelimitedString();
            });
            Pointer.DoAt(ref reader, element.off_name2, () => {
                element.name2 = reader.ReadNullDelimitedString();
            });

            MapLoader.Loader.print(element.ToString());

            return element;
        }

        public override string ToString() {
            string result = "EntryAction @ " + offset;
            if (name != null && name.Trim() != "") result += " (" + name + ")";
            if (name2 != null && name2.Trim() != "") result += "[" + name2 + "]";
            if (firstKeyWord != null) {
                result += ": " + firstKeyWord.ToString();
            }
            return result;
        }

        public string ToBasicString()
        {
            string result = "<NullEntryAction>";
            if (firstKeyWord != null)
            {
                result = firstKeyWord.ToString();
            }
            if (name != null && name.Trim() != "") result += " (" + name + ")";
            if (name2 != null && name2.Trim() != "") result += "[" + name2 + "]";
            return result;
        }

        public static EntryAction FromOffset(Pointer offset) {
            if (offset == null) return null;
            InputStructure i = MapLoader.Loader.inputStruct;
            if (i == null || i.entryActions == null) return null;
            return i.entryActions.FirstOrDefault(a => a.offset == offset);
        }

        public static EntryAction FromOffsetOrRead(Pointer offset, Reader reader) {
            if (offset == null) return null;
            InputStructure i = MapLoader.Loader.inputStruct;
            if (i == null || i.entryActions == null) return null;
            EntryAction e = EntryAction.FromOffset(offset);
            if (e == null) {
                Pointer.DoAt(ref reader, offset, () => {
                    e = EntryAction.Read(reader, offset);
                    i.entryActions.Add(e);
                });
            }
            return e;
        }
    }
}
