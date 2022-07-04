namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_GlobalPointers_Fix : BinarySerializable {
		public Pointer<POS_CompletePosition> IdentityMatrix { get; set; }
		public Pointer<POS_CompletePosition>[] MatrixStack { get; set; }
		public uint MatrixCountInStack { get; set; }
		public Pointer<GEO_GeometricObject> CollisionGeometricObject { get; set; }
		public Pointer<GEO_GeometricObject> StaticCollisionGeometricObject { get; set; }
		public Pointer<IPT_EntryElement>[] EntryActions_3DOS { get; set; }
		public Pointer<IPT_KeyAndPadDefineArray> KeyAndPadDefine { get; set; }
		public IPT_Input InputStructure { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				IdentityMatrix = s.SerializePointer<POS_CompletePosition>(IdentityMatrix, name: nameof(IdentityMatrix))?.Resolve(s);
				MatrixStack = s.SerializePointerArray<POS_CompletePosition>(MatrixStack, 50, name: nameof(MatrixStack))?.Resolve(s);
				MatrixCountInStack = s.Serialize<uint>(MatrixCountInStack, name: nameof(MatrixCountInStack));
				CollisionGeometricObject = s.SerializePointer<GEO_GeometricObject>(CollisionGeometricObject, name: nameof(CollisionGeometricObject));
				StaticCollisionGeometricObject = s.SerializePointer<GEO_GeometricObject>(StaticCollisionGeometricObject, name: nameof(StaticCollisionGeometricObject));
				EntryActions_3DOS = s.SerializePointerArray<IPT_EntryElement>(EntryActions_3DOS, s.GetCPASettings().EntryActionsCount, name: nameof(EntryActions_3DOS));
				KeyAndPadDefine = s.SerializePointer<IPT_KeyAndPadDefineArray>(KeyAndPadDefine, name: nameof(KeyAndPadDefine))?.Resolve(s);
				InputStructure = s.SerializeObject<IPT_Input>(InputStructure, name: nameof(InputStructure));
			}
		}
	}
}
