namespace BinarySerializer.Ubisoft.CPA {
	public class IPT_CommandElementList : BinarySerializable, LST2_IEntry<IPT_CommandElementList> {
		public LST2_DynamicListElement<IPT_CommandElementList> ListElement { get; set; }
		public IPT_CommandElement Value { get; set; }

		public Pointer<IPT_CommandElementList> LST2_Next => ((LST2_IEntry<IPT_CommandElementList>)ListElement).LST2_Next;
		public Pointer<IPT_CommandElementList> LST2_Previous => ((LST2_IEntry<IPT_CommandElementList>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			ListElement = s.SerializeObject<LST2_DynamicListElement<IPT_CommandElementList>>(ListElement, name: nameof(ListElement));
			Value = s.SerializeObject<IPT_CommandElement>(Value, name: nameof(Value));
		}
	}
}
