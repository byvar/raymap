using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GLI_VisualMaterial : U64_Struct {
		public RGBA5551Color Color { get; set; }
		public RGBA5551Color Ambient { get; set; }
		public RGBA8888Color Color64 { get; set; }
		public float AddU { get; set; }
		public float AddV { get; set; }
		public U64_ArrayReference<U64_Placeholder> TextureList { get; set; }
		public ushort TextureListCount { get; set; }
		public ushort TotalTime { get; set; }
		public MaterialType Type { get; set; }
		public MaterialFlags Flags { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().Platform == Platform.N64) {
				Color64 = s.SerializeObject<RGBA8888Color>(Color64, name: nameof(Color64));
			} else {
				Color = s.SerializeObject<RGBA5551Color>(Color, name: nameof(Color));
				Ambient = s.SerializeObject<RGBA5551Color>(Ambient, name: nameof(Ambient));
			}
			AddU = s.Serialize<float>(AddU, name: nameof(AddU));
			AddV = s.Serialize<float>(AddV, name: nameof(AddV));
			TextureList = s.SerializeObject<U64_ArrayReference<U64_Placeholder>>(TextureList, name: nameof(TextureList));
			TextureListCount = s.Serialize<ushort>(TextureListCount, name: nameof(TextureListCount));
			TotalTime = s.Serialize<ushort>(TotalTime, name: nameof(TotalTime));
			s.DoBits<ushort>(b => {
				Type = b.SerializeBits<MaterialType>(Type, 8, name: nameof(Type));
				Flags = b.SerializeBits<MaterialFlags>(Flags, 8, name: nameof(Flags));
			});

			TextureList?.Resolve(s, TextureListCount);
		}

		public enum MaterialType : byte {
			Unspecified = 0,
			Gouraud = 1,
			Flat = 2,
			GouraudAlpha = 3
		}

		[Flags]
		public enum MaterialFlags : byte {
			None = 0,
			BackFace = 1 << 0,
		}
	}

}
