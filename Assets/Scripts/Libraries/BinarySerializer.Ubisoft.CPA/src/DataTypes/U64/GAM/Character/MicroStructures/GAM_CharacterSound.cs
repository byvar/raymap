using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_CharacterSound : U64_Struct {
		public FixedPointInt32 SaturationDistance { get; set; }
		public FixedPointInt32 BackgroundDistance { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			SaturationDistance = s.SerializeObject<FixedPointInt32>(SaturationDistance, onPreSerialize: d => d.Pre_PointPosition = 16, name: nameof(SaturationDistance));
			BackgroundDistance = s.SerializeObject<FixedPointInt32>(BackgroundDistance, onPreSerialize: d => d.Pre_PointPosition = 16, name: nameof(BackgroundDistance));
		}
	}
}
