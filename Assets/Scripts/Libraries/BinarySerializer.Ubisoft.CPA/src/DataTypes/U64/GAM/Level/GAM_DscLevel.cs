using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_DscLevel : U64_Struct {
		// public uint SoundThemeId { get; set; }
		public U64_Index<U64_Placeholder> BeginMapSoundEvent { get; set; }
		public ushort AlwaysCount { get; set; } // MaxNumberOfAlways
		// public ushort NumberOfTurningSectors { get; set; }
		// public ushort NumberOfMovingSurfaces { get; set; }
		// public ushort ReserveString { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			BeginMapSoundEvent = s.SerializeObject<U64_Index<U64_Placeholder>>(BeginMapSoundEvent, name: nameof(BeginMapSoundEvent));
			AlwaysCount = s.Serialize<ushort>(AlwaysCount, name: nameof(AlwaysCount));
		}
	}
}
