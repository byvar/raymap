using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

namespace OpenSpace.Input {
    public class EntryAction : ILinkedListEntry { // IPT_tdstEntryElement
        public Pointer offset;

        public uint num_keywords;
        public Pointer off_keywords;
        public Pointer off_name;
        public Pointer off_name2;
        public byte active;

        public string name = null;
        public string name2 = null;
        public LinkedList<KeyWord> keywords = null;

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

        public string ExportName
        {
            get
            {
                return (name != null && name != "") ? name : "EntryAction_" + this.offset.offset;
            }
        }

        public EntryAction(Pointer offset) {
            this.offset = offset;
        }

        public static EntryAction Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            EntryAction ea = new EntryAction(offset);
            //l.print("EntryAction " + offset);
			l.entryActions.Add(ea);

            if (Settings.s.game == Settings.Game.TTSE) {
                ea.off_entryAction_next = Pointer.Read(reader);
                ea.off_entryAction_prev = Pointer.Read(reader);
                reader.ReadUInt32(); //element.off_entryAction_hdr = Pointer.Read(reader); // hdr pointer doesn't work here
                ea.keywords = LinkedList<KeyWord>.Read(ref reader, Pointer.Current(reader),
                    (off_element) => {
                        return KeyWord.Read(reader, off_element);
                    },
                    flags: Settings.s.hasLinkedListHeaderPointers ?
                            LinkedList.Flags.HasHeaderPointers :
                            LinkedList.Flags.NoPreviousPointersForDouble,
                    type: LinkedList.Type.Default);
                ea.off_name = Pointer.Read(reader);
                reader.ReadInt32(); // -2
                reader.ReadUInt32();
                reader.ReadByte();
                ea.active = reader.ReadByte();
                reader.ReadBytes(2);
            } else {
                if (Settings.s.hasExtraInputData) {
                    reader.ReadBytes(0x18);
                }
                if (Settings.s.platform == Settings.Platform.PS2 &&
                    (Settings.s.game == Settings.Game.RM
                    || Settings.s.game == Settings.Game.RA
                    || Settings.s.mode == Settings.Mode.Rayman3PS2Demo_2002_12_18)) {
                    reader.ReadBytes(0x8);
                }
                ea.num_keywords = reader.ReadUInt32();
                ea.off_keywords = Pointer.Read(reader);
				ea.keywords = new LinkedList<KeyWord>(Pointer.Current(reader), ea.off_keywords, ea.num_keywords, type: LinkedList.Type.SingleNoElementPointers);
                if (Settings.s.engineVersion < Settings.EngineVersion.R2) reader.ReadUInt32(); // Offset of extra input data in tmp memory? It's different by 0x18 every time
                ea.off_name = Pointer.Read(reader);
                if (Settings.s.hasExtraInputData || Settings.s.platform == Settings.Platform.DC || Settings.s.engineVersion == Settings.EngineVersion.R3) ea.off_name2 = Pointer.Read(reader);  
				reader.ReadInt32(); // -2
                reader.ReadUInt32();
                ea.active = reader.ReadByte();
                reader.ReadBytes(3);

                ea.keywords.ReadEntries(ref reader, (off_element) => {
					return KeyWord.Read(reader, off_element);
				});
            }
            if (ea.keywords != null && ea.keywords.Count > 0) {
                int keywordsRead = ea.keywords[0].FillInSubKeywords(ref reader, ea.keywords, 0);
                if (keywordsRead != ea.keywords.Count) {
                    Debug.LogError(offset + " - Keywords read was: " + keywordsRead + " vs " + ea.keywords.Count);
                    Debug.LogError(ea.ToString());
                }
            }
            Pointer.DoAt(ref reader, ea.off_name, () => {
                ea.name = reader.ReadNullDelimitedString();
            });
            Pointer.DoAt(ref reader, ea.off_name2, () => {
                ea.name2 = reader.ReadNullDelimitedString();
            });

            return ea;
        }

        public override string ToString() {
            string result = "EntryAction @ " + offset;
            if (name != null && name.Trim() != "") result += " (" + name + ")";
            if (name2 != null && name2.Trim() != "") result += "[" + name2 + "]";
            if (keywords != null && keywords.Count > 0) {
                result += ": " + keywords[0].ToString();
			}
            return result;
        }

        public string ToBasicString()
        {
			//string result = "<NullEntryAction>";
			string result = "EntryAction{";
			if (name != null && name.Trim() != "") {
				result += "\"" + name + "\", ";
			} else {
                result += "\"" + ExportName + "\", ";
            }
			if (name2 != null && name2.Trim() != "") {
				result += "\"" + name2 + "\", ";
			}
			if (keywords != null && keywords.Count > 0) {
				result += keywords[0].ToString() + ", ";
			}
			if (result.EndsWith("(")) {
				result = "<NullEntryAction>";
			} else if (result.EndsWith(", ")) {
				result = result.Substring(0, result.Length - 2) + "}";
			} else {
				result += "}";
			}
            return result;
        }

        public static EntryAction FromOffset(Pointer offset) {
            if (offset == null) return null;
            return MapLoader.Loader.entryActions.FirstOrDefault(a => a.offset == offset);
        }

        public static EntryAction FromOffsetOrRead(Pointer offset, Reader reader) {
            if (offset == null) return null;
            EntryAction e = EntryAction.FromOffset(offset);
            if (e == null) {
                Pointer.DoAt(ref reader, offset, () => {
                    e = EntryAction.Read(reader, offset);
					MapLoader.Loader.print(e.ToString());
                });
            }
            return e;
        }
    }
}
