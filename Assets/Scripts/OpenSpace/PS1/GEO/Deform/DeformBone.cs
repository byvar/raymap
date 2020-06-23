using OpenSpace.Loader;
using OpenSpace.PS1.GLI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class DeformBone : OpenSpaceStruct {
		public ushort num_vertices;
		public short short_02;
		public short x;
		public short y;
		public short z;
		public short short_0A;
		public Pointer off_vertices;

		public short rot_m00;
		public short rot_m01;
		public short rot_m02;
		public short rot_m10;
		public short rot_m11;
		public short rot_m12;
		public short rot_m20;
		public short rot_m21;
		public short rot_m22;

		public short unk0;
		public short unk1;
		public short unk2;
		public short unk3;
		public short unk4;
		public short unk5;
		public short unk6;
		/*public short rot_m00;
		public short rot_m01;
		public short rot_m02;
		public short rot_m03;
		public short rot_m10;
		public short rot_m11;
		public short rot_m12;
		public short rot_m13;
		public short rot_m20;
		public short rot_m21;
		public short rot_m22;
		public short rot_m23;
		public short rot_m30;
		public short rot_m31;
		public short rot_m32;
		public short rot_m33;*/
		// Parsed
		public ushort[] vertices;

		protected override void ReadInternal(Reader reader) {
			num_vertices = reader.ReadUInt16();
			short_02 = reader.ReadInt16();
			x = reader.ReadInt16();
			y = reader.ReadInt16();
			z = reader.ReadInt16();
			short_0A = reader.ReadInt16();
			off_vertices = Pointer.Read(reader);

			rot_m00 = reader.ReadInt16();
			rot_m01 = reader.ReadInt16();
			rot_m02 = reader.ReadInt16();
			rot_m10 = reader.ReadInt16();
			rot_m11 = reader.ReadInt16();
			rot_m12 = reader.ReadInt16();
			rot_m20 = reader.ReadInt16();
			rot_m21 = reader.ReadInt16();
			rot_m22 = reader.ReadInt16();

			unk0 = reader.ReadInt16();
			unk1 = reader.ReadInt16();
			unk2 = reader.ReadInt16();
			unk3 = reader.ReadInt16();
			unk4 = reader.ReadInt16();
			unk5 = reader.ReadInt16();
			unk6 = reader.ReadInt16();

			vertices = new ushort[num_vertices];
			Pointer.DoAt(ref reader, off_vertices, () => {
				for (int i = 0; i < num_vertices; i++) {
					vertices[i] = reader.ReadUInt16();
				}
			});
		}

		public GameObject GetGameObject(GameObject parent) {
			GameObject gao = new GameObject("Bone @ " + Offset.ToString());
			gao.transform.parent = parent.transform;

			// Visualization for debugging
			/*MeshRenderer mr = gao.AddComponent<MeshRenderer>();
			MeshFilter mf = gao.AddComponent<MeshFilter>();
			Mesh mesh = Util.CreateBox(0.05f);
			mf.mesh = mesh;
			mr.material = MapLoader.Loader.collideMaterial;*/

			Apply(gao);

			return gao;
		}

		public void Apply(GameObject gao) {
			if (gao == null) return;
			Matrix mat = Matrix;
			gao.transform.localPosition = mat.GetPosition(convertAxes: true);
			gao.transform.localRotation = mat.GetRotation(convertAxes: true);
			gao.transform.localScale = mat.GetScale(convertAxes: true);
		}

		public Matrix Matrix {
			get {
				Vector3 pos = new Vector3(x / 256f, y / 256f, z / 256f);
				Quaternion rot = Quaternion.identity;
				//Vector3 scale = scaleMatrix != null ? scaleMatrix.GetScale() : Vector3.one;
				//Loader.print(scale);
				//Matrix4x4 matUnity = Matrix4x4.TRS(pos, rot, Vector3.one);
				Matrix4x4 matUnity = new Matrix4x4();
				/*matUnity.SetColumn(0, new Vector4(RotationToFloat(rot_m00), RotationToFloat(rot_m10), RotationToFloat(rot_m20), 0f));
				matUnity.SetColumn(1, new Vector4(RotationToFloat(rot_m01), RotationToFloat(rot_m11), RotationToFloat(rot_m21), 0f));
				matUnity.SetColumn(2, new Vector4(RotationToFloat(rot_m02), RotationToFloat(rot_m12), RotationToFloat(rot_m22), 0f));*/
				// Use rows instead of columns
				matUnity.SetRow(0, new Vector4(RotationToFloat(rot_m00), RotationToFloat(rot_m10), RotationToFloat(rot_m20), 0f));
				matUnity.SetRow(1, new Vector4(RotationToFloat(rot_m01), RotationToFloat(rot_m11), RotationToFloat(rot_m21), 0f));
				matUnity.SetRow(2, new Vector4(RotationToFloat(rot_m02), RotationToFloat(rot_m12), RotationToFloat(rot_m22), 0f));
				matUnity.SetRow(3, new Vector4(0, 0, 0, 1));
				matUnity.SetColumn(3, new Vector4(pos.x, pos.y, pos.z, 1f));



				/*if (position.HasValue) {
					mat = Matrix4x4.Translate(position.Value);
				} else {
					mat = Matrix4x4.identity;
				}*/
				Matrix mat = new Matrix(null, 0, matUnity, Vector4.one);
				/*if (scaleMatrix != null) {
					mat.SetScaleMatrix(scaleMatrix.Matrix);
				};*/
				return mat;
			}
		}

		private float RotationToFloat(short rot) {
			/*uint sign = (uint)Util.ExtractBits(rot, 1, 15);
			uint fraction = (uint)Util.ExtractBits(rot, 12, 0);
			uint integral = (uint)Util.ExtractBits(rot, 3, 12);
			if (sign == 1) {
				fraction ^= (0b111111111111);
				integral ^= (0b111);
			}
			float f = (sign == 1 ? -1f : 1f) * integral;
			f += fraction / (float)(1 << 12);
			return f;*/
			return (rot / 4096f);
		}
	}
}
