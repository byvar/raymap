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


        public Quaternion GetRotation(bool convertAxes = false) {
            Vector3 s = GetScale();

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
            q.z /= qMagnitude;

            if (convertAxes) {
                Vector3 tempRot = q.eulerAngles;
                tempRot = new Vector3(tempRot.y, -tempRot.z, tempRot.x);
                q = Quaternion.Euler(tempRot);
            }

            return q;
        }

        public static Matrix Read(EndianBinaryReader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            UInt32 type = reader.ReadUInt32(); // 0x02: always at the start of a transformation matrix
            Matrix4x4 transMatrix = new Matrix4x4();
            Vector4 vec;
            if (l.mode == MapLoader.Mode.Rayman2PC) {
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
