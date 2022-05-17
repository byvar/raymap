namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class AI_Node_Long : U64_Struct {
		public int Value { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Value = s.Serialize<int>(Value, name: nameof(Value));
		}
	}
}
