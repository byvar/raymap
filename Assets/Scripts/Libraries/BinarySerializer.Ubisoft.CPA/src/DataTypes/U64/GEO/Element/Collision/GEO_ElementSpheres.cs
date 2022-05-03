using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GEO_ElementSpheres : U64_Struct {
		public LST_List<GEO_Sphere> Spheres { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Spheres = s.SerializeObject<LST_List<GEO_Sphere>>(Spheres, name: nameof(Spheres))?.Resolve(s);
		}
	}
}
