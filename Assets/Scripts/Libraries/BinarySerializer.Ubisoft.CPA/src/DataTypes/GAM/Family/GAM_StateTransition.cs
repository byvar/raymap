namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_StateTransition : BinarySerializable, ILST2_StaticEntry<GAM_StateTransition> {
		public LST2_StaticListElement<GAM_StateTransition> ListElement { get; set; }
		public Pointer<GAM_State> TargetState { get; set; }
		public Pointer<GAM_State> StateToGo { get; set; }
		public GAM_StateTransitionLinkType LinkingType { get; set; }

		// LST2 Implementation
		public Pointer<LST2_StaticList<GAM_StateTransition>> LST2_Parent => ((ILST2_StaticEntry<GAM_StateTransition>)ListElement).LST2_Parent;
		public Pointer<GAM_StateTransition> LST2_Next => ((ILST2_Entry<GAM_StateTransition>)ListElement).LST2_Next;
		public Pointer<GAM_StateTransition> LST2_Previous => ((ILST2_Entry<GAM_StateTransition>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			ListElement = s.SerializeObject<LST2_StaticListElement<GAM_StateTransition>>(ListElement, name: nameof(ListElement));
			TargetState = s.SerializePointer<GAM_State>(TargetState, name: nameof(TargetState))?.ResolveObject(s);
			StateToGo = s.SerializePointer<GAM_State>(StateToGo, name: nameof(StateToGo))?.ResolveObject(s);
			LinkingType = s.Serialize<GAM_StateTransitionLinkType>(LinkingType, name: nameof(LinkingType));
			s.Align(4, Offset);
		}
	}
}
