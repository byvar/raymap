namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class WAY_Graph : U64_Struct {
		public LST_ReferenceList<WAY_GraphNode> Nodes { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Nodes = s.SerializeObject<LST_ReferenceList<WAY_GraphNode>>(Nodes, name: nameof(Nodes))?.Resolve(s);
		}
	}
}
