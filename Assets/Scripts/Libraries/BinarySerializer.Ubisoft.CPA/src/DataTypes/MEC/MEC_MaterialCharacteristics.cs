namespace BinarySerializer.Ubisoft.CPA {
	public class MEC_MaterialCharacteristics : BinarySerializable {
		public float Slide { get; set; }
		public float Rebound { get; set; }

		public float Adhesion { get; set; }
		public float Absorption { get; set; }
		public float Friction { get; set; }
		public float Progression { get; set; }
		public float PenetrationSpeed { get; set; }
		public float PenetrationMax { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				Slide = s.Serialize<float>(Slide, name: nameof(Slide));
				Rebound = s.Serialize<float>(Rebound, name: nameof(Rebound));
			} else {
				// Tonic Trouble, Montreal
				Adhesion = s.Serialize<float>(Adhesion, name: nameof(Adhesion));
				Absorption = s.Serialize<float>(Absorption, name: nameof(Absorption));
				Friction = s.Serialize<float>(Friction, name: nameof(Friction));
				Slide = s.Serialize<float>(Slide, name: nameof(Slide));
				Progression = s.Serialize<float>(Progression, name: nameof(Progression));
				PenetrationSpeed = s.Serialize<float>(PenetrationSpeed, name: nameof(PenetrationSpeed));
				PenetrationMax = s.Serialize<float>(PenetrationMax, name: nameof(PenetrationMax));
			}
		}
	}
}
