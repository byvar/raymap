using BinarySerializer.Audio;
using System.Text;

namespace BinarySerializer.Ubisoft.CPA {
	public class USND_WaveFile : BinarySerializable {
		public WAV WaveFile { get; set; }
		public uint DataSize { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			WaveFile = s.SerializeObject<WAV>(WaveFile, name: nameof(WaveFile));
			if (s.GetCPASettings().Platform == Platform.PS2) {
				DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
			}
		}
	}
}