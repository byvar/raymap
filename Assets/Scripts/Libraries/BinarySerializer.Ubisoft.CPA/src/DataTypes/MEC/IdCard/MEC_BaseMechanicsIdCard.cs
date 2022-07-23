namespace BinarySerializer.Ubisoft.CPA {
	public class MEC_BaseMechanicsIdCard : MEC_MechanicsIdCardData {
		public MEC_CardFlags Flags { get; set; }
		public float Gravity { get; set; }
		public float Slide { get; set; }
		public float Rebound { get; set; }
		public float SlopeLimit { get; set; }
		public MTH3D_Vector Inertia { get; set; }
		public float TiltIntensity { get; set; }
		public float TiltInertia { get; set; }
		public float TiltOrigin { get; set; }
		public MTH3D_Vector MaxSpeed { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Flags = s.Serialize<MEC_CardFlags>(Flags, name: nameof(Flags));
			Gravity = s.Serialize<float>(Gravity, name: nameof(Gravity));
			Slide = s.Serialize<float>(Slide, name: nameof(Slide));
			Rebound = s.Serialize<float>(Rebound, name: nameof(Rebound));
			SlopeLimit = s.Serialize<float>(SlopeLimit, name: nameof(SlopeLimit));
			Inertia = s.SerializeObject<MTH3D_Vector>(Inertia, name: nameof(Inertia));
			TiltIntensity = s.Serialize<float>(TiltIntensity, name: nameof(TiltIntensity));
			TiltInertia = s.Serialize<float>(TiltInertia, name: nameof(TiltInertia));
			TiltOrigin = s.Serialize<float>(TiltOrigin, name: nameof(TiltOrigin));
			MaxSpeed = s.SerializeObject<MTH3D_Vector>(MaxSpeed, name: nameof(MaxSpeed));
		}
	}
}
