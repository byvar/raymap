using DDSImageParser;
using LibGC.Texture;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace OpenSpace.FileFormat.Texture {
	// PS2 TBF Textures file
	public class TBF {
		public uint Count { get; } = 0;

		public string path;

		public Texture2D[] textures = null;
		public TBFHeader[] headers = null;

		public TBF(string path, bool hasNames = false) {
			this.path = path;
			Stream tbf = FileSystem.GetFileReadStream(path);
			using (Reader reader = new Reader(tbf, Settings.s.IsLittleEndian)) {
				if (hasNames) { // for example, TXC file in Rayman M/Arena
					Count = reader.ReadUInt32();
					headers = new TBFHeader[Count];
					for (uint i = 0; i < Count; i++) {
						TBFHeader h = new TBFHeader();
						uint len = reader.ReadUInt32();
						h.name = reader.ReadString((int)len);
						h.offset = reader.ReadUInt32();
						headers[i] = h;
					}
					for (uint i = 0; i < Count; i++) {
						reader.BaseStream.Position = headers[i].offset;
						ReadTexture(reader, h: headers[i]);
					}
				} else {
					List<Texture2D> textures = new List<Texture2D>();
					List<TBFHeader> headers = new List<TBFHeader>();
					while (reader.BaseStream.Position < reader.BaseStream.Length) {
						TBFHeader h = ReadTexture(reader);
						headers.Add(h);
					}
					Count = (uint)headers.Count;
					this.textures = headers.Select(h => h.texture).ToArray();
					this.headers = headers.ToArray();
				}
			}
		}

		public Texture2D GetTextureByName(string name) {
			// Remove point
			name = name.ToUpper().Substring(0, name.LastIndexOf('.'));
			return headers.FirstOrDefault(h => h.name == name + ".TXR")?.texture;
		}

		private TBFHeader ReadTexture(Reader reader, TBFHeader h = null) {
			if (h == null) {
				h = new TBFHeader();
				h.offset = (uint)reader.BaseStream.Position;
			}
			h.signature = reader.ReadUInt32();
			h.flags = reader.ReadUInt32();
			h.width = reader.ReadUInt32();
			h.height = reader.ReadUInt32();
			switch (h.TypeNumber) {
				case 0x1: {
						Color[] palette = ReadPalette(reader, 16, h.HasAlpha);
						byte[] texData = reader.ReadBytes((int)(h.height * h.width / 2));
						if (Settings.s.game == Settings.Game.R3) {
							ezSwizzle s = new ezSwizzle();
							s.writeTexPSMCT32(0, (int)h.width / 128, 0, 0, (int)h.width / 2, (int)h.height / 4, texData);
							texData = new byte[h.height * h.width];
							s.readTexPSMT4_mod(0, (int)h.width / 64, 0, 0, (int)h.width, (int)h.height, ref texData);
						} else {
							texData = texData.SelectMany(b => new byte[] { (byte)(b & 0xF), (byte)(b >> 4) }).ToArray();
						}
						//Util.ByteArrayToFile(MapLoader.Loader.gameDataBinFolder + "/textures/lol_" + h.width + "_" + h.height +".bin", texData);
						h.texture = CreateTexture(palette, texData, h.width, h.height);
					}
					break;
				case 0x2: {
						Color[] palette = ReadPalette(reader, 256, h.HasAlpha);
						byte[] texData = reader.ReadBytes((int)(h.height * h.width));
						if (Settings.s.game == Settings.Game.R3) {
							ezSwizzle s = new ezSwizzle();
							s.writeTexPSMCT32(0, (int)h.width / 128, 0, 0, (int)h.width / 2, (int)h.height / 2, texData);
							texData = new byte[h.height * h.width];
							s.readTexPSMT8(0, (int)h.width / 64, 0, 0, (int)h.width, (int)h.height, ref texData);
						}
						h.texture = CreateTexture(palette, texData, h.width, h.height);
					}
					break;
				case 0:
					// Do nothing, this is empty
					if (h.width != 0 || h.height != 0) {
						throw new InvalidDataException(path + " - " + string.Format("{0:X8}", h.offset) + " - Type 0, but width & height aren't 0!");
					}
					break;
				// Rayman 3 supports types 3 and 4 as well
				default:
					throw new InvalidDataException(path + " - " + string.Format("{0:X8}", h.offset) + " - Unknown type: " + h.TypeNumber);
			}
			return h;
		}

		private Color[] ReadPalette(Reader reader, int length, bool hasAlpha = true) {
			Color[] palette = new Color[length];
			for (int i = 0; i < length; i++) {
				byte r = reader.ReadByte();
				byte g = reader.ReadByte();
				byte b = reader.ReadByte();
				byte a = reader.ReadByte();
				palette[i] = new Color(r / 255f, g / 255f, b / 255f, hasAlpha ? (a / 128f) : 1f);
			}
			Color[] pal = palette;
			if (length == 256 && Settings.s.game == Settings.Game.R3) {
				// Tile
				pal = new Color[palette.Length];
				for (int i = 0; i < length; i++) {
					int indInBlock = i % 32;
					int indOldBlock = indInBlock / 8;
					switch (indOldBlock) {
						case 0:
						case 3:
							pal[i] = palette[i];
							break;
						case 1:
							pal[i] = palette[i + 8];
							break;
						case 2:
							pal[i] = palette[i - 8];
							break;
					}
				}
			}
			return pal;
		}

		private Texture2D CreateTexture(Color[] palette, byte[] indices, uint width, uint height) {
			Texture2D tex = new Texture2D((int)width, (int)height);
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					Color col = palette[indices[x + y * width]];
					tex.SetPixel(x, y, col);
				}
			}
			tex.Apply();
			return tex;
		}

		public class TBFHeader {
			public uint offset;
			public uint signature;
			public uint flags;
			public uint width;
			public uint height;

			public uint TypeNumber => flags & 0xF;
			public bool HasAlpha => (flags & 0x30) == 0x30;

			public string name;
			public Texture2D texture;
		}
	}
}