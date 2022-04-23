namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class GEO_DeformationUnknown : BinarySerializable {
		public ushort VertexIndex { get; set; }
		public ushort UShort_02 { get; set; }

		public short[] Unknown { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			VertexIndex = s.Serialize<ushort>(VertexIndex, name: nameof(VertexIndex));
			UShort_02 = s.Serialize<ushort>(UShort_02, name: nameof(UShort_02));

			Unknown = s.SerializeArray<short>(Unknown, 4, name: nameof(Unknown));
		}
	}
}