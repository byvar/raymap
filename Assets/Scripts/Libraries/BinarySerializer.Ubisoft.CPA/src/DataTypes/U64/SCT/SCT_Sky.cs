using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class SCT_Sky : BinarySerializable {
		public float AddU { get; set; }
		public float AddV { get; set; }
		public U64_Reference<GLI_VisualMaterial> SkyVisualMaterial { get; set; }
		public byte FogIntensity { get; set; }
		public SkyType UseSky { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			AddU = s.Serialize<float>(AddU, name: nameof(AddU));
			AddV = s.Serialize<float>(AddV, name: nameof(AddV));
			SkyVisualMaterial = s.SerializeObject<U64_Reference<GLI_VisualMaterial>>(SkyVisualMaterial, name: nameof(SkyVisualMaterial))?.Resolve(s);
			FogIntensity = s.Serialize<byte>(FogIntensity, name: nameof(FogIntensity));
			UseSky = s.Serialize<SkyType>(UseSky, name: nameof(UseSky));
		}

		public enum SkyType : byte {
			NoSky = 0,
			Sky = 1,
			WaterSky = 2
		}
	}
}
