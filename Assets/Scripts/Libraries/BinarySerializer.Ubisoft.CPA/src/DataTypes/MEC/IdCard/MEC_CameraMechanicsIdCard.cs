namespace BinarySerializer.Ubisoft.CPA {
	public class MEC_CameraMechanicsIdCard : MEC_MechanicsIdCardData {
		public float LinearSpeed { get; set; }
		public float AngularSpeed { get; set; }
		public float SpeedingUp { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LinearSpeed = s.Serialize<float>(LinearSpeed, name: nameof(LinearSpeed));
			AngularSpeed = s.Serialize<float>(AngularSpeed, name: nameof(AngularSpeed));
			SpeedingUp = s.Serialize<float>(SpeedingUp, name: nameof(SpeedingUp));
		}
	}
}
