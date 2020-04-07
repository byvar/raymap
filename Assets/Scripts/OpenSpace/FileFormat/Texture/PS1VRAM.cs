// Adapted from Rayman2Lib by szymski
// https://github.com/szymski/Rayman2Lib/blob/master/csharp_tools/Rayman2Lib/CNTFile.cs

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.FileFormat.Texture {
    public class PS1VRAM {
        public const int page_height = 256;
        public const int page_width = 128; // for 8-bit CLUT.

		public Page[][] pages = new Page[2][]; // y, x
		public int currentXPage = 0;
		public int currentYPage = 0;
		public int nextYInPage = 0;
		public List<ReservedBlock> reservedBlocks = new List<ReservedBlock>();

		public void ReserveBlock(int x, int y, int width, int height) {
			reservedBlocks.Add(new ReservedBlock() { x = x, y = y, width = width, height = height });
		}
		private bool AnyBlockReserved(int x, int y) {
			return reservedBlocks.Any(b => (x >= b.x && x < b.x + b.width) && (y >= b.y && y < b.y + b.height));
		}

		public void AddData(byte[] data, int width) {
			if (data == null) return;
			/*if (pages[currentYPage] == null || pages[currentYPage].Length == 0) {
				pages[currentYPage] = new Page[currentXPage + (width / page_width)];
			}
			if (pages[currentYPage].Length < currentXPage + (width / page_width)) {
				Array.Resize(ref pages[currentYPage], currentXPage + (width / page_width));
			}*/
			int height = data.Length / width + ((data.Length % width != 0) ? 1 : 0);
			//int xInPageMod = 0;
			int yInPageMod = 0;
			int curPageXMod = 0;
			if (height > 0 && width > 0) {
				int xInPage = 0, yInPage = 0;
				int curPageX = currentXPage, curPageY = currentYPage, curStartPageX = currentXPage;
				for (int y = 0; y < height; y++) {
					for (int x = 0; x < width; x++) {
						curStartPageX = currentXPage + curPageXMod;
						curPageX = curStartPageX + (x / page_width);
						curPageY = currentYPage + ((nextYInPage + yInPageMod + y) / page_height);
						xInPage = x % page_width;
						yInPage = (nextYInPage + yInPageMod + y) % page_height;
						while (curPageY > 1) {
							// Wrap
							curPageY -= 2;
							curPageX += (width / page_width);
							curStartPageX += (width / page_width);
						}
						int totalX = curPageX * page_width + xInPage;
						int totalY = curPageY * page_height + yInPage;
						while (AnyBlockReserved(totalX, totalY)) {
							//UnityEngine.Debug.Log("reserved " + totalX + " - " + totalY + " - " + curPageX + " - " + curPageY + " - " + x + " - " + y);
							// wrap
							curPageX += (width / page_width);
							curStartPageX += (width / page_width);

							yInPageMod += (page_height - yInPage) + (curPageY == 0 ? page_height : 0);
							curPageY = 0;
							yInPage = (nextYInPage + yInPageMod + y) % page_height;

							totalX = curPageX * page_width + xInPage;
							totalY = curPageY * page_height + yInPage;
						}
						if (pages[curPageY] == null || pages[curPageY].Length == 0) {
							pages[curPageY] = new Page[curPageX + 1];
						}
						if (pages[curPageY].Length < curPageX + 1) {
							Array.Resize(ref pages[curPageY], curPageX + 1);
						}
						if (pages[curPageY][curPageX] == null) {
							//UnityEngine.Debug.Log("Created page " + curPageX + "," + curPageY);
							pages[curPageY][curPageX] = new Page();
						}
						pages[curPageY][curPageX].SetByte(xInPage, yInPage, data[y * width + x]);
					}
				}
				currentXPage = curStartPageX;
				currentYPage = curPageY;


				nextYInPage = yInPage + 1;
				if (nextYInPage >= page_height) {
					// Change page
					nextYInPage -= page_height;
					currentYPage++;
					if (currentYPage > 1) {
						//  Wrap
						currentXPage += (width / page_width);
						currentYPage -= 2;
					}
				}
			}
			//UnityEngine.Debug.Log(currentXPage + " - " + currentYPage + " - " + curPageX + " - " + ((width / page_width) - 1));
			/*if (data != null) {
				if (pages.Count == 0) {
					pages.Add(new Page(pageWidth));
				}
				byte[] newData = pages.Last().AddData(data);
				while (newData != null) {
					pages.Add(new Page(pageWidth));
					newData = pages.Last().AddData(data);
				}
			}*/
		}


		public void AddDataReverse(byte[] data, int width) {
			if (data == null) return;
			/*if (pages[currentYPage] == null || pages[currentYPage].Length == 0) {
				pages[currentYPage] = new Page[currentXPage + (width / page_width)];
			}
			if (pages[currentYPage].Length < currentXPage + (width / page_width)) {
				Array.Resize(ref pages[currentYPage], currentXPage + (width / page_width));
			}*/
			int height = data.Length / width + ((data.Length % width != 0) ? 1 : 0);
			//int xInPageMod = 0;
			int yInPageMod = 0;
			int curPageXMod = 0;
			if (height > 0 && width > 0) {
				int xInPage = 0, yInPage = 0;
				int curPageX = currentXPage, curPageY = currentYPage, curStartPageX = currentXPage;
				for (int y = 0; y < height; y++) {
					for (int x = 0; x < width; x++) {
						curStartPageX = currentXPage + curPageXMod;
						curPageX = curStartPageX + (x / page_width);
						curPageY = currentYPage + ((nextYInPage + yInPageMod + y) / page_height);
						xInPage = x % page_width;
						yInPage = (nextYInPage + yInPageMod + y) % page_height;
						while (curPageY > 1) {
							// Wrap
							curPageY -= 2;
							curPageX += (width / page_width);
							curStartPageX += (width / page_width);
						}
						int totalX = curPageX * page_width + xInPage;
						int totalY = curPageY * page_height + yInPage;
						while (AnyBlockReserved(totalX, totalY)) {
							//UnityEngine.Debug.Log("reserved " + totalX + " - " + totalY + " - " + curPageX + " - " + curPageY + " - " + x + " - " + y);
							// wrap
							curPageX += (width / page_width);
							curStartPageX += (width / page_width);

							yInPageMod += (page_height - yInPage) + (curPageY == 0 ? page_height : 0);
							curPageY = 0;
							yInPage = (nextYInPage + yInPageMod + y) % page_height;

							totalX = curPageX * page_width + xInPage;
							totalY = curPageY * page_height + yInPage;
						}
						if (pages[1 - curPageY] == null || pages[1 - curPageY].Length == 0) {
							pages[1 - curPageY] = new Page[curPageX + 1];
						}
						if (pages[1 - curPageY].Length < curPageX + 1) {
							Array.Resize(ref pages[1 - curPageY], curPageX + 1);
						}
						if (pages[1 - curPageY][curPageX] == null) {
							//UnityEngine.Debug.Log("Created page " + curPageX + "," + curPageY);
							pages[1 - curPageY][curPageX] = new Page();
						}
						pages[1 - curPageY][curPageX].SetByte(xInPage, page_height - 1 - yInPage, data[y * width + x]);
					}
				}
				currentXPage = curStartPageX;
				currentYPage = curPageY;


				nextYInPage = yInPage + 1;
				if (nextYInPage >= page_height) {
					// Change page
					nextYInPage -= page_height;
					currentYPage++;
					if (currentYPage > 1) {
						//  Wrap
						currentXPage += (width / page_width);
						currentYPage -= 2;
					}
				}
			}
			//UnityEngine.Debug.Log(currentXPage + " - " + currentYPage + " - " + curPageX + " - " + ((width / page_width) - 1));
			/*if (data != null) {
				if (pages.Count == 0) {
					pages.Add(new Page(pageWidth));
				}
				byte[] newData = pages.Last().AddData(data);
				while (newData != null) {
					pages.Add(new Page(pageWidth));
					newData = pages.Last().AddData(data);
				}
			}*/
		}

		public void AddDataAt(int startXPage, int startYPage, int startX, int startY, byte[] data, int width) {
			if (data == null) return;
			int height = (startX + data.Length) / width + (((startX + data.Length) % width != 0) ? 1 : 0);
			if (height > 0 && width > 0) {
				int xInPage = startX, yInPage = 0;
				int curPageX = startXPage, curPageY = startYPage, curStartPageX = startXPage;
				for (int x = 0; x < width; x++) {
					for (int y = 0; y < height; y++) {
						curPageX = startXPage + ((startX + x) / page_width);
						curStartPageX = startXPage;
						curPageY = startYPage + ((startY + y) / page_height);
						xInPage = (startX + x) % page_width;
						yInPage = (startY + y) % page_height;
						while (curPageY > 1) {
							// Wrap
							curPageY -= 2;
							curPageX += (width / page_width);
							curStartPageX += (width / page_width);
						}
						if (pages[curPageY] == null || pages[curPageY].Length == 0) {
							pages[curPageY] = new Page[curPageX + 1];
						}
						if (pages[curPageY].Length < curPageX + 1) {
							Array.Resize(ref pages[curPageY], curPageX + 1);
						}
						if (pages[curPageY][curPageX] == null) {
							//UnityEngine.Debug.Log("Created page " + curPageX + "," + curPageY);
							pages[curPageY][curPageX] = new Page();
						}
						pages[curPageY][curPageX].SetByte(xInPage, yInPage, data[y * width + x]);
						//UnityEngine.Debug.Log(curPageX + "," + curPageY + " - " + xInPage + "," + yInPage);
					}
				}
			}
		}

		private Page GetPage(int x, int y) {
			try {
				if (x >= pages[y].Length) return null;
				return pages[y][x];
			} catch (Exception) {
				UnityEngine.Debug.LogError(x + " - " + y);
				throw;
			}
		}

		public void Export(string path) {
			Texture2D vramTex = new Texture2D(16 * page_width, 2 * page_height);
			for (int x = 0; x < 16 * page_width; x++) {
				for (int y = 0; y < 2 * page_height; y++) {
					byte val = GetPixel8(0, y / 256, x, y % 256);
					vramTex.SetPixel(x, vramTex.height - 1 - y, new Color(val / 255f, val / 255f, val / 255f));
				}
			}
			vramTex.Apply();
			Util.ByteArrayToFile(path, vramTex.EncodeToPNG());
		}

		public byte GetPixel8(int pageX, int pageY, int x, int y) {
			//UnityEngine.Debug.Log(pageX + " - " + pageY + " - " + x + " - " + y);
			//pageX -= skippedPagesX; // We're not loading backgrounds for now
			int initialX = x;
			while (x >= page_width) {
				pageX++;
				x -= page_width;
			}
			if (y > page_height) {
				pageY++;
				y -= page_height;
			}
			Page page = GetPage(pageX, pageY);
			//UnityEngine.Debug.Log(pageX + " - " + pageY + " - " + x + " - " + y);
			if (page == null) return 0;
			return page.GetByte(x, y);
		}
		public ushort GetUShort(int pageX, int pageY, int x, int y) {
			byte b0 = GetPixel8(pageX, pageY, x * 2, y);
			byte b1 = GetPixel8(pageX, pageY, (x * 2) + 1, y);
			return (ushort)(b0 | (b1 << 8));
		}
		public Color GetColor1555(int pageX, int pageY, int x, int y) {
			ushort col = GetUShort(pageX, pageY, x, y);

			float r = Util.ExtractBits(col, 5, 0) / 31f;
			float g = Util.ExtractBits(col, 5, 5) / 31f;
			float b = Util.ExtractBits(col, 5, 10) / 31f;
			float a = Util.ExtractBits(col, 1, 15);
			return new Color(r, g, b, a);
		}

		public class Page {
			public byte[] data = new byte[page_width * page_height];
			public byte GetByte(int x, int y) {
				return data[y * page_width + x];
			}
			public void SetByte(int x, int y, byte value) {
				data[y * page_width + x] = value;
			}
			/*public int width;
			public byte[] data;
			public Page(int width) {
				this.width = width;
			}
			public byte GetByte(int x, int y) {
				return data[y * width + x];
			}
			// We assume that page data is 8bit colors
			public byte[] AddData(byte[] data) {
				int curDataCount = data?.Length ?? 0;
				int newDataCount = Math.Min(curDataCount + data.Length, width * 256);
				int dataToCopy = newDataCount - curDataCount;
				if (dataToCopy > 0) {
					if (this.data == null) {
						this.data = new byte[newDataCount];
					} else {
						Array.Resize(ref this.data, newDataCount);
					}
					Array.Copy(data, 0, this.data, curDataCount, dataToCopy);
					if (data.Length > dataToCopy) {
						byte[] newData = new byte[data.Length - dataToCopy];
						Array.Copy(data, dataToCopy, newData, 0, newData.Length);
						return newData;
					} else {
						return null;
					}
				}
				return data;
			}*/
		}

		public struct ReservedBlock {
			public int x;
			public int y;
			public int width;
			public int height;
		}
	}
}