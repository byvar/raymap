namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class AI_Declaration_UnsignedLong : U64_Struct {
		public uint Value { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Value = s.Serialize<uint>(Value, name: nameof(Value));
		}
	}
}
