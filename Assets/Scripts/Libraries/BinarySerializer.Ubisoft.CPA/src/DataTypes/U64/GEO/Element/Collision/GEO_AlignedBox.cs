using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GEO_AlignedBox : U64_Struct {
		public U64_Reference<GMT_GameMaterial> Material { get; set; }
		public ushort MinPointIndex { get; set; }
		public ushort MaxPointIndex { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Material = s.SerializeObject<U64_Reference<GMT_GameMaterial>>(Material, name: nameof(Material))?.Resolve(s);
			MinPointIndex = s.Serialize<ushort>(MinPointIndex, name: nameof(MinPointIndex));
			MaxPointIndex = s.Serialize<ushort>(MaxPointIndex, name: nameof(MaxPointIndex));
		}
	}
}
