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
			if (Settings.s.platform == Settings.Platform.N64) {
				vertices = new Vertex[length / 16];
				for (int i = 0; i < vertices.Length; i++) {
					vertices[i] = new Vertex(reader);
				}
			} else {
				data = reader.ReadBytes(length);
			}
		}
		public void ResetVertexBuffer() {
			if (Settings.s.platform == Settings.Platform.N64) {
				foreach(Vertex v in vertices) {
					v.ResetWorkingCopy();
				}
			}
		}


		public class Vertex {
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

			private Vertex workingCopy;
			private Vertex original;

			public Vertex() { }
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
				normal = new Vector3(r - 128f, g - 128f, b - 128f);
				normal = new Vector3(
					normal.x < 0 ? normal.x / 128f : normal.x / 127f,
					normal.y < 0 ? normal.y / 128f : normal.y / 127f,
					normal.z < 0 ? normal.z / 128f : normal.z / 127f
					);
			}
			public Vertex WorkingCopy {
				get {
					if (workingCopy == null) {
						return this;
					} else {
						return workingCopy;
					}
				}
			}
			public void PrepareWorkingCopy() {
				if (workingCopy == null) {
					workingCopy = new Vertex() {
						x = x,
						y = y,
						z = z,
						flag = flag,
						u = u,
						v = v,
						r = r,
						g = g,
						b = b,
						a = a,
						color = color,
						normal = normal,
						original = original != null ? original : this
					};
					if (original != null) {
						original.workingCopy = workingCopy;
					}
				}
			}

			public void ResetWorkingCopy() {
				workingCopy = null;
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
