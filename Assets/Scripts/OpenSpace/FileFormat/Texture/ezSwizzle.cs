using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.FileFormat.Texture {
	public class ezSwizzle {
		static byte[] gs = new byte[1024 * 1024 * 4];

		#region Constants
		static readonly int[] block32 = new int[32] {
			 0,  1,  4,  5, 16, 17, 20, 21,
			 2,  3,  6,  7, 18, 19, 22, 23,
			 8,  9, 12, 13, 24, 25, 28, 29,
			10, 11, 14, 15, 26, 27, 30, 31
		};


		static readonly int[] columnWord32 = new int[16] {
			 0,  1,  4,  5,  8,  9, 12, 13,
			 2,  3,  6,  7, 10, 11, 14, 15
		};

		static readonly int[] block16 = new int[32] {
			 0,  2,  8, 10,
			 1,  3,  9, 11,
			 4,  6, 12, 14,
			 5,  7, 13, 15,
			16, 18, 24, 26,
			17, 19, 25, 27,
			20, 22, 28, 30,
			21, 23, 29, 31
		};

		static readonly int[] columnWord16 = new int[32] {
			 0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,
			 2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15
		};

		static readonly int[] columnHalf16 = new int[32] {
			0, 0, 0, 0, 0, 0, 0, 0,  1, 1, 1, 1, 1, 1, 1, 1,
			0, 0, 0, 0, 0, 0, 0, 0,  1, 1, 1, 1, 1, 1, 1, 1
		};


		static readonly int[] block8 = new int[32] {
			 0,  1,  4,  5, 16, 17, 20, 21,
			 2,  3,  6,  7, 18, 19, 22, 23,
			 8,  9, 12, 13, 24, 25, 28, 29,
			10, 11, 14, 15, 26, 27, 30, 31
		};

		static readonly int[][] columnWord8 = new int[2][] {
			new int[64] {
				 0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,
				 2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15,

				 8,  9, 12, 13,  0,  1,  4,  5,   8,  9, 12, 13,  0,  1,  4,  5,
				10, 11, 14, 15,  2,  3,  6,  7,  10, 11, 14, 15,  2,  3,  6,  7
			},
			new int[64] {
				 8,  9, 12, 13,  0,  1,  4,  5,   8,  9, 12, 13,  0,  1,  4,  5,
				10, 11, 14, 15,  2,  3,  6,  7,  10, 11, 14, 15,  2,  3,  6,  7,

				 0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,
				 2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15
			}
		};

		static readonly int[] columnByte8 = new int[64] {
			0, 0, 0, 0, 0, 0, 0, 0,  2, 2, 2, 2, 2, 2, 2, 2,
			0, 0, 0, 0, 0, 0, 0, 0,  2, 2, 2, 2, 2, 2, 2, 2,

			1, 1, 1, 1, 1, 1, 1, 1,  3, 3, 3, 3, 3, 3, 3, 3,
			1, 1, 1, 1, 1, 1, 1, 1,  3, 3, 3, 3, 3, 3, 3, 3
		};

		static readonly int[] block4 = new int[32] {
			0,  2,  8, 10,
			1,  3,  9, 11,
			4,  6, 12, 14,
			5,  7, 13, 15,
			16, 18, 24, 26,
			17, 19, 25, 27,
			20, 22, 28, 30,
			21, 23, 29, 31
		};

		static readonly int[][] columnWord4 = new int[2][] {
			new int[128] {
				 0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,
				 2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15,

				 8,  9, 12, 13,  0,  1,  4,  5,   8,  9, 12, 13,  0,  1,  4,  5,   8,  9, 12, 13,  0,  1,  4,  5,   8,  9, 12, 13,  0,  1,  4,  5,
				10, 11, 14, 15,  2,  3,  6,  7,  10, 11, 14, 15,  2,  3,  6,  7,  10, 11, 14, 15,  2,  3,  6,  7,  10, 11, 14, 15,  2,  3,  6,  7
			},
			new int[128] {
				 8,  9, 12, 13,  0,  1,  4,  5,   8,  9, 12, 13,  0,  1,  4,  5,   8,  9, 12, 13,  0,  1,  4,  5,   8,  9, 12, 13,  0,  1,  4,  5,
				10, 11, 14, 15,  2,  3,  6,  7,  10, 11, 14, 15,  2,  3,  6,  7,  10, 11, 14, 15,  2,  3,  6,  7,  10, 11, 14, 15,  2,  3,  6,  7,

				 0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,   0,  1,  4,  5,  8,  9, 12, 13,
				 2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15,   2,  3,  6,  7, 10, 11, 14, 15
			}
		};

		static readonly int[] columnByte4 = new int[128] {
			0, 0, 0, 0, 0, 0, 0, 0,  2, 2, 2, 2, 2, 2, 2, 2,  4, 4, 4, 4, 4, 4, 4, 4,  6, 6, 6, 6, 6, 6, 6, 6,
			0, 0, 0, 0, 0, 0, 0, 0,  2, 2, 2, 2, 2, 2, 2, 2,  4, 4, 4, 4, 4, 4, 4, 4,  6, 6, 6, 6, 6, 6, 6, 6,

			1, 1, 1, 1, 1, 1, 1, 1,  3, 3, 3, 3, 3, 3, 3, 3,  5, 5, 5, 5, 5, 5, 5, 5,  7, 7, 7, 7, 7, 7, 7, 7,
			1, 1, 1, 1, 1, 1, 1, 1,  3, 3, 3, 3, 3, 3, 3, 3,  5, 5, 5, 5, 5, 5, 5, 5,  7, 7, 7, 7, 7, 7, 7, 7
		};
		#endregion

		public void writeTexPSMT4(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, byte[] data) {
			dbw >>= 1;
			int src = 0;
			int startBlockPos = dbp * 64;

			bool odd = false;

			for (int y = dsay; y < dsay + rrh; y++) {
				for (int x = dsax; x < dsax + rrw; x++) {
					int pageX = x / 128;
					int pageY = y / 128;
					int page = pageX + pageY * dbw;

					int px = x - (pageX * 128);
					int py = y - (pageY * 128);

					int blockX = px / 32;
					int blockY = py / 16;
					int block = block4[blockX + blockY * 4];

					int bx = px - blockX * 32;
					int by = py - blockY * 16;

					int column = by / 4;

					int cx = bx;
					int cy = by - column * 4;
					int cw = columnWord4[column & 1][cx + cy * 32];
					int cb = columnByte4[cx + cy * 32];

					int dst = startBlockPos + page * 2048 + block * 64 + column * 16 + cw;

			if ((cb & 1) != 0) {
				if (odd)
					gs[4 * dst + cb >> 1] = (byte)((gs[4 * dst + cb >> 1] & 0x0f) | ((data[src]) & 0xf0));
				else
					gs[4 * dst + cb >> 1] = (byte)((gs[4 * dst + cb >> 1] & 0x0f) | (((data[src]) << 4) & 0xf0));
			} else {
				if (odd)
					gs[4 * dst + cb >> 1] = (byte)((gs[4 * dst + cb >> 1] & 0xf0) | (((data[src]) >> 4) & 0x0f));
				else
					gs[4 * dst + cb >> 1] = (byte)((gs[4 * dst + cb >> 1] & 0xf0) | ((data[src]) & 0x0f));
			}

			if (odd)
				src++;

			odd = !odd;
		}
	}
}

		public void readTexPSMT4(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, ref byte[] data) {
			dbw >>= 1;
			//unsigned char* src = (unsigned char*)data;
			int src = 0;
			int startBlockPos = dbp * 64;

			bool odd = false;

			for (int y = dsay; y < dsay + rrh; y++) {
				for (int x = dsax; x < dsax + rrw; x++) {
					int pageX = x / 128;
					int pageY = y / 128;
					int page = pageX + pageY * dbw;

					int px = x - (pageX * 128);
					int py = y - (pageY * 128);

					int blockX = px / 32;
					int blockY = py / 16;
					int block = block4[blockX + blockY * 4];

					int bx = px - blockX * 32;
					int by = py - blockY * 16;

					int column = by / 4;

					int cx = bx;
					int cy = by - column * 4;
					int cw = columnWord4[column & 1][cx + cy * 32];
					int cb = columnByte4[cx + cy * 32];

					int dst = startBlockPos + page * 2048 + block * 64 + column * 16 + cw + cb >> 1;
					//unsigned char* dst = (unsigned char*)&gsmem[startBlockPos + page * 2048 + block * 64 + column * 16 + cw];

					if ((cb & 1) != 0) {
						if (odd)
							data[src] = (byte)(((data[src]) & 0x0f) | (byte)(gs[4 * dst + cb >> 1] & 0xf0));
						else
							data[src] = (byte)(((data[src]) & 0xf0) | ((byte)(gs[4 * dst + cb >> 1] >> 4) & 0x0f));
					} else {
						if (odd)
							data[src] = (byte)(((data[src]) & 0x0f) | (((byte)gs[4 * dst + cb >> 1] << 4) & 0xf0));
						else
							data[src] = (byte)(((data[src]) & 0xf0) | ((byte)gs[4 * dst + cb >> 1] & 0x0f));
					}

					if (odd)
						src++;

					odd = !odd;
				}
			}
		}


		public void readTexPSMT4_mod(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, ref byte[] data) {
			dbw >>= 1;
			//unsigned char* src = (unsigned char*)data;
			int src = 0;
			int startBlockPos = dbp * 64;

			bool odd = false;

			for (int y = dsay; y < dsay + rrh; y++) {
				for (int x = dsax; x < dsax + rrw; x++) {
					int pageX = x / 128;
					int pageY = y / 128;
					int page = pageX + pageY * dbw;

					int px = x - (pageX * 128);
					int py = y - (pageY * 128);

					int blockX = px / 32;
					int blockY = py / 16;
					int block = block4[blockX + blockY * 4];

					int bx = px - blockX * 32;
					int by = py - blockY * 16;

					int column = by / 4;

					int cx = bx;
					int cy = by - column * 4;
					int cb = columnByte4[cx + cy * 32];
					int cw = columnWord4[column & 1][cx + cy * 32];
					//int cb = columnByte4[cx + cy * 32];

					int dst = startBlockPos + page * 2048 + block * 64 + column * 16 + cw;
					//unsigned char* dst = (unsigned char*)&gsmem[startBlockPos + page * 2048 + block * 64 + column * 16 + cw];

					if ((cb & 1) != 0) {
						if (odd)
							data[src] = (byte)((gs[4 * dst + (cb >> 1)] >> 4) & 0x0f);
						else
							data[src] = (byte)((gs[4 * dst + (cb >> 1)] >> 4) & 0x0f);
					} else {
						if (odd)
							data[src] = (byte)(gs[4 * dst + (cb >> 1)] & 0x0f);
						else
							data[src] = (byte)(gs[4 * dst + (cb >> 1)] & 0x0f);
					}

					src++;

					odd = !odd;
				}
			}
		}

		public void readTexPSMT8(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, ref byte[] data) {
			dbw >>= 1;
			int src = 0;
			int startBlockPos = dbp * 64;

			for (int y = dsay; y < dsay + rrh; y++) {
				for (int x = dsax; x < dsax + rrw; x++) {
					int pageX = x / 128;
					int pageY = y / 64;
					int page = pageX + pageY * dbw;

					int px = x - (pageX * 128);
					int py = y - (pageY * 64);

					int blockX = px / 16;
					int blockY = py / 16;
					int block = block8[blockX + blockY * 8];

					int bx = px - blockX * 16;
					int by = py - blockY * 16;

					int column = by / 4;

					int cx = bx;
					int cy = by - column * 4;
					int cw = columnWord8[column & 1][cx + cy * 16];
					int cb = columnByte8[cx + cy * 16];

					int dst = startBlockPos + page * 2048 + block * 64 + column * 16 + cw;
					data[src] = gs[4 * dst + cb];
					src++;
				}
			}
		}

		public void writeTexPSMT8(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, byte[] data) {
			dbw >>= 1;
			int src = 0;
			int startBlockPos = dbp * 64;

			for (int y = dsay; y < dsay + rrh; y++) {
				for (int x = dsax; x < dsax + rrw; x++) {
					int pageX = x / 128;
					int pageY = y / 64;
					int page = pageX + pageY * dbw;

					int px = x - (pageX * 128);
					int py = y - (pageY * 64);

					int blockX = px / 16;
					int blockY = py / 16;
					int block = block8[blockX + blockY * 8];

					int bx = px - (blockX * 16);
					int by = py - (blockY * 16);

					int column = by / 4;

					int cx = bx;
					int cy = by - column * 4;
					int cw = columnWord8[column & 1][cx + cy * 16];
					int cb = columnByte8[cx + cy * 16];

					int dst = startBlockPos + page * 2048 + block * 64 + column * 16 + cw;
					gs[4 * dst + cb] = data[src];
					src++;
				}
			}
		}

		public void writeTexPSMCT16(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, byte[] data) {
			//dbw >>= 1;
			int src = 0;
			int startBlockPos = dbp * 64;

			for (int y = dsay; y < dsay + rrh; y++) {
				for (int x = dsax; x < dsax + rrw; x++) {
					int pageX = x / 64;
					int pageY = y / 64;
					int page = pageX + pageY * dbw;

					int px = x - (pageX * 64);
					int py = y - (pageY * 64);

					int blockX = px / 16;
					int blockY = py / 8;
					int block = block16[blockX + blockY * 4];

					int bx = px - blockX * 16;
					int by = py - blockY * 8;

					int column = by / 2;

					int cx = bx;
					int cy = by - column * 2;
					int cw = columnWord16[cx + cy * 16];
					int ch = columnHalf16[cx + cy * 16];

					int dst = startBlockPos + page * 2048 + block * 64 + column * 16 + cw;
					for (int i = 0; i < 2; i++) {
						gs[4 * dst + 2 * ch + i] = data[src + i];
					}
					src += 2;
				}
			}
		}

		void readTexPSMCT16(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, ref byte[] data) {
			//dbw >>= 1;
			int src = 0;
			int startBlockPos = dbp * 64;

			for (int y = dsay; y < dsay + rrh; y++) {
				for (int x = dsax; x < dsax + rrw; x++) {
					int pageX = x / 64;
					int pageY = y / 64;
					int page = pageX + pageY * dbw;

					int px = x - (pageX * 64);
					int py = y - (pageY * 64);

					int blockX = px / 16;
					int blockY = py / 8;
					int block = block16[blockX + blockY * 4];

					int bx = px - blockX * 16;
					int by = py - blockY * 8;

					int column = by / 2;

					int cx = bx;
					int cy = by - column * 2;
					int cw = columnWord16[cx + cy * 16];
					int ch = columnHalf16[cx + cy * 16];

					int dst = startBlockPos + page * 2048 + block * 64 + column * 16 + cw;
					for (int i = 0; i < 2; i++) {
						data[src+i] = gs[4 * dst + 2 * ch + i];
					}
					src += 2;
				}
			}
		}

		public void writeTexPSMCT32(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, byte[] data) {
			int src = 0;
			int startBlockPos = dbp * 64;

			for (int y = dsay; y < dsay + rrh; y++) {
				for (int x = dsax; x < dsax + rrw; x++) {
					int pageX = x / 64;
					int pageY = y / 32;
					int page = pageX + pageY * dbw;

					int px = x - (pageX * 64);
					int py = y - (pageY * 32);

					int blockX = px / 8;
					int blockY = py / 8;
					int block = block32[blockX + blockY * 8];

					int bx = px - blockX * 8;
					int by = py - blockY * 8;

					int column = by / 2;

					int cx = bx;
					int cy = by - column * 2;
					int cw = columnWord32[cx + cy * 8];

					int dst = startBlockPos + page * 2048 + block * 64 + column * 16 + cw;
					for (int i = 0; i < 4; i++) {
						gs[4 * dst + i] = data[src + i];
					}
					src += 4;
				}
			}
		}

		public void readTexPSMCT32(int dbp, int dbw, int dsax, int dsay, int rrw, int rrh, ref byte[] data) {
			int src = 0;
			int startBlockPos = dbp * 64;

			for (int y = dsay; y < dsay + rrh; y++) {
				for (int x = dsax; x < dsax + rrw; x++) {
					int pageX = x / 64;
					int pageY = y / 32;
					int page = pageX + pageY * dbw;

					int px = x - (pageX * 64);
					int py = y - (pageY * 32);

					int blockX = px / 8;
					int blockY = py / 8;
					int block = block32[blockX + blockY * 8];

					int bx = px - blockX * 8;
					int by = py - blockY * 8;

					int column = by / 2;

					int cx = bx;
					int cy = by - column * 2;
					int cw = columnWord32[cx + cy * 8];


					int dst = startBlockPos + page * 2048 + block * 64 + column * 16 + cw;
					for (int i = 0; i < 4; i++) {
						data[src + i] = gs[4 * dst + i];
					}
					src += 4;
				}
			}
		}
	}
}
