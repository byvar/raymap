namespace BinarySerializer.Ubisoft.CPA {
	public class PO_PhysicalObject : BinarySerializable, IGAM_ObjectsTableTarget {
		public Pointer<GEO_VisualSet> VisualSet { get; set; }
		public Pointer<GEO_GeometricObject> GeometricObject { get; set; }

		public Pointer<GAM_Placeholder> CollideSet { get; set; }

		public Pointer<GEO_BoundingSphere> BoundingVolume { get; set; }
		public Pointer<GEO_BoundingSphere> VisualBoundingVolume { get; set; }
		public Pointer<GEO_BoundingSphere> CollideBoundingVolume { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2Revolution)) {
				GeometricObject = s.SerializePointer<GEO_GeometricObject>(GeometricObject, name: nameof(GeometricObject))?.ResolveObject(s);
			} else {
				VisualSet = s.SerializePointer<GEO_VisualSet>(VisualSet, name: nameof(VisualSet))?.ResolveObject(s);
			}

			CollideSet = s.SerializePointer<GAM_Placeholder>(CollideSet, name: nameof(CollideSet))?.ResolveObject(s);

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				VisualBoundingVolume = s.SerializePointer<GEO_BoundingSphere>(VisualBoundingVolume, name: nameof(VisualBoundingVolume))?.ResolveObject(s);
				CollideBoundingVolume = s.SerializePointer<GEO_BoundingSphere>(CollideBoundingVolume, name: nameof(CollideBoundingVolume))?.ResolveObject(s);
			} else {
				BoundingVolume = s.SerializePointer<GEO_BoundingSphere>(BoundingVolume, name: nameof(BoundingVolume))?.ResolveObject(s);
			}
		}
	}
}
