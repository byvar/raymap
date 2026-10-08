namespace BinarySerializer.Ubisoft.CPA {
	public class USND_SwitchResData : USND_ResData {
		public const string ClassName = "CSwitchResData";

		public float Volume { get; set; }
		public uint InfoType { get; set; }
		public uint DefaultIndex { get; set; }

		public uint ElementsCount { get; set; }
		public SwitchElement[] Elements { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);

			Volume = s.Serialize<float>(Volume, name: nameof(Volume));
			InfoType = s.Serialize<uint>(InfoType, name: nameof(InfoType));
			DefaultIndex = s.Serialize<uint>(DefaultIndex, name: nameof(DefaultIndex));

			ElementsCount = s.Serialize<uint>(ElementsCount, name: nameof(ElementsCount));
			Elements = s.SerializeObjectArray<SwitchElement>(Elements, ElementsCount, name: nameof(Elements));
		}

		public class SwitchElement : BinarySerializable {
			public uint Value { get; set; }
			public USND_UUID ResDataId { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Value = s.Serialize<uint>(Value, name: nameof(Value));
				ResDataId = s.SerializeObject<USND_UUID>(ResDataId, name: nameof(ResDataId));
			}
		}
	}
}