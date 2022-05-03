using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class AI_Rule : U64_Struct {
		public LST_List<AI_Node> Nodes { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Nodes = s.SerializeObject<LST_List<AI_Node>>(Nodes, name: nameof(Nodes))?.Resolve(s);
		}
	}
}
