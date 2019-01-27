using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VrSharp.PvrTexture;

namespace OpenSpace.FileFormat.RenderWare {
	// R2 PS2 texture reference format
	// https://gtamods.com/wiki/Texture_(RW_Section)
	public class MaterialTexture {
		public byte textureFilteringMode;
		public FilterMode textureFiltering;
		public byte addressingMode;
		public AddressingMode wrapModeU;
		public AddressingMode wrapModeV;
		public bool useMipLevels;

		public static MaterialTexture Read(Reader reader) {
			MapLoader.Loader.print("reading material texture");
			MaterialTexture mt = new MaterialTexture();
			mt.textureFilteringMode = reader.ReadByte();
			mt.textureFiltering = (FilterMode)mt.textureFilteringMode;
			mt.addressingMode = reader.ReadByte();
			mt.wrapModeU = (AddressingMode)(mt.addressingMode & 0x0F);
			mt.wrapModeV = (AddressingMode)((mt.addressingMode & 0xF0) << 4);
			mt.useMipLevels = (reader.ReadUInt16() & 0x1) != 0;
			return mt;
		}

		public enum FilterMode {
			NONE = 0, //filtering is disabled
			NEAREST = 1, //Point sampled
			LINEAR = 2, //Bilinear
			MIPNEAREST = 3, //Point sampled per pixel mip map
			MIPLINEAR = 4, //Bilinear per pixel mipmap
			LINEARMIPNEAREST = 5, //MipMap interp point sampled
			LINEARMIPLINEAR = 6, //Trilinear
		}

		public enum AddressingMode {
			NONE = 0, //no tiling
			WRAP = 1, //tile in U or V direction
			MIRROR = 2, //mirror in U or V direction
			CLAMP = 3,
			BORDER = 4,
		}
	}
}