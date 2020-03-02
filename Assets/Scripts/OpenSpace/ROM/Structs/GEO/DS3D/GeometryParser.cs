using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.ROM.DS3D {
	public class GeometryParser {
		private static uint extractBits(uint number, int count, int offset) {
			return (uint)(((1 << count) - 1) & (number >> (offset)));
		}
		public static byte[] Decompress(byte[] compressed) {
			byte[] data = new byte[compressed.Length];
			Array.Copy(compressed, data, compressed.Length);
			uint src = 0, dst = 0;
			byte compressionType = compressed[0];
			uint size = 0;
			using (MemoryStream str = new MemoryStream(compressed)) {
				using (Reader reader = new Reader(str, Settings.s.IsLittleEndian)) {
					size = reader.ReadUInt32();
					size = extractBits(size, 24, 8);
				}
			}
			//MapLoader.Loader.print("Size: " + string.Format("{0:X8}", size));
			Array.Resize(ref data, (int)size);
			src += 4;
			while (dst < size) {
				byte flags = compressed[src];
				src++;
				for (int i = 0; i < 8; i++) {
					bool isCompressed = (flags & 0x80) != 0;
					flags <<= 1;
					if (isCompressed) {
						uint length = 3;
						if (compressionType == 0x11) {
							if (compressed[src] / 0x10 > 1) {
								length = 0x001;
							} else if (compressed[src] / 0x10 < 1) {
								length = 0x011 + (uint)(compressed[src] & 0xF) * 0x10;
								src++;
							} else {
								length = 0x111 + (uint)(compressed[src] & 0xF) * 0x1000 + (uint)compressed[src+1] * 0x10;
								src += 2;
							}
						}
						length = length + ((uint)compressed[src] >> 4);
						uint offset = ((uint)(compressed[src] & 0xF) << 8) + (uint)compressed[src + 1] + 1;
						src += 2;
						for (int j = 0; j < length; j++) {
							data[dst] = data[dst - offset];
							dst++;
						}
					} else {
						// Not compressed
						if (src >= compressed.Length) {
							MapLoader.Loader.print(src + " - " + compressed.Length + " - " + size);
						}
						data[dst] = compressed[src];
						dst++;
						src++;
					}
					if (dst >= size) break;
				}
			}
			return data;
		}
		public static byte[] ReadCompressed(Reader reader) {
			uint size = reader.ReadUInt32();
			byte compressionType = (byte)extractBits(size, 8, 0);
			size = extractBits(size, 24, 8);
			byte[] data = new byte[size];
			uint dst = 0;

			while (dst < size) {
				byte flags = reader.ReadByte();
				for (int i = 0; i < 8; i++) {
					bool isCompressed = (flags & 0x80) != 0;
					flags <<= 1;
					if (isCompressed) {
						uint length = 3;
						byte curByte = reader.ReadByte();
						if (compressionType == 0x11) {
							if (curByte / 0x10 > 1) {
								length = 0x001;
							} else if (curByte / 0x10 < 1) {
								length = 0x011 + (uint)(curByte & 0xF) * 0x10;
								curByte = reader.ReadByte();
							} else {
								byte nextByteTemp = reader.ReadByte();
								length = 0x111 + (uint)(curByte & 0xF) * 0x1000 + (uint)nextByteTemp * 0x10;
								curByte = reader.ReadByte();
							}
						}
						byte nextByte = reader.ReadByte();
						length = length + ((uint)curByte >> 4);
						uint offset = ((uint)(curByte & 0xF) << 8) + (uint)nextByte + 1;
						for (int j = 0; j < length; j++) {
							data[dst] = data[dst - offset];
							dst++;
						}
					} else {
						// Not compressed
						data[dst] = reader.ReadByte();
						dst++;
					}
					if (dst >= size) break;
				}
			}
			return data;
		}
		public static GeometryCommand[] ReadCommands(byte[] data) {
			List<GeometryCommand> commands = new List<GeometryCommand>();
			/*if (Settings.s.game == Settings.Game.RRR) {
				data = Decompress(data);
			}*/
			using (MemoryStream str = new MemoryStream(data)) {
				using (Reader dataReader = new Reader(str, Settings.s.IsLittleEndian)) {
					while (dataReader.BaseStream.Position < data.Length) {
						byte com0 = dataReader.ReadByte();
						byte com1 = dataReader.ReadByte();
						byte com2 = dataReader.ReadByte();
						byte com3 = dataReader.ReadByte();
						if (com0 != 0) commands.Add(new GeometryCommand(com0, dataReader));
						if (com1 != 0) commands.Add(new GeometryCommand(com1, dataReader));
						if (com2 != 0) commands.Add(new GeometryCommand(com2, dataReader));
						if (com3 != 0) commands.Add(new GeometryCommand(com3, dataReader));
					}
				}
			}
			return commands.ToArray();
		}

		public static Mesh Parse(GeometryCommand[] commands, GeometricObject go, bool backfaceCulling, Material mat) {
			List<Vertex> verts = new List<Vertex>();
			Vertex wipVertex = new Vertex();
			List<Triangle> triangles = new List<Triangle>();
			GeometryCommand.PrimitiveType primitive = GeometryCommand.PrimitiveType.Triangles;
			int lastVertexCount = 0;
			Texture tex = mat.GetTexture("_Tex0");
			float wFactor = 64f, hFactor = 64f;
			if (tex != null) {
				wFactor = tex.width;
				hFactor = tex.height;
			}
			
			Mesh mesh = new Mesh();
			bool parsing = true;
			foreach (GeometryCommand c in commands) {
				switch (c.Command) {
					case GeometryCommand.Type.TEXCOORD:
						wipVertex.uv = new Vector2(c.u / c.scale / wFactor, c.v / c.scale / hFactor);
						break;
					case GeometryCommand.Type.COLOR:
						wipVertex.color = new Color(c.r / c.scale, c.g / c.scale, c.b / c.scale, 1f);
						break;
					case GeometryCommand.Type.NORMAL:
						wipVertex.normal = new Vector3(c.x / c.scale, c.y / c.scale, c.z / c.scale);
						break;
					case GeometryCommand.Type.VTX_XY:
						wipVertex.x = c.x / c.scale;
						wipVertex.y = c.y / c.scale;
						verts.Add(wipVertex.Clone());
						break;
					case GeometryCommand.Type.VTX_XZ:
						wipVertex.x = c.x / c.scale;
						wipVertex.z = c.z / c.scale;
						verts.Add(wipVertex.Clone());
						break;
					case GeometryCommand.Type.VTX_YZ:
						wipVertex.y = c.y / c.scale;
						wipVertex.z = c.z / c.scale;
						verts.Add(wipVertex.Clone());
						break;
					case GeometryCommand.Type.VTX_16:
						wipVertex.x = c.x / c.scale;
						wipVertex.y = c.y / c.scale;
						wipVertex.z = c.z / c.scale;
						verts.Add(wipVertex.Clone());
						break;
					case GeometryCommand.Type.BEGIN_VTXS:
						AddTriangles(triangles, verts.Count, lastVertexCount, primitive);
						primitive = c.Primitive;
						/*if (primitive != GeometryCommand.PrimitiveType.Triangles && primitive != GeometryCommand.PrimitiveType.TriangleStrip) {
							Debug.LogError("QUADS???? FUUUU! " + primitive);
						}*/
			lastVertexCount = verts.Count;
						break;
					case GeometryCommand.Type.END_VTXS:
						break;
					default:
						Debug.LogError("Unparsed command: " + c.Command);
						break;
				}
				if (!parsing) break;
			}
			AddTriangles(triangles, verts.Count, lastVertexCount, primitive);

			mesh.vertices = verts.Select(v => new Vector3(v.x/go.ScaleFactor, v.z/go.ScaleFactor, v.y/go.ScaleFactor)).ToArray();
			mesh.normals = verts.Select(v => new Vector3(v.normal.x, v.normal.z, v.normal.y)).ToArray();
			mesh.SetUVs(0, verts.Select(v => new Vector3(v.uv.x, v.uv.y, 1f)).ToList());
			mesh.SetColors(verts.Select(v => new Color(v.color.r, v.color.g, v.color.b, v.color.a)).ToList());
			mesh.triangles = triangles.SelectMany(t => backfaceCulling ? new int[] { t.v1, t.v2, t.v3 } : new int[] { t.v1, t.v2, t.v3, t.v2, t.v1, t.v3 }).ToArray();
			mesh.RecalculateNormals();

			return mesh;
		}


		public static Vector3[] ParseVerticesOnly(GeometryCommand[] commands, GeometricObject go) {
			List<Vertex> verts = new List<Vertex>();
			Vertex wipVertex = new Vertex();
			List<Triangle> triangles = new List<Triangle>();
			GeometryCommand.PrimitiveType primitive = GeometryCommand.PrimitiveType.Triangles;
			int lastVertexCount = 0;

			Mesh mesh = new Mesh();
			bool parsing = true;
			foreach (GeometryCommand c in commands) {
				switch (c.Command) {
					case GeometryCommand.Type.VTX_XY:
						wipVertex.x = c.x / c.scale;
						wipVertex.y = c.y / c.scale;
						verts.Add(wipVertex.Clone());
						break;
					case GeometryCommand.Type.VTX_XZ:
						wipVertex.x = c.x / c.scale;
						wipVertex.z = c.z / c.scale;
						verts.Add(wipVertex.Clone());
						break;
					case GeometryCommand.Type.VTX_YZ:
						wipVertex.y = c.y / c.scale;
						wipVertex.z = c.z / c.scale;
						verts.Add(wipVertex.Clone());
						break;
					case GeometryCommand.Type.VTX_16:
						wipVertex.x = c.x / c.scale;
						wipVertex.y = c.y / c.scale;
						wipVertex.z = c.z / c.scale;
						verts.Add(wipVertex.Clone());
						break;
					case GeometryCommand.Type.BEGIN_VTXS:
						AddTriangles(triangles, verts.Count, lastVertexCount, primitive);
						primitive = c.Primitive;
						/*if (primitive != GeometryCommand.PrimitiveType.Triangles && primitive != GeometryCommand.PrimitiveType.TriangleStrip) {
							Debug.LogError("QUADS???? FUUUU! " + primitive);
						}*/
						lastVertexCount = verts.Count;
						break;
					case GeometryCommand.Type.END_VTXS:
						break;
				}
				if (!parsing) break;
			}
			return verts.Select(v => new Vector3(v.x / go.ScaleFactor, v.z / go.ScaleFactor, v.y / go.ScaleFactor)).ToArray();
		}

		private static void AddTriangles(List<Triangle> triangles, int curVertexCount, int lastVertexCount, GeometryCommand.PrimitiveType primitive) {
			if (lastVertexCount != curVertexCount) {
				int numTriangles;
				switch (primitive) {
					case GeometryCommand.PrimitiveType.Triangles:
						numTriangles = (curVertexCount - lastVertexCount) / 3;
						for (int i = 0; i < numTriangles; i++) {
							triangles.Add(new Triangle() {
								v1 = lastVertexCount + (i * 3) + 0,
								v2 = lastVertexCount + (i * 3) + 2,
								v3 = lastVertexCount + (i * 3) + 1,
							});
						}
						break;
					case GeometryCommand.PrimitiveType.TriangleStrip:
						numTriangles = (curVertexCount - lastVertexCount) - 2;
						for (int i = 0; i < numTriangles; i++) {
							triangles.Add(new Triangle() {
								v1 = lastVertexCount + i + 0,
								v2 = lastVertexCount + i + 1,
								v3 = lastVertexCount + i + 2,
							});
						}
						break;
					case GeometryCommand.PrimitiveType.Quads:
						numTriangles = (curVertexCount - lastVertexCount) / 4;
						for (int i = 0; i < numTriangles; i++) {
							triangles.Add(new Triangle() {
								v1 = lastVertexCount + i + 0,
								v2 = lastVertexCount + i + 2,
								v3 = lastVertexCount + i + 1,
							});
							triangles.Add(new Triangle() {
								v1 = lastVertexCount + i + 2,
								v2 = lastVertexCount + i + 1,
								v3 = lastVertexCount + i + 3,
							});
						}
						break;
					case GeometryCommand.PrimitiveType.QuadStrip:
						numTriangles = (curVertexCount - lastVertexCount) / 2 - 1;
						for (int i = 0; i < numTriangles; i++) {
							triangles.Add(new Triangle() {
								v1 = lastVertexCount + (i * 2) + 0,
								v2 = lastVertexCount + (i * 2) + 1,
								v3 = lastVertexCount + (i * 2) + 2,
							});
							triangles.Add(new Triangle() {
								v1 = lastVertexCount + (i * 2) + 1,
								v2 = lastVertexCount + (i * 2) + 2,
								v3 = lastVertexCount + (i * 2) + 3,
							});
						}
						break;
				}
			}
		}

		private class Vertex {
			public float x;
			public float y;
			public float z;
			public Vector3 normal;
			public Vector2 uv;
			public Color color = Color.white;

			public Vertex Clone() {
				return new Vertex() {
					x = x,
					y = y,
					z = z,
					normal = normal,
					uv = uv,
					color = color
				};
			}
		}
		private class Triangle {
			public int v1;
			public int v2;
			public int v3;
		}
	}
}
