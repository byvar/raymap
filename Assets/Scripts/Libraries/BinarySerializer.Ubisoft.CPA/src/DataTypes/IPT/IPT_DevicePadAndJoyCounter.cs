namespace BinarySerializer.Ubisoft.CPA {
	public class IPT_DevicePadAndJoyCounter : BinarySerializable {
		public byte[] PadAndJoyCounter { get; set; }

		public uint PadAndJoyActionCount {
			get {
				if (Context.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
					return 44;
				} else if(Context.GetCPASettings().Platform == Platform.iOS || Context.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.RedPlanet)) {
					return 22;
				} else {
					return 74;
				}
			}
		}

		public override void SerializeImpl(SerializerObject s) {
			PadAndJoyCounter = s.SerializeArray<byte>(PadAndJoyCounter, PadAndJoyActionCount, name: nameof(PadAndJoyCounter));
		}
	}
}
