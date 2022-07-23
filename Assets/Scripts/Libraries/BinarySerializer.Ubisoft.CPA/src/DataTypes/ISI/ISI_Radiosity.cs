namespace BinarySerializer.Ubisoft.CPA {
	public class ISI_Radiosity : BinarySerializable {
		public ushort LODCount { get; set; }
		public Pointer<ISI_LOD[]> LOD { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LODCount = s.Serialize<ushort>(LODCount, name: nameof(LODCount));
			s.Align(4, Offset);
			LOD = s.SerializePointer<ISI_LOD[]>(LOD, name: nameof(LOD))?.ResolveObjectArray(s, LODCount);
		}
	}
}
