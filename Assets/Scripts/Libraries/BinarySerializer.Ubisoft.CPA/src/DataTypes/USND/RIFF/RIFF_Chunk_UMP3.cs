using BinarySerializer.Audio.RIFF;

namespace BinarySerializer.Ubisoft.CPA {
	public class RIFF_Chunk_UMP3 : RIFF_ChunkData {
		public override string ChunkIdentifier => "ump3";

		public uint MP3Offset { get; set; }
		public uint PCMOriginalDataSize { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			MP3Offset = s.Serialize<uint>(MP3Offset, name: nameof(MP3Offset));
			PCMOriginalDataSize = s.Serialize<uint>(PCMOriginalDataSize, name: nameof(PCMOriginalDataSize));
		}
	}
}