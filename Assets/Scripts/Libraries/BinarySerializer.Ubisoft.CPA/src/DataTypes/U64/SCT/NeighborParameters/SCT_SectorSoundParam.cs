using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class SCT_SectorSoundParam : U64_Struct {
		public int Volume { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Volume = s.Serialize<int>(Volume, name: nameof(Volume));
		}
	}
}
