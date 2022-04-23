using BinarySerializer.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BinarySerializer.Ubisoft.CPA.PS1 {
	public static class GEO_GeometricObjectExtensions {


		public static GameObject GetGameObject(this GEO_GeometricObject geo, out GameObject[] boneGaos, GEO_GeometricObject morphObject = null, float morphProgress = 0f) {
			GameObject parentGao = new GameObject(geo.Offset.ToString());

			// Bones
			boneGaos = null;
			if (geo.Bones != null && geo.Bones.Length > 0) {
				GameObject rootBone = new GameObject("Root bone");
				boneGaos = new GameObject[] { rootBone };
				boneGaos[0].transform.SetParent(parentGao.transform);
				boneGaos[0].transform.localPosition = Vector3.zero;
				boneGaos[0].transform.localRotation = Quaternion.identity;
				boneGaos[0].transform.localScale = Vector3.one;
				boneGaos = boneGaos.Concat(geo.Bones.Select(b => b.GetGameObject(parentGao))).ToArray();
			}

			// Morph
			Vector3[] mainVertices = geo.Vertices.Select(s => s.Position.GetUnityVector(convertAxes: true)).ToArray();
			Color[] mainColors = geo.Vertices.Select(s => new Color(
					s.Color.R / (float)0x80,
					s.Color.G / (float)0x80,
					s.Color.B / (float)0x80,
					1f)).ToArray();
			if (morphProgress > 0f && morphObject != null && morphObject.Vertices.Length == geo.Vertices.Length) {
				Vector3[] morphVertices = morphObject.Vertices.Select(s => s.Position.GetUnityVector(convertAxes: true)).ToArray();
				Color[] morphColors = morphObject.Vertices.Select(s => new Color(
					s.Color.R / (float)0x80,
					s.Color.G / (float)0x80,
					s.Color.B / (float)0x80,
					1f)).ToArray();
				for (int i = 0; i < geo.Vertices.Length; i++) {
					mainVertices[i] = Vector3.Lerp(mainVertices[i], morphVertices[i], morphProgress);
					mainColors[i] = Color.Lerp(mainColors[i], morphColors[i], morphProgress);
				}
			}

			// First pass
			// TODO: everything between this
			throw new NotImplementedException();

			return parentGao;
		}
	}
}
