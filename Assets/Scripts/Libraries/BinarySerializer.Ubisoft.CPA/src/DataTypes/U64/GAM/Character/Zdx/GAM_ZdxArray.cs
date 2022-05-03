using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_ZdxArray : U64_Struct {
		public LST_ReferenceList<GEO_GeometricObject> GeometricObjects { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			GeometricObjects = s.SerializeObject<LST_ReferenceList<GEO_GeometricObject>>(GeometricObjects, name: nameof(GeometricObjects))?.Resolve(s);
		}
	}
}
