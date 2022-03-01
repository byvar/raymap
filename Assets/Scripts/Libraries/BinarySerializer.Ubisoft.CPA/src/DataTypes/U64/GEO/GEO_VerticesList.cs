using BinarySerializer.N64;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GEO_VerticesList : U64_Struct {
		public uint Pre_Size { get; set; }

		public GSP_Vertex[] Vertices { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().Platform == Platform.N64) {
				Vertices = s.SerializeObjectArrayUntil<GSP_Vertex>(Vertices,
					_ => s.CurrentAbsoluteOffset >= Offset.AbsoluteOffset + Pre_Size,
					name: nameof(Vertices));
			} else {
				throw new BinarySerializableException(this, $"Trying to serialize {GetType()} for unimplemented platform!");
			}
		}
	}
}
