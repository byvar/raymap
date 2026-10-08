using BinarySerializer.Audio.RIFF;

namespace BinarySerializer.Ubisoft.CPA {
	// External data chunk
	public class RIFF_Chunk_DatX : RIFF_ChunkData {
		public override string ChunkIdentifier => "datx";

		public uint SizeInExternalFile { get; set; }
		public uint OffsetInExternalFile { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			SizeInExternalFile = s.Serialize<uint>(SizeInExternalFile, name: nameof(SizeInExternalFile));
			OffsetInExternalFile = s.Serialize<uint>(OffsetInExternalFile, name: nameof(OffsetInExternalFile));
		}
	}
}