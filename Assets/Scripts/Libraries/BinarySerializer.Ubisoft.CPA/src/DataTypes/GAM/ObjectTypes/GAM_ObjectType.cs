namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_ObjectType : BinarySerializable {
		public LST2_DynamicList<GAM_ObjectTypeElement> FamilyTypes { get; set; }
		public LST2_DynamicList<GAM_ObjectTypeElement> ModelTypes { get; set; }
		public LST2_DynamicList<GAM_ObjectTypeElement> PersonalTypes { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			FamilyTypes = s.SerializeObject<LST2_DynamicList<GAM_ObjectTypeElement>>(FamilyTypes, name: nameof(FamilyTypes));
			ModelTypes = s.SerializeObject<LST2_DynamicList<GAM_ObjectTypeElement>>(ModelTypes, name: nameof(ModelTypes));
			PersonalTypes = s.SerializeObject<LST2_DynamicList<GAM_ObjectTypeElement>>(PersonalTypes, name: nameof(PersonalTypes));
		}
		public GAM_ObjectType RepairLists(SerializerObject s) {
			FamilyTypes?.Validate(s);
			ModelTypes?.Validate(s);
			PersonalTypes?.Validate(s);

			return this;
		}

		public GAM_ObjectType Resolve(SerializerObject s) {
			FamilyTypes?.Resolve(s, name: nameof(FamilyTypes));
			ModelTypes?.Resolve(s, name: nameof(ModelTypes));
			PersonalTypes?.Resolve(s, name: nameof(PersonalTypes));

			return this;
		}
	}
}
