using UnityEngine;
using UnityEditor;

namespace OpenSpace.Input {
    public class EntryElement { // IPT_tdstEntryElement
        public Pointer offset;

        public int field0;
        public Pointer off_firstKeyWord;
        public KeyWord firstKeyWord  = null;

        public EntryElement(Pointer offset)
        {
            this.offset = offset;
        }

        public static EntryElement Read(EndianBinaryReader reader, Pointer offset)
        {
            EntryElement element = new EntryElement(offset);

            element.field0 = reader.ReadInt32();

            // iOS has 24 unknown bytes before offset to keyword
            if (Settings.s.platform == Settings.Platform.iOS) {
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();
            }

            element.off_firstKeyWord = Pointer.Read(reader);
            Pointer original = Pointer.Goto(ref reader, element.off_firstKeyWord);
            element.firstKeyWord = KeyWord.Read(reader, element.off_firstKeyWord);

            Pointer.Goto(ref reader, original);

            return element;
        }

        public override string ToString()
        {
            if (firstKeyWord != null)
                return "EntryElement: " + firstKeyWord.ToString();
            else
                return "EntryElement(NoKeyword)";
        }
    }
}
