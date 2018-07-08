using UnityEngine;
using UnityEditor;

namespace OpenSpace.Input {
    public class InputEntryElement { // IPT_tdstEntryElement
        public Pointer offset;

        public uint functionTableIndex;
        public uint unknown2;

        public InputFunctions.FunctionType functionType;

        public InputEntryElement(Pointer offset)
        {
            this.offset = offset;
        }

        public static InputEntryElement Read(EndianBinaryReader reader, Pointer offset)
        {
            InputEntryElement i = new InputEntryElement(offset);
            i.functionTableIndex = reader.ReadUInt32();
            i.functionType = InputFunctions.GetFunctionType(i.functionTableIndex);

            i.unknown2 = reader.ReadUInt32();

            return i;
        }

        public override string ToString()
        {
            return functionType.ToString();
        }
    }
}
