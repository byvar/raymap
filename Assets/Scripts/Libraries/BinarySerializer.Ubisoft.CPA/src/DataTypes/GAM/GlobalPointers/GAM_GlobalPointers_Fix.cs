namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_GlobalPointers_Fix : BinarySerializable {
		public Pointer<MAT_Transformation> IdentityMatrix { get; set; }
		public Pointer<MAT_Transformation>[] MatrixStack { get; set; }
		public uint MatrixCountInStack { get; set; }
		public Pointer<COL_CollideObject> CollisionGeometricObject { get; set; }
		public Pointer<COL_CollideObject> StaticCollisionGeometricObject { get; set; }
		public Pointer<IPT_EntryElement>[] EntryActions_3DOS { get; set; }
		public Pointer<IPT_KeyAndPadDefineArray> KeyAndPadDefine { get; set; }
		public IPT_Input InputStructure { get; set; }
		public Pointer<IPT_EntryElement[]> EntryElementList { get; set; }
		public FON_General Text { get; set; }
		public A3D_AnimationBank Animations { get; set; }
		public Pointer<GAM_AlwaysModelList> LastAlwaysInFix { get; set; }
		public Pointer<GAM_DemoGMTList> DemoGMTList { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				IdentityMatrix = s.SerializePointer<MAT_Transformation>(IdentityMatrix, name: nameof(IdentityMatrix))?.ResolveObject(s);
				MatrixStack = s.SerializePointerArray<MAT_Transformation>(MatrixStack, 50, name: nameof(MatrixStack))?.ResolveObject(s);
				MatrixCountInStack = s.Serialize<uint>(MatrixCountInStack, name: nameof(MatrixCountInStack));
				CollisionGeometricObject = s.SerializePointer<COL_CollideObject>(CollisionGeometricObject, name: nameof(CollisionGeometricObject))?.ResolveObject(s);
				StaticCollisionGeometricObject = s.SerializePointer<COL_CollideObject>(StaticCollisionGeometricObject, name: nameof(StaticCollisionGeometricObject))?.ResolveObject(s);
				EntryActions_3DOS = s.SerializePointerArray<IPT_EntryElement>(EntryActions_3DOS, s.GetCPASettings().EntryActionsCount, name: nameof(EntryActions_3DOS))?.ResolveObject(s);
				KeyAndPadDefine = s.SerializePointer<IPT_KeyAndPadDefineArray>(KeyAndPadDefine, name: nameof(KeyAndPadDefine))?.ResolveObject(s);
				InputStructure = s.SerializeObject<IPT_Input>(InputStructure, name: nameof(InputStructure));
				EntryElementList = s.SerializePointer<IPT_EntryElement[]>(EntryElementList, name: nameof(EntryElementList))?.ResolveObjectArray(s, InputStructure.EntryElementsCount);
				Text = s.SerializeObject<FON_General>(Text, name: nameof(Text));
				Animations = s.SerializeObject<A3D_AnimationBank>(Animations, name: nameof(Animations));
				Animations?.SerializeData(s, A3D_AnimationBank.SerializeMode.Pointer);
				LastAlwaysInFix = s.SerializePointer<GAM_AlwaysModelList>(LastAlwaysInFix, name: nameof(LastAlwaysInFix));
				if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2)) {
					DemoGMTList = s.SerializePointer<GAM_DemoGMTList>(DemoGMTList, name: nameof(DemoGMTList))?.ResolveObject(s);
				}
			}
		}

		public void ResolveFixLevelReferences(SerializerObject s) {
			LastAlwaysInFix?.ResolveObject(s);
		}
	}
}
