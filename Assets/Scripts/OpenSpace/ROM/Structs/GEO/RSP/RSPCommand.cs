using System;

namespace OpenSpace.ROM.RSP {
	public class RSPCommand {
		private ulong data;
		private byte command;
		public GBI1_Vtx vtx;
		public GBI1_ModifyVtx modifyVtx;
		public GBI1_Tri2 tri2;
		public GBI1_Tri1 tri1;

		public Type Command {
			get {
				return (Type)command;
			}
		}

		public RSPCommand(Reader reader) {
			data = reader.ReadUInt64();
			Parse();
		}
		private void Parse() {
			command = (byte)extractBits(data, 8, 7*8);
			switch (Command) {
				case Type.RSP_GBI1_Vtx: vtx = new GBI1_Vtx(data); break;
				case Type.RSP_GBI1_Tri2: tri2 = new GBI1_Tri2(data); break;
				case Type.RSP_GBI1_ModifyVtx: modifyVtx = new GBI1_ModifyVtx(data); break;
				case Type.RSP_GBI1_EndDL: break;
				case Type.RSP_GBI1_Tri1: tri1 = new GBI1_Tri1(data); break;
				default:
					MapLoader.Loader.print("Unparsed RSP command: " + string.Format("{0:X2}", command));
					break;
			}
		}

		private static ulong extractBits(ulong number, int count, int offset) {
			return ((((ulong)1 << count) - 1) & (number >> (offset)));
		}



		public class GBI1_Vtx {
			public byte v0; // Position in vertex buffer to write to
			public byte n; // Number of vertices to load into vertex buffer. max 32
			public ushort length; // Length of data to write in vertex buffer
			public byte segment; // RSP memory works with segments: http://n64.icequake.net/doc/n64intro/kantan/step1/1-4.html
			public uint address; // Address of vertex in segment to load. Since each vertex is 0x10 bytes, this is a multiple of 0x10
			public GBI1_Vtx(ulong data) {
				v0 = (byte)extractBits(data, 7, 6 * 8 + 1);
				n = (byte)extractBits(data, 6, 4*8 + 10);
				length = (byte)extractBits(data, 10, 4 * 8);
				segment = (byte)extractBits(data, 8, 3 * 8);
				address = (uint)extractBits(data, 3 * 8, 0 * 8);
			}
		}
		public class GBI1_ModifyVtx {
			public byte command;
			public ushort vertex;

			public byte r;
			public byte g;
			public byte b;
			public byte a;
			public short u;
			public short v;

			public short x;
			public short y;
			public short z;


			public enum Type {
				RSP_MV_WORD_OFFSET_POINT_RGBA = 0x10,
				RSP_MV_WORD_OFFSET_POINT_ST = 0x14,
				RSP_MV_WORD_OFFSET_POINT_XYSCREEN = 0x18,
				RSP_MV_WORD_OFFSET_POINT_ZSCREEN = 0x1C
			}
			public Type Command {
				get {
					return (Type)command;
				}
			}
			public GBI1_ModifyVtx(ulong data) {
				command = (byte)extractBits(data, 8, 6 * 8);
				vertex = (ushort)extractBits(data, 15, 4 * 8 + 1);
				switch (Command) {
					case Type.RSP_MV_WORD_OFFSET_POINT_RGBA:
						r = (byte)extractBits(data, 8, 0 * 8);
						g = (byte)extractBits(data, 8, 1 * 8);
						b = (byte)extractBits(data, 8, 2 * 8);
						a = (byte)extractBits(data, 8, 3 * 8);
						break;
					case Type.RSP_MV_WORD_OFFSET_POINT_ST:
						v = unchecked((short)extractBits(data, 16, 0 * 16));
						u = unchecked((short)extractBits(data, 16, 1 * 16));
						break;
					case Type.RSP_MV_WORD_OFFSET_POINT_XYSCREEN:
						x = unchecked((short)extractBits(data, 14, 0 * 16 + 2));
						y = unchecked((short)extractBits(data, 14, 1 * 16 + 2));
						break;
					case Type.RSP_MV_WORD_OFFSET_POINT_ZSCREEN:
						z = unchecked((short)extractBits(data, 16, 0 * 16));
						break;
				}
			}
		}
		public class GBI1_Tri2 {
			public byte v0;
			public byte v1;
			public byte v2;
			public byte flag; // Contains which of the vertices contains the normal/color for the face (for flat shading)
			public byte v3;
			public byte v4;
			public byte v5;
			public GBI1_Tri2(ulong data) {
				v5 =   (byte)extractBits(data, 7, 6 * 8 + 1);
				v4 =   (byte)extractBits(data, 7, 5 * 8 + 1);
				v3 =   (byte)extractBits(data, 7, 4 * 8 + 1);
				flag = (byte)extractBits(data, 8, 3 * 8);
				v2 =   (byte)extractBits(data, 7, 2 * 8 + 1);
				v1 =   (byte)extractBits(data, 7, 1 * 8 + 1);
				v0 =   (byte)extractBits(data, 7, 0 * 8 + 1);
			}
		}
		public class GBI1_Tri1 {
			public byte v0;
			public byte v1;
			public byte v2;
			public byte flag; // Contains which of the vertices contains the normal/color for the face (for flat shading). 0, 1 or 2
			public GBI1_Tri1(ulong data) {
				flag = (byte)extractBits(data, 8, 3 * 8);
				v2 = (byte)extractBits(data, 7, 2 * 8 + 1);
				v1 = (byte)extractBits(data, 7, 1 * 8 + 1);
				v0 = (byte)extractBits(data, 7, 0 * 8 + 1);
			}
		}

