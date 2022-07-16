namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_GlobalPointers_Level : BinarySerializable {
		public GAM_FixInfo FixInfo { get; set; }
		public Pointer<HIE_SuperObject> ActualWorld { get; set; }
		public Pointer<HIE_SuperObject> DynamicWorld { get; set; }
		public Pointer<HIE_SuperObject> InactiveDynamicWorld { get; set; }
		public Pointer<HIE_SuperObject> FatherSector { get; set; }
		public Pointer<GAM_SubMapPosition> FirstSubmapPosition { get; set; }
		public GAM_Always Always { get; set; }
		public Pointer First { get; set; }
		public Pointer Last { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				FixInfo = s.SerializeObject<GAM_FixInfo>(FixInfo, name: nameof(FixInfo));
				ActualWorld = s.SerializePointer<HIE_SuperObject>(ActualWorld, name: nameof(ActualWorld))?.ResolveObject(s);
				DynamicWorld = s.SerializePointer<HIE_SuperObject>(DynamicWorld, name: nameof(DynamicWorld))?.ResolveObject(s);
				InactiveDynamicWorld = s.SerializePointer<HIE_SuperObject>(InactiveDynamicWorld, name: nameof(InactiveDynamicWorld))?.ResolveObject(s);
				FatherSector = s.SerializePointer<HIE_SuperObject>(FatherSector, name: nameof(FatherSector))?.ResolveObject(s);
				FirstSubmapPosition = s.SerializePointer<GAM_SubMapPosition>(FirstSubmapPosition, name: nameof(FirstSubmapPosition))?.ResolveObject(s);
				Always = s.SerializeObject<GAM_Always>(Always, name: nameof(Always));
				First = s.SerializePointer(First, name: nameof(First));
				Last = s.SerializePointer(Last, name: nameof(Last));
			}
		}
	}
}
