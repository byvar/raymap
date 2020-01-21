using OpenSpace.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.ROM {
	public class ROMTransform {
		public ROMMatrix rotationMatrix;
		public ROMMatrix scaleMatrix;
		public Vector3? position;

		public Matrix Matrix {
			get {
				Vector3 pos = position.HasValue ? position.Value : Vector3.zero;
				Quaternion rot = rotationMatrix != null ? rotationMatrix.GetRotation() : Quaternion.identity;
				//Vector3 scale = scaleMatrix != null ? scaleMatrix.GetScale() : Vector3.one;
				//Loader.print(scale);
				Matrix4x4 matUnity = Matrix4x4.TRS(pos, rot, Vector3.one);

				/*if (position.HasValue) {
					mat = Matrix4x4.Translate(position.Value);
				} else {
					mat = Matrix4x4.identity;
				}*/
				Matrix mat = new Matrix(null, 0, matUnity, Vector4.one);
				if (scaleMatrix != null) {
					mat.SetScaleMatrix(scaleMatrix.Matrix);
				};
				return mat;
			}
		}
		public void Apply(GameObject gao) {
			Matrix mat = Matrix;
			gao.transform.localPosition = mat.GetPosition(convertAxes: true);
			gao.transform.localRotation = mat.GetRotation(convertAxes: true);
			gao.transform.localScale = mat.GetScale(convertAxes: true);
		}
		public static void Apply(ROMTransform transform, GameObject gao) {
			if (gao == null) return;
			if (transform != null) {
				transform.Apply(gao);
			} else {
				gao.transform.localPosition = Vector3.zero;
				gao.transform.localRotation = Quaternion.identity;
				gao.transform.localScale = Vector3.one;
			}
		}
		public ROMTransform(ushort index) {
			if (index == 0xFFFF) return;
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			LevelHeader lh = l.Get<LevelHeader>((ushort)(l.CurrentLevel | (ushort)FATEntry.Flag.Fix));
			if (lh != null) {
				Short3Array indices = lh.indices.Value;
				Vector3Array vectors = lh.vectors.Value;
				Short3Array.Triangle tri = lh.indices.Value.triangles[index];
				rotationMatrix = ROMMatrix.Get(tri.v1, indices, vectors);
				scaleMatrix = ROMMatrix.Get(tri.v2, indices, vectors);
				if (tri.v3 != 0xFFFF && tri.v3 < vectors.length) {
					//MapLoader.Loader.print(tri.v3);
					position = vectors.vectors[tri.v3];
					//MapLoader.Loader.print(transform.position);
				}
			}
		}
		public class ROMMatrix {
			public Vector3? v1;
			public Vector3? v2;
			public Vector3? v3;

			public Matrix4x4 Matrix {
				get {
					Matrix4x4 mat = new Matrix4x4(
						v1.HasValue ? new Vector4(v1.Value.x, v1.Value.y, v1.Value.z) : new Vector4(1f, 0f, 0f),
						v2.HasValue ? new Vector4(v2.Value.z, v2.Value.x, v2.Value.y) : new Vector4(0f, 1f, 0f),
						v3.HasValue ? new Vector4(v3.Value.y, v3.Value.z, v3.Value.x) : new Vector4(0f, 0f, 1f),
						new Vector4(0f, 0f, 0f, 1f));

					return mat;
				}
			}

			/*public Quaternion ToQuaternion() {
				Matrix4x4 m1 = Matrix;
				float w = Mathf.Sqrt(1f + m1.m00 + m1.m11 + m1.m22) / 2f;
				float w4 = (4f * w);
				float x = (m1.m21 - m1.m12) / w4;
				float y = (m1.m02 - m1.m20) / w4;
				float z = (m1.m10 - m1.m01) / w4;
				return new Quaternion(x, y, z, w);
			}*/


			public Quaternion GetRotation(bool convertAxes = false) {
				Matrix4x4 m = Matrix;
				float m00, m01, m02, m10, m11, m12, m20, m21, m22;
				m00 = m.m00;
				m01 = m.m01;
				m02 = m.m02;
				m10 = m.m10;
				m11 = m.m11;
				m12 = m.m12;
				m20 = m.m20;
				m21 = m.m21;
				m22 = m.m22;

				float tr = m00 + m11 + m22;
				Quaternion q = new Quaternion();
				if (tr > 0) {
					float S = Mathf.Sqrt(tr + 1.0f) * 2; // S=4*qw 
					q.w = 0.25f * S;
					q.x = (m21 - m12) / S;
					q.y = (m02 - m20) / S;
					q.z = (m10 - m01) / S;
				} else if ((m00 > m11) && (m00 > m22)) {
					float S = Mathf.Sqrt(1.0f + m00 - m11 - m22) * 2; // S=4*qx 
					q.w = (m21 - m12) / S;
					q.x = 0.25f * S;
					q.y = (m01 + m10) / S;
					q.z = (m02 + m20) / S;
				} else if (m11 > m22) {
					float S = Mathf.Sqrt(1.0f + m11 - m00 - m22) * 2; // S=4*qy
					q.w = (m02 - m20) / S;
					q.x = (m01 + m10) / S;
					q.y = 0.25f * S;
					q.z = (m12 + m21) / S;
				} else {
					float S = Mathf.Sqrt(1.0f + m22 - m00 - m11) * 2; // S=4*qz
					q.w = (m10 - m01) / S;
					q.x = (m02 + m20) / S;
					q.y = (m12 + m21) / S;
					q.z = 0.25f * S;
				}

				if (convertAxes) {
					q = new Quaternion(q.x, q.z, q.y, -q.w);
				}
				// Normalize this
				float f = 1f / Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
				q = new Quaternion(q.x * f, q.y * f, q.z * f, q.w * f);

				return q;
			}

			public Vector3 GetScale(bool convertAxes = false) {
				Matrix4x4 m = Matrix;
				if (convertAxes) {
					return new Vector3(m.GetColumn(0).magnitude, m.GetColumn(2).magnitude, m.GetColumn(1).magnitude);
				} else {
					return new Vector3(m.GetColumn(0).magnitude, m.GetColumn(1).magnitude, m.GetColumn(2).magnitude);
				}
			}

			public static ROMMatrix Get(ushort index, Short3Array indices, Vector3Array vectors) {
				if (index == 0xFFFF || indices == null || vectors == null || index >= indices.length) return null;
				Short3Array.Triangle tri = indices.triangles[index];
				return new ROMMatrix() {
					v1 = tri.v1 != 0xFFFF && tri.v1 < vectors.length ? new Vector3?(vectors.vectors[tri.v1]) : null,
					v2 = tri.v2 != 0xFFFF && tri.v2 < vectors.length ? new Vector3?(vectors.vectors[tri.v2]) : null,
					v3 = tri.v3 != 0xFFFF && tri.v3 < vectors.length ? new Vector3?(vectors.vectors[tri.v3]) : null
				};
			}
		}
	}
}
