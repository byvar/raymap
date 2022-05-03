using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_ZdxIndex : U64_Struct {
		public short Index { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Index = s.Serialize<short>(Index, name: nameof(Index));
		}
	}
}
