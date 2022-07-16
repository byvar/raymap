using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_CharacterLight : U64_Struct {
		public float Far { get; set; }
		public float Near { get; set; }
		public float LittleAlpha { get; set; }
		public float BigAlpha { get; set; }
		public float LittleTangent { get; set; }
		public float BigTangent { get; set; }
		public GLI_FloatColor_RGBA Color { get; set; }
		public float Gyrophare { get; set; }
		public float PulseStep { get; set; }
		public float PulseMaxRange { get; set; }
		public MTH3D_Vector LightOffset { get; set; }
		public MTH3D_Vector LightDirection { get; set; }
		public MTH3D_Vector InterMinPos { get; set; }
		public MTH3D_Vector InterMaxPos { get; set; }
		public MTH3D_Vector ExterMinPos { get; set; }
		public MTH3D_Vector ExterMaxPos { get; set; }
		public float IntensityMin { get; set; }
		public float IntensityMax { get; set; }

		// Flags
		public LightType Type { get; set; }
		public bool IsOn { get; set; }
		public ushort Flags { get; set; }

		public bool IsGyroPhare { get; set; }
		public bool IsPulse { get; set; }
		public bool IsLocal { get; set; } // Unused on DS
		public bool IsOnlyLocal { get; set; } // Unused on DS

		public override void SerializeImpl(SerializerObject s) {
			Far = s.Serialize<float>(Far, name: nameof(Far));
			Near = s.Serialize<float>(Near, name: nameof(Near));
			LittleAlpha = s.Serialize<float>(LittleAlpha, name: nameof(LittleAlpha));
			BigAlpha = s.Serialize<float>(BigAlpha, name: nameof(BigAlpha));
			LittleTangent = s.Serialize<float>(LittleTangent, name: nameof(LittleTangent));
			BigTangent = s.Serialize<float>(BigTangent, name: nameof(BigTangent));
			Color = s.SerializeObject<GLI_FloatColor_RGBA>(Color, name: nameof(Color));
			Gyrophare = s.Serialize<float>(Gyrophare, name: nameof(Gyrophare));
			PulseStep = s.Serialize<float>(PulseStep, name: nameof(PulseStep));
			PulseMaxRange = s.Serialize<float>(PulseMaxRange, name: nameof(PulseMaxRange));
			LightOffset = s.SerializeObject<MTH3D_Vector>(LightOffset, name: nameof(LightOffset));
			LightDirection = s.SerializeObject<MTH3D_Vector>(LightDirection, name: nameof(LightDirection));
			InterMinPos = s.SerializeObject<MTH3D_Vector>(InterMinPos, name: nameof(InterMinPos));
			InterMaxPos = s.SerializeObject<MTH3D_Vector>(InterMaxPos, name: nameof(InterMaxPos));
			ExterMinPos = s.SerializeObject<MTH3D_Vector>(ExterMinPos, name: nameof(ExterMinPos));
			ExterMaxPos = s.SerializeObject<MTH3D_Vector>(ExterMaxPos, name: nameof(ExterMaxPos));
			IntensityMin = s.Serialize<float>(IntensityMin, name: nameof(IntensityMin));
			IntensityMax = s.Serialize<float>(IntensityMax, name: nameof(IntensityMax));
			s.DoBits<ushort>(b => {
				Type = b.SerializeBits<LightType>(Type, 2, name: nameof(Type));
				IsOn = b.SerializeBits<bool>(IsOn, 1, name: nameof(IsOn));
				Flags = b.SerializeBits<ushort>(Flags, 16-3, name: nameof(Flags));
			});
			IsGyroPhare = s.Serialize<bool>(IsGyroPhare, name: nameof(IsGyroPhare));
			IsPulse = s.Serialize<bool>(IsPulse, name: nameof(IsPulse));
			IsLocal = s.Serialize<bool>(IsLocal, name: nameof(IsLocal));
			IsOnlyLocal = s.Serialize<bool>(IsOnlyLocal, name: nameof(IsOnlyLocal));
			s.SerializePadding(2, logIfNotNull: true);
		}

		public enum LightType : byte {
			Ambient = 0,
			Parallel = 1,
			Spherical = 2,
			HotSpot = 3
		}
	}
}
