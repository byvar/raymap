namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_MainCharacter : BinarySerializable, ILST2_DynamicEntry<GAM_MainCharacter> {
		public Pointer<HIE_SuperObject> NewCharacterForTheNextFrame { get; set; }
		public Pointer<HIE_SuperObject> Character { get; set; }
		public LST2_DynamicListElement<GAM_MainCharacter> ListElement { get; set; }

		// LST2 Implementation
		public Pointer<LST2_DynamicList<GAM_MainCharacter>> LST2_Parent => ((ILST2_DynamicEntry<GAM_MainCharacter>)ListElement).LST2_Parent;
		public Pointer<GAM_MainCharacter> LST2_Next => ((ILST2_Entry<GAM_MainCharacter>)ListElement).LST2_Next;
		public Pointer<GAM_MainCharacter> LST2_Previous => ((ILST2_Entry<GAM_MainCharacter>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			NewCharacterForTheNextFrame = s.SerializePointer<HIE_SuperObject>(NewCharacterForTheNextFrame, name: nameof(NewCharacterForTheNextFrame))?.ResolveObject(s);
			Character = s.SerializePointer<HIE_SuperObject>(Character, name: nameof(Character))?.ResolveObject(s);
			ListElement = s.SerializeObject<LST2_DynamicListElement<GAM_MainCharacter>>(ListElement, name: nameof(ListElement))?.Resolve(s);
		}
	}
}
