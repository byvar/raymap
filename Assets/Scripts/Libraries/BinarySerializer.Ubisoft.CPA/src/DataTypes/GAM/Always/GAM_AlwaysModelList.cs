namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_AlwaysModelList : BinarySerializable, ILST2_DynamicEntry<GAM_AlwaysModelList> {
		public LST2_DynamicListElement<GAM_AlwaysModelList> ListElement { get; set; }

		// LST2 Implementation
		public Pointer<LST2_DynamicList<GAM_AlwaysModelList>> LST2_Parent => ListElement?.LST2_Parent;
		public Pointer<GAM_AlwaysModelList> LST2_Next => ListElement?.LST2_Next;
		public Pointer<GAM_AlwaysModelList> LST2_Previous => ListElement?.LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			ListElement = s.SerializeObject<LST2_DynamicListElement<GAM_AlwaysModelList>>(ListElement, name: nameof(ListElement))?.Resolve(s);
			throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");
		}
	}
}
