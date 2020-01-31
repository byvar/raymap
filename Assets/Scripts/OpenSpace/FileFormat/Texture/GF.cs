// Adapted from Rayman2Lib by szymski
// https://github.com/szymski/Rayman2Lib/blob/master/d_tools/rayman2lib/source/formats/gf.d

using System;
using System.IO;
using UnityEngine;
/*
GF Header:
4 bytes - signature
4 bytes - width
4 bytes - height
1 byte  - channel count
1 byte  - repeat byte

Now, we need to read the channels

Channel:
For each pixel (width*height):
1 byte - color value

If color value 1 equals repeat byte from header, we read more values:
1 byte - color value
1 byte - repeat count

Otherwise:
Channel pixel = color value

*/

namespace OpenSpace.FileFormat.Texture {
    public class GF {
        public uint width, height;
        public byte channels;
        public byte repeatByte;
        public uint format;
        public uint channelPixels;
        public byte byte1;
        public byte byte2;
        public byte byte3;
        public uint num4;
        public bool isTransparent = false;
        public bool isLittleEndian = true;
        public byte montrealType;
        public ushort paletteNumColors;
        public byte paletteBytesPerColor;
        public byte[] palette = null;
        public Color[] pixels;

        public GF(byte[] bytes) : this(new MemoryStream(bytes)) {}
        /*public GF(byte[] bytes) {
            Util.ByteArrayToFile(MapLoader.Loader.gameDataBinFolder + "hi" + bytes.Length + ".lol", bytes);
            GF gf = new GF(new MemoryStream(bytes));
            pixels = gf.pixels;
            throw new Exception("exported");
        }*/

        public GF(string filePath) : this(FileSystem.GetFileReadStream(filePath)) { }

