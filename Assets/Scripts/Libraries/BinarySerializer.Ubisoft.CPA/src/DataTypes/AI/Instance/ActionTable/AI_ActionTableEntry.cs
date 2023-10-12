namespace BinarySerializer.Ubisoft.CPA {
	public class AI_ActionTableEntry : BinarySerializable {
		public AI_ActionParam ActionParam { get; set; }
		public Pointer<AI_NodeInterpret> Node { get; set; }
		public bool Used { get; set; }
		public byte RuleIndex { get; set; }
		public bool UseDefaultActionReturn { get; set; }
		public byte NewActionReturn { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			ActionParam = s.SerializeObject<AI_ActionParam>(ActionParam, name: nameof(ActionParam));
			Node = s.SerializePointer<AI_NodeInterpret>(Node, name: nameof(Node))?.ResolveObject(s);

			Used = s.Serialize<bool>(Used, name: nameof(Used));
			RuleIndex = s.Serialize<byte>(RuleIndex, name: nameof(RuleIndex));
			UseDefaultActionReturn = s.Serialize<bool>(UseDefaultActionReturn, name: nameof(UseDefaultActionReturn));
			NewActionReturn = s.Serialize<byte>(NewActionReturn, name: nameof(NewActionReturn));
		}
	}
}
