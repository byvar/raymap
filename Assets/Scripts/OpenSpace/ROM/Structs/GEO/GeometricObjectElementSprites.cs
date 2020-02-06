using OpenSpace.Loader;
using System;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class GeometricObjectElementSprites : ROMStruct {
		public float scaleX;
		public float scaleY;
		public Reference<VisualMaterial> visualMaterial;
		public ushort vertexIndex;
		public ushort wordC;
		public ushort wordE;

		protected override void ReadInternal(Reader reader) {
			scaleX = reader.ReadSingle();
			scaleY = reader.ReadSingle();
			visualMaterial = new Reference<VisualMaterial>(reader, true);
			vertexIndex = reader.ReadUInt16();
			wordC = reader.ReadUInt16(); // read 1 => save 1, 2 => 8, other => 0? what is this? lookat mode?
			wordE = reader.ReadUInt16();
		}

		public GameObject GetGameObject(GeometricObject.Type type, GeometricObject go) {
			if (type == GeometricObject.Type.Visual) {
				float curScaleX = scaleX / 2f;
				float curScaleY = scaleY / 2f;
				GameObject gao = new GameObject("ElementSprites @ " + Offset);
				gao.layer = LayerMask.NameToLayer("Visual");
				gao.transform.localPosition = Vector3.zero;

				GameObject spr_gao = new GameObject("Sprite 0");
				spr_gao.transform.SetParent(gao.transform);
				spr_gao.transform.localPosition = go.verticesCollide.Value.vectors[vertexIndex].GetVector(go.ScaleFactor);
				BillboardBehaviour billboard = spr_gao.AddComponent<BillboardBehaviour>();
				billboard.mode = BillboardBehaviour.LookAtMode.ViewRotation;
				MeshFilter mf = spr_gao.AddComponent<MeshFilter>();
				MeshRenderer mr = spr_gao.AddComponent<MeshRenderer>();
				BoxCollider bc = spr_gao.AddComponent<BoxCollider>();
				bc.size = new Vector3(0, curScaleY * 2, curScaleX * 2);
				spr_gao.layer = LayerMask.NameToLayer("Visual");
				mr.material = visualMaterial.Value.GetMaterial(VisualMaterial.Hint.Billboard, gao: spr_gao);

				bool mirrorX = false;
				bool mirrorY = false;
				if (visualMaterial.Value.num_textures > 0
					&& visualMaterial.Value.textures.Value.vmTex[0].texRef.Value != null
					&& visualMaterial.Value.textures.Value.vmTex[0].texRef.Value.texInfo.Value != null) {
					TextureInfo ti = visualMaterial.Value.textures.Value.vmTex[0].texRef.Value.texInfo.Value;
					if (ti.IsMirrorX) mirrorX = true;
					if (ti.IsMirrorY) mirrorY = true;
					/*spr_gao.name += " " + string.Format("0x{0:X4}", visualMaterial.Value.num_textures) + " " + string.Format("0x{0:X4}", visualMaterial.Value.num_animTextures);
					for (int i = 0; i < visualMaterial.Value.num_textures; i++) {
						spr_gao.name += " " + visualMaterial.Value.textures.Value.vmTex[i].time;
					}*/
				}
				/*if (visualMaterial.Value.num_textures > 1) {
					MultiTextureMaterial mtmat = mr.gameObject.AddComponent<MultiTextureMaterial>();
					mtmat.visMat = sprites[i].visualMaterial;
					mtmat.mat = mr.sharedMaterial;
				}*/
				Mesh meshUnity = new Mesh();
				Vector3[] vertices = new Vector3[4];
				vertices[0] = new Vector3(0, -curScaleY, -curScaleX);
				vertices[1] = new Vector3(0, -curScaleY, curScaleX);
				vertices[2] = new Vector3(0, curScaleY, -curScaleX);
				vertices[3] = new Vector3(0, curScaleY, curScaleX);
				Vector3[] normals = new Vector3[4];
				normals[0] = Vector3.forward;
				normals[1] = Vector3.forward;
				normals[2] = Vector3.forward;
				normals[3] = Vector3.forward;
				Vector3[] uvs = new Vector3[4];
				if (Settings.s.platform == Settings.Platform.N64) {
					uvs[0] = new Vector3(0, - (mirrorY ? 1 : 0), 1);
					uvs[1] = new Vector3(1 + (mirrorX ? 1 : 0), -(mirrorY ? 1 : 0), 1);
					uvs[2] = new Vector3(0, 1, 1);
					uvs[3] = new Vector3(1 + (mirrorX ? 1 : 0), 1, 1);
				} else {
					uvs[0] = new Vector3(0, 1 + (mirrorY ? 1 : 0), 1);
					uvs[1] = new Vector3(1 + (mirrorX ? 1 : 0), 1 + (mirrorY ? 1 : 0), 1);
					uvs[2] = new Vector3(0, 0, 1);
					uvs[3] = new Vector3(1 + (mirrorX ? 1 : 0), 0, 1);
				}
				int[] triangles = new int[] { 0, 2, 1, 1, 2, 3 };

				meshUnity.vertices = vertices;
				meshUnity.normals = normals;
				meshUnity.triangles = triangles;
				meshUnity.SetUVs(0, uvs.ToList());


				mf.sharedMesh = meshUnity;
				return gao;
			} else {
				return null;
			}
		}
	}
}