		public enum Type {
			RSP_GBI1_SpNoop = 0x0,
			RSP_GBI0_Mtx = 0x1,

			RSP_GBI1_MoveMem = 0x3,
			RSP_GBI1_Vtx = 0x4,
			RSP_GBI0_DL = 0x6,
			RSP_GBI1_Sprite2DBase = 0x9,

			RSP_GBI1_LoadUCode = 0xAF,
			RSP_GBI1_BranchZ = 0xB0,
			RSP_GBI1_Tri2 = 0xB1,
			RSP_GBI1_ModifyVtx = 0xB2,
			RSP_GBI1_RDPHalf_2 = 0xB3,
			RSP_GBI1_RDPHalf_1 = 0xB4,
			RSP_GBI1_Line3D = 0xB5,
			RSP_GBI1_ClearGeometryMode = 0xB6,
			RSP_GBI1_SetGeometryMode = 0xB7,
			RSP_GBI1_EndDL = 0xB8,
			RSP_GBI1_SetOtherModeL = 0xB9,
			RSP_GBI1_SetOtherModeH = 0xBA,
			RSP_GBI1_Texture = 0xBB,
			RSP_GBI1_MoveWord = 0xBC,
			RSP_GBI1_PopMtx = 0xBD,
			RSP_GBI1_CullDL = 0xBE,
			RSP_GBI1_Tri1 = 0xBF,
			RSP_GBI1_Noop = 0xC0,
			RSP_S2DEX_SPObjLoadTxtr_Ucode1 = 0xC1,

			RDP_TriFill = 0xC8,
			RDP_TriFillZ = 0xC9,
			RDP_TriTxtr = 0xCA,
			RDP_TriTxtrZ = 0xCB,
			RDP_TriShade = 0xCC,
			RDP_TriShadeZ = 0xCD,
			RDP_TriShadeTxtr = 0xCE,
			RDP_TriShadeTxtrZ = 0xCF,

			DLParser_TexRect = 0xE4,
			DLParser_TexRectFlip = 0xE5,
			DLParser_RDPLoadSync = 0xE6,
			DLParser_RDPPipeSync = 0xE7,
			DLParser_RDPTileSync = 0xE8,
			DLParser_RDPFullSync = 0xE9,
			DLParser_SetKeyGB = 0xEA,
			DLParser_SetKeyR = 0xEB,
			DLParser_SetConvert = 0xEC,
			DLParser_SetScissor = 0xED,
			DLParser_SetPrimDepth = 0xEE,
			DLParser_RDPSetOtherMode = 0xEF,
			DLParser_LoadTLut = 0xF0,

			DLParser_SetTileSize = 0xF2,
			DLParser_LoadBlock = 0xF3,
			DLParser_LoadTile = 0xF4,
			DLParser_SetTile = 0xF5,
			DLParser_FillRect = 0xF6,
			DLParser_SetFillColor = 0xF7,
			DLParser_SetFogColor = 0xF8,
			DLParser_SetBlendColor = 0xF9,
			DLParser_SetPrimColor = 0xFA,
			DLParser_SetEnvColor = 0xFB,
			DLParser_SetCombine = 0xFC,
			DLParser_SetTImg = 0xFD,
			DLParser_SetZImg = 0xFE,
			DLParser_SetCImg = 0xFF
		}

	}
}
