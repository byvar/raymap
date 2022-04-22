using System;

namespace BinarySerializer.Ubisoft.CPA {
	public class MTH3D_Matrix : BinarySerializable {
		public MTH3D_Vector Column0 { get; set; }
		public MTH3D_Vector Column1 { get; set; }
		public MTH3D_Vector Column2 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Column0 = s.SerializeObject<MTH3D_Vector>(Column0, name: nameof(Column0));
			Column1 = s.SerializeObject<MTH3D_Vector>(Column1, name: nameof(Column1));
			Column2 = s.SerializeObject<MTH3D_Vector>(Column2, name: nameof(Column2));
		}

		public MTH4D_Vector GetRotation(MTH3D_Vector scaleVector = null) {
			float m00, m01, m02; // Row 0
			float m10, m11, m12; // Row 1
			float m20, m21, m22; // Row 2

			m00 = Column0.X;
			m01 = Column1.X;
			m02 = Column2.X;
			m10 = Column0.Y;
			m11 = Column1.Y;
			m12 = Column2.Y;
			m20 = Column0.Z;
			m21 = Column1.Z;
			m22 = Column2.Z;

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
		public MTH3D_Vector GetScale() {
			return new MTH3D_Vector((float)Column0.Magnitude, (float)Column1.Magnitude, (float)Column2.Magnitude);
		}
	}
}
