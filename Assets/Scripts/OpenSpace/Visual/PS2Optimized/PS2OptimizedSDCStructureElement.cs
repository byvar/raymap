using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.Visual.PS2Optimized {
	public class PS2OptimizedSDCStructureElement {
		public Pointer offset;
		public PS2OptimizedSDCStructure geo;
		public int index;

		public uint unk0;
		public uint num_vertices;
		public uint num_uvs;
		public uint unk3;
		public Vertex[] vertices;
		public Vertex[] normals;
		public UV0[] uv0;
		public UV1[] uv1;
		public VertexColor[] blendWeights;
		public VectorForSinusEffect[] sinusState;

		public UV0[][] uv_tex;

		public PS2OptimizedSDCStructureElement(PS2OptimizedSDCStructure geo, int index) {
			this.geo = geo;
			this.index = index;
		}

		public void Read(Reader reader) {
			offset = Pointer.Current(reader);
			if (geo.Type == 4 || geo.Type == 5 || geo.Type == 6) {
				// Optimized
				unk0 = reader.ReadUInt32();
				num_vertices = reader.ReadUInt32();
				num_uvs = reader.ReadUInt32();
				unk3 = reader.ReadUInt32();
			} else {
				num_vertices = geo.num_triangles[index] * 3;
				num_uvs = (num_vertices + 3) >> 2;
			}
			VisualMaterial vm = geo.visualMaterials[index];
			bool hasUv0 = true, hasUv1 = false, hasUv4 = false;
			uint num_textures = 1;
			if (vm != null && vm.num_textures_in_material > 0) {
				hasUv0 = false;
				num_textures = vm.num_textures_in_material;
				for (int i = 0; i < vm.num_textures_in_material; i++) {
					switch (vm.textures[i].uvFunction) {
						case 4: hasUv4 = true; break;
						case 1: hasUv1 = true; break;
						case 0: hasUv0 = true; break;
					}
				}
			}
			vertices = new Vertex[num_vertices];
			for (int i = 0; i < num_vertices; i++) {
				vertices[i] = new Vertex(reader);
			}
			if (geo.Type == 1) {
				blendWeights = new VertexColor[num_vertices];
				for (int i = 0; i < blendWeights.Length; i++) {
					blendWeights[i] = new VertexColor(reader);
				}
			} else {
				if (hasUv0) {
					uv0 = new UV0[num_uvs];
					for (int i = 0; i < uv0.Length; i++) {
						uv0[i] = new UV0(reader);
					}
				}
				if (hasUv1) {
					uv1 = new UV1[num_uvs];
					for (int i = 0; i < uv1.Length; i++) {
						uv1[i] = new UV1(reader);
					}
				}
				if (hasUv4) {
					blendWeights = new VertexColor[num_vertices];
					for (int i = 0; i < blendWeights.Length; i++) {
						blendWeights[i] = new VertexColor(reader);
					}
				}
			}
			uv_tex = new UV0[num_textures][];
			for (int i = 0; i < uv_tex.Length; i++) {
				uv_tex[i] = new UV0[num_uvs];
				for (int j = 0; j < uv_tex[i].Length; j++) {
					uv_tex[i][j] = new UV0(reader);
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
				UnityEngine.Debug.LogWarning("B " + geo.Offset + " - " + offset + " - " + hasUv0 + " - " + hasUv1 + " - " + hasUv4);
			} else {
				//UnityEngine.Debug.LogWarning("G " + geo.Offset + " - " + offset + " - " + hasUv0 + " - " + hasUv1 + " - " + hasUv4);
			}
			/*normals = new Vertex[num_vertices];
			for (int i = 0; i < num_vertices; i++) {
				normals[i] = new Vertex(reader);
			}*/
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
		}
		public class UV0 {
			public uint unk0;
			public uint unk1;
			public uint unk2;
			public uint unk3;

			public UV0(Reader reader) {
				unk0 = reader.ReadUInt32();
				unk1 = reader.ReadUInt32();
				unk2 = reader.ReadUInt32();
				unk3 = reader.ReadUInt32();
			}
		}
		public class UV1 {
			public uint unk0;
			public uint unk1;
			public uint unk2;
			public uint unk3;

			public UV1(Reader reader) {
				unk0 = reader.ReadUInt32();
				unk1 = reader.ReadUInt32();
				unk2 = reader.ReadUInt32();
				unk3 = reader.ReadUInt32();
			}
		}
		public class VertexColor {
			public float x;
			public float y;
			public float z;
			public float w;

			public VertexColor(Reader reader) {
				x = reader.ReadSingle();
				y = reader.ReadSingle();
				z = reader.ReadSingle();
				w = reader.ReadSingle();
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
