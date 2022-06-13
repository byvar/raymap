using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GLI_BackgroundCI8 : U64_Struct {
		public byte[] Bitmap { get; set; }

		public uint Pre_Length { get; set; }

		public uint ScreenWidth =>
			Pre_Length switch {
				0x0C000 => 256, // DS base
				0x10680 => 300, // N64 base
				0x12C00 => 320, // N64 high-res
				_ => throw new NotImplementedException($"{GetType()}: Unknown length {Context.GetCPASettings().Platform}")
			};

		public uint ScreenHeight =>
			Pre_Length switch {
				0x0C000 => 192, // DS base
				0x10680 => 224, // N64 base
				0x12C00 => 240, // N64 high-res
				_ => throw new NotImplementedException($"{GetType()}: Unknown length {Context.GetCPASettings().Platform}")
			};

		public override void SerializeImpl(SerializerObject s) {
			Bitmap = s.SerializeArray<byte>(Bitmap, Pre_Length, name: nameof(Bitmap));
		}
	}

}
