namespace BinarySerializer.Ubisoft.CPA {
	public class WAY_Arc : BinarySerializable, ILST2_DynamicEntry<WAY_Arc> {
		public LST2_DynamicListElement<WAY_Arc> ListElement { get; set; }
		public Pointer<WAY_GraphNode> GraphNode { get; set; }

		public GAM_ActorCapabilities Capabilities { get; set; }
		public GAM_ActorCapabilities CapabilitiesInit { get; set; }
		public uint Weight { get; set; } // Weight of the transition, always positive
		public uint WeightInit { get; set; }

		// LST2 Implementation
		public Pointer<LST2_DynamicList<WAY_Arc>> LST2_Parent => ((ILST2_DynamicEntry<WAY_Arc>)ListElement).LST2_Parent;
		public Pointer<WAY_Arc> LST2_Next => ((ILST2_Entry<WAY_Arc>)ListElement).LST2_Next;
		public Pointer<WAY_Arc> LST2_Previous => ((ILST2_Entry<WAY_Arc>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			ListElement = s.SerializeObject<LST2_DynamicListElement<WAY_Arc>>(ListElement, name: nameof(ListElement))?.Resolve(s);

			Capabilities = s.Serialize<GAM_ActorCapabilities>(Capabilities, name: nameof(Capabilities));
			CapabilitiesInit = s.Serialize<GAM_ActorCapabilities>(CapabilitiesInit, name: nameof(CapabilitiesInit));
			Weight = s.Serialize<uint>(Weight, name: nameof(Weight));
			WeightInit = s.Serialize<uint>(WeightInit, name: nameof(WeightInit));
		}
	}
}
