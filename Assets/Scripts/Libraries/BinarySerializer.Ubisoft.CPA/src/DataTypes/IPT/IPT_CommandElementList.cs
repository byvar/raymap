namespace BinarySerializer.Ubisoft.CPA {
	public class IPT_CommandElementList : BinarySerializable, ILST2_DynamicEntry<IPT_CommandElementList> {
		public LST2_DynamicListElement<IPT_CommandElementList> ListElement { get; set; }
		public IPT_CommandElement Value { get; set; }

		// LST2 Implementation
		public Pointer<LST2_DynamicList<IPT_CommandElementList>> LST2_Parent => ListElement?.LST2_Parent;
		public Pointer<IPT_CommandElementList> LST2_Next => ListElement?.LST2_Next;
		public Pointer<IPT_CommandElementList> LST2_Previous => ListElement?.LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			ListElement = s.SerializeObject<LST2_DynamicListElement<IPT_CommandElementList>>(ListElement, name: nameof(ListElement))?.Resolve(s);
			Value = s.SerializeObject<IPT_CommandElement>(Value, name: nameof(Value));
		}
	}
}
