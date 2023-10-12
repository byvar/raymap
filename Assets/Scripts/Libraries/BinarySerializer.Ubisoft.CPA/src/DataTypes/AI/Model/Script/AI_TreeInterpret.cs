namespace BinarySerializer.Ubisoft.CPA {
	public class AI_TreeInterpret : BinarySerializable {
		public Pointer<AI_NodeInterpret[]> Nodes { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Nodes = s.SerializePointer<AI_NodeInterpret[]>(Nodes, name: nameof(Nodes))
				?.ResolveObjectArrayUntil(s, x => x.Depth == 0);
		}
	}
}
