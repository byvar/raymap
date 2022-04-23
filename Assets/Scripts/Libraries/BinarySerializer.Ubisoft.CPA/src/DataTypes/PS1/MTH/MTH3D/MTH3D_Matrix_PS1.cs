using System;

namespace BinarySerializer.Ubisoft.CPA {
	public class MTH3D_Matrix_PS1 : BinarySerializable {
		// TODO: Serialize as array [3][3]
		// But watch out: row-major order! not column-major like regular CPA. So Row0, Row1, Row2.
		public FixedPointInt16 M00 { get; set; }
		public FixedPointInt16 M01 { get; set; }
		public FixedPointInt16 M02 { get; set; }
		public FixedPointInt16 M10 { get; set; }
		public FixedPointInt16 M11 { get; set; }
		public FixedPointInt16 M12 { get; set; }
		public FixedPointInt16 M20 { get; set; }
		public FixedPointInt16 M21 { get; set; }
		public FixedPointInt16 M22 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			M00 = s.SerializeObject<FixedPointInt16>(M00, x => x.Pre_PointPosition = 12, name: nameof(M00));
			M01 = s.SerializeObject<FixedPointInt16>(M01, x => x.Pre_PointPosition = 12, name: nameof(M01));
			M02 = s.SerializeObject<FixedPointInt16>(M02, x => x.Pre_PointPosition = 12, name: nameof(M02));
			M10 = s.SerializeObject<FixedPointInt16>(M10, x => x.Pre_PointPosition = 12, name: nameof(M10));
			M11 = s.SerializeObject<FixedPointInt16>(M11, x => x.Pre_PointPosition = 12, name: nameof(M11));
			M12 = s.SerializeObject<FixedPointInt16>(M12, x => x.Pre_PointPosition = 12, name: nameof(M12));
			M20 = s.SerializeObject<FixedPointInt16>(M20, x => x.Pre_PointPosition = 12, name: nameof(M20));
			M21 = s.SerializeObject<FixedPointInt16>(M21, x => x.Pre_PointPosition = 12, name: nameof(M21));
			M22 = s.SerializeObject<FixedPointInt16>(M22, x => x.Pre_PointPosition = 12, name: nameof(M22));
		}

		public MTH3D_Matrix_PS1 InvertedMatrix => new MTH3D_Matrix_PS1() {
			M00 = M00,
			M01 = M10,
			M02 = M20,
			M10 = M01,
			M11 = M11,
			M12 = M21,
			M20 = M02,
			M21 = M12,
			M22 = M22,
		};

		public MTH4D_Vector GetRotation(MTH3D_Vector scaleVector = null) {
			float m00, m01, m02; // Row 0
			float m10, m11, m12; // Row 1
			float m20, m21, m22; // Row 2

			m00 = M00;
			m01 = M01;
			m02 = M02;
			m10 = M10;
			m11 = M11;
			m12 = M12;
			m20 = M20;
			m21 = M21;
			m22 = M22;

			if (scaleVector != null && scaleVector.X != 0 && scaleVector.Y != 0 && scaleVector.Z != 0) {
				m00 /= scaleVector.X;
				m01 /= scaleVector.Y;
				m02 /= scaleVector.Z;
				m10 /= scaleVector.X;
				m11 /= scaleVector.Y;
				m12 /= scaleVector.Z;
				m20 /= scaleVector.X;
				m21 /= scaleVector.Y;
				m22 /= scaleVector.Z;
			}

			//float tr = m00 + m11 + m22;
			MTH4D_Vector quaternion; // = new MTH4D_Vector();
			float t;

			if (m22 < 0) {
				if (m00 > m11) {
					t = 1 + m00 - m11 - m22;
					quaternion = new MTH4D_Vector(t, m01 + m10, m20 + m02, m12 - m21);
				} else {
					t = 1 - m00 + m11 - m22;
					quaternion = new MTH4D_Vector(m01 + m10, t, m12 + m21, m20 - m02);
				}
			} else {
				if (m00 < -m11) {
					t = 1 - m00 - m11 + m22;
					quaternion = new MTH4D_Vector(m20 + m02, m12 + m21, t, m01 - m10);
				} else {
					t = 1 + m00 + m11 + m22;
					quaternion = new MTH4D_Vector(m12 - m21, m20 - m02, m01 - m10, t);
				}
			}
			float factor = (0.5f / (float)Math.Sqrt(t));
			quaternion.X = quaternion.X * factor;
			quaternion.Y = quaternion.Y * factor;
			quaternion.Z = quaternion.Z * factor;
			quaternion.W = quaternion.W * -factor;

			return quaternion;
		}
	}
}
