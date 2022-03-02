using BinarySerializer.N64;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GEO_GraphicsList : U64_Struct {
		public long Pre_Size { get; set; }

		public RSP_Command[] Commands { get; set; }
		public byte[] Bytes { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().Platform == Platform.N64) {
				Commands = s.SerializeObjectArray<RSP_Command>(Commands, Pre_Size / RSP_Command.StructSize, name: nameof(Commands));
			} else {
				s.LogWarning($"TODO: Parse GEO_GraphicsList on DS");
				Bytes = s.SerializeArray<byte>(Bytes, Pre_Size, name: nameof(Bytes));
			}
		}
	}
}
