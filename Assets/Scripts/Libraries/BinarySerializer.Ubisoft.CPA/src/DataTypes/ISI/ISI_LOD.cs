namespace BinarySerializer.Ubisoft.CPA {
	public class ISI_LOD : BinarySerializable {
		public ushort VertexRLICount { get; set; }
		public Pointer<ISI_Color[]> VertexRLI { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			VertexRLICount = s.Serialize<ushort>(VertexRLICount, name: nameof(VertexRLICount));
			s.Align(4, Offset);
			VertexRLI = s.SerializePointer<ISI_Color[]>(VertexRLI, name: nameof(VertexRLI))?.ResolveObjectArray(s, VertexRLICount);
		}
	}
}
