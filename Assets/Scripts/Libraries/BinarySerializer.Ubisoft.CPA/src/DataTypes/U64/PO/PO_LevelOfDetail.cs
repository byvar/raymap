using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	// VisualSets aren't used in R2 N64
	public class PO_LevelOfDetail : U64_Struct {
		public float Threshold { get; set; }
		public U64_Reference<GEO_GeometricObject> GeometricObject { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Threshold = s.Serialize<float>(Threshold, name: nameof(Threshold));
			GeometricObject = s.SerializeObject<U64_Reference<GEO_GeometricObject>>(GeometricObject, name: nameof(GeometricObject))?.Resolve(s);
			s.SerializePadding(2, logIfNotNull: true);
		}
	}
}
