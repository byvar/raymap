using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class PO_Zoo : U64_Struct { // Converts to PO_PhysicalCollSet
		public U64_Reference<GEO_GeometricObject> GeometricObject { get; set; }
		public Type ZoneType { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			GeometricObject = s.SerializeObject<U64_Reference<GEO_GeometricObject>>(GeometricObject, name: nameof(GeometricObject))?.Resolve(s);
			ZoneType = s.Serialize<Type>(ZoneType, name: nameof(ZoneType));
		}

		public enum Type : ushort {
			None = 0,
			ZDR = 1,
			ZDE = 2,
			ZDD = 3,
			ZDM = 4
		}
	}
}
