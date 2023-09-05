namespace BinarySerializer.Ubisoft.CPA {
	public class DNM_DynamicsComplexBlock : BinarySerializable {
		public float TiltIntensity { get; set; }
		public float TiltInertia { get; set; }
		public float TiltOrigin { get; set; }

		public float TiltAngle { get; set; }
		public float HangingLimit { get; set; }

		public MTH3D_Vector Contact { get; set; }
		public MTH3D_Vector FallTranslation { get; set; }

		public DNM_MACDPID ExternalData { get; set; } // Cursed struct

		public Pointer<HIE_SuperObject> Platform { get; set; }
		public MAT_Transformation AbsolutePreviousMatrix { get; set; }
		public MAT_Transformation PreviousPreviousMatrix { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			TiltIntensity = s.Serialize<float>(TiltIntensity, name: nameof(TiltIntensity));
			TiltInertia = s.Serialize<float>(TiltInertia, name: nameof(TiltInertia));
			TiltOrigin = s.Serialize<float>(TiltOrigin, name: nameof(TiltOrigin));
			TiltAngle = s.Serialize<float>(TiltAngle, name: nameof(TiltAngle));

			HangingLimit = s.Serialize<float>(HangingLimit, name: nameof(HangingLimit));

			Contact = s.SerializeObject<MTH3D_Vector>(Contact, name: nameof(Contact));
			FallTranslation = s.SerializeObject<MTH3D_Vector>(FallTranslation, name: nameof(FallTranslation));

			ExternalData = s.SerializeObject<DNM_MACDPID>(ExternalData, name: nameof(ExternalData));

			Platform = s.SerializePointer<HIE_SuperObject>(Platform, name: nameof(Platform))?.ResolveObject(s);
			AbsolutePreviousMatrix = s.SerializeObject<MAT_Transformation>(AbsolutePreviousMatrix, name: nameof(AbsolutePreviousMatrix));
			PreviousPreviousMatrix = s.SerializeObject<MAT_Transformation>(PreviousPreviousMatrix, name: nameof(PreviousPreviousMatrix));
		}
	}
}
