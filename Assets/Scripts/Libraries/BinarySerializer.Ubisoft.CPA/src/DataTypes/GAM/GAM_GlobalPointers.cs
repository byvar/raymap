namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_GlobalPointers : BinarySerializable {
		public Pointer<POS_CompletePosition> IdentityMatrix { get; set; }
		public Pointer<POS_CompletePosition>[] MatrixStack { get; set; }
		public uint MatrixCountInStack { get; set; }
		public Pointer<GEO_GeometricObject> CollisionGeometricObject { get; set; }
		public Pointer<GEO_GeometricObject> StaticCollisionGeometricObject { get; set; }
		public Pointer<IPT_EntryAction>[] EntryActions_3DOS { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				IdentityMatrix = s.SerializePointer<POS_CompletePosition>(IdentityMatrix, name: nameof(IdentityMatrix));
				MatrixStack = s.SerializePointerArray<POS_CompletePosition>(MatrixStack, 50, name: nameof(MatrixStack));
				MatrixCountInStack = s.Serialize<uint>(MatrixCountInStack, name: nameof(MatrixCountInStack));
				CollisionGeometricObject = s.SerializePointer<GEO_GeometricObject>(CollisionGeometricObject, name: nameof(CollisionGeometricObject));
				StaticCollisionGeometricObject = s.SerializePointer<GEO_GeometricObject>(StaticCollisionGeometricObject, name: nameof(StaticCollisionGeometricObject));
				EntryActions_3DOS = s.SerializePointerArray<IPT_EntryAction>(EntryActions_3DOS, s.GetCPASettings().EntryActionsCount, name: nameof(EntryActions_3DOS));

			}
		}
	}
}
