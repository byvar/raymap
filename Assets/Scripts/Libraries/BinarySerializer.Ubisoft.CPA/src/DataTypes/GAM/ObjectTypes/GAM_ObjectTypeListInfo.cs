namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_ObjectTypeListInfo : BinarySerializable {
		public Pointer<GAM_ObjectTypeElement> FirstFamilyTypeInLevel { get; set; }
		public Pointer<GAM_ObjectTypeElement> FirstModelTypeInLevel { get; set; }
		public Pointer<GAM_ObjectTypeElement> FirstPersonalTypeInLevel { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			FirstFamilyTypeInLevel = s.SerializePointer<GAM_ObjectTypeElement>(FirstFamilyTypeInLevel, name: nameof(FirstFamilyTypeInLevel));
			FirstModelTypeInLevel = s.SerializePointer<GAM_ObjectTypeElement>(FirstModelTypeInLevel, name: nameof(FirstModelTypeInLevel));
			FirstPersonalTypeInLevel = s.SerializePointer<GAM_ObjectTypeElement>(FirstPersonalTypeInLevel, name: nameof(FirstPersonalTypeInLevel));
		}
	}
}
