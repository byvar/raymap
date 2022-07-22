namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_State : BinarySerializable, ILST2_StaticEntry<GAM_State> {
		public string StateName { get; set; }
		public LST2_StaticListElement<GAM_State> ListElement { get; set; }
		public Pointer<A3D_Animation> Animation { get; set; }

		public LST2_StaticList<GAM_StateTransition> Transitions { get; set; }
		public LST2_StaticList<GAM_StateProhibit> ProhibitedStates { get; set; }
		public Pointer<GAM_State> NextState { get; set; }

		// LST2 Implementation
		public Pointer<LST2_StaticList<GAM_State>> LST2_Parent => ((ILST2_StaticEntry<GAM_State>)ListElement).LST2_Parent;
		public Pointer<GAM_State> LST2_Next => ((ILST2_Entry<GAM_State>)ListElement).LST2_Next;
		public Pointer<GAM_State> LST2_Previous => ((ILST2_Entry<GAM_State>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().HasNames)
				StateName = s.SerializeString(StateName, length: 80, name: nameof(StateName));

			ListElement = s.SerializeObject<LST2_StaticListElement<GAM_State>>(ListElement, name: nameof(ListElement));

			Animation = s.SerializePointer<A3D_Animation>(Animation, name: nameof(Animation))
				?.Resolve(s, (s, val, name) => {
					if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.TonicTrouble)) {
						return s.SerializeObject<A3D_Animation_A3I>((A3D_Animation_A3I)val, name: name);
					} else {
						return s.SerializeObject<A3D_Animation_A3B>((A3D_Animation_A3B)val, name: name);
					}
				});

			Transitions = s.SerializeObject<LST2_StaticList<GAM_StateTransition>>(Transitions, name: nameof(Transitions))
				?.Resolve(s, name: nameof(Transitions));

			ProhibitedStates = s.SerializeObject<LST2_StaticList<GAM_StateProhibit>>(ProhibitedStates, name: nameof(ProhibitedStates))
				?.Resolve(s, name: nameof(ProhibitedStates));

			NextState = s.SerializePointer<GAM_State>(NextState, nullValue: 0xFFFFFFFF, name: nameof(NextState))?.ResolveObject(s);

			throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");
		}
	}
}