        public GF(Stream stream) {
            MapLoader l = MapLoader.Loader;
            Reader reader = new Reader(stream, isLittleEndian);
            if (Settings.s.engineVersion == Settings.EngineVersion.Montreal) {
                byte version = reader.ReadByte();
                format = 1555;
            } else {
                format = 8888;
                if (Settings.s.platform != Settings.Platform.iOS && Settings.s.game != Settings.Game.TTSE) format = reader.ReadUInt32();
            }

            width = reader.ReadUInt32();
            height = reader.ReadUInt32();
            channelPixels = width * height;

            channels = reader.ReadByte();
            byte enlargeByte = 0;
            if (Settings.s.engineVersion == Settings.EngineVersion.R3 && Settings.s.game != Settings.Game.Dinosaur && Settings.s.game != Settings.Game.LargoWinch) enlargeByte = reader.ReadByte();
			if (enlargeByte > 1) {
                uint w = width, h = height, e = enlargeByte;
                if (w != 1) w >>= 1;
                if (h != 1) h >>= 1;
                while(e > 1) {
					channelPixels += (w * h);
                    if (w != 1) w >>= 1;
                    if (h != 1) h >>= 1;
                    e --;
                }
			}
            repeatByte = reader.ReadByte();
            if (Settings.s.engineVersion == Settings.EngineVersion.Montreal) {
                paletteNumColors = reader.ReadUInt16();
                paletteBytesPerColor = reader.ReadByte();
                byte1 = reader.ReadByte();
                byte2 = reader.ReadByte();
                byte3 = reader.ReadByte();
                num4 = reader.ReadUInt32();
                channelPixels = reader.ReadUInt32(); // Hype has mipmaps
                montrealType = reader.ReadByte();
                if (paletteNumColors != 0 && paletteBytesPerColor != 0) {
                    palette = reader.ReadBytes(paletteBytesPerColor * paletteNumColors);
                }
                switch (montrealType) {
                    case 5: format = 0; break; // palette
                    case 10: format = 565; break; // unsure
                    case 11: format = 1555; break;
                    case 12: format = 4444; break; // unsure
                    default: throw new Exception("unknown Montreal GF format " + montrealType + "!");
                }
            }

            pixels = new Color[width * height];
            if (Settings.s.engineVersion == Settings.EngineVersion.R3 && channels == 1) {
                paletteBytesPerColor = 4;
                paletteNumColors = 256;
                palette = reader.ReadBytes(paletteBytesPerColor * paletteNumColors);
            }
            byte[] pixelData = ReadChannels(reader);

            if (channels >= 3) {
                if (channels == 4) isTransparent = true;
                uint pos = 0;
                for (int i = 0; i < width * height; i++) {
                    byte b = pixelData[pos + 0];
                    byte g = pixelData[pos + 1];
                    byte r = pixelData[pos + 2];
                    if (channels == 4) {
                        byte a = pixelData[pos + 3];
                        pixels[i] = new Color(r / 255f, g / 255f, b / 255f, a / 255f);
                    } else {
                        float alphaValue = 1f;
                        //if (red_channel[i] == 0 && green_channel[i] == 0 && blue_channel[i] == 0) alphaValue = 0f;
                        pixels[i] = new Color(r / 255f, g / 255f, b / 255f, alphaValue);
                    }
                    pos += channels;
                }
            } else if (channels == 2) {
                if (format == 1555 || format == 4444) isTransparent = true;

                uint pos = 0;
                for (int i = 0; i < width * height; i++) {
                    ushort pixel = BitConverter.ToUInt16(new byte[] { pixelData[pos], pixelData[pos + 1] }, 0); // RRRRR, GGGGGG, BBBBB (565)
                    uint r, g, b, a;
                    switch (format) {
                        case 88:
                            pixels[i] = new Color(
                                pixelData[pos] / 255,
                                pixelData[pos] / 255,
                                pixelData[pos] / 255,
                                pixelData[pos + 1] / 255);
                            break;
                        case 4444:
                            a = extractBits(pixel, 4, 12);
                            r = extractBits(pixel, 4, 8);
                            g = extractBits(pixel, 4, 4);
                            b = extractBits(pixel, 4, 0);

                            pixels[i] = new Color(
                                (r / 15.0f),
                                (g / 15.0f),
                                (b / 15.0f),
                                (a / 15.0f));
                            break;
                        case 1555:
                            /*
                            alpha = extractBits(pixel, 1, 15);
                            red = extractBits(pixel, 5, 10);
                            green = extractBits(pixel, 5, 5);
                            blue = extractBits(pixel, 5, 0);
							*/
                            a = extractBits(pixel, 1, 15);
                            r = extractBits(pixel, 5, 10);
                            g = extractBits(pixel, 5, 5);
                            b = extractBits(pixel, 5, 0);

                            pixels[i] = new Color(
                                (r / 31.0f),
                                (g / 31.0f),
                                (b / 31.0f),
                                a);
                            break;
                        case 565:
                        default: // 565
                            r = extractBits(pixel, 5, 11);
                            g = extractBits(pixel, 6, 5);
                            b = extractBits(pixel, 5, 0);

                            pixels[i] = new Color(
                                (r / 31.0f),
                                (g / 63.0f),
                                (b / 31.0f),
                                1f);
                            break;
                    }
                    pos += channels;
                }
            } else if (channels == 1) {
                if (palette != null && paletteBytesPerColor == 4) isTransparent = true;
                for (int i = 0; i < width * height; i++) {
                    byte r, g, b;
                    byte a = 255;
                    if (palette != null) {
                        if (isTransparent) a = palette[pixelData[i] * paletteBytesPerColor + 3];
                        r = palette[pixelData[i] * paletteBytesPerColor + 2];
                        g = palette[pixelData[i] * paletteBytesPerColor + 1];
                        b = palette[pixelData[i] * paletteBytesPerColor + 0];
                    } else {
                        if (isTransparent) a = pixelData[i];
                        r = pixelData[i];
                        g = pixelData[i];
                        b = pixelData[i];
                    }
                    pixels[i] = new Color(r / 255f, g / 255f, b / 255f, a / 255f);
                }
            }


            if (reader.BaseStream.Position != reader.BaseStream.Length) {
                Debug.LogError("Assertion failed: GF not fully read! Remaining bytes: " + (reader.BaseStream.Length - reader.BaseStream.Position));
            }
            /*if (r.BaseStream.Position != r.BaseStream.Length) {
                Debug.LogError((r.BaseStream.Length - r.BaseStream.Position));
                r.BaseStream.Position = 0;
                byte[] bytes = r.ReadBytes((int)r.BaseStream.Length);
                Util.ByteArrayToFile(MapLoader.Loader.gameDataBinFolder + "hi" + bytes.Length + ".lol", bytes);
                throw new Exception("exported");
            }*/
            reader.Close();
        }

        byte[] ReadChannels(Reader reader) {
            byte[] data = new byte[channels * channelPixels];
            int channel = 0;
            while(channel < channels) {
                int pixel = 0;
                while (pixel < channelPixels) {
                    byte b1 = reader.ReadByte();
                    if (b1 == repeatByte) {
                        byte value = reader.ReadByte();
                        byte count = reader.ReadByte();

                        for (int i = 0; i < count; ++i) {
                            data[channel + pixel * channels] = value;
                            pixel++;
                        }
                    } else {
                        data[channel + pixel * channels] = b1;
                        pixel++;
                    }
                }
                channel++;
            }
            return data;
        }

        byte[] ReadChannel(Reader r, byte repeatByte, uint pixels) {
            byte[] channel = new byte[pixels];

            int pixel = 0;

            while (pixel < pixels) {
                byte b1 = r.ReadByte();
                if (b1 == repeatByte) {
                    byte value = r.ReadByte();
                    byte count = r.ReadByte();

                    for (int i = 0; i < count; ++i) {
                        if (pixel < pixels) {
                            channel[pixel] = value;
                        } else {
                            Debug.LogError("outside bounds: " + channels);
                        }
                        pixel++;
                    }
                } else {
                    channel[pixel] = b1;
                    pixel++;
                }
            }

            return channel;
        }

        public Texture2D GetTexture() {
            Texture2D tex = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        static uint extractBits(int number, int count, int offset) {
            return (uint)(((1 << count) - 1) & (number >> (offset)));
        }
    }
}