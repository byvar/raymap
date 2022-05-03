using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class AI_Rule : U64_Struct {
		public LST_List<U64_Placeholder> Nodes { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Nodes = s.SerializeObject<LST_List<U64_Placeholder>>(Nodes, name: nameof(Nodes))?.Resolve(s);
		}
	}
}
