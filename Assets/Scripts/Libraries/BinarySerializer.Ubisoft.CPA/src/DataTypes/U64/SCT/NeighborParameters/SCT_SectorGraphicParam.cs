using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class SCT_SectorGraphicParam : U64_Struct {
		public short LevelOfDetail { get; set; }
		public short DisplayMode { get; set; } // Maybe not included in TT?

		public override void SerializeImpl(SerializerObject s) {
			LevelOfDetail = s.Serialize<short>(LevelOfDetail, name: nameof(LevelOfDetail));
			DisplayMode = s.Serialize<short>(DisplayMode, name: nameof(DisplayMode));
		}
	}
}
