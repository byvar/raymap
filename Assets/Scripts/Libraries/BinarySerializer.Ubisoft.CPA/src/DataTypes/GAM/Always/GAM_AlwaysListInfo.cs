namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_AlwaysListInfo : BinarySerializable {
		public Pointer FirstAlwaysModel { get; set; }
		public Pointer LastAlwaysModel { get; set; }
		public Pointer<GAM_AlwaysModelList> FirstAlwaysInLevel { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			FirstAlwaysModel = s.SerializePointer(FirstAlwaysModel, name: nameof(FirstAlwaysModel));
			LastAlwaysModel = s.SerializePointer(LastAlwaysModel, name: nameof(LastAlwaysModel));
			FirstAlwaysInLevel = s.SerializePointer<GAM_AlwaysModelList>(FirstAlwaysInLevel, name: nameof(FirstAlwaysInLevel));
		}

		public GAM_AlwaysListInfo RepairLists(SerializerObject s, GAM_Always always) {
			LST2_ListHelpers.Validate(s, LastAlwaysModel, always.AlwaysModels.Offset, LST2_ListType.DoubleLinked);

			// Not really necessary to restore FirstAlwaysInLevel - it's fixed as part of the previous Validate

			return this;
		}

		public GAM_AlwaysListInfo Resolve(SerializerObject s) {
			FirstAlwaysInLevel?.ResolveObject(s);

			return this;
		}
	}
}
