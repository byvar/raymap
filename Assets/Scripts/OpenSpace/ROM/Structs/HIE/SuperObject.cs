using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class SuperObject : ROMStruct {
		// Size: 20
		public ushort matrixIndex; // 0, references global indices. check load_so_matrices.
		public ushort dataIndex; // 2
		public Reference<SuperObjectArray> children; // 4
		public Reference<CompressedVector3Array> comprVec3Array; // 6. Size: 12, so 2 vector3s
		public ushort type; // 8
		public ushort num_children; // 10
		public ushort unk; // 12
		public ushort unk2;
		public uint flags; // 16

		public GenericReference data;
		public Transform transform;

		protected override void ReadInternal(Reader reader) {
			matrixIndex = reader.ReadUInt16();
			dataIndex = reader.ReadUInt16();
			children = new Reference<SuperObjectArray>(reader, false);
			comprVec3Array = new Reference<CompressedVector3Array>(reader, true, v => v.length = 2);
			type = reader.ReadUInt16();
			num_children = reader.ReadUInt16();
			unk = reader.ReadUInt16();
			unk2 = reader.ReadUInt16();
			flags = reader.ReadUInt32();

			ResolveMatrices(reader);
			children.Resolve(reader, soa => soa.length = num_children);
			data = new GenericReference(type, dataIndex, reader, true);
		}

		protected void ResolveMatrices(Reader reader) {
			transform = new Transform();
			if (matrixIndex == 0xFFFF) return;
			LevelHeader lh = Loader.Get<LevelHeader>((ushort)(Loader.CurrentLevel | (ushort)FATEntry.Flag.Fix));
			if (lh != null) {
				Short3Array indices = lh.indices.Value;
				Vector3Array vectors = lh.vectors.Value;
				Short3Array.Triangle tri = lh.indices.Value.triangles[matrixIndex];
				transform.rotationMatrix = ResolveMatrix(reader, tri.v1, indices, vectors);
				transform.scaleMatrix = ResolveMatrix(reader, tri.v2, indices, vectors);
				if (tri.v3 != 0xFFFF) {
					transform.position = vectors.vectors[tri.v3];
					//MapLoader.Loader.print(transform.position);
				}
			}
		}

		public static ROMMatrix ResolveMatrix(Reader reader, ushort index, Short3Array indices, Vector3Array vectors) {
			if (index == 0xFFFF || indices == null || vectors == null) return null;
			Short3Array.Triangle tri = indices.triangles[index];
			return new ROMMatrix() {
				v1 = vectors.vectors[tri.v1],
				v2 = vectors.vectors[tri.v2],
				v3 = vectors.vectors[tri.v3]
			};
		}

		public GameObject GetGameObject() {
			GameObject gao = new GameObject("SO @ " + Offset + " - " + type);
			if (data.Value != null && data.Value is PhysicalObject) {
				GameObject po = ((PhysicalObject)data.Value).GetGameObject();
				if(po !=  null) po.transform.SetParent(gao.transform);
			}
			if (children.Value != null) {
				foreach (Reference<SuperObject> so in children.Value.superObjects) {
					if (so.Value != null) {
						GameObject soGao = so.Value.GetGameObject();
						if (soGao != null) {
							soGao.transform.SetParent(gao.transform);
							so.Value.SetTransform(soGao);
						}
					}
				}
			}
			return gao;
		}
		public void SetTransform(GameObject gao) {
			transform.Apply(gao);
			//Loader.print("applied " + (transform.position.HasValue ? transform.position.Value.ToString() : ""));
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
		}
		public class Transform {
			public ROMMatrix rotationMatrix;
			public ROMMatrix scaleMatrix;
			public Vector3? position;

			public Matrix Matrix {
				get {
					Vector3 pos = position.HasValue ? position.Value : Vector3.zero;
					Quaternion rot = rotationMatrix != null ? rotationMatrix.GetRotation() : Quaternion.identity;
					Vector3 scale = scaleMatrix != null ? scaleMatrix.GetScale() : Vector3.one;
					//Loader.print(scale);
					Matrix4x4 matUnity = Matrix4x4.TRS(pos, rot, Vector3.one);

					/*if (position.HasValue) {
						mat = Matrix4x4.Translate(position.Value);
					} else {
						mat = Matrix4x4.identity;
					}*/
					Matrix mat = new Matrix(null, 0, matUnity, Vector4.one);
					if(scaleMatrix != null) {
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
		}
	}
}
