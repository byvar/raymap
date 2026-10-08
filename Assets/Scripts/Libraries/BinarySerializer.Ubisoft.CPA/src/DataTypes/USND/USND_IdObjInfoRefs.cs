using System;

namespace BinarySerializer.Ubisoft.CPA {
	public class USND_IdObjInfoRefs : BinarySerializable {
		public uint Pre_Version { get; set; }

		public uint ReferencesCount { get; set; }
		public USND_UUID[] References { get; set; }
		public uint LanguagesCount { get; set; }
		public USND_IdObjInfoRefsLanguage[] Languages { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			ReferencesCount = s.Serialize<uint>(ReferencesCount, name: nameof(ReferencesCount));
			References = s.SerializeObjectArray<USND_UUID>(References, ReferencesCount, name: nameof(References));

			if (Pre_Version >= 3) {
				throw new NotImplementedException();
			} else {
				LanguagesCount = s.Serialize<uint>(LanguagesCount, name: nameof(LanguagesCount));
				Languages = s.SerializeObjectArray<USND_IdObjInfoRefsLanguage>(Languages, LanguagesCount, name: nameof(Languages));
			}
		}
	}
}