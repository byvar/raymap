using System;

namespace BinarySerializer.Ubisoft.CPA.U64
{
    public class LDR_EntryRef : BinarySerializable
    {
        public uint Address { get; set; }
        public ushort Type { get; set; }
        public ushort Index { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
			Address = s.Serialize<uint>(Address, name: nameof(Address));
			Type = s.Serialize<ushort>(Type, name: nameof(Type));
			Index = s.Serialize<ushort>(Index, name: nameof(Index));
		}

		public override bool UseShortLog => true;
		public override string ShortLog => $"LDR_EntryRef({U64_StructType_Defines.GetType(Context, Type)?.ToString() ?? Type.ToString()}, {Index}, {Context.GetLoader().GetStructPointer(this)})";


        public static string DataKey => "data";
	}
}