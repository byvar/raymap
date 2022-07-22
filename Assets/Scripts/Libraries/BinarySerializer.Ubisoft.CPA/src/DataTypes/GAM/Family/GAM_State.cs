namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_State : BinarySerializable, ILST2_StaticEntry<GAM_State> {
		public string StateName { get; set; }
		public LST2_StaticListElement<GAM_State> ListElement { get; set; }
		public Pointer Animation { get; set; }

		// LST2 Implementation
		public Pointer<LST2_StaticList<GAM_State>> LST2_Parent => ((ILST2_StaticEntry<GAM_State>)ListElement).LST2_Parent;
		public Pointer<GAM_State> LST2_Next => ((ILST2_Entry<GAM_State>)ListElement).LST2_Next;
		public Pointer<GAM_State> LST2_Previous => ((ILST2_Entry<GAM_State>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().HasNames)
				StateName = s.SerializeString(StateName, length: 80, name: nameof(StateName));

			ListElement = s.SerializeObject<LST2_StaticListElement<GAM_State>>(ListElement, name: nameof(ListElement));

			Animation = s.SerializePointer(Animation, name: nameof(Animation));

			throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");
		}
	}
}
