using System;

namespace BinarySerializer.Ubisoft.CPA {
	public abstract class USND_WaveFileIdObj : USND_IdObj {
		public uint Version { get; set; }
		public float Volume { get; set; }
		public USND_WaveFileFlags Flags { get; set; }

		public string ExternalFileName { get; set; }

		public USND_WaveFile WaveFile { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);

			Version = s.Serialize<uint>(Version, name: nameof(Version));
			Volume = s.Serialize<float>(Volume, name: nameof(Volume));

			if (Version >= 3) {
				Flags = s.Serialize<USND_WaveFileFlags>(Flags, name: nameof(Flags));
			} else {
				throw new NotImplementedException();
			}

			if (Flags.HasFlag(USND_WaveFileFlags.External)) {
				ExternalFileName = s.SerializeLengthPrefixedString<uint>(ExternalFileName, name: nameof(ExternalFileName));

				if (Version < 2) {
					throw new NotImplementedException();
				}

				if (ExternalFileName.EndsWith("HXB", StringComparison.InvariantCultureIgnoreCase)) {
					throw new NotImplementedException();
				} else {
					WaveFile = s.SerializeObject<USND_WaveFile>(WaveFile, name: nameof(WaveFile));
				}
			} else {
				throw new NotImplementedException();
			}
		}
	}
}