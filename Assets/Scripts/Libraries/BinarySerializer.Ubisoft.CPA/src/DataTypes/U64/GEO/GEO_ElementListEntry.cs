using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public abstract class GEO_ElementListEntry : U64_Struct {
		public U64_GenericReference Element { get; set; }
		public ushort BoundingVolume { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Element = s.SerializeObject<U64_GenericReference>(Element, name: nameof(Element))?.Resolve(s);
			BoundingVolume = s.Serialize<ushort>(BoundingVolume, name: nameof(BoundingVolume));
			s.SerializePadding(2, logIfNotNull: true);
		}
	}

	public class GEO_VisualElementListEntry : GEO_ElementListEntry { }
	public class GEO_CollisionElementListEntry : GEO_ElementListEntry { }

}
