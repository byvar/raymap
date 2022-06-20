namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GLI_VignettesCount : U64_Struct {
		public ushort VignettesCount { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			VignettesCount = s.Serialize<ushort>(VignettesCount, name: nameof(VignettesCount));
		}
	}
}
