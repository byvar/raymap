namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_SubAnimation : BinarySerializable, ILST2_StaticEntry<GAM_SubAnimation> {
		public LST2_StaticListElement<GAM_SubAnimation> ListElement { get; set; }
		public Pointer<A3D_Animation> Animation { get; set; }

		// LST2 Implementation
		public Pointer<LST2_StaticList<GAM_SubAnimation>> LST2_Parent => ((ILST2_StaticEntry<GAM_SubAnimation>)ListElement).LST2_Parent;
		public Pointer<GAM_SubAnimation> LST2_Next => ((ILST2_Entry<GAM_SubAnimation>)ListElement).LST2_Next;
		public Pointer<GAM_SubAnimation> LST2_Previous => ((ILST2_Entry<GAM_SubAnimation>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			ListElement = s.SerializeObject<LST2_StaticListElement<GAM_SubAnimation>>(ListElement, name: nameof(ListElement));
			Animation = s.SerializePointer<A3D_Animation>(Animation, name: nameof(Animation))
				?.Resolve(s, (s, val, name) => {
					if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.TonicTrouble)) {
						return s.SerializeObject<A3D_Animation_A3I>((A3D_Animation_A3I)val, name: name);
					} else {
						return s.SerializeObject<A3D_Animation_A3B>((A3D_Animation_A3B)val, name: name);
					}
				});
		}
	}
}
