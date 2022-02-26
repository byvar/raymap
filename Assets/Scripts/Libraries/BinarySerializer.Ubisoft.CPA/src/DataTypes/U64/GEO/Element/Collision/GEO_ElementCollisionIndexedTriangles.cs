using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GEO_ElementCollisionIndexedTriangles : U64_Struct {
		public U64_GenericReference Material { get; set; }
		public U64_ArrayReference<GEO_TripledIndex> FacesTripledIndices { get; set; }
		public U64_Reference<U64_Placeholder> PointedSector { get; set; } // SuperObject
		public ushort FacesCount { get; set; }
		public ushort UVsCount { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Material = s.SerializeObject<U64_GenericReference>(Material, onPreSerialize: m => m.ImmediateSerializeType = U64_GenericReference.ImmediateSerialize.Index, name: nameof(Material));
			FacesTripledIndices = s.SerializeObject<U64_ArrayReference<GEO_TripledIndex>>(FacesTripledIndices, name: nameof(FacesTripledIndices));
			PointedSector = s.SerializeObject<U64_Reference<U64_Placeholder>>(PointedSector, name: nameof(PointedSector))?.Resolve(s);
			FacesCount = s.Serialize<ushort>(FacesCount, name: nameof(FacesCount));
			UVsCount = s.Serialize<ushort>(UVsCount, name: nameof(UVsCount));
			Material?.SerializeType(s);

			// Resolve
			FacesTripledIndices?.Resolve(s, FacesCount);
			Material?.Resolve(s);
		}
	}

}
