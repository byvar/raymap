namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Description_Color : SNA_Description_Data {
		public GLI_FloatColor Value { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Value = s.SerializeObject<GLI_FloatColor>(Value, name: nameof(Value));
		}
	}
}
