namespace BinarySerializer.Ubisoft.CPA {
	public class DNM_DynamicsBaseBlock : BinarySerializable {
		public MEC_MechanicsId ObjectType { get; set; }
		public Pointer<MEC_MechanicsIdCard> CurrentIdCard { get; set; }
		public DNM_DynamicsFlags Flags { get; set; }
		public DNM_DynamicsEndFlags EndFlags { get; set; }

		public float Gravity { get; set; } // g
		public float SlopeLimit { get; set; } // tan of slope limit
		public float CosSlope { get; set; } // cos of slope limit
		public float Slide { get; set; }
		public float Rebound { get; set; }

		public MTH3D_Vector ImposeSpeed { get; set; }
		public MTH3D_Vector ProposeSpeed { get; set; }
		public MTH3D_Vector PreviousSpeed { get; set; }
		public MTH3D_Vector Scale { get; set; }
		public MTH3D_Vector AnimationSpeed { get; set; }
		public MTH3D_Vector SafeTranslation { get; set; }
		public MTH3D_Vector AddTranslation { get; set; }

		public MAT_Transformation PreviousMatrix { get; set; }
		public MAT_Transformation CurrentMatrix { get; set; }
		public MTH3D_Matrix ImposeRotationMatrix { get; set; }

		public byte Frame { get; set; }

		public Pointer<DNM_Report> Report { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			ObjectType = s.Serialize<MEC_MechanicsId>(ObjectType, name: nameof(ObjectType));
			CurrentIdCard = s.SerializePointer<MEC_MechanicsIdCard>(CurrentIdCard, name: nameof(CurrentIdCard))?.ResolveObject(s);
			Flags = s.Serialize<DNM_DynamicsFlags>(Flags, name: nameof(Flags));
			EndFlags = s.Serialize<DNM_DynamicsEndFlags>(EndFlags, name: nameof(EndFlags));

			if (s.GetCPASettings().IsPressDemo) {
				SlopeLimit = s.Serialize<float>(SlopeLimit, name: nameof(SlopeLimit));
				Gravity = s.Serialize<float>(Gravity, name: nameof(Gravity));
			} else {
				Gravity = s.Serialize<float>(Gravity, name: nameof(Gravity));
				SlopeLimit = s.Serialize<float>(SlopeLimit, name: nameof(SlopeLimit));
			}
			CosSlope = s.Serialize<float>(CosSlope, name: nameof(CosSlope));
			Slide = s.Serialize<float>(Slide, name: nameof(Slide));
			Rebound = s.Serialize<float>(Rebound, name: nameof(Rebound));

			ImposeSpeed = s.SerializeObject<MTH3D_Vector>(ImposeSpeed, name: nameof(ImposeSpeed));
			ProposeSpeed = s.SerializeObject<MTH3D_Vector>(ProposeSpeed, name: nameof(ProposeSpeed));
			PreviousSpeed = s.SerializeObject<MTH3D_Vector>(PreviousSpeed, name: nameof(PreviousSpeed));
			
			Scale = s.SerializeObject<MTH3D_Vector>(Scale, name: nameof(Scale));
			AnimationSpeed = s.SerializeObject<MTH3D_Vector>(AnimationSpeed, name: nameof(AnimationSpeed));
			SafeTranslation = s.SerializeObject<MTH3D_Vector>(SafeTranslation, name: nameof(SafeTranslation));
			AddTranslation = s.SerializeObject<MTH3D_Vector>(AddTranslation, name: nameof(AddTranslation));

			PreviousMatrix = s.SerializeObject<MAT_Transformation>(PreviousMatrix, name: nameof(PreviousMatrix));
			CurrentMatrix = s.SerializeObject<MAT_Transformation>(CurrentMatrix, name: nameof(CurrentMatrix));
			ImposeRotationMatrix = s.SerializeObject<MTH3D_Matrix>(ImposeRotationMatrix, name: nameof(ImposeRotationMatrix));

			Frame = s.Serialize<byte>(Frame, name: nameof(Frame));
			s.Align(4, Offset);
			Report = s.SerializePointer<DNM_Report>(Report, name: nameof(Report))?.ResolveObject(s);
		}
	}
}
