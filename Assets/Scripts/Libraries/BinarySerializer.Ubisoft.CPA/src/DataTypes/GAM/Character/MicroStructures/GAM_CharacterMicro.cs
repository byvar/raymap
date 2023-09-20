namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_CharacterMicro : BinarySerializable {
		public Pointer<MAT_Transformation> MicroMatrix { get; set; }
		public bool IsActive { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			MicroMatrix = s.SerializePointer<MAT_Transformation>(MicroMatrix, name: nameof(MicroMatrix))?.ResolveObject(s);
			IsActive = s.Serialize<bool>(IsActive, name: nameof(IsActive));
		}
	}
}
