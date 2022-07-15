namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_GlobalPointers_Level : BinarySerializable {
		public GAM_FixInfo FixInfo { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				FixInfo = s.SerializeObject<GAM_FixInfo>(FixInfo, name: nameof(FixInfo));
			}
		}
	}
}
