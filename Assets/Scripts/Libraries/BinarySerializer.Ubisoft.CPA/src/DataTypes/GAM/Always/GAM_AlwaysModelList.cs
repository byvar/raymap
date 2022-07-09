namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_AlwaysModelList : BinarySerializable, LST2_IEntry<GAM_AlwaysModelList> {
		public LST2_DynamicListElement<GAM_AlwaysModelList> ListElement { get; set; }

		// List implementation
		public Pointer<GAM_AlwaysModelList> LST2_Next => ((LST2_IEntry<GAM_AlwaysModelList>)ListElement).LST2_Next;
		public Pointer<GAM_AlwaysModelList> LST2_Previous => ((LST2_IEntry<GAM_AlwaysModelList>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			ListElement = s.SerializeObject<LST2_DynamicListElement<GAM_AlwaysModelList>>(ListElement, name: nameof(ListElement));
			throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");
		}
	}
}
