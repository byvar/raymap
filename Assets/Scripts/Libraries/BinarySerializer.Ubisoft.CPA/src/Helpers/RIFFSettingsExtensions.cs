using BinarySerializer.Audio.RIFF;

namespace BinarySerializer.Ubisoft.CPA {
	public static class RIFFSettingsExtensions {
		public static void RegisterUSND(this RIFFSettings settings) {
			settings.RegisterChunkResolver("datx", (s, data, chunkSize, name) =>
				s.SerializeObject<RIFF_Chunk_DatX>((RIFF_Chunk_DatX)data, x => x.Pre_ChunkSize = chunkSize, name: name));
			settings.RegisterChunkResolver("labx", (s, data, chunkSize, name) =>
				s.SerializeObject<RIFF_Chunk_LabX>((RIFF_Chunk_LabX)data, x => x.Pre_ChunkSize = chunkSize, name: name));
			settings.RegisterChunkResolver("ump3", (s, data, chunkSize, name) =>
				s.SerializeObject<RIFF_Chunk_UMP3>((RIFF_Chunk_UMP3)data, x => x.Pre_ChunkSize = chunkSize, name: name));
		}
	}
}