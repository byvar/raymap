using UnityEngine;
using UnityEditor;
using System.Linq;

namespace OpenSpace.Input {
    public class EntryAction { // IPT_tdstEntryElement
        public Pointer offset;

        public uint num_keywords;
        public Pointer off_firstKeyWord;
        public Pointer off_name;
        public Pointer off_name2;
        public byte active;

        public string name = null;
        public string name2 = null;
        public KeyWord firstKeyWord  = null;

        public EntryAction(Pointer offset) {
            this.offset = offset;
        }

        public static EntryAction Read(Reader reader, Pointer offset) {
            //MapLoader.Loader.print("Off: " + offset);
            EntryAction element = new EntryAction(offset);

            if (Settings.s.hasExtraInputData) {
                reader.ReadBytes(0x18);
            }

            element.num_keywords = reader.ReadUInt32();

            element.off_firstKeyWord = Pointer.Read(reader);
            element.off_name = Pointer.Read(reader);
            if (Settings.s.hasExtraInputData || Settings.s.engineMode == Settings.EngineMode.R3) element.off_name2 = Pointer.Read(reader);
            reader.ReadInt32(); // -2
            reader.ReadUInt32();
            element.active = reader.ReadByte();
            reader.ReadBytes(3);

            if (element.off_name != null) {
                Pointer off_current = Pointer.Goto(ref reader, element.off_name);
                element.name = reader.ReadNullDelimitedString();
                Pointer.Goto(ref reader, off_current);
            }
            if (element.off_name2 != null) {
                Pointer off_current = Pointer.Goto(ref reader, element.off_name2);
                element.name2 = reader.ReadNullDelimitedString();
                Pointer.Goto(ref reader, off_current);
            }
            if (element.off_firstKeyWord != null) {
                Pointer off_current = Pointer.Goto(ref reader, element.off_firstKeyWord);
                element.firstKeyWord = KeyWord.Read(reader, element.off_firstKeyWord);
                Pointer.Goto(ref reader, off_current);
            }

            MapLoader.Loader.print(element.ToString());

            return element;
        }

        public override string ToString() {
            string result = "EntryAction";
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
    }
}
