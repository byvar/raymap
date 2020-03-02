using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.Visual.PS2Optimized {
	public class PS2OptimizedSDCStructureElement {
		public Pointer offset;
		public PS2OptimizedSDCStructure geo;
		public int index;

		public uint num_vertices_actual;
		public uint num_vertices;
		public uint num_uvs;
		public uint unk3;
		public Vertex[] vertices;
		public UVUnoptimized[] uvUnoptimized;
		public TexCoord[] uv0;
		public TexCoord[] uv1;
		public Normal[] normals;
		public TexCoord[] uv_unk;
		public VectorForSinusEffect[] sinusState;

		public VertexColor[][] colors;

		public PS2OptimizedSDCStructureElement(PS2OptimizedSDCStructure geo, int index) {
			this.geo = geo;
			this.index = index;
		}

		public void Read(Reader reader) {
			offset = Pointer.Current(reader);
			if (geo.Type == 4 || geo.Type == 5 || geo.Type == 6) {
				// Optimized
				num_vertices_actual = reader.ReadUInt32();
				num_vertices = reader.ReadUInt32();
				num_uvs = reader.ReadUInt32();
				unk3 = reader.ReadUInt32();
			} else {
				num_vertices = geo.num_triangles[index] * 3;
				num_uvs = (num_vertices + 3) >> 2;
				num_vertices_actual = num_vertices;
			}
			VisualMaterial vm = geo.visualMaterials[index];
			bool hasUv0 = true, hasUv1 = false, hasNormals = false;
			uint num_textures = 1;
			if (vm != null && vm.num_textures_in_material > 0) {
				hasUv0 = false;
				num_textures = vm.num_textures_in_material;
				for (int i = 0; i < vm.num_textures_in_material; i++) {
					switch (vm.textures[i].uvFunction) {
						case 4: hasNormals = true; break;
						case 1: hasUv1 = true; break;
						case 0: hasUv0 = true; break;
					}
				}
			}
			vertices = new Vertex[num_vertices];
			for (int i = 0; i < num_vertices; i++) {
				vertices[i] = new Vertex(reader);
			}
			if (geo.Type == 1 && Settings.s.game == Settings.Game.R3) {
				uvUnoptimized = new UVUnoptimized[num_vertices];
				for (int i = 0; i < uvUnoptimized.Length; i++) {
					uvUnoptimized[i] = new UVUnoptimized(reader);
				}
			} else {
				if (hasUv0) {
					uv0 = new TexCoord[num_uvs];
					for (int i = 0; i < uv0.Length; i++) {
						uv0[i] = new TexCoord(reader);
					}
				}
				if (hasUv1) {
					uv1 = new TexCoord[num_uvs];
					for (int i = 0; i < uv1.Length; i++) {
						uv1[i] = new TexCoord(reader);
					}
				}
				if (hasNormals) {
					normals = new Normal[num_vertices];
					for (int i = 0; i < normals.Length; i++) {
						normals[i] = new Normal(reader);
					}
				}
				if ((geo.flags & 0x100) != 0) {
					uv_unk = new TexCoord[num_uvs];
					for (int i = 0; i < uv_unk.Length; i++) {
						uv_unk[i] = new TexCoord(reader);
					}
				}
			}
			colors = new VertexColor[num_textures][]; // Seem to be in a color-like format? 7F 7F 7F 80, repeated 4 times
			for (int i = 0; i < colors.Length; i++) {
				colors[i] = new VertexColor[num_uvs];
				for (int j = 0; j < colors[i].Length; j++) {
					colors[i][j] = new VertexColor(reader);
				}
			}
			if(geo.isSinus != 0) {
				sinusState = new VectorForSinusEffect[num_uvs];
				for (int i = 0; i < sinusState.Length; i++) {
					sinusState[i] = new VectorForSinusEffect(reader);
				}
			}
			if ((index < geo.num_elements - 1 && Pointer.Current(reader) != geo.off_elements_array[index + 1])
				|| (index == geo.num_elements - 1 && Pointer.Current(reader) != geo.off_uint1)) {
				UnityEngine.Debug.LogWarning("B " + geo.Offset + " - " + offset + " - " + hasUv0 + " - " + hasUv1 + " - " + hasNormals);
			} else {
				//UnityEngine.Debug.LogWarning("G " + geo.Offset + " - " + offset + " - " + hasUv0 + " - " + hasUv1 + " - " + hasUv4);
			}
			/*normals = new Vertex[num_vertices];
			for (int i = 0; i < num_vertices; i++) {
				normals[i] = new Vertex(reader);
			}*/
		}

		public Vector3 GetUV(int index, int texIndex, bool applyBlendWeight) {
			Vector3 baseUV = Vector3.zero;
			if (geo.Type == 1 && Settings.s.game == Settings.Game.R3) {
				baseUV = new Vector3(uvUnoptimized[index].u, uvUnoptimized[index].v, 1f);
			} else {
				byte uvFunction = 0;
				if (geo.visualMaterials[this.index].textures.Count > texIndex) {
					uvFunction = geo.visualMaterials[this.index].textures[texIndex].uvFunction;
				}
				switch (uvFunction) {
					case 0: baseUV = GetUV0(index); break;
					case 1: baseUV = GetUV1(index); break;
					default: baseUV = new Vector3(0, 0, 1f); break;
				}
			}
			if (applyBlendWeight && colors != null && texIndex < colors.Length) {
				float weight = (float)(GetColor(texIndex, index).a) / 0x80;
				baseUV = new Vector3(baseUV.x, baseUV.y, weight);
			}
			return baseUV;
		}
		public Vector3 GetNormal(int index) {
			if (normals != null && normals.Length > index) {
				return normals[index].Vector;
			}
			return Vector3.zero;
		}

		public Vector4 GetColor(int index) {
			if (colors != null && colors.Length > 0) {
				return GetColor(0, index).Color;
				/*Vector4 currentCol = Vector4.one;
				for (int i = 0; i < colors.Length; i++) {
					Vector4 col = GetColor(i, index).ColorAndAlpha;
					if (i == 0 || col.w > 0) currentCol = new Vector4(col.x, col.y, col.z, 1f);
				}
				return currentCol;*/
			}
			return Vector4.one;
		}
		public Vector3 GetUV0(int index) {
			if (uv0 != null) {
				int uv0Index = index / 4;
				int indexInUV = index % 4;
				TexCoord.UV uv = uv0[uv0Index].uv[indexInUV];
				return new Vector3(uv.u / 4096f, uv.v / 4096f, 1f);
			}
			return Vector3.zero;
		}
		public VertexColor.Col GetColor(int texIndex, int index) {
			if (colors != null) {
				int uv0Index = index / 4;
				int indexInUV = index % 4;
				VertexColor.Col w = colors[texIndex][uv0Index].uv[indexInUV];
				return w;
			}
			return default;
		}
		public Vector3 GetUV1(int index) {
			if (uv1 != null) {
				int uv0Index = index / 4;
				int indexInUV = index % 4;
				TexCoord.UV uv = uv1[uv0Index].uv[indexInUV];
				return new Vector3(uv.u / 4096f, uv.v / 4096f, 1f);
			}
			return Vector3.zero;
		}

		public class Vertex {
			public float x;
			public float y;
			public float z;
			public float w;

			public Vertex(Reader reader) {
				x = reader.ReadSingle();
				y = reader.ReadSingle();
				z = reader.ReadSingle();
				w = reader.ReadSingle();
			}

			// override object.Equals
			public override bool Equals(object obj) {
				//       
				// See the full list of guidelines at
				//   http://go.microsoft.com/fwlink/?LinkID=85237  
				// and also the guidance for operator== at
				//   http://go.microsoft.com/fwlink/?LinkId=85238
				//

				if (obj == null || GetType() != obj.GetType()) {
					return false;
				}

				Vertex vo = obj as Vertex;
				return (x == vo.x && y == vo.y && z == vo.z);
			}

			// override object.GetHashCode
			public override int GetHashCode() {
				return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
			}
		}
		public class TexCoord {
			public UV[] uv;

			public TexCoord(Reader reader) {
				uv = new UV[4];
				for (int i = 0; i < 4; i++) {
					uv[i] = new UV(reader);
				}
			}

			public struct UV {
				public short u;
				public short v;

				public UV(Reader reader) {
					u = reader.ReadInt16();
					v = reader.ReadInt16();
				}
			}
		}
		public class VertexColor {
			public Col[] uv;
			public VertexColor(Reader reader) {
				uv = new Col[4];
				for (int i = 0; i < 4; i++) {
					uv[i] = new Col(reader);
				}
			}
			public struct Col {
				public byte r;
				public byte g;
				public byte b;
				// Blend weight
				public byte a;
				public Col(Reader reader) {
					r = reader.ReadByte();
					g = reader.ReadByte();
					b = reader.ReadByte();
					a = reader.ReadByte();
				}

				/*public Vector3 Normal {
					get {
						float x = ((this.x & 0x7F) / (float)0x7F) * (((this.x & 0x80) != 0) ? -1 : 1);
						float y = ((this.y & 0x7F) / (float)0x7F) * (((this.y & 0x80) != 0) ? -1 : 1);
						float z = ((this.z & 0x7F) / (float)0x7F) * (((this.z & 0x80) != 0) ? -1 : 1);
						return new Vector3(x, z, y);
					}
				}*/
				/*public Vector3 Normal {
					get {
						float x = (this.b - 128) / 127f;
						float y = (this.g - 128) / 127f;
						float z = (this.r - 128) / 127f;
						return new Vector3(x, z, y);
					}
				}*/
				public Vector4 Color {
					get {
						return new Vector4(r / 127f, g / 127f, b / 127f, 1f);
					}
				}
				public Vector4 ColorAndAlpha {
					get {
						return new Vector4(r / 127f, g / 127f, b / 127f, a / 0x80);
					}
				}
			}
		}
		public class UVUnoptimized {
			public float u;
			public float v;
			public float z;
			public float w;
			public UVUnoptimized(Reader reader) {
				u = reader.ReadSingle();
				v = reader.ReadSingle();
				z = reader.ReadSingle();
				w = reader.ReadSingle();
			}
		}
		public class Normal {
			public float x;
			public float y;
			public float z;
			public float w;

			public Normal(Reader reader) {
				x = reader.ReadSingle();
				y = reader.ReadSingle();
				z = reader.ReadSingle();
				w = reader.ReadSingle();
			}

			public Vector3 Vector {
				get { return new Vector3(x, z, y); }
			}
		}
		public class VectorForSinusEffect {
			public float x;
			public float y;
			public float z;
			public float w;

			public VectorForSinusEffect(Reader reader) {
				x = reader.ReadSingle();
				y = reader.ReadSingle();
				z = reader.ReadSingle();
				w = reader.ReadSingle();
			}
		}
	}
}
