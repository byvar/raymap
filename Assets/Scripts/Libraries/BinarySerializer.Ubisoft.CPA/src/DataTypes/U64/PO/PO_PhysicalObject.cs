using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class PO_PhysicalObject : U64_Struct {
		public U64_Reference<GEO_GeometricObject> GeometricObject { get; set; }
		public U64_Reference<U64_Placeholder> CollisionSet { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			GeometricObject = s.SerializeObject<U64_Reference<GEO_GeometricObject>>(GeometricObject, name: nameof(GeometricObject))?.Resolve(s);
			CollisionSet = s.SerializeObject<U64_Reference<U64_Placeholder>>(CollisionSet, name: nameof(CollisionSet));
		}
	}
}
