namespace BinarySerializer.Ubisoft.CPA {
	public class SCT_Character : BinarySerializable, ILST2_DynamicEntry<SCT_Character> {
		public Pointer<HIE_SuperObject> Character { get; set; }
		public LST2_DynamicListElement<SCT_Character> ListElement { get; set; }

		// LST2 Implementation
		public Pointer<LST2_DynamicList<SCT_Character>> LST2_Parent => ((ILST2_DynamicEntry<SCT_Character>)ListElement).LST2_Parent;
		public Pointer<SCT_Character> LST2_Next => ((ILST2_Entry<SCT_Character>)ListElement).LST2_Next;
		public Pointer<SCT_Character> LST2_Previous => ((ILST2_Entry<SCT_Character>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			Character = s.SerializePointer<HIE_SuperObject>(Character, name: nameof(Character))?.ResolveObject(s);
			ListElement = s.SerializeObject<LST2_DynamicListElement<SCT_Character>>(ListElement, name: nameof(ListElement))?.Resolve(s);
		}
	}
}
