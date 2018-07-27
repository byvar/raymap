using UnityEngine;
using UnityEditor;
using OpenSpace;

public class FontStruct
{
    public Pointer offset;
    public uint field_0x0;
    public uint field_0x4;
    public uint field_0x8;
    public Pointer off_dialogStartOffset; //0xC
    public Pointer off_miscStartOffset; //0x10

    public FontStruct(Pointer offset)
    {
        this.offset = offset;
    }

    public static FontStruct Read(EndianBinaryReader reader, Pointer offset)
    {
        FontStruct fontStruct = new FontStruct(offset);

        fontStruct.field_0x0 = reader.ReadUInt32();
        fontStruct.field_0x4 = reader.ReadUInt32();
        fontStruct.field_0x8 = reader.ReadUInt32();
        fontStruct.off_dialogStartOffset = Pointer.Read(reader);
        fontStruct.off_miscStartOffset = Pointer.Read(reader);

        return fontStruct;
    }

    public string GetTextForHandleAndLanguageID(EndianBinaryReader reader, int index, uint currentLanguageId) // FON_fn_szGetTextPointerForHandle(index)
    {
        string result = "???";
        if (index == -1)
        {
            return "";
        }
        else if (index >= 20000) // *(*fontStructure_0x10 + 4 * a1 - 80000);
        {
            Pointer offset = off_miscStartOffset + ((4 * index) - 80000);
            Pointer original = Pointer.Goto(ref reader, offset);
            //Pointer next = Pointer.Read(reader);
            //Pointer.Goto(ref reader, next);
            result = reader.ReadNullDelimitedString();

            Pointer.Goto(ref reader, original);
        }
        else // *(*(dialogStartOffset + 8 * currentLanguageID) + 4 * index);
        {
            Pointer offset = off_dialogStartOffset + (8 * currentLanguageId);
            Pointer original = Pointer.Goto(ref reader, offset);
            Pointer next = Pointer.Read(reader) + (4 * index);
            Pointer.Goto(ref reader, next);
            result = reader.ReadNullDelimitedString();

            Pointer.Goto(ref reader, original);
        }
        return result;
    }
}