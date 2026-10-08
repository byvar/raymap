using System;

namespace BinarySerializer.Ubisoft.CPA {
	public abstract class USND_WavResData : USND_ResData {
		public string Name { get; set; }

		// Unknown values - volume?
		public float Float1 { get; set; }
		public float Float2 { get; set; }
		public float Float3 { get; set; }

		public USND_WavResDataFlags Flags { get; set; }
		public USND_UUID FileId { get; set; } // Default language if localized, usually English
		public uint LocalizedFileIdsCount { get; set; }
		public LocalizedFileId[] LocalizedFileIds { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);

			if (Version >= 2) {
				Name = s.SerializeLengthPrefixedString<uint>(Name, name: nameof(Name));
			}

			Float1 = s.Serialize<float>(Float1, name: nameof(Float1));
			Float2 = s.Serialize<float>(Float2, name: nameof(Float2));
			Float3 = s.Serialize<float>(Float3, name: nameof(Float3));

			if (Version >= 3) {
				Flags = s.Serialize<USND_WavResDataFlags>(Flags, name: nameof(Flags));
			} else {
				throw new NotImplementedException();
			}

			FileId = s.SerializeObject<USND_UUID>(FileId, name: nameof(FileId));

			if (Version < 3) {
				throw new NotImplementedException();
			}

			if (Flags.HasFlag(USND_WavResDataFlags.Localized)) {
				LocalizedFileIdsCount = s.Serialize<uint>(LocalizedFileIdsCount, name: nameof(LocalizedFileIdsCount));
				LocalizedFileIds = s.SerializeObjectArray<LocalizedFileId>(LocalizedFileIds, LocalizedFileIdsCount, name: nameof(LocalizedFileIds));
			}
		}

		public class LocalizedFileId : BinarySerializable {
			public string LanguageCode { get; set; }
			public USND_UUID FileId { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				LanguageCode = s.SerializeString(LanguageCode, length: 4, name: nameof(LanguageCode));
				FileId = s.SerializeObject<USND_UUID>(FileId, name: nameof(FileId));
			}
		}
	}
}