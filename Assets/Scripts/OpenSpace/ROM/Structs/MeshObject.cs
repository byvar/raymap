using OpenSpace.Loader;
using System;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class MeshObject : ROMStruct {
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
			reader.ReadUInt16();
			reader.ReadUInt16();
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

			MapLoader.Loader.print("Vertices: " + num_vectors_1 + " or " + string.Format("{0:X4}", num_vectors_1));

			vectors_0.Resolve(reader, v => { v.length = num_vectors_0; });
			vectors_1.Resolve(reader, v => { v.length = num_vectors_1; });
			vectors_2.Resolve(reader, v => { v.length = num_vectors_1; });
			elements_0.Resolve(reader, v => { v.length = num_elements_0; });
			elements_1.Resolve(reader, v => { v.length = num_elements_1; });
			
			if (Settings.s.platform == Settings.Platform._3DS) {
				MapLoader l = MapLoader.Loader;
				if (elements_1.Value != null) {
					foreach (GeometricElementList2.GeometricElementListEntry entry in elements_1.Value.elements) {
						l.print(entry.element.type);
						if (entry.element.Value != null) {
							if (entry.element.Value is MeshElement) {
								MeshElement el = entry.element.Value as MeshElement;
								GameObject gao = new GameObject("Element @ " + Offset);
								gao.transform.position = new Vector3(UnityEngine.Random.Range(-100f, 100f), UnityEngine.Random.Range(-100f, 100f), UnityEngine.Random.Range(-100f, 100f));
								MeshRenderer mr = gao.AddComponent<MeshRenderer>();
								MeshFilter mf = gao.AddComponent<MeshFilter>();
								Mesh mesh = new Mesh();
								mesh.vertices = vectors_1.Value.GetVectors((float)256f);
								mesh.normals = vectors_2.Value.GetVectors((float)Int16.MaxValue);
								mesh.SetUVs(0, el.triangles.Value.uvs.Select(u => new Vector3(u.x, u.y, 1f)).ToList());
								mesh.triangles = el.triangles.Value.triangles.SelectMany(t => new int[] { t.v1, t.v2, t.v3 }).ToArray();
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
