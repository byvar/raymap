namespace BinarySerializer.Ubisoft.CPA {
	public class AI_DsgMem : BinarySerializable {
		public Pointer<Pointer<AI_DsgVar>> DsgVar { get; set; } // Pointer to DsgVar Pointer in AI model. NOT a pointer to the start of a struct!
		public Pointer InitBuffer { get; set; } // Init values for this perso
		public Pointer Buffer { get; set; } // Current values for this perso

		public override void SerializeImpl(SerializerObject s) {
			DsgVar = s.SerializePointer(DsgVar, name: nameof(DsgVar))?.ResolvePointer<AI_DsgVar>(s);
			InitBuffer = s.SerializePointer(InitBuffer, name: nameof(InitBuffer));
			Buffer = s.SerializePointer(Buffer, name: nameof(Buffer));

			DsgVar?.Value?.ResolveObject(s);
		}
	}
}
