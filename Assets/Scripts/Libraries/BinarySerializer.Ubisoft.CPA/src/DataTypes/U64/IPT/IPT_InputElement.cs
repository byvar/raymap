namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class IPT_InputElement : U64_Struct {
		public short IndexOrKeyCode { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			IndexOrKeyCode = s.Serialize<short>(IndexOrKeyCode, name: nameof(IndexOrKeyCode));
		}
	}
}
