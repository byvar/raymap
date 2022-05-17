namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class WAY_GraphNode : U64_Struct {
		public U64_Reference<WAY_WayPoint> WayPoint { get; set; }
		public U64_ArrayReference<LST_ReferenceElement<WAY_GraphNode>> Nodes { get; set; }
		public U64_ArrayReference<WAY_Capacity> Capacities { get; set; }
		public U64_ArrayReference<WAY_Valuation> Valuations { get; set; }
		public ushort NodesCount { get; set; }
		public U64_Reference<WAY_Capacity> TypeOfWP { get; set; } // Capacity

		public override void SerializeImpl(SerializerObject s) {
			WayPoint = s.SerializeObject<U64_Reference<WAY_WayPoint>>(WayPoint, name: nameof(WayPoint))?.Resolve(s);
			Nodes = s.SerializeObject<U64_ArrayReference<LST_ReferenceElement<WAY_GraphNode>>>(Nodes, name: nameof(Nodes));
			Capacities = s.SerializeObject<U64_ArrayReference<WAY_Capacity>>(Capacities, name: nameof(Capacities));
			Valuations = s.SerializeObject<U64_ArrayReference<WAY_Valuation>>(Valuations, name: nameof(Valuations));
			NodesCount = s.Serialize<ushort>(NodesCount, name: nameof(NodesCount));
			TypeOfWP = s.SerializeObject<U64_Reference<WAY_Capacity>>(TypeOfWP, name: nameof(TypeOfWP))?.Resolve(s);

			Nodes?.Resolve(s, NodesCount);
			Capacities?.Resolve(s, NodesCount);
			Valuations?.Resolve(s, NodesCount);
		}
	}
}
