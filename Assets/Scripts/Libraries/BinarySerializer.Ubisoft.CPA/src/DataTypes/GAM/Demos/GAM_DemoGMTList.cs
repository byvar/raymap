namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_DemoGMTList : BinarySerializable {
		public LST2_StaticList<GAM_DemoGMT> DemoGMTList { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			DemoGMTList = s.SerializeObject<LST2_StaticList<GAM_DemoGMT>>(DemoGMTList, name: nameof(DemoGMTList))?.Resolve(s, name: nameof(DemoGMTList));
		}
	}
}
