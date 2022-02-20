using System;

namespace BinarySerializer.Ubisoft.CPA.ROM
{
    public class LDR_FatTable : BinarySerializable
    {
        public uint Pre_ObjectsCount { get; set; }
        public Pointer Pre_DataPointer { get; set; }

        public LDR_EntryRef[] Entries { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
			Entries = s.SerializeObjectArray<LDR_EntryRef>(Entries, Pre_ObjectsCount, name: nameof(Entries));

		}
    }
}