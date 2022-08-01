namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_AlwaysActiveCharacter : BinarySerializable, ILST2_DynamicEntry<GAM_AlwaysActiveCharacter> {
		public Pointer<HIE_SuperObject> AlwaysActiveSuperObject { get; set; } 
		public LST2_DynamicListElement<GAM_AlwaysActiveCharacter> ListElement { get; set; }
		public bool DynamicAlwaysActive { get; set; }

		// LST2 Implementation
		public Pointer<LST2_DynamicList<GAM_AlwaysActiveCharacter>> LST2_Parent => ((ILST2_DynamicEntry<GAM_AlwaysActiveCharacter>)ListElement).LST2_Parent;
		public Pointer<GAM_AlwaysActiveCharacter> LST2_Next => ((ILST2_Entry<GAM_AlwaysActiveCharacter>)ListElement).LST2_Next;
		public Pointer<GAM_AlwaysActiveCharacter> LST2_Previous => ((ILST2_Entry<GAM_AlwaysActiveCharacter>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				ListElement = s.SerializeObject<LST2_DynamicListElement<GAM_AlwaysActiveCharacter>>(ListElement, name: nameof(ListElement))?.Resolve(s);
				AlwaysActiveSuperObject = s.SerializePointer<HIE_SuperObject>(AlwaysActiveSuperObject, name: nameof(AlwaysActiveSuperObject))?.ResolveObject(s);
				DynamicAlwaysActive = s.Serialize<bool>(DynamicAlwaysActive, name: nameof(DynamicAlwaysActive));
				s.Align(4, Offset);
			} else {
				AlwaysActiveSuperObject = s.SerializePointer<HIE_SuperObject>(AlwaysActiveSuperObject, name: nameof(AlwaysActiveSuperObject))?.ResolveObject(s);
				ListElement = s.SerializeObject<LST2_DynamicListElement<GAM_AlwaysActiveCharacter>>(ListElement, name: nameof(ListElement))?.Resolve(s);
			}
		}
	}
}
