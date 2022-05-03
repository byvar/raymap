using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class AI_Node : U64_Struct {
		public byte Type { get; set; }
		public byte Depth { get; set; }
		public short IdOrValue { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Type = s.Serialize<byte>(Type, name: nameof(Type));
			Depth = s.Serialize<byte>(Depth, name: nameof(Depth));
			IdOrValue = s.Serialize<short>(IdOrValue, name: nameof(IdOrValue));
		}

		public override bool UseShortLog => true;
		public override string ShortLog => ToString();

		public string ToString() {
			var nodeType = Context.GetCPASettings().AITypes.GetNodeType(Type);
			return $"{new string(' ',4*Depth)}{nodeType}_{IdOrValue}";
		}
	}
}
