namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_Always : BinarySerializable {
		public uint MaxAlwaysCount { get; set; }
		public LST2_DynamicList<GAM_AlwaysModelList> AlwaysModels { get; set; }
		public Pointer<HIE_SuperObject[]> AlwaysSuperObjects { get; set; }
		public Pointer AlwaysEngineObjects { get; set; }
		public Pointer AlwaysGenerators { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			MaxAlwaysCount = s.Serialize<uint>(MaxAlwaysCount, name: nameof(MaxAlwaysCount));
			AlwaysModels = s.SerializeObject<LST2_DynamicList<GAM_AlwaysModelList>>(AlwaysModels, name: nameof(AlwaysModels));
			AlwaysSuperObjects = s.SerializePointer<HIE_SuperObject[]>(AlwaysSuperObjects, name: nameof(AlwaysSuperObjects))?.ResolveObjectArray(s, MaxAlwaysCount);
			AlwaysEngineObjects = s.SerializePointer(AlwaysEngineObjects, name: nameof(AlwaysEngineObjects));
			AlwaysGenerators = s.SerializePointer(AlwaysGenerators, name: nameof(AlwaysGenerators));
		}

		public GAM_Always Resolve(SerializerObject s) {
			AlwaysModels?.Resolve(s, name: nameof(AlwaysModels));

			return this;
		}
	}
}
