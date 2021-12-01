using OpenSpace.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class PS1Matrix : OpenSpaceStruct {
		public short rot_m00;
		public short rot_m01;
		public short rot_m02;
		public short rot_m10;
		public short rot_m11;
		public short rot_m12;
		public short rot_m20;
		public short rot_m21;
		public short rot_m22;
		public ushort ushort_12;

		public int x;
		public int y;
		public int z;
		public ushort ushort_20;
		public ushort ushort_22;



		protected override void ReadInternal(Reader reader) {
			rot_m00 = reader.ReadInt16();
			rot_m01 = reader.ReadInt16();
			rot_m02 = reader.ReadInt16();
			rot_m10 = reader.ReadInt16();
			rot_m11 = reader.ReadInt16();
			rot_m12 = reader.ReadInt16();
			rot_m20 = reader.ReadInt16();
			rot_m21 = reader.ReadInt16();
			rot_m22 = reader.ReadInt16();
			ushort_12 = reader.ReadUInt16();
			x = reader.ReadInt32();
			y = reader.ReadInt32();
			z = reader.ReadInt32();
			ushort_20 = reader.ReadUInt16();
			ushort_22 = reader.ReadUInt16();
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
				Vector3 pos = new Vector3(x, y, z) / R2PS1Loader.CoordinateFactor;
				Quaternion rot = Quaternion.identity;
				//Vector3 scale = scaleMatrix != null ? scaleMatrix.GetScale() : Vector3.one;
				//Loader.print(scale);
				//Matrix4x4 matUnity = Matrix4x4.TRS(pos, rot, Vector3.one);
				Matrix4x4 matUnity = new Matrix4x4();
				matUnity.SetColumn(0, new Vector4(RotationToFloat(rot_m00), RotationToFloat(rot_m10), RotationToFloat(rot_m20), 0f));
				matUnity.SetColumn(1, new Vector4(RotationToFloat(rot_m01), RotationToFloat(rot_m11), RotationToFloat(rot_m21), 0f));
				matUnity.SetColumn(2, new Vector4(RotationToFloat(rot_m02), RotationToFloat(rot_m12), RotationToFloat(rot_m22), 0f));
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
