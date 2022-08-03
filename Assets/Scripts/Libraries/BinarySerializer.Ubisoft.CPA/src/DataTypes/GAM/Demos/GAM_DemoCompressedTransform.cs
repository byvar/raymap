namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_DemoCompressedTransform : BinarySerializable {
		public ushort Type { get; set; }
		public short[] Coef { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				Type = s.Serialize<ushort>(Type, name: nameof(Type));
				Coef = s.SerializeArray<short>(Coef, 20, name: nameof(Coef));
			} else {
				Coef = s.SerializeArray<short>(Coef, 9, name: nameof(Coef));
			}
		}
	}
}
