using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace {
    /// <summary>
    /// Transformation matrix storing position, rotation and scale. Also, an unknown vector4 and type.
    /// </summary>
    public class Matrix {
        public Pointer offset;
        public UInt32 type;
        public Matrix4x4 m;
        public Vector4 v;

        public Matrix(Pointer offset, uint type, Matrix4x4 matrix, Vector4 vec) {
            this.offset = offset;
            this.type = type;
            this.m = matrix;
            this.v = vec;
        }
        public Vector3 GetPosition(bool convertAxes = false) {
            if (convertAxes) {
                return new Vector3(m[0, 3], m[2, 3], m[1, 3]);
            } else {
                return new Vector3(m[0, 3], m[1, 3], m[2, 3]);
            }
        }
        
        public Vector3 GetScale(bool convertAxes = false) {
            if (convertAxes) {
                return new Vector3(m.GetColumn(0).magnitude, m.GetColumn(2).magnitude, m.GetColumn(1).magnitude);
            } else {
                return new Vector3(m.GetColumn(0).magnitude, m.GetColumn(1).magnitude, m.GetColumn(2).magnitude);
            }
        }


        public static Matrix operator *(Matrix x, Matrix y) {
            return new Matrix(x.offset, x.type, x.m * y.m, x.v);
        }

        public static Matrix Invert(Matrix src) {
            Matrix dest = new Matrix(src.offset, src.type, new Matrix4x4(), src.v);
            /*m.v.x = 1f / m.m[0, 0];
            m.v.y = 1f / m.m[1, 1];
            m.v.z = 1f / m.m[2, 2];*/
            /*m.m[0, 0] = m.v.x;
            m.m[1, 1] = m.v.y;
            m.m[2, 2] = m.v.z;*/
            dest.m.SetRow(0, src.m.GetRow(0));
            dest.m.SetRow(1, src.m.GetRow(1));
            dest.m.SetRow(2, src.m.GetRow(2));
            dest.m.SetRow(3, src.m.GetRow(3));
            
            /*float v4 = src.m.m22 * src.m.m11 - src.m.m12 * src.m.m21;
            dest.m.m00 = v4;
            float v5 = v4 * src.m.m00;
            float v6 = src.m.m02 * src.m.m21 - src.m.m01 * src.m.m22;
            dest.m.m01 = v6;
            float v7 = v5 + src.m.m10 * v6;
            float v8 = src.m.m01 * src.m.m12 - src.m.m02 * src.m.m11;
            dest.m.m02 = v8;
            float determinant = v7 + src.m.m20 * v8;
            dest.m.m10 = src.m.m20 * src.m.m12 - src.m.m10 * src.m.m22;
            dest.m.m20 = src.m.m10 * src.m.m21 - src.m.m20 * src.m.m11;
            dest.m.m11 = src.m.m22 * src.m.m00 - src.m.m20 * src.m.m02;
            dest.m.m21 = src.m.m01 * src.m.m20 - src.m.m21 * src.m.m00;
            dest.m.m12 = src.m.m02 * src.m.m10 - src.m.m12 * src.m.m00;
            dest.m.m22 = src.m.m11 * src.m.m00 - src.m.m01 * src.m.m10;


            float invertedDeterminant = 1f / determinant;

            dest.m.m00 *= invertedDeterminant;
            dest.m.m10 *= invertedDeterminant;
            dest.m.m20 *= invertedDeterminant;

            dest.m.m01 *= invertedDeterminant;
            dest.m.m11 *= invertedDeterminant;
            dest.m.m21 *= invertedDeterminant;

            dest.m.m02 *= invertedDeterminant;
            dest.m.m12 *= invertedDeterminant;
            dest.m.m22 *= invertedDeterminant;*/
            dest.m.SetColumn(0, src.m.GetRow(0));
            dest.m.SetColumn(1, src.m.GetRow(1));
            dest.m.SetColumn(2, src.m.GetRow(2));

            dest.m.m03 = dest.m.m01 * -src.m.m13 + dest.m.m02 * -src.m.m23 + dest.m.m00 * -src.m.m03;
            dest.m.m13 = dest.m.m11 * -src.m.m13 + dest.m.m12 * -src.m.m23 + dest.m.m10 * -src.m.m03;
            dest.m.m23 = dest.m.m21 * -src.m.m13 + dest.m.m22 * -src.m.m23 + dest.m.m20 * -src.m.m03;

            dest.m.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
            return dest;
        }

        public Quaternion GetRotation(bool convertAxes = false) {
            float tr = m.m00 + m.m11 + m.m22;
            Quaternion q = new Quaternion();
            if (tr > 0) {
                float S = Mathf.Sqrt(tr + 1.0f) * 2; // S=4*qw 
                q.w = 0.25f * S;
                q.x = (m.m21 - m.m12) / S;
                q.y = (m.m02 - m.m20) / S;
                q.z = (m.m10 - m.m01) / S;
            } else if ((m.m00 > m.m11) && (m.m00 > m.m22)) {
                float S = Mathf.Sqrt(1.0f + m.m00 - m.m11 - m.m22) * 2; // S=4*qx 
                q.w = (m.m21 - m.m12) / S;
                q.x = 0.25f * S;
                q.y = (m.m01 + m.m10) / S;
                q.z = (m.m02 + m.m20) / S;
            } else if (m.m11 > m.m22) {
                float S = Mathf.Sqrt(1.0f + m.m11 - m.m00 - m.m22) * 2; // S=4*qy
                q.w = (m.m02 - m.m20) / S;
                q.x = (m.m01 + m.m10) / S;
                q.y = 0.25f * S;
                q.z = (m.m12 + m.m21) / S;
            } else {
                float S = Mathf.Sqrt(1.0f + m.m22 - m.m00 - m.m11) * 2; // S=4*qz
                q.w = (m.m10 - m.m01) / S;
                q.x = (m.m02 + m.m20) / S;
                q.y = (m.m12 + m.m21) / S;
                q.z = 0.25f * S;
            }

            /*Vector3 s = GetScale();

            // Normalize Scale from Matrix4x4
            float m00 = m[0, 0] / s.x;
            float m01 = m[0, 1] / s.y;
            float m02 = m[0, 2] / s.z;
            float m10 = m[1, 0] / s.x;
            float m11 = m[1, 1] / s.y;
            float m12 = m[1, 2] / s.z;
            float m20 = m[2, 0] / s.x;
            float m21 = m[2, 1] / s.y;
            float m22 = m[2, 2] / s.z;

            Quaternion q = new Quaternion();
            q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m00 + m11 + m22)) / 2;
            q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m00 - m11 - m22)) / 2;
            q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m00 + m11 - m22)) / 2;
            q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m00 - m11 + m22)) / 2;
            q.x *= Mathf.Sign(q.x * (m21 - m12));
            q.y *= Mathf.Sign(q.y * (m02 - m20));
            q.z *= Mathf.Sign(q.z * (m10 - m01));

            // q.Normalize()
            float qMagnitude = Mathf.Sqrt(q.w * q.w + q.x * q.x + q.y * q.y + q.z * q.z);
            q.w /= qMagnitude;
            q.x /= qMagnitude;
            q.y /= qMagnitude;
            q.z /= qMagnitude;*/

            if (convertAxes) {
                q = new Quaternion(q.x, q.z, q.y, -q.w);
                //q = q * Quaternion.Euler(new Vector3(0f, 0f, 0f));
                //Vector3 tempRot = q.eulerAngles;
                //if (isBoneMatrix) {
                    //tempRot = new Vector3(-tempRot.x, -tempRot.z, -tempRot.y); // z = tempRot.y * sign(something)
                    /*float signX = m00 == 0 ? 0 : Mathf.Sign(m00);
                    float signY = m11 == 0 ? 0 : Mathf.Sign(m11);
                    float signZ = m22 == 0 ? 0 : Mathf.Sign(m22);*/
                    //float signX = 1f, signY = 1f, signZ = 1f;
                    //tempRot = new Vector3(-tempRot.y * signY, -tempRot.x * signX, tempRot.z * signZ);
                //}
                //tempRot = new Vector3(tempRot.y, -tempRot.z, tempRot.x);
                
                //q = Quaternion.Euler(tempRot);
            }

            return q;
        }

        public static Matrix Read(EndianBinaryReader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            UInt32 type = reader.ReadUInt32(); // 0x02: always at the start of a transformation matrix
            Matrix4x4 transMatrix = new Matrix4x4();
            Vector4 vec;
            if (Settings.s.engineMode == Settings.EngineMode.R2) {
                transMatrix.SetColumn(3, new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 1f));
                Vector4 colX1 = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0f);
                Vector4 colY1 = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0f);
                Vector4 colZ1 = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0f);
                Vector4 colX2 = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0f);
                Vector4 colY2 = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0f);
                Vector4 colZ2 = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0f);
                if (type != 4) {
                    transMatrix.SetColumn(0, colX1);
                    transMatrix.SetColumn(1, colY1);
                    transMatrix.SetColumn(2, colZ1);
                } else {
                    transMatrix.SetColumn(0, colX2);
                    transMatrix.SetColumn(1, colY2);
                    transMatrix.SetColumn(2, colZ2);
                }
                vec = new Vector4(1f, 1f, 1f, 1f);
            } else {
                transMatrix.SetColumn(0, new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
                transMatrix.SetColumn(1, new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
                transMatrix.SetColumn(2, new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
                transMatrix.SetColumn(3, new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
                vec = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            }
            return new Matrix(offset, type, transMatrix, vec);
        }

        // For writing
        public void SetTRS(Vector3 pos, Quaternion rot, Vector3 scale, bool convertAxes = false, bool setVec = false) {
            if (convertAxes) {
                Vector3 tempRot = rot.eulerAngles;
                tempRot = new Vector3(tempRot.z, tempRot.x, -tempRot.y);
                rot = Quaternion.Euler(tempRot);
                scale = new Vector3(scale.x, scale.z, scale.y);
                pos = new Vector3(pos.x, pos.z, pos.y);
            }
            m.SetTRS(pos, rot, scale);
            if (setVec) {
                v = new Vector4(1f * scale.x, 1f * scale.y, 1f * scale.z, v.w);
            }
        }

        public void Write(EndianBinaryWriter writer) {
            Pointer.Goto(ref writer, offset);
            writer.Write(type);
            for (int i = 0; i < 4; i++) {
                Vector4 col = m.GetColumn(i);
                for (int j = 0; j < 4; j++) {
                    writer.Write(col[j]);
                }
            }
            for (int j = 0; j < 4; j++) {
                writer.Write(v[j]);
            }
        }
    }
}
