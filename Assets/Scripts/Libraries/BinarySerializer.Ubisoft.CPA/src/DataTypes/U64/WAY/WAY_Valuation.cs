namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class WAY_Valuation : U64_Struct {
		public short Valuation { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Valuation = s.Serialize<short>(Valuation, name: nameof(Valuation));
		}
	}
}
