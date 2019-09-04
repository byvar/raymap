using OpenSpace.Loader;
using System;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class MeshObject : ROMStruct {
		public short scaleFactor;
		public short factor_1;
		public Reference<CompressedVector3Array> vectors_0;
		public Reference<CompressedVector3Array> vectors_1;
		public Reference<CompressedVector3Array> vectors_2;
		public Reference<GeometricElementList1> elements_0;
		public Reference<GeometricElementList2> elements_1;
		public ushort num_vectors_0;
		public ushort num_vectors_1;
		public ushort num_elements_0;
		public ushort num_elements_1;

		protected override void ReadInternal(Reader reader) {
			scaleFactor = reader.ReadInt16();
			factor_1 = reader.ReadInt16();
			reader.ReadUInt16();
			reader.ReadUInt16();
			vectors_0 = new Reference<CompressedVector3Array>(reader);
			vectors_1 = new Reference<CompressedVector3Array>(reader);
			vectors_2 = new Reference<CompressedVector3Array>(reader);
			elements_0 = new Reference<GeometricElementList1>(reader);
			elements_1 = new Reference<GeometricElementList2>(reader);
			num_vectors_0 = reader.ReadUInt16();
			num_vectors_1 = reader.ReadUInt16();
			num_elements_0 = reader.ReadUInt16();
			num_elements_1 = reader.ReadUInt16();
			reader.ReadUInt16();
			reader.ReadUInt16();
			reader.ReadUInt16();
			reader.ReadUInt16();

			//MapLoader.Loader.print("Vertices: " + num_vectors_1 + " or " + string.Format("{0:X4}", num_vectors_1));

			vectors_0.Resolve(reader, v => { v.length = num_vectors_0; });
			vectors_1.Resolve(reader, v => { v.length = num_vectors_1; });
			vectors_2.Resolve(reader, v => { v.length = num_vectors_1; });
			elements_0.Resolve(reader, v => { v.length = num_elements_0; });
			elements_1.Resolve(reader, v => { v.length = num_elements_1; });
			
			if (Settings.s.platform == Settings.Platform._3DS) {
				MapLoader l = MapLoader.Loader;
				if (elements_1.Value != null) {
					GameObject gao = new GameObject("MeshObject @ " + Offset);
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
										+ " - 2:" + string.Format("{0:X8}", el.visualMaterial.Value.unk2)
										+ " - 3:" + string.Format("{0:X8}", el.visualMaterial.Value.unk3)
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
								elGao.transform.parent = gao.transform;
								elGao.transform.localPosition = Vector3.zero;
								MeshRenderer mr = elGao.AddComponent<MeshRenderer>();
								MeshFilter mf = elGao.AddComponent<MeshFilter>();
								Mesh mesh = new Mesh();
								/*mesh.vertices = vectors_1.Value.GetVectors(scaleFactor != 0 ? scaleFactor : 1);
								mesh.normals = vectors_2.Value.GetVectors(Int16.MaxValue);*/
								if (el.num_uvs == 0) {
									mesh.vertices = vectors_1.Value.GetVectors(scaleFactor != 0 ? scaleFactor : 1);
									mesh.normals = vectors_2.Value.GetVectors(Int16.MaxValue);
								} else {
									mesh.vertices = el.triangles.Value.verts.Select(v => v.GetVector(scaleFactor != 0 ? scaleFactor : 1)).ToArray();
									mesh.normals = el.triangles.Value.normals.Select(v => v.GetVector(Int16.MaxValue)).ToArray();
								}

								mesh.SetUVs(0, el.triangles.Value.uvs.Select(u => new Vector3(u.x, u.y, 1f)).ToList());
								mesh.triangles = el.triangles.Value.triangles.SelectMany(t => new int[] { t.v2, t.v1, t.v3 }).ToArray();
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
