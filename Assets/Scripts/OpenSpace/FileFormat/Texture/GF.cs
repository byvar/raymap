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
        public uint format;
        public uint channelPixels;
        public bool isTransparent = false;
        public bool isLittleEndian = true;
        public Color[] pixels;

        public GF(byte[] bytes) : this(new MemoryStream(bytes)) { }
        /*public GF3(byte[] bytes) {
            ByteArrayToFile("hi.lol", bytes);
        }*/

        /*public bool ByteArrayToFile(string fileName, byte[] byteArray) {
            try {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write)) {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            } catch (Exception ex) {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }*/

        public GF(string filePath) : this(File.OpenRead(filePath)) { }

        public GF(Stream stream) {
            MapLoader l = MapLoader.Loader;
            Reader r = new Reader(stream, isLittleEndian);

            format = 8888;
            if(Settings.s.platform != Settings.Platform.iOS) format = r.ReadUInt32();

            width = r.ReadUInt32();
            height = r.ReadUInt32();
            channelPixels = width * height;

            byte channels = r.ReadByte();
            byte enlargeByte = 0;
            if (Settings.s.engineVersion == Settings.EngineVersion.R3) enlargeByte = r.ReadByte();
            uint w = width, h = height;
            if (enlargeByte > 0) channelPixels = 0;
            for (int i = 0; i < enlargeByte; i++) {
                channelPixels += (w * h);
                w /= 2;
                h /= 2;
            }

            pixels = new Color[width * height];
            byte repeatByte = r.ReadByte();
            byte[] blue_channel = null, green_channel = null, red_channel = null, alpha_channel = null;

            if (channels >= 3) {
                blue_channel = ReadChannel(r, repeatByte, channelPixels);
                green_channel = ReadChannel(r, repeatByte, channelPixels);
                red_channel = ReadChannel(r, repeatByte, channelPixels);
                if (channels == 4) {
                    alpha_channel = ReadChannel(r, repeatByte, channelPixels);
                    isTransparent = true;
                }
            } else if (channels == 2) {
                byte[] channel_1 = ReadChannel(r, repeatByte, channelPixels);
                byte[] channel_2 = ReadChannel(r, repeatByte, channelPixels);

                red_channel = new byte[channelPixels];
                green_channel = new byte[channelPixels];
                blue_channel = new byte[channelPixels];
                alpha_channel = new byte[channelPixels];
                if (format == 1555) isTransparent = true;

                for (int i = 0; i < channelPixels; i++) {
                    ushort pixel = BitConverter.ToUInt16(new byte[] { channel_1[i], channel_2[i] }, 0); // RRRRR, GGGGGG, BBBBB (565)
                    uint red, green, blue, alpha;
                    switch (format) {
                        case 88:
                            alpha_channel[i] = channel_2[i];
                            red_channel[i] = channel_1[i];
                            blue_channel[i] = channel_1[i];
                            green_channel[i] = channel_1[i];
                            break;
                        case 4444:
                            alpha = extractBits(pixel, 4, 12);
                            blue = extractBits(pixel, 4, 8);
                            green = extractBits(pixel, 4, 4);
                            red = extractBits(pixel, 4, 0);
                            red_channel[i] = (byte)((red / 15.0f) * 255.0f);
                            green_channel[i] = (byte)((green / 15.0f) * 255.0f);
                            blue_channel[i] = (byte)((blue / 15.0f) * 255.0f);
                            alpha_channel[i] = (byte)((alpha / 15.0f) * 255.0f);
                            break;
                        case 1555:
                            alpha = extractBits(pixel, 1, 15);
                            red = extractBits(pixel, 5, 10);
                            green = extractBits(pixel, 5, 5);
                            blue = extractBits(pixel, 5, 0);

                            red_channel[i] = (byte)((red / 31.0f) * 255.0f);
                            green_channel[i] = (byte)((green / 31.0f) * 255.0f);
                            blue_channel[i] = (byte)((blue / 31.0f) * 255.0f);
                            alpha_channel[i] = (byte)(alpha * 255);
                            break;
                        case 565:
                        default: // 565
                            red = extractBits(pixel, 5, 11);
                            green = extractBits(pixel, 6, 5);
                            blue = extractBits(pixel, 5, 0);

                            red_channel[i] = (byte)((red / 31.0f) * 255.0f);
                            green_channel[i] = (byte)((green / 63.0f) * 255.0f);
                            blue_channel[i] = (byte)((blue / 31.0f) * 255.0f);
                            break;
                    }
                }
            }
            for (int i = 0; i < width * height; i++) {
                if (isTransparent) {
                    pixels[i] = new Color(red_channel[i] / 255f, green_channel[i] / 255f, blue_channel[i] / 255f, alpha_channel[i] / 255f);
                } else {
                    float alphaValue = 1f;
                    //if (red_channel[i] == 0 && green_channel[i] == 0 && blue_channel[i] == 0) alphaValue = 0f;
                    pixels[i] = new Color(red_channel[i] / 255f, green_channel[i] / 255f, blue_channel[i] / 255f, alphaValue);
                }
            }
            /*for (int y = 0; y < height / 2; y++) {
                for (int x = 0; x < width / 2; x++) {
                    Color temp = pixels[y * width + x];
                    pixels[y * width + x] = pixels[(height - 1 - y) * width + x];
                    pixels[(height - 1 - y) * width + x] = temp;
                }
            }*/
            r.Close();
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
                        if (pixel < pixels) channel[pixel] = value;
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