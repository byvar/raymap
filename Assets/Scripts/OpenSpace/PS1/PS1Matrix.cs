using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class PS1Matrix : OpenSpaceStruct {
		public int int_00;
		public int int_04;
		public int int_08;
		public int int_0C;
		public int int_10;

		public int x;
		public int y;
		public int z;
		public int int_20;
		public int int_24;



		protected override void ReadInternal(Reader reader) {
			int_00 = reader.ReadInt32();
			int_04 = reader.ReadInt32();
			int_08 = reader.ReadInt32();
			int_0C = reader.ReadInt32();
			int_10 = reader.ReadInt32();
			x = reader.ReadInt32();
			y = reader.ReadInt32();
			z = reader.ReadInt32();
			int_20 = reader.ReadInt32();
			int_24 = reader.ReadInt32();
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
				Matrix4x4 matUnity = Matrix4x4.TRS(pos, rot, Vector3.one);

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
	}
}
