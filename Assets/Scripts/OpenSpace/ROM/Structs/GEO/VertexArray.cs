using OpenSpace.Loader;
using OpenSpace.ROM.RSP;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class VertexArray : ROMStruct {
		public ushort length;
		public byte[] data;
		public Vertex[] vertices;


		protected override void ReadInternal(Reader reader) {
			data = reader.ReadBytes(length);
			if (Settings.s.platform == Settings.Platform.N64) {
				using (MemoryStream str = new MemoryStream(data)) {
					using (Reader dataReader = new Reader(str, Settings.s.IsLittleEndian)) {
						vertices = new Vertex[length / 16];
						for (int i = 0; i < vertices.Length; i++) {
							vertices[i] = new Vertex(dataReader);
						}
					}
				}
			}
		}


		public struct Vertex {
			public short x;
			public short y;
			public short z;
			public ushort flag; // No meaning
			public short u;
			public short v;

			public byte r;
			public byte g;
			public byte b;
			public byte a;
			public Color color;
			public Vector3 normal;

			public Vertex(Reader reader) {
				x = reader.ReadInt16();
				y = reader.ReadInt16();
				z = reader.ReadInt16();
				flag = reader.ReadUInt16();
				u = reader.ReadInt16();
				v = reader.ReadInt16();
				r = reader.ReadByte();
				g = reader.ReadByte();
				b = reader.ReadByte();
				a = reader.ReadByte();
				color = new Color(r / 255f, g / 255f, b / 255f, a / 255f);
				normal = new Vector3(r - 128, g - 128, b - 128);
				normal = new Vector3(
					normal.x < 0 ? normal.x / 128f : normal.x / 127f,
					normal.y < 0 ? normal.y / 128f : normal.y / 127f,
					normal.z < 0 ? normal.z / 128f : normal.z / 127f
					);
			}

			public Vector3 GetVector(float factor, bool switchAxes = true) {
				if (switchAxes) {
					return new Vector3(x / factor, z / factor, y / factor);
				} else {
					return new Vector3(x / factor, y / factor, z / factor);
				}
			}
		}
	}
}
