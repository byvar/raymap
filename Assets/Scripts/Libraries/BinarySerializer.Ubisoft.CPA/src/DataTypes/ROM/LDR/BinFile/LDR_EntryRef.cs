using System;

namespace BinarySerializer.Ubisoft.CPA.ROM
{
    public class LDR_EntryRef : BinarySerializable
    {
        //public Pointer Address { get; set; }
        public uint Address { get; set; }
        public ushort Type { get; set; }
        public ushort Index { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            var anchor = Context.GetStoredObject<LDR_FatFile>(LDR_FatFile.ContextKey).Pre_DataPointer;
			//Address = s.SerializePointer(Address, anchor: anchor, name: nameof(Address));
			Address = s.Serialize<uint>(Address, name: nameof(Address));
			Type = s.Serialize<ushort>(Type, name: nameof(Type));
			Index = s.Serialize<ushort>(Index, name: nameof(Index));
		}

		public override bool UseShortLog => true;
		public override string ShortLog => $"LDR_EntryRef({CPA_ROM_StructType_Defines.GetType(Context, Type).ToString() ?? Type.ToString()}, {Index}, {Context.GetStoredObject<LDR_FatFile>(LDR_FatFile.ContextKey).Pre_DataPointer + Address})";


        public static string DataKey => "data";
	}
}