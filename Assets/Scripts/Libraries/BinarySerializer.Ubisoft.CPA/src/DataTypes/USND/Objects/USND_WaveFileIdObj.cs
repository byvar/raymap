using System;

namespace BinarySerializer.Ubisoft.CPA {
	public abstract class USND_WaveFileIdObj : USND_IdObj {
		public uint Version { get; set; }
		public float Volume { get; set; }
		public WaveFileFlags Flags { get; set; }

		public string ExternalFileName { get; set; }

		public USND_WaveFile WaveFile { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);

			Version = s.Serialize<uint>(Version, name: nameof(Version));
			Volume = s.Serialize<float>(Volume, name: nameof(Volume));

			if (Version >= 3) {
				Flags = s.Serialize<WaveFileFlags>(Flags, name: nameof(Flags));
			} else {
				throw new NotImplementedException();
			}

			if (Flags.HasFlag(WaveFileFlags.External)) {
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

		[Flags]
		public enum WaveFileFlags : byte {
			None = 0,
			External = 1 << 0,
			Flag1 = 1 << 1,
			Flag2 = 1 << 2,
			Flag3 = 1 << 3,
			Flag4 = 1 << 4,
			Flag5 = 1 << 5,
			Flag6 = 1 << 6,
			Flag7 = 1 << 7,
		}
	}
}