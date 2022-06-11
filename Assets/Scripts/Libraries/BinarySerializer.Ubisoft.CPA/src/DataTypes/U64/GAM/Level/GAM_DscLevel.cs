using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_DscLevel : U64_Struct {
		// public uint SoundThemeId { get; set; }
		public ushort BeginMapSoundEventId { get; set; }
		public ushort AlwaysCount { get; set; } // MaxNumberOfAlways
		// public ushort NumberOfTurningSectors { get; set; }
		// public ushort NumberOfMovingSurfaces { get; set; }
		// public ushort ReserveString { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			BeginMapSoundEventId = s.Serialize<ushort>(BeginMapSoundEventId, name: nameof(BeginMapSoundEventId));
			AlwaysCount = s.Serialize<ushort>(AlwaysCount, name: nameof(AlwaysCount));
		}
	}
}
