namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_StateProhibit : BinarySerializable, ILST2_StaticEntry<GAM_StateProhibit> {
		public LST2_StaticListElement<GAM_StateProhibit> ListElement { get; set; }
		public Pointer<GAM_State> ProhibitedState { get; set; }

		// LST2 Implementation
		public Pointer<LST2_StaticList<GAM_StateProhibit>> LST2_Parent => ((ILST2_StaticEntry<GAM_StateProhibit>)ListElement).LST2_Parent;
		public Pointer<GAM_StateProhibit> LST2_Next => ((ILST2_Entry<GAM_StateProhibit>)ListElement).LST2_Next;
		public Pointer<GAM_StateProhibit> LST2_Previous => ((ILST2_Entry<GAM_StateProhibit>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			ListElement = s.SerializeObject<LST2_StaticListElement<GAM_StateProhibit>>(ListElement, name: nameof(ListElement));
			ProhibitedState = s.SerializePointer<GAM_State>(ProhibitedState, name: nameof(ProhibitedState))?.ResolveObject(s);
		}
	}
}
