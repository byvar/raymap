using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class PS1StreamFrameChannel : OpenSpaceStruct {
		public ushort flags;
		public short xShort;
		public short yShort;
		public short zShort;
		public int xInt;
		public int yInt;
		public int zInt;
		public short qx;
		public short qy;
		public short qz;
		public short qw;
		public short sx;
		public short sy;
		public short sz;

		// parsed
		public Quaternion quaternion;
		public Quaternion quaternionCamera;

		public int NTTO => (flags & 0x1ff);

		protected override void ReadInternal(Reader reader) {
			flags = reader.ReadUInt16();
			if (!HasFlag(StreamFlags.IntPosition)) {
				xShort = reader.ReadInt16();
				yShort = reader.ReadInt16();
				zShort = reader.ReadInt16();
			} else {
				xInt = reader.ReadInt32();
				yInt = reader.ReadInt32();
				zInt = reader.ReadInt32();
			}
			qx = reader.ReadInt16();
			qy = reader.ReadInt16();
			qz = reader.ReadInt16();
			qw = reader.ReadInt16();
			if (HasFlag(StreamFlags.Camera)) {
				// Set camera position
				quaternion = new Quaternion(
					qx / (float)Int16.MaxValue,
					qy / (float)Int16.MaxValue,
					qz / (float)Int16.MaxValue,
					-qw / (float)Int16.MaxValue);
			} else {
				quaternion = new Quaternion(
					qx / (float)Int16.MaxValue,
					qy / (float)Int16.MaxValue,
					qz / (float)Int16.MaxValue,
					-qw / (float)Int16.MaxValue);
				if (HasFlag(StreamFlags.Scale)) {
					// Scale
					sx = reader.ReadInt16();
					sy = reader.ReadInt16();
					sz = reader.ReadInt16();
				}
			}
			ConvertRotation();
		}


		[Flags]
		public enum StreamFlags : ushort {
			None = 0,
			IntPosition = 0x200,
			Camera = 0x300,
			Scale = 0x400,
			Parent = 0x4000,
			FlipX = 0x8000,
		}

		public bool HasFlag(StreamFlags flags) {
			return (this.flags & (ushort)flags) == (ushort)flags;
		}


		public Vector3 GetPosition(float factor = 256f, bool switchAxes = true) {
			int x, y, z;
			if (HasFlag(StreamFlags.IntPosition)) {
				x = xInt;
				y = yInt;
				z = zInt;
			} else {
				x = xShort;
				y = yShort;
				z = zShort;
			}
			if (switchAxes) {
				return new Vector3(x / factor, z / factor, y / factor);
			} else {
				return new Vector3(x / factor, y / factor, z / factor);
			}
		}
		public Vector3 GetScale(float factor = 256f, bool switchAxes = true) {
			if (!HasFlag(StreamFlags.Scale)) {
				return new Vector3(1f, 1f, 1f);
			}
			int x = sx;
			int y = sy;
			int z = sz;
			if (switchAxes) {
				return new Vector3(x / factor, z / factor, y / factor);
			} else {
				return new Vector3(x / factor, y / factor, z / factor);
			}
		}

		public Matrix ToMatrix() {
		Matrix4x4 m = new Matrix4x4();
		float x = quaternion.x;
		float y = quaternion.y;
		float z = quaternion.z;
		float w = quaternion.w;
		float qMagnitude = 1.0f / Mathf.Sqrt(x * x + y * y + z * z + w * w);
		x *= qMagnitude;
		y *= qMagnitude;
		z *= qMagnitude;
		w *= qMagnitude;

		float twoX = 2 * x;
		float twoY = 2 * y;
		float twoZ = 2 * z;

		float xw = twoX * w;
		float yw = twoY * w;
		float zw = twoZ * w;

		float xx = twoX * x;
		float yx = twoY * x;
		float zx = twoZ * x;

		float yy = twoY * y;
		float zy = twoZ * y;
		float zz = twoZ * z;
		m.m00 = 1.0f - (zz + yy);
		m.m01 = yx + zw;
		m.m02 = zx - yw;

		m.m10 = yx - zw;
		m.m11 = 1.0f - (zz + xx);
		m.m12 = zy + xw;

		m.m20 = zx + yw;
		m.m21 = zy - xw;
		m.m22 = 1.0f - (yy + xx);
		m.SetColumn(3, new Vector4(0, 0, 0, 1f));
		m.SetRow(3, new Vector4(0, 0, 0, 1f));
		Matrix mat = new Matrix(null, 8, m, null);
		return mat;
	}

	public void ConvertRotation() {
		quaternion = ToMatrix().GetRotation(convertAxes: true);
	}
}
}
