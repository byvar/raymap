namespace BinarySerializer.Ubisoft.CPA {
	public class PO_PhysicalObject : BinarySerializable, IGAM_ObjectsTableTarget, IHIE_LinkedObject {
		public Pointer<GEO_VisualSet> VisualSet { get; set; }
		public Pointer<GEO_GeometricObject> GeometricObject { get; set; }

		public Pointer<PO_PhysicalCollideSet> CollideSet { get; set; }
		public Pointer<COL_CollideObject> CollideObject { get; set; }

		public Pointer<COL_BoundingSphere> BoundingVolume { get; set; }
		public Pointer<COL_BoundingSphere> VisualBoundingVolume { get; set; }
		public Pointer<COL_BoundingSphere> CollideBoundingVolume { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2Revolution)) {
				GeometricObject = s.SerializePointer<GEO_GeometricObject>(GeometricObject, name: nameof(GeometricObject))?.ResolveObject(s);
			} else {
				VisualSet = s.SerializePointer<GEO_VisualSet>(VisualSet, name: nameof(VisualSet))?.ResolveObject(s);
			}

			if(s.GetCPASettings().EngineVersion == EngineVersion.Rayman2Revolution) {
				CollideObject = s.SerializePointer<COL_CollideObject>(CollideObject, name: nameof(CollideObject))?.ResolveObject(s);
			} else {
				CollideSet = s.SerializePointer<PO_PhysicalCollideSet>(CollideSet, name: nameof(CollideSet))?.ResolveObject(s);
			}

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				VisualBoundingVolume = s.SerializePointer<COL_BoundingSphere>(VisualBoundingVolume, name: nameof(VisualBoundingVolume))?.ResolveObject(s);
				CollideBoundingVolume = s.SerializePointer<COL_BoundingSphere>(CollideBoundingVolume, name: nameof(CollideBoundingVolume))?.ResolveObject(s);
			} else {
				BoundingVolume = s.SerializePointer<COL_BoundingSphere>(BoundingVolume, name: nameof(BoundingVolume))?.ResolveObject(s);
			}
		}
	}
}
