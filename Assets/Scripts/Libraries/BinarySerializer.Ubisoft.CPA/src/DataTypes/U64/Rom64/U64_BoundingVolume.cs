using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Ubisoft.CPA.U64 {
    public class U64_BoundingVolume : U64_Struct {
        public CPA_Vector3D SphereCenter { get; set; }
        public float SphereRadius { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            SphereCenter = s.SerializeObject<CPA_Vector3D>(SphereCenter, name: nameof(SphereCenter));
            SphereRadius = s.Serialize<float>(SphereRadius, name: nameof(SphereRadius));
        }
    }
}
