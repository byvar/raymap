using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GLI_Light : U64_Struct {
		public float Near { get; set; }
		public float Far { get; set; }
		public float IntensityMin { get; set; }
		public float IntensityMax { get; set; }
		public U64_Color BackgroundColor { get; set; }
		public U64_Color Color { get; set; }
		public ObjectLighted ObjectLightedFlags { get; set; }
		public PaintOrAlpha PaintOrAlphaFlags { get; set; }
		public MTH3D_Vector InterMinPos { get; set; }
		public MTH3D_Vector InterMaxPos { get; set; }
		public MTH3D_Vector ExterMinPos { get; set; }
		public MTH3D_Vector ExterMaxPos { get; set; }
		public POS_CompletePosition Matrix { get; set; }
		// Flags ushort:
		public LightType Type { get; set; }
		public bool IsOn { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Near = s.Serialize<float>(Near, name: nameof(Near));
			Far = s.Serialize<float>(Far, name: nameof(Far));
			IntensityMin = s.Serialize<float>(IntensityMin, name: nameof(IntensityMin));
			IntensityMax = s.Serialize<float>(IntensityMax, name: nameof(IntensityMax));
			BackgroundColor = s.SerializeObject<U64_Color>(BackgroundColor, name: nameof(BackgroundColor));
			Color = s.SerializeObject<U64_Color>(Color, name: nameof(Color));
			ObjectLightedFlags = s.Serialize<ObjectLighted>(ObjectLightedFlags, name: nameof(ObjectLightedFlags));
			PaintOrAlphaFlags = s.Serialize<PaintOrAlpha>(PaintOrAlphaFlags, name: nameof(PaintOrAlphaFlags));
			InterMinPos = s.SerializeObject<MTH3D_Vector>(InterMinPos, name: nameof(InterMinPos));
			InterMaxPos = s.SerializeObject<MTH3D_Vector>(InterMaxPos, name: nameof(InterMaxPos));
			ExterMinPos = s.SerializeObject<MTH3D_Vector>(ExterMinPos, name: nameof(ExterMinPos));
			ExterMaxPos = s.SerializeObject<MTH3D_Vector>(ExterMaxPos, name: nameof(ExterMaxPos));
			Matrix = s.SerializeObject<POS_CompletePosition>(Matrix, name: nameof(Matrix));
			s.DoBits<ushort>(b => {
				Type = b.SerializeBits<LightType>(Type, 4, name: nameof(Type));
				b.SerializePadding(11, logIfNotNull: true);
				IsOn = b.SerializeBits<bool>(IsOn, 1, name: nameof(IsOn));
			});
		}

		public enum LightType : ushort {
			Parallel = 1,
			Spherical = 2,
			HotSpot = 3,
			Ambient = 4,
			Box = 5, // Parallel light with limit
			Fog = 6,
		}

		[Flags]
		public enum ObjectLighted : ushort {
			None = 0,
			Map = 1 << 0,
			Perso = 1 << 1,
			All = Map | Perso
		}

		[Flags]
		public enum PaintOrAlpha : ushort {
			None = 0,
			PaintOn = 1 << 0,
			NoAlphaLight = 1 << 1,
			AlphaOn = 1 << 15
		}
	}

}
