namespace BinarySerializer.Ubisoft.CPA {
	public class GLI_Light : BinarySerializable {
		public bool IsOn { get; set; }
		public bool IsZBuffered { get; set; }
		public GLI_LightType Type { get; set; }
		public float Far { get; set; }
		public float Near { get; set; }
		public float LittleAlpha { get; set; }
		public float BigAlpha { get; set; }
		public float LittleTangent { get; set; }
		public float BigTangent { get; set; }
		public MAT_Transformation Matrix { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			s.DoBits<uint>(b => {
				IsOn = b.SerializeBits<bool>(IsOn, 1, name: nameof(IsOn));
				b.SerializePadding(31, logIfNotNull: true);
			});
			s.DoBits<uint>(b => {
				IsZBuffered = b.SerializeBits<bool>(IsZBuffered, 1, name: nameof(IsZBuffered));
				b.SerializePadding(31, logIfNotNull: true);
			});
			Type = s.Serialize<GLI_LightType>(Type, name: nameof(Type));
			Far = s.Serialize<float>(Far, name: nameof(Far));
			Near = s.Serialize<float>(Near, name: nameof(Near));
			LittleAlpha = s.Serialize<float>(LittleAlpha, name: nameof(LittleAlpha));
			BigAlpha = s.Serialize<float>(BigAlpha, name: nameof(BigAlpha));
			LittleTangent = s.Serialize<float>(LittleTangent, name: nameof(LittleTangent));
			BigTangent = s.Serialize<float>(BigTangent, name: nameof(BigTangent));
			Matrix = s.SerializeObject<MAT_Transformation>(Matrix, name: nameof(Matrix));

			throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");

		}
	}
}
