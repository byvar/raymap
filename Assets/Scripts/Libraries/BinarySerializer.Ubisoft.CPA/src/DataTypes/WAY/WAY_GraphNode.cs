namespace BinarySerializer.Ubisoft.CPA {
	public class WAY_GraphNode : BinarySerializable, ILST2_DynamicEntry<WAY_GraphNode> {
		public LST2_DynamicListElement<WAY_GraphNode> ListElement { get; set; }

		public Pointer<WAY_WayPoint> WayPoint { get; set; }
		public GAM_ActorCapabilities TypeOfWP { get; set; }
		public GAM_ActorCapabilities TypeOfWPInit { get; set; }

		public Pointer<WAY_ArcList> ArcList { get; set; }

		// LST2 Implementation
		public Pointer<LST2_DynamicList<WAY_GraphNode>> LST2_Parent => ((ILST2_DynamicEntry<WAY_GraphNode>)ListElement).LST2_Parent;
		public Pointer<WAY_GraphNode> LST2_Next => ((ILST2_Entry<WAY_GraphNode>)ListElement).LST2_Next;
		public Pointer<WAY_GraphNode> LST2_Previous => ((ILST2_Entry<WAY_GraphNode>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			ListElement = s.SerializeObject<LST2_DynamicListElement<WAY_GraphNode>>(ListElement, name: nameof(ListElement))?.Resolve(s);
			
			WayPoint = s.SerializePointer<WAY_WayPoint>(WayPoint, name: nameof(WayPoint))?.ResolveObject(s);
			TypeOfWP = s.Serialize<GAM_ActorCapabilities>(TypeOfWP, name: nameof(TypeOfWP));
			TypeOfWPInit = s.Serialize<GAM_ActorCapabilities>(TypeOfWPInit, name: nameof(TypeOfWPInit));

			ArcList = s.SerializePointer<WAY_ArcList>(ArcList, name: nameof(ArcList))?.ResolveObject(s);
		}
	}
}
