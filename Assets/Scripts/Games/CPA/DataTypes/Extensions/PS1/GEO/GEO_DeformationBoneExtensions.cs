using BinarySerializer.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BinarySerializer.Ubisoft.CPA.PS1 {
	public static class GEO_DeformationBoneExtensions {
		public static GameObject GetGameObject(this GEO_DeformationBone bone, GameObject parent) {

			GameObject gao = new GameObject($"Bone @ {bone.Offset}");
			gao.transform.parent = parent.transform;

			// Visualization for debugging
			/*MeshRenderer mr = gao.AddComponent<MeshRenderer>();
			MeshFilter mf = gao.AddComponent<MeshFilter>();
			Mesh mesh = Util.CreateBox(0.05f);
			mf.mesh = mesh;
			mr.material = MapLoader.Loader.collideMaterial;*/

			bone.Apply(gao);

			return gao;
		}

		public static void Apply(this GEO_DeformationBone bone, GameObject gao) {
			if (gao == null) return;
			gao.transform.localPosition = bone.TranslationVector.GetUnityVector(convertAxes: true);
			gao.transform.localRotation = bone.RotationMatrix.InvertedMatrix.GetRotation().GetUnityQuaternion(convertAxes: true);
			gao.transform.localScale = Vector3.one;
		}
	}
}
