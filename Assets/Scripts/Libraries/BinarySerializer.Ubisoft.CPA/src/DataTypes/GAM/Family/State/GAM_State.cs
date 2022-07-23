namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_State : BinarySerializable, ILST2_StaticEntry<GAM_State> {
		public string StateName { get; set; }
		public LST2_StaticListElement<GAM_State> ListElement { get; set; }
		public Pointer<A3D_Animation> Animation { get; set; }

		public LST2_StaticList<GAM_StateTransition> Transitions { get; set; }
		public LST2_StaticList<GAM_StateProhibit> ProhibitedStates { get; set; }
		public Pointer<GAM_State> NextState { get; set; }

		public Pointer<MEC_MechanicsIdCard> MechanicsIDCard { get; set; }

		// CPA_3: For cinematics
		public Pointer<string> LevelList { get; set; }
		public Pointer<string> AnimationSectionName { get; set; }

		public byte RepeatAnimation { get; set; } // How many time the animation must be repeated. The animation is played {RepeatAnimation+1} times.
		public sbyte SpeedAnimation { get; set; } // Animation playback speed in VBLs per frame
		public GAM_StateTransitionStatus TransitionStatusFlag { get; set; }
		public byte CustomBits { get; set; } // TODO: What are these?

		// TT/Montreal
		public uint LinkMechanics { get; set; }
		public uint TestPointsList { get; set; }
		public byte[] UnknownBytes0 { get; set; }
		public byte[] UnknownBytes1 { get; set; }

		// LST2 Implementation
		public Pointer<LST2_StaticList<GAM_State>> LST2_Parent => ((ILST2_StaticEntry<GAM_State>)ListElement).LST2_Parent;
		public Pointer<GAM_State> LST2_Next => ((ILST2_Entry<GAM_State>)ListElement).LST2_Next;
		public Pointer<GAM_State> LST2_Previous => ((ILST2_Entry<GAM_State>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().HasNames)
				StateName = s.SerializeString(StateName, length: 80, name: nameof(StateName));

			ListElement = s.SerializeObject<LST2_StaticListElement<GAM_State>>(ListElement, name: nameof(ListElement));

			Animation = s.SerializePointer<A3D_Animation>(Animation, name: nameof(Animation))
				?.ResolveValue(s, (s, val, name) => {
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

			MechanicsIDCard = s.SerializePointer<MEC_MechanicsIdCard>(MechanicsIDCard, name: nameof(MechanicsIDCard))?.ResolveObject(s);

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.RaymanM)) {
				LevelList = s.SerializePointer<string>(LevelList, name: nameof(LevelList))?.ResolveString(s);
				AnimationSectionName = s.SerializePointer<string>(AnimationSectionName, name: nameof(AnimationSectionName))?.ResolveString(s);
			}
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				RepeatAnimation = s.Serialize<byte>(RepeatAnimation, name: nameof(RepeatAnimation));
				SpeedAnimation = s.Serialize<sbyte>(SpeedAnimation, name: nameof(SpeedAnimation));
				TransitionStatusFlag = s.Serialize<GAM_StateTransitionStatus>(TransitionStatusFlag, name: nameof(TransitionStatusFlag));
				CustomBits = s.Serialize<byte>(CustomBits, name: nameof(CustomBits));
			} else {
				LinkMechanics = s.Serialize<uint>(LinkMechanics, name: nameof(LinkMechanics));
				TestPointsList = s.Serialize<uint>(TestPointsList, name: nameof(TestPointsList));
				UnknownBytes0 = s.SerializeArray<byte>(UnknownBytes0, 2, name: nameof(UnknownBytes0));
				RepeatAnimation = s.Serialize<byte>(RepeatAnimation, name: nameof(RepeatAnimation));
				SpeedAnimation = s.Serialize<sbyte>(SpeedAnimation, name: nameof(SpeedAnimation));
				TransitionStatusFlag = s.Serialize<GAM_StateTransitionStatus>(TransitionStatusFlag, name: nameof(TransitionStatusFlag));
				CustomBits = s.Serialize<byte>(CustomBits, name: nameof(CustomBits));
				UnknownBytes1 = s.SerializeArray<byte>(UnknownBytes1, 2, name: nameof(UnknownBytes1));
			}
		}
	}
}
