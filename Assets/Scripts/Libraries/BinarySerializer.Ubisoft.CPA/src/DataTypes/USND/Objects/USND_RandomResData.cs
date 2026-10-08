namespace BinarySerializer.Ubisoft.CPA {
	public class USND_RandomResData : USND_ResData {
		public const string ClassName = "CRandomResData";

		// Unknown values
		public uint Uint_08 { get; set; }
		public float Float_0C { get; set; }

		public uint ElementsCount { get; set; }
		public RandomElement[] Elements { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);

			Uint_08 = s.Serialize<uint>(Uint_08, name: nameof(Uint_08));
			Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
			ElementsCount = s.Serialize<uint>(ElementsCount, name: nameof(ElementsCount));
			Elements = s.SerializeObjectArray<RandomElement>(Elements, ElementsCount, name: nameof(Elements));
		}

		public class RandomElement : BinarySerializable {
			public float Condition { get; set; }
			public USND_UUID ResDataId { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Condition = s.Serialize<float>(Condition, name: nameof(Condition));
				ResDataId = s.SerializeObject<USND_UUID>(ResDataId, name: nameof(ResDataId));
			}
		}
	}
}