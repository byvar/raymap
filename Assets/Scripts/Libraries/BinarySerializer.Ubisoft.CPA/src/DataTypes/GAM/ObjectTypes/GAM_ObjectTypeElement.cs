namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_ObjectTypeElement : BinarySerializable, ILST2_DynamicEntry<GAM_ObjectTypeElement> {
		public LST2_DynamicListElement<GAM_ObjectTypeElement> ListElement { get; set; }
		public Pointer<string> Name { get; set; }
		public byte ElementPriority { get; set; }
		public byte PermanentId { get; set; }

		// LST2 Implementation
		public Pointer<LST2_DynamicList<GAM_ObjectTypeElement>> LST2_Parent => ListElement?.LST2_Parent;
		public Pointer<GAM_ObjectTypeElement> LST2_Next => ListElement?.LST2_Next;
		public Pointer<GAM_ObjectTypeElement> LST2_Previous => ListElement?.LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			ListElement = s.SerializeObject<LST2_DynamicListElement<GAM_ObjectTypeElement>>(ListElement, name: nameof(ListElement))?.Resolve(s);
			Name = s.SerializePointer<string>(Name, name: nameof(Name))?.ResolveString(s);
			ElementPriority = s.Serialize<byte>(ElementPriority, name: nameof(ElementPriority));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				PermanentId = s.Serialize<byte>(PermanentId, name: nameof(PermanentId));
			}
			s.Align(4, Offset);
		}
	}
}
