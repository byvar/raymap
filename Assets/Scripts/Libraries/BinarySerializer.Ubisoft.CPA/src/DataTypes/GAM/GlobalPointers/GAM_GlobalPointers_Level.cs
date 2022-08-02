namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_GlobalPointers_Level : BinarySerializable {
		public GAM_FixInfo FixInfo { get; set; }
		public Pointer<HIE_SuperObject> ActualWorld { get; set; }
		public Pointer<HIE_SuperObject> DynamicWorld { get; set; }
		public Pointer<HIE_SuperObject> InactiveDynamicWorld { get; set; }
		public Pointer<HIE_SuperObject> FatherSector { get; set; }
		public Pointer<GAM_SubMapPosition> FirstSubmapPosition { get; set; }
		public GAM_Always Always { get; set; }
		public GAM_AlwaysListInfo AlwaysListInfo { get; set; }
		public GAM_ObjectTypeListInfo ObjectTypeListInfo { get; set; }
		public GAM_ObjectType ObjectType { get; set; }
		public GAM_EngineStructure EngineStructure { get; set; }
		public Pointer<GLI_Light> Light { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				FixInfo = s.SerializeObject<GAM_FixInfo>(FixInfo, name: nameof(FixInfo));
				ActualWorld = s.SerializePointer<HIE_SuperObject>(ActualWorld, name: nameof(ActualWorld))?.ResolveObject(s);
				DynamicWorld = s.SerializePointer<HIE_SuperObject>(DynamicWorld, name: nameof(DynamicWorld))?.ResolveObject(s);
				InactiveDynamicWorld = s.SerializePointer<HIE_SuperObject>(InactiveDynamicWorld, name: nameof(InactiveDynamicWorld))?.ResolveObject(s);
				FatherSector = s.SerializePointer<HIE_SuperObject>(FatherSector, name: nameof(FatherSector))?.ResolveObject(s);
				FirstSubmapPosition = s.SerializePointer<GAM_SubMapPosition>(FirstSubmapPosition, name: nameof(FirstSubmapPosition))?.ResolveObject(s);
				Always = s.SerializeObject<GAM_Always>(Always, name: nameof(Always));
				AlwaysListInfo = s.SerializeObject<GAM_AlwaysListInfo>(AlwaysListInfo, name: nameof(AlwaysListInfo))
					?.RepairLists(s, Always)
					?.Resolve(s);
				Always?.Resolve(s);
				ObjectTypeListInfo = s.SerializeObject<GAM_ObjectTypeListInfo>(ObjectTypeListInfo, name: nameof(ObjectTypeListInfo));
				ObjectType = s.SerializeObject<GAM_ObjectType>(ObjectType, name: nameof(ObjectType))
					?.RepairLists(s)
					?.Resolve(s);
				EngineStructure = s.SerializeObject<GAM_EngineStructure>(EngineStructure, name: nameof(EngineStructure));
				Light = s.SerializePointer<GLI_Light>(Light, name: nameof(Light))?.ResolveObject(s);
			}
		}
	}
}
