namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Description_Color : SNA_Description_Data, ISerializerShortLog {
		public GLI_FloatColor_RGBA Value { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Value = s.SerializeObject<GLI_FloatColor_RGBA>(Value, name: nameof(Value));
		}
		public string ShortLog => Value.ToString();
	}
}
