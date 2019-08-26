using Autodesk.Fbx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Fbx {
	public class FbxUtil {
		internal const float UnitScaleFactor = 100f;
		const char MayaNamespaceSeparator = ':';

		// replace invalid chars with this one
		const char InvalidCharReplacement = '_';


		public static string ConvertToMayaCompatibleName(string name) {
			string newName = RemoveDiacritics(name);

			if (char.IsDigit(newName[0])) {
				newName = newName.Insert(0, InvalidCharReplacement.ToString());
			}

			for (int i = 0; i < newName.Length; i++) {
				if (!char.IsLetterOrDigit(newName, i)) {
					if (i < newName.Length - 1 && newName[i] == MayaNamespaceSeparator) {
						continue;
					}
					newName = newName.Replace(newName[i], InvalidCharReplacement);
				}
			}
			return newName;
		}

		/// <summary>
		/// Takes in a left-handed UnityEngine.Vector3 denoting a normal,
		/// returns a right-handed FbxVector4.
		///
		/// Unity is left-handed, Maya and Max are right-handed.
		/// The FbxSdk conversion routines can't handle changing handedness.
		///
		/// Remember you also need to flip the winding order on your polygons.
		/// </summary>
		internal static FbxVector4 ConvertToRightHanded(Vector3 leftHandedVector, float unitScale = 1f) {
			// negating the x component of the vector converts it from left to right handed coordinates
			return unitScale * new FbxVector4(
				-leftHandedVector[0],
				leftHandedVector[1],
				leftHandedVector[2]);
		}

		/// <summary>
		/// Removes the diacritics (i.e. accents) from letters.
		/// e.g. é becomes e
		/// </summary>
		/// <returns>Text with accents removed.</returns>
		/// <param name="text">Text.</param>
		private static string RemoveDiacritics(string text) {
			var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
			var stringBuilder = new System.Text.StringBuilder();

			foreach (var c in normalizedString) {
				var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
				if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark) {
					stringBuilder.Append(c);
				}
			}

			return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC);
		}

		/// <summary>
		/// Takes a Quaternion and returns a Euler with XYZ rotation order.
		/// Also converts from left (Unity) to righthanded (Maya) coordinates.
		/// 
		/// Note: Cannot simply use the FbxQuaternion.DecomposeSphericalXYZ()
		///       function as this returns the angle in spherical coordinates 
		///       instead of Euler angles, which Maya does not import properly. 
		/// </summary>
		/// <returns>Euler with XYZ rotation order.</returns>
		internal static FbxDouble3 ConvertQuaternionToXYZEuler(Quaternion q) {
			FbxQuaternion quat = new FbxQuaternion(q.x, q.y, q.z, q.w);
			FbxAMatrix m = new FbxAMatrix();
			m.SetQ(quat);
			var vector4 = m.GetR();

			// Negate the y and z values of the rotation to convert 
			// from Unity to Maya coordinates (left to righthanded).
			return new FbxDouble3(vector4.X, -vector4.Y, -vector4.Z);
		}
	}
}
