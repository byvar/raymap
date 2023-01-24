namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_SubAnimationInUse : BinarySerializable, ILST2_DynamicEntry<GAM_SubAnimationInUse> {
		public LST2_DynamicListElement<GAM_SubAnimationInUse> ListElement { get; set; }
		public Pointer<GAM_SubAnimation> SubAnimation { get; set; }

		// LST2 Implementation
		public Pointer<LST2_DynamicList<GAM_SubAnimationInUse>> LST2_Parent => ((ILST2_DynamicEntry<GAM_SubAnimationInUse>)ListElement).LST2_Parent;
		public Pointer<GAM_SubAnimationInUse> LST2_Next => ((ILST2_Entry<GAM_SubAnimationInUse>)ListElement).LST2_Next;
		public Pointer<GAM_SubAnimationInUse> LST2_Previous => ((ILST2_Entry<GAM_SubAnimationInUse>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			ListElement = s.SerializeObject<LST2_DynamicListElement<GAM_SubAnimationInUse>>(ListElement, name: nameof(ListElement));
			SubAnimation = s.SerializePointer<GAM_SubAnimation>(SubAnimation, name: nameof(SubAnimation))
				?.ResolveObject(s);
			throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");
		}
	}
}
