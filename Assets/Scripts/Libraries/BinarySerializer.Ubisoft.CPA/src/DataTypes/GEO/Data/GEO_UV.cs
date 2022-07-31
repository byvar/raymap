using System;

namespace BinarySerializer.Ubisoft.CPA {
	public class GEO_UV : BinarySerializable {
		public float U { get; set; }
		public float V { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			U = s.Serialize<float>(U, name: nameof(U));
			V = s.Serialize<float>(V, name: nameof(V));
		}
		public override bool UseShortLog => true;
		public override string ToString() => $"UV({U}, {V})";

		public GEO_UV() { }
		public GEO_UV(float u, float v) {
			U = u;
			V = v;
		}
	}
}
