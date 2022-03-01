using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GLI_BitmapInfo : U64_Struct {
		public U64_MainTableReference<GLI_Bitmap> Texture { get; set; }
		public U64_MainTableReference<GLI_PaletteRGBA16> Palette { get; set; }
		public U64_Reference<GLI_PaletteRGBA16> PaletteN64 { get; set; }
		public U64_MainTableReference<GLI_BitmapCI4> Alpha { get; set; }
		public byte WidthLog { get; set; }
		public byte HeightLog { get; set; }
		public BitmapFlags Flags { get; set; }
		public BitmapType Type { get; set; }
		public DrawFlags MiscFlags { get; set; }
		public ushort AlphaChannel { get; set; } // Is this a color?
		public ushort PaletteSize { get; set; } = 256;

		// 3DS
		public ushort Flags3DS { get; set; }
		public ushort ImageDataSize { get; set; }
		public ushort BitsPerPixel { get; set; }
		public string Name { get; set; }
		public byte[] ImageData { get; set; } // TODO: Parse as ETC

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().Platform == Platform._3DS) {
				WidthLog = s.Serialize<byte>(WidthLog, name: nameof(WidthLog));
				HeightLog = s.Serialize<byte>(HeightLog, name: nameof(HeightLog));
				s.DoBits<ushort>(b => {
					Flags = b.SerializeBits<BitmapFlags>(Flags, 8, name: nameof(Flags));
					Type = b.SerializeBits<BitmapType>(Type, 4, name: nameof(Type));
					MiscFlags = b.SerializeBits<DrawFlags>(MiscFlags, 4, name: nameof(MiscFlags));
				});
				Flags3DS = s.Serialize<ushort>(Flags3DS, name: nameof(Flags3DS));
				ImageDataSize = s.Serialize<ushort>(ImageDataSize, name: nameof(ImageDataSize));
				BitsPerPixel = s.Serialize<ushort>(BitsPerPixel, name: nameof(BitsPerPixel));
				Name = s.SerializeString(Name, 200, name: nameof(Name));
				ImageData = s.SerializeArray<byte>(ImageData, ImageDataSize, name: nameof(ImageData));
			} else {
				Texture = s.SerializeObject<U64_MainTableReference<GLI_Bitmap>>(Texture, name: nameof(Texture));
				if (s.GetCPASettings().Platform == Platform.N64) {
					PaletteN64 = s.SerializeObject<U64_Reference<GLI_PaletteRGBA16>>(PaletteN64, name: nameof(PaletteN64))?.Resolve(s);
				} else {
					Palette = s.SerializeObject<U64_MainTableReference<GLI_PaletteRGBA16>>(Palette, name: nameof(Palette));
				}
				Alpha = s.SerializeObject<U64_MainTableReference<GLI_BitmapCI4>>(Alpha, name: nameof(Alpha));
				WidthLog = s.Serialize<byte>(WidthLog, name: nameof(WidthLog));
				HeightLog = s.Serialize<byte>(HeightLog, name: nameof(HeightLog));
				s.DoBits<ushort>(b => {
					Flags = b.SerializeBits<BitmapFlags>(Flags, 8, name: nameof(Flags));
					Type = b.SerializeBits<BitmapType>(Type, 4, name: nameof(Type));
					MiscFlags = b.SerializeBits<DrawFlags>(MiscFlags, 4, name: nameof(MiscFlags));
				});
				AlphaChannel = s.Serialize<ushort>(AlphaChannel, name: nameof(AlphaChannel));
				if (s.GetCPASettings().Platform == Platform.DS) {
					PaletteSize = s.Serialize<ushort>(PaletteSize, name: nameof(PaletteSize));
				}
				if (Flags.HasFlag(BitmapFlags.CI4)) {
					Texture?.ResolveAs<GLI_BitmapCI4>(s, onPreSerialize: (_, t) => {
						t.Pre_HeightLog = HeightLog;
						t.Pre_WidthLog = WidthLog;
					});
				} else if (Flags.HasFlag(BitmapFlags.CI8)) {
					Texture?.ResolveAs<GLI_BitmapCI8>(s, onPreSerialize: (_, t) => {
						t.Pre_HeightLog = HeightLog;
						t.Pre_WidthLog = WidthLog;
					});
				} else if (Flags.HasFlag(BitmapFlags.RGBA16)) {
					Texture?.ResolveAs<GLI_BitmapRGBA16>(s, onPreSerialize: (_, t) => {
						t.Pre_HeightLog = HeightLog;
						t.Pre_WidthLog = WidthLog;
					});
				}
				Palette?.Resolve(s, onPreSerialize: (_, p) => p.Length = PaletteSize);
				Alpha?.Resolve(s, onPreSerialize: (_, a) => {
					a.Pre_HeightLog = HeightLog;
					a.Pre_WidthLog = WidthLog;
				});
			}
		}

		[Flags]
		public enum BitmapFlags : byte {
			None    = 0,
			CI4     = 1 << 0, // CI = Color Indexed (palette)
			CI8     = 1 << 1,
			RGBA16  = 1 << 2, // 5551
			RGBA32  = 1 << 3, // 8888

			MirrorX = 1 << 4,
			MirrorY = 1 << 5,
			TileX   = 1 << 6,
			TileY   = 1 << 7
		}

		public enum BitmapType : byte {
			Standard            = 0,
			Alpha               = 1,
			Add                 = 2,
			AlphaOnly           = 3,
			AddAlpha            = 4,
			AlphaNZ             = 5,
			MultiAlphaOnly      = 6,
			Multi_Opaque        = 7,
			Multi_Opaque_Detail = 8,
			AlphaUZ             = 9,
			Alpha5i3            = 10,
			Alpha3i5            = 11,
		}

		[Flags]
		public enum DrawFlags : byte {
			None              = 0,
			Reflect           = 1 << 0,
			Chromaset         = 1 << 1,
			TransparentObject = 1 << 2,
			AutoLum           = 1 << 3,
		}
	}

}
