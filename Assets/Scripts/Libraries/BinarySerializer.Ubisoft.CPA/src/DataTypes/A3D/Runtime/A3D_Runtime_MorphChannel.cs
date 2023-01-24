namespace BinarySerializer.Ubisoft.CPA {
	public class A3D_Runtime_MorphChannel : BinarySerializable, ILST2_DynamicEntry<A3D_Runtime_MorphChannel> {
		public LST2_DynamicListElement<A3D_Runtime_MorphChannel> ListElement { get; set; }
		public ushort ChannelIndex { get; set; }
		public ushort MorphListIndex { get; set; }

		// LST2 Implementation
		public Pointer<LST2_DynamicList<A3D_Runtime_MorphChannel>> LST2_Parent => ((ILST2_DynamicEntry<A3D_Runtime_MorphChannel>)ListElement).LST2_Parent;
		public Pointer<A3D_Runtime_MorphChannel> LST2_Next => ((ILST2_Entry<A3D_Runtime_MorphChannel>)ListElement).LST2_Next;
		public Pointer<A3D_Runtime_MorphChannel> LST2_Previous => ((ILST2_Entry<A3D_Runtime_MorphChannel>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			ListElement = s.SerializeObject<LST2_DynamicListElement<A3D_Runtime_MorphChannel>>(ListElement, name: nameof(ListElement))?.Resolve(s);
			ChannelIndex = s.Serialize<ushort>(ChannelIndex, name: nameof(ChannelIndex));
			MorphListIndex = s.Serialize<ushort>(MorphListIndex, name: nameof(MorphListIndex));
		}
	}
}