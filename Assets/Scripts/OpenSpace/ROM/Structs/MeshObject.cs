using OpenSpace.Loader;
using System;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class MeshObject : ROMStruct {
		public float scaleFactor;
		public float factor_1;
		public Reference<CompressedVector3Array> vertices_0;
		public Reference<CompressedVector3Array> vertices_1;
		public Reference<CompressedVector3Array> normals;
		public Reference<GeometricElementList1> elements_0;
		public Reference<GeometricElementList2> elements_1;
		public ushort num_vectors_0;
		public ushort num_vectors_1;
		public ushort num_elements_0;
		public ushort num_elements_1;
		public ushort unk0;
		public ushort unk1;
		public ushort unk2;
		public ushort unk3;

		public float ScaleFactor {
			get {
				if (scaleFactor != 0f) return scaleFactor;
				return 1f;
			}
		}

		protected override void ReadInternal(Reader reader) {
			scaleFactor = reader.ReadSingle();
			factor_1 = reader.ReadSingle();
			vertices_0 = new Reference<CompressedVector3Array>(reader);
			vertices_1 = new Reference<CompressedVector3Array>(reader);
			normals = new Reference<CompressedVector3Array>(reader);
			elements_0 = new Reference<GeometricElementList1>(reader);
			elements_1 = new Reference<GeometricElementList2>(reader);
			num_vectors_0 = reader.ReadUInt16();
			num_vectors_1 = reader.ReadUInt16();
			num_elements_0 = reader.ReadUInt16();
			num_elements_1 = reader.ReadUInt16();
			unk0 = reader.ReadUInt16();
			unk1 = reader.ReadUInt16();
			unk2 = reader.ReadUInt16();
			unk3 = reader.ReadUInt16();

			//MapLoader.Loader.print("Vertices: " + num_vectors_1 + " or " + string.Format("{0:X4}", num_vectors_1));

			vertices_0.Resolve(reader, v => { v.length = num_vectors_0; });
			vertices_1.Resolve(reader, v => { v.length = num_vectors_1; });
			normals.Resolve(reader, v => { v.length = num_vectors_1; });
			elements_0.Resolve(reader, v => { v.length = num_elements_0; });
			elements_1.Resolve(reader, v => { v.length = num_elements_1; });
			
			if (Settings.s.platform == Settings.Platform._3DS) {
				MapLoader l = MapLoader.Loader;
				if (elements_1.Value != null) {
					GameObject gao = new GameObject("MeshObject @ " + Offset);
					gao.name = gao.name + " - S0:" + scaleFactor
										+ " - S1:" + factor_1
										+ " - U0:" + string.Format("{0:X4}", unk0)
										+ " - U1:" + string.Format("{0:X4}", unk1)
										+ " - U2:" + string.Format("{0:X4}", unk2)
										+ " - U3:" + string.Format("{0:X4}", unk3);
					//gao.transform.position = new Vector3(UnityEngine.Random.Range(-100f, 100f), UnityEngine.Random.Range(-100f, 100f), UnityEngine.Random.Range(-100f, 100f));
					foreach (GeometricElementList2.GeometricElementListEntry entry in elements_1.Value.elements) {
						if (entry.element.Value == null) {
							l.print(entry.element.type);
						}
						if (entry.element.Value != null) {
							if (entry.element.Value is MeshElement) {
								MeshElement el = entry.element.Value as MeshElement;
								GameObject elGao = new GameObject("Element @ " + el.Offset);
								/*if (el.visualMaterial.Value != null) {
									elGao.name = elGao.name
										+ " - F:" + string.Format("{0:X4}", el.visualMaterial.Value.flags)
										+ " - 0:" + string.Format("{0:X4}", el.visualMaterial.Value.unk0)
										+ " - 1:" + string.Format("{0:X4}", el.visualMaterial.Value.unk1)
										+ " - 4:" + string.Format("{0:X4}", el.visualMaterial.Value.unk4);
								}*/
								/*if (el.visualMaterial.Value != null
									&& el.visualMaterial.Value.textures.Value != null
									&& el.visualMaterial.Value.textures.Value.length > 0) {
									TextureInfo tex = el.visualMaterial.Value.textures.Value.vmTex[0].texRef.Value.texInfo;
									elGao.name = elGao.name
										+ " - F1:" + string.Format("{0:X4}", tex.flags)
										+ " - F2:" + string.Format("{0:X4}", tex.flags2);
								}*/
								bool backfaceCulling = !el.visualMaterial.Value.RenderBackFaces;
								elGao.transform.parent = gao.transform;
								elGao.transform.localPosition = Vector3.zero;
								MeshRenderer mr = elGao.AddComponent<MeshRenderer>();
								MeshFilter mf = elGao.AddComponent<MeshFilter>();
								Mesh mesh = new Mesh();
								/*mesh.vertices = vectors_1.Value.GetVectors(scaleFactor != 0 ? scaleFactor : 1);
								mesh.normals = vectors_2.Value.GetVectors(Int16.MaxValue);*/
								if (el.num_vertices == 0) {
									mesh.vertices = vertices_1.Value.GetVectors(ScaleFactor);
									mesh.normals = normals.Value.GetVectors(Int16.MaxValue);
								} else {
									// Use vertices located in element
									mesh.vertices = el.triangles.Value.verts.Select(v => v.GetVector(ScaleFactor)).ToArray();
									mesh.normals = el.triangles.Value.normals.Select(v => v.GetVector(Int16.MaxValue)).ToArray();
								}

								mesh.SetUVs(0, el.triangles.Value.uvs.Select(u => new Vector3(u.x, u.y, 1f)).ToList());
								mesh.triangles = el.triangles.Value.triangles.SelectMany(t => backfaceCulling ? new int[] { t.v2, t.v1, t.v3 } : new int[] { t.v2, t.v1, t.v3, t.v1, t.v2, t.v3 }).ToArray();
								mf.mesh = mesh;
								mr.material = el.visualMaterial.Value.Mat;
							}
						}
					}
				}
			}
		}
    }
}
