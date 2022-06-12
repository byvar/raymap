using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class SCT_SectorSoundEventParam : U64_Struct {
		public uint SoundEvent { get; set; } // Sound Event ID

		public override void SerializeImpl(SerializerObject s) {
			SoundEvent = s.Serialize<uint>(SoundEvent, name: nameof(SoundEvent));
		}
	}
}
