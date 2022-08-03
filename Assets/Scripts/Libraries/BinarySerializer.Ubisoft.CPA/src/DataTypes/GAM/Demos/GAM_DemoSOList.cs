namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_DemoSOList : BinarySerializable {
		public LST2_StaticList<GAM_DemoSO> DemoSOList { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			DemoSOList = s.SerializeObject<LST2_StaticList<GAM_DemoSO>>(DemoSOList, name: nameof(DemoSOList))?.Resolve(s, name: nameof(DemoSOList));
		}
	}
}
