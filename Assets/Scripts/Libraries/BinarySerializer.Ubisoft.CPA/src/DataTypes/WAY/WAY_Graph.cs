namespace BinarySerializer.Ubisoft.CPA {
	public class WAY_Graph : BinarySerializable {
		public LST2_DynamicList<WAY_GraphNode> Nodes { get; set; }
		public Pointer<string> Name { get; set; }
		public Pointer<string> ReferenceSection { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Nodes = s.SerializeObject<LST2_DynamicList<WAY_GraphNode>>(Nodes, name: nameof(Nodes))?.Resolve(s, name: nameof(Nodes));

			if(!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)
				&& s.GetCPASettings().Platform != Platform.PS2
				&& s.GetCPASettings().Platform != Platform.DC) {
				Name = s.SerializePointer<string>(Name, name: nameof(Name))?.ResolveString(s);
				ReferenceSection = s.SerializePointer<string>(ReferenceSection, name: nameof(ReferenceSection))?.ResolveString(s);
			}
		}
	}
}
