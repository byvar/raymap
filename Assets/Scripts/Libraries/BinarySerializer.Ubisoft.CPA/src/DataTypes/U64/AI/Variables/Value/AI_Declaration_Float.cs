namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class AI_Declaration_Float : U64_Struct {
		public float Value { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Value = s.Serialize<float>(Value, name: nameof(Value));
		}
	}
}
