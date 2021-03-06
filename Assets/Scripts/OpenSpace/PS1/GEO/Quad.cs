﻿using OpenSpace.FileFormat.Texture;
using OpenSpace.Loader;
using OpenSpace.PS1.GLI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class Quad : OpenSpaceStruct, IPS1Polygon {
		public ushort v0;
		public ushort v1;
		public ushort v2;
		public ushort v3;
		public byte materialFlags;
		public byte scroll;
		public ushort ushort_0A;
		public byte x0;
		public byte y0;
		public ushort paletteInfo;
		public byte x1;
		public byte y1;
		public ushort pageInfo;
		public byte x2;
		public byte y2;
		public byte x3;
		public byte y3;

		protected override void ReadInternal(Reader reader) {
			v0 = reader.ReadUInt16();
			v1 = reader.ReadUInt16();
			v2 = reader.ReadUInt16();
			v3 = reader.ReadUInt16();
			materialFlags = reader.ReadByte();
			scroll = reader.ReadByte();
			ushort_0A = reader.ReadUInt16();
			x0 = reader.ReadByte();
			y0 = reader.ReadByte();
			paletteInfo = reader.ReadUInt16();// palette info?
			x1 = reader.ReadByte();
			y1 = reader.ReadByte();
			pageInfo = reader.ReadUInt16();// page info?
			x2 = reader.ReadByte();
			y2 = reader.ReadByte();
			x3 = reader.ReadByte();
			y3 = reader.ReadByte();

			byte[] x = new[] { x0, x1, x2, x3 };
			byte[] y = new[] { y0, y1, y2, y3 };
			int xMin = x.Min();
			int xMax = x.Max() + 1;
			int yMin = y.Min();
			int yMax = y.Max() + 1;
			int w = xMax - xMin;
			int h = yMax - yMin;



			R2PS1Loader l = Load as R2PS1Loader;
			l.RegisterTexture(pageInfo, paletteInfo, xMin, xMax, yMin, yMax);
			/*
			R2PS1Loader l = Load as R2PS1Loader;
			PS1VRAM vram = l.vram;
			Texture2D tex = vram.GetTexture((ushort)w, (ushort)h, pageInfo, paletteInfo, PS1VRAM.PixelMode.Byte, xMin, yMin);
			Util.ByteArrayToFile(l.gameDataBinFolder + "test_tex/" + Offset.StringFileOffset + $"_{w}_{h}" + ".png", tex.EncodeToPNG());
			*/
		}

		public TextureBounds Texture {
			get {
				R2PS1Loader l = Load as R2PS1Loader;
				return l.GetTextureBounds(pageInfo, paletteInfo, x0, y0);
			}
		}

		public VisualMaterial Material {
			get {
				return new VisualMaterial() {
					materialFlags = materialFlags,
					scroll = scroll,
					texture = Texture
				};
			}
		}
	}
}
