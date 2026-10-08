namespace BinarySerializer.Ubisoft.CPA {
	public class USND_EventResData : USND_ResData {
		public const string ClassName = "CEventResData";

		// Name of the event
		public string Name { get; set; }

		// Type and the resource to control
		public USND_EventType Type { get; set; }
		public USND_UUID TargetRes { get; set; }

		public float CoeffA { get; set; }
		public float CoeffB { get; set; }
		public float CoeffC { get; set; }
		public float CoeffD { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);

			if (Version >= 2) {
				Name = s.SerializeLengthPrefixedString<uint>(Name, name: nameof(Name));
			}

			Type = s.Serialize<USND_EventType>(Type, name: nameof(Type));
			TargetRes = s.SerializeObject<USND_UUID>(TargetRes, name: nameof(TargetRes));
			CoeffA = s.Serialize<float>(CoeffA, name: nameof(CoeffA));
			CoeffB = s.Serialize<float>(CoeffB, name: nameof(CoeffB));
			CoeffC = s.Serialize<float>(CoeffC, name: nameof(CoeffC));
			CoeffD = s.Serialize<float>(CoeffD, name: nameof(CoeffD));
		}
	}
}