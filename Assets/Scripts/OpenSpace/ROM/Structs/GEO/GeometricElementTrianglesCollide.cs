using OpenSpace.Loader;
using System;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class GeometricElementTrianglesCollide : ROMStruct {
		public GenericReference material;
		public ushort ind_material;
		public Reference<GeometricElementTrianglesCollideData> triangles;
		public ushort ind_37;
		public ushort num_triangles;
		public ushort type_material;
		public ushort unk;

		protected override void ReadInternal(Reader reader) {
			ind_material = reader.ReadUInt16();
			triangles = new Reference<GeometricElementTrianglesCollideData>(reader);
			ind_37 = reader.ReadUInt16();
			num_triangles = reader.ReadUInt16();
			unk = reader.ReadUInt16();
			type_material = reader.ReadUInt16();
			
			triangles.Resolve(reader, t => t.length = num_triangles);
			material = new GenericReference(type_material, ind_material, reader, true);
		}

		public GameObject GetGameObject(GeometricObject.Type type, GeometricObject go) {
			GameObject gao = null;
			if (type == GeometricObject.Type.Collide) {
				gao = new GameObject("Element @ " + Offset);
				gao.layer = LayerMask.NameToLayer("Collide");
				gao.transform.localPosition = Vector3.zero;
				MeshRenderer mr = gao.AddComponent<MeshRenderer>();
				MeshFilter mf = gao.AddComponent<MeshFilter>();
				mr.material = MapLoader.Loader.collideMaterial;
				if (material.Value != null && material.Value is GameMaterial) {
					GameMaterial gmt = material.Value as GameMaterial;
					MapLoader.Loader.print(gmt.collideMaterial);
					if (gmt.collideMaterial.Value != null) {
						gmt.collideMaterial.Value.SetMaterial(mr);
					}
				} else {
					MapLoader.Loader.print("Type: " + type_material + " - Ind: " + ind_material);
				}
				Mesh mesh = new Mesh();
				mesh.vertices = go.verticesCollide.Value.GetVectors(go.ScaleFactor);
				//mesh.normals = go.normals.Value.GetVectors(Int16.MaxValue);
				//mesh.SetUVs(0, triangles.Value.uvs.Select(u => new Vector3(u.x, u.y, 1f)).ToList());
				mesh.triangles = triangles.Value.triangles.SelectMany(t => new int[] { t.v2, t.v1, t.v3 }).ToArray();
				mesh.RecalculateNormals();
				mf.mesh = mesh;
			}
			return gao;
		}
	}
}
