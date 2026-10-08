using System.Text;

namespace BinarySerializer.Ubisoft.CPA {
	public class USND_WaveFile : BinarySerializable {
		public uint ChunksDataSize { get; set; }
		public byte[] ChunksData { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			s.SerializeMagicString("RIFF", 4, Encoding.ASCII);
			ChunksDataSize = s.Serialize<uint>(ChunksDataSize, name: nameof(ChunksDataSize));
			ChunksData = s.SerializeArray<byte>(ChunksData, ChunksDataSize + 4, name: nameof(ChunksData));
		}
	}
}