using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GMT_CollideMaterial : U64_Struct {
		public ushort Identifier { get; set; }
		public GMT_ZoneType ZoneType { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Identifier = s.Serialize<ushort>(Identifier, name: nameof(Identifier));
			ZoneType = s.Serialize<GMT_ZoneType>(ZoneType, name: nameof(ZoneType));
		}
	}
}
