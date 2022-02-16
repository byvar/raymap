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
			if (y >= page_height) {
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
		public Texture2D GetTexture(ushort width, ushort height, ushort texturePageInfo, ushort paletteInfo, int xInPage, int yInPage) {
			// see http://hitmen.c02.at/files/docs/psx/psx.pdf page 37
			int pageX = Util.ExtractBits(texturePageInfo, 4, 0);
			int pageY = Util.ExtractBits(texturePageInfo, 1, 4);
			int abr = Util.ExtractBits(texturePageInfo, 2, 5);
			int tp = Util.ExtractBits(texturePageInfo, 2, 7); // 0: 4-bit, 1: 8-bit, 2: 15-bit direct
			/*int dtd = Util.ExtractBits(texturePageInfo, 1, 9);
			int dfe = Util.ExtractBits(texturePageInfo, 1, 10);
			int md = Util.ExtractBits(texturePageInfo, 1, 11);
			int me = Util.ExtractBits(texturePageInfo, 1, 12);
			MapLoader.Loader.print(abr + " - " + dtd + " - " + dfe + " - " + md + " - " + me);*/

			/*
			 * abr 00 0.5xB +  0.5 x F Semi transparent state
			 *     01 1.0xB +  1.0 x F
			 *     10 1.0xB -  1.0 x F
			 *     11 1.0xB + 0.25 x F
			 */

			if (pageX < 5)
				return null;

			// Get palette coordinates
			int paletteX = Util.ExtractBits(paletteInfo, 6, 0) * 16;
			int paletteY = Util.ExtractBits(paletteInfo, 10, 6);

			//Debug.Log((paletteX*2) + " - " + paletteY + " - " + pageX + " - " + pageY + " - " + tp);

			// Get the palette size
			Color?[] palette = tp == 0 ? new Color?[16] : new Color?[256];

			// Create the texture
			Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
			tex.SetPixels(Enumerable.Repeat(Color.clear, width * height).ToArray());

			if (tp == 1) {
				for (int y = 0; y < height; y++) {
					for (int x = 0; x < width; x++) {
						var paletteIndex = GetPixel8(pageX, pageY, xInPage + x, yInPage + y);

						// Get the color from the palette
						if (!palette[paletteIndex].HasValue) {
							palette[paletteIndex] = GetColor1555(0, 0, paletteX + paletteIndex, paletteY);
							if (Legacy_Settings.s.game == Legacy_Settings.Game.DD || Legacy_Settings.s.game == Legacy_Settings.Game.JungleBook) {
								Color c = palette[paletteIndex].Value;
								if (c.r == 0 && c.g == 0 && c.b == 0 && c.a == 0) {
									palette[paletteIndex] = new Color(c.r, c.g, c.b, 0f);
								} else {
									palette[paletteIndex] = new Color(c.r, c.g, c.b, 1f);
								}
							}
						}
						/*var palettedByte0 = vram.GetPixel8(0, 0, paletteX * 16 + paletteIndex, paletteY);
                        var palettedByte1 = vram.GetPixel8(0, 0, paletteX * 16 + paletteIndex + 1, paletteY);
                        var color = palette[paletteIndex];*/

						// Set the pixel
						//palette[paletteIndex] = new Color(palette[paletteIndex].r, palette[paletteIndex].g, palette[paletteIndex].b, 1f);
						tex.SetPixel(x, height - 1 - y, palette[paletteIndex].Value);
					}
				}
			} else if (tp == 0) {
				if (xInPage % 2 != 0) xInPage--;
				for (int y = 0; y < height; y++) {
					for (int x = 0; x < width; x++) {
						var paletteIndex = GetPixel8(pageX, pageY, (xInPage + x) / 2, yInPage + y);
						if (x % 2 == 0)
							paletteIndex = (byte)Util.ExtractBits(paletteIndex, 4, 0);
						else
							paletteIndex = (byte)Util.ExtractBits(paletteIndex, 4, 4);


						// Get the color from the palette
						if (!palette[paletteIndex].HasValue) {
							palette[paletteIndex] = GetColor1555(0, 0, paletteX + paletteIndex, paletteY);

							if (Legacy_Settings.s.game == Legacy_Settings.Game.DD || Legacy_Settings.s.game == Legacy_Settings.Game.JungleBook) {
								Color c = palette[paletteIndex].Value;
								if (c.r == 0 && c.g == 0 && c.b == 0 && c.a == 0) {
									palette[paletteIndex] = new Color(c.r, c.g, c.b, 0f);
								} else {
									palette[paletteIndex] = new Color(c.r, c.g, c.b, 1f);
								}
							}
						}
						/*var palettedByte0 = vram.GetPixel8(0, 0, paletteX * 16 + paletteIndex, paletteY);
                        var palettedByte1 = vram.GetPixel8(0, 0, paletteX * 16 + paletteIndex + 1, paletteY);*/

						// Set the pixel
						//palette[paletteIndex] = new Color(palette[paletteIndex].r, palette[paletteIndex].g, palette[paletteIndex].b, 1f);
						tex.SetPixel(x, height - 1 - y, palette[paletteIndex].Value);
					}
				}
			}
			tex.Apply();

			return tex;
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
		public enum PixelMode {
			Ushort = 1,
			Byte = 2,
			Nibble = 3
		}
	}
}