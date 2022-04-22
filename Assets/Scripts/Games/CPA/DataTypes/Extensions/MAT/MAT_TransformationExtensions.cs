using UnityEngine;

namespace BinarySerializer.Ubisoft.CPA {
	public static class MAT_TransformationExtensions {
		public static void Apply(this MAT_Transformation mat, GameObject gao) {
			gao.transform.localPosition = mat.Position.GetUnityVector(convertAxes: true);
			gao.transform.localRotation = mat.Rotation.GetUnityQuaternion(convertAxes: true);
			gao.transform.localScale = mat.Scale.GetUnityVector(convertAxes: true);
		}
	}
}
