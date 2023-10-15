namespace BinarySerializer.Ubisoft.CPA {
	public class AI_ListOfMacros : BinarySerializable {
		public Pointer<AI_Macro[]> Macros { get; set; }
		public uint MacrosCount { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Macros = s.SerializePointer<AI_Macro[]>(Macros, name: nameof(Macros));
			MacrosCount = s.Serialize<uint>(MacrosCount, name: nameof(MacrosCount));

			Macros?.ResolveObjectArray(s, MacrosCount);
		}
	}
}
