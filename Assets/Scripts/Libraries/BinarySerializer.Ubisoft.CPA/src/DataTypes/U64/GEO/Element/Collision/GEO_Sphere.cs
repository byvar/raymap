using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GEO_Sphere : U64_Struct {
		public float Radius { get; set; }
		public U64_Reference<GMT_GameMaterial> Material { get; set; }
		public ushort CenterPointIndex { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Radius = s.Serialize<float>(Radius, name: nameof(Radius));
			Material = s.SerializeObject<U64_Reference<GMT_GameMaterial>>(Material, name: nameof(Material))?.Resolve(s);
			CenterPointIndex = s.Serialize<ushort>(CenterPointIndex, name: nameof(CenterPointIndex));
		}
	}
}
