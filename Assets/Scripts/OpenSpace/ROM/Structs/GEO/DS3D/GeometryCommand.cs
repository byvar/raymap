using System;
using UnityEngine;

namespace OpenSpace.ROM.DS3D {
	public class GeometryCommand {
		public byte command;
		public Type Command => (Type)command;
		private byte primitiveType;
		public PrimitiveType Primitive => (PrimitiveType)primitiveType;
		public short x;
		public short y;
		public short z;
		public float scale;
		public short u;
		public short v;
		public byte r;
		public byte g;
		public byte b;
		public byte a;

		public GeometryCommand(byte command, Reader reader) {
			this.command = command;
			Parse(reader);
		}
		private void Parse(Reader reader) {
			uint param1;
			switch (Command) {
				case Type.BEGIN_VTXS:
					param1 = reader.ReadUInt32();
					primitiveType = (byte)extractBits(param1, 2, 0);
					break;
				case Type.END_VTXS:
					break;
				case Type.TEXCOORD:
					u = reader.ReadInt16();
					v = reader.ReadInt16();
					scale = (1 << 4);
					break;
				case Type.NORMAL:
					param1 = reader.ReadUInt32();
					x = (short)((extractBits(param1, 10,  0) << 6) / (1 << 6));
					y = (short)((extractBits(param1, 10, 10) << 6) / (1 << 6));
					z = (short)((extractBits(param1, 10, 20) << 6) / (1 << 6));
					scale = (1 << 9);
					break;
				case Type.COLOR:
					param1 = reader.ReadUInt32();
					r = (byte)extractBits(param1, 5, 0);
					g = (byte)extractBits(param1, 5, 5);
					b = (byte)extractBits(param1, 5, 10);
					scale = ((float)(1 << 5)) - 1f;
					break;
				case Type.VTX_16:
					x = reader.ReadInt16();
					y = reader.ReadInt16();
					z = reader.ReadInt16();
					reader.ReadInt16();
					scale = 1f;
					break;
				case Type.VTX_XY:
					x = reader.ReadInt16();
					y = reader.ReadInt16();
					scale = 1f;
					break;
				case Type.VTX_XZ:
					x = reader.ReadInt16();
					z = reader.ReadInt16();
					scale = 1f;
					break;
				case Type.VTX_YZ:
					y = reader.ReadInt16();
					z = reader.ReadInt16();
					scale = 1f;
					break;
				default:
					Debug.LogError("Unparsed DS Geometry command: " + string.Format("{0:X2}", command));
					break;
			}
		}

		private static uint extractBits(uint number, int count, int offset) {
			return (uint)(((1 << count) - 1) & (number >> (offset)));
		}

		public enum Type {
			Unknown = 0,
			MTX_MODE = 0x10,
			COLOR = 0x20,
			NORMAL = 0x21,
			TEXCOORD = 0x22,
			VTX_16 = 0x23,
			VTX_10 = 0x24,
			VTX_XY = 0x25,
			VTX_XZ = 0x26,
			VTX_YZ = 0x27,
			VTX_DIFF = 0x28,
			POLYGON_ATTR = 0x29,
			TEXIMAGE_PARAM = 0x2A,
			PLTT_BASE = 0x2B,
			DIF_AMB = 0x30,
			SPE_EMI = 0x31,
			LIGHT_VECTOR = 0x32,
			LIGHT_COLOR = 0x33,
			SHININESS = 0x34,
			BEGIN_VTXS = 0x40,
			END_VTXS = 0x41,
		}
		public enum PrimitiveType {
			Triangles = 0,
			Quads = 1,
			TriangleStrip = 2, // 3+(N-1) vertices per N triangles. Nv = N-2t
			QuadStrip = 3, // 4+(N-1)*2 vertices per N quads        Nv = N/2-1t
		}
	}
}
