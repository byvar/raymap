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
        public Matrix4x4? scaleMatrix;
        public Vector4? v;

        public Matrix(Pointer offset, uint type, Matrix4x4 matrix, Vector4? vec) {
            this.offset = offset;
            this.type = type;
            this.m = matrix;
            this.v = vec;
        }

        public static Matrix Identity {
            get {
                return new Matrix(null, 0, Matrix4x4.identity, Vector4.one);
            }
        }

        public void SetScaleMatrix(Matrix4x4 scaleMatrix) {
            this.scaleMatrix = scaleMatrix;
        }

        public Vector3 GetPosition(bool convertAxes = false) {
            if (convertAxes) {
                return new Vector3(m[0, 3], m[2, 3], m[1, 3]);
            } else {
                return new Vector3(m[0, 3], m[1, 3], m[2, 3]);
            }
        }
        
        public Vector3 GetScale(bool convertAxes = false) {
			if (scaleMatrix.HasValue) {
				if (convertAxes) {
					return new Vector3(scaleMatrix.Value.GetColumn(0).magnitude, scaleMatrix.Value.GetColumn(2).magnitude, scaleMatrix.Value.GetColumn(1).magnitude);
				} else {
					return new Vector3(scaleMatrix.Value.GetColumn(0).magnitude, scaleMatrix.Value.GetColumn(1).magnitude, scaleMatrix.Value.GetColumn(2).magnitude);
				}
			} else if (v.HasValue) {
				if (convertAxes) {
					return new Vector3(v.Value.x, v.Value.z, v.Value.y);
				} else {
					return new Vector3(v.Value.x, v.Value.y, v.Value.z);
				}
			} else {
                if (convertAxes) {
                    return new Vector3(m.GetColumn(0).magnitude, m.GetColumn(2).magnitude, m.GetColumn(1).magnitude);
                } else {
                    return new Vector3(m.GetColumn(0).magnitude, m.GetColumn(1).magnitude, m.GetColumn(2).magnitude);
                }
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
			float m00, m01, m02, m10, m11, m12, m20, m21, m22;
			if (v.HasValue && v.Value.x != 0 && v.Value.y != 0 && v.Value.z != 0) {
				m00 = m.m00 / v.Value.x;
				m01 = m.m01 / v.Value.y;
				m02 = m.m02 / v.Value.z;
				m10 = m.m10 / v.Value.x;
				m11 = m.m11 / v.Value.y;
				m12 = m.m12 / v.Value.z;
				m20 = m.m20 / v.Value.x;
				m21 = m.m21 / v.Value.y;
				m22 = m.m22 / v.Value.z;
			} else {
				m00 = m.m00;
				m01 = m.m01;
				m02 = m.m02;
				m10 = m.m10;
				m11 = m.m11;
				m12 = m.m12;
				m20 = m.m20;
				m21 = m.m21;
				m22 = m.m22;
			}

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

        public static Matrix Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
			//l.print("MATRIX " + offset);
            UInt32 type = Settings.s.game != Settings.Game.R2Revolution ? reader.ReadUInt32() : 0; // 0x02: always at the start of a transformation matrix
            Matrix mat = new Matrix(offset, type, new Matrix4x4(), null);
            if (Settings.s.engineVersion < Settings.EngineVersion.R3 && Settings.s.game != Settings.Game.R2Revolution) {
                Vector3 pos = Vector3.zero;

                if (Settings.s.platform != Settings.Platform.DC) {
                    pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                }
                mat.m.SetColumn(0, new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0f));
                mat.m.SetColumn(1, new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0f));
                mat.m.SetColumn(2, new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0f));
                Matrix4x4 sclMatrix = new Matrix4x4();
                sclMatrix.SetColumn(0, new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), Settings.s.platform == Settings.Platform.DC ? reader.ReadSingle() : 0f));
                sclMatrix.SetColumn(1, new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), Settings.s.platform == Settings.Platform.DC ? reader.ReadSingle() : 0f));
                sclMatrix.SetColumn(2, new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), Settings.s.platform == Settings.Platform.DC ? reader.ReadSingle() : 0f));
                if (Settings.s.platform == Settings.Platform.DC) {
                    pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                }

                mat.m.SetColumn(3, new Vector4(pos.x, pos.y, pos.z, 1f));
                sclMatrix.SetColumn(3, new Vector4(0, 0, 0, 1f));
                mat.SetScaleMatrix(sclMatrix);

                /*if (type != 4) {
                    transMatrix.SetColumn(0, rotColX);
                    transMatrix.SetColumn(1, rotColY);
                    transMatrix.SetColumn(2, rotColZ);
                } else {
                    transMatrix.SetColumn(0, sclColX);
                    transMatrix.SetColumn(1, sclColY);
                    transMatrix.SetColumn(2, sclColZ);
                }*/
            } else {
                if (Settings.s.platform == Settings.Platform.PS2 && Settings.s.game != Settings.Game.R2Revolution) {
                    Vector3 v = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                }
                mat.m.SetColumn(0, new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
                mat.m.SetColumn(1, new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
                mat.m.SetColumn(2, new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
                mat.m.SetColumn(3, new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
				mat.v = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
				/*mat.SetScaleMatrix(new Matrix4x4(
					new Vector4(v.x, 0, 0, 0), 
					new Vector4(0,v.y, 0, 0), 
					new Vector4(0, 0, v.z, 0), 
					new Vector4(0, 0, 0, v.w)));*/

			}
			if (Settings.s.game == Settings.Game.R2Revolution) {
				mat.type = reader.ReadUInt32();
				// There's 0x8c more?
			}
            return mat;
        }

        public static Matrix ReadCompressed(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            ushort type = reader.ReadUInt16();
            Vector4 vec = new Vector4(1f, 1f, 1f, 1f);
            Vector3? pos = null;
            Quaternion? rot = null;
            Matrix4x4? sclM = null;
            float x, y, z, w;
            /*the first byte & 0xF is the type
            1: translation
            2: rotation only
            3: translation & rotation
            7: translation, rotation, zoom (1 word, conv to float). zoom is basically scale for all axes
            11: translation, rotation, scale per axis (3 words, conv to float)
            15: translation, rotation, scale (6 words, conv to float)*/
            int actualType = type < 128 ? (type & 0xF) : 128;
            if (actualType == 1 || actualType == 3 || actualType == 7 || actualType == 11 || actualType == 15) {
                // Translation
                x = (float)reader.ReadInt16() / (float)512;
                y = (float)reader.ReadInt16() / (float)512;
                z = (float)reader.ReadInt16() / (float)512;
                pos = new Vector3(x, y, z);
            }
            if (actualType == 2 || actualType == 3 || actualType == 7 || actualType == 11 || actualType == 15) {
                // Rotation
                w = (float)reader.ReadInt16() / (float)Int16.MaxValue;
                x = (float)reader.ReadInt16() / (float)Int16.MaxValue;
                y = (float)reader.ReadInt16() / (float)Int16.MaxValue;
                z = (float)reader.ReadInt16() / (float)Int16.MaxValue;
                //rot = new Quaternion(x, y, z, w);
                //rot = Quaternion.Euler(rot.Value.eulerAngles);
                Animation.Component.AnimQuaternion q = new Animation.Component.AnimQuaternion();
                q.quaternion = new Quaternion(x, y, z, w);
                Matrix rotM = q.ToMatrix();
                rot = rotM.GetRotation(convertAxes: false);
            }
            if (actualType == 7) {
                // Zoom scale
                x = (float)reader.ReadInt16() / (float)256;
                sclM = Matrix4x4.Scale(new Vector3(x,x,x));
            } else if (actualType == 11) {
                // Axial scale
                x = (float)reader.ReadInt16() / (float)256;
                y = (float)reader.ReadInt16() / (float)256;
                z = (float)reader.ReadInt16() / (float)256;
                sclM = Matrix4x4.Scale(new Vector3(x,y,z));
            } else if (actualType == 15) {
                // Matrix scale
                float m0 = (float)reader.ReadInt16() / (float)256;
                float m1 = (float)reader.ReadInt16() / (float)256;
                float m2 = (float)reader.ReadInt16() / (float)256;
                float m3 = (float)reader.ReadInt16() / (float)256;
                float m4 = (float)reader.ReadInt16() / (float)256;
                float m5 = (float)reader.ReadInt16() / (float)256;
                // Actually it should be like this, but oh well:
                /*sclM = new Matrix4x4();
                sclM.Value.SetColumn(0, new Vector4(m0, m1, m2, 0f));
                sclM.Value.SetColumn(1, new Vector4(m1, m3, m4, 0f));
                sclM.Value.SetColumn(2, new Vector4(m2, m4, m5, 0f));
                sclM.Value.SetColumn(3, new Vector4(0f, 0f, 0f, 1f));*/
                sclM = Matrix4x4.Scale(new Vector3(m0, m3, m5));
            }
            if (!pos.HasValue) pos = Vector3.zero;
            if (!rot.HasValue) rot = Quaternion.identity;
            Matrix mat = new Matrix(offset, type, Matrix4x4.TRS(pos.Value, rot.Value, Vector3.one), vec);
            if (sclM.HasValue) {
                mat.SetScaleMatrix(sclM.Value);
            }
            return mat;
        }

		public static Matrix ReadRenderware(Reader reader) {
			MapLoader l = MapLoader.Loader;
			Matrix mat = new Matrix(null, 0, new Matrix4x4(), Vector4.one);
			mat.m.SetColumn(0, new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0f));
			mat.m.SetColumn(1, new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0f));
			mat.m.SetColumn(2, new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0f));
			mat.m.SetColumn(3, new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 1f)); // position
			return mat;
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
                v = new Vector4(1f * scale.x, 1f * scale.y, 1f * scale.z, v.HasValue ? v.Value.w : 1f);
            }
        }

        public void Write(Writer writer) {
            Pointer.Goto(ref writer, offset);
            writer.Write(type);

            if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                Vector4 pos = m.GetColumn(3);
                writer.Write(pos.x);
                writer.Write(pos.y);
                writer.Write(pos.z);
                if (type == 4) {
                    writer.BaseStream.Seek(36, System.IO.SeekOrigin.Current);
                }
                for (int j = 0; j < 3; j++) {
                    Vector4 col = m.GetColumn(j);
                    writer.Write(col.x);
                    writer.Write(col.y);
                    writer.Write(col.z);
                }
            } else {
                for (int i = 0; i < 4; i++) {
                    Vector4 col = m.GetColumn(i);
                    for (int j = 0; j < 4; j++) {
                        writer.Write(col[j]);
                    }
                }
				if (v.HasValue) {
					for (int j = 0; j < 4; j++) {
						writer.Write(v.Value[j]);
					}
				}
            }
        }
    }
}
