namespace BinarySerializer.Ubisoft.CPA {
	public class AI_DsgMem : BinarySerializable {
		public Pointer<Pointer<AI_DsgVar>> DsgVar { get; set; } // Pointer to DsgVar Pointer in AI model. NOT a pointer to the start of a struct!
		public Pointer<AI_DsgVarBuffer> InitBuffer { get; set; } // Init values for this perso
		public Pointer<AI_DsgVarBuffer> Buffer { get; set; } // Current values for this perso

		public AI_DsgVarValue[] InitValues { get; set; }
		public AI_DsgVarValue[] Values { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			DsgVar = s.SerializePointer(DsgVar, name: nameof(DsgVar))?.ResolvePointer<AI_DsgVar>(s);
			InitBuffer = s.SerializePointer<AI_DsgVarBuffer>(InitBuffer, name: nameof(InitBuffer));
			Buffer = s.SerializePointer<AI_DsgVarBuffer>(Buffer, name: nameof(Buffer));

			DsgVar?.Value?.ResolveObject(s);
			InitBuffer?.ResolveObject(s, onPreSerialize: b => b.Pre_DsgVar = DsgVar?.Value?.Value);
			Buffer?.ResolveObject(s, onPreSerialize: b => {
				b.Pre_DsgVar = DsgVar?.Value?.Value;
				b.Pre_Read = s?.GetCPASettings().Platform != Platform.DC; // Current MemBuffer is cleared in DC files
			});
		}
	}
}
