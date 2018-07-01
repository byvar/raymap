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
            EndianBinaryReader r = new EndianBinaryReader(stream, isLittleEndian);

            if(l.mode != MapLoader.Mode.Rayman2IOS) r.ReadInt32(); // Signature

            width = r.ReadUInt32();
            height = r.ReadUInt32();
            channelPixels = width * height;

            byte channels = r.ReadByte();
            byte enlargeByte = 0;
            if (Settings.s.engineMode == Settings.EngineMode.R3) enlargeByte = r.ReadByte();
            uint w = width, h = height;
            if (enlargeByte > 0) channelPixels = 0;
            for (int i = 0; i < enlargeByte; i++) {
                channelPixels += (w * h);
                w /= 2;
                h /= 2;
            }

            pixels = new Color[width * height];
            byte repeatByte = r.ReadByte();

            byte[] blue_channel = ReadChannel(r, repeatByte, channelPixels);
            byte[] green_channel = ReadChannel(r, repeatByte, channelPixels);
            byte[] red_channel = ReadChannel(r, repeatByte, channelPixels);
            byte[] alpha_channel = null;
            if (channels == 4) {
                alpha_channel = ReadChannel(r, repeatByte, channelPixels);
                isTransparent = true;
            }
            for (int i = 0; i < width * height; i++) {
                if (isTransparent) {
                    pixels[i] = new Color(red_channel[i] / 255f, green_channel[i] / 255f, blue_channel[i] / 255f, alpha_channel[i] / 255f);
                } else {
                    float alphaValue = 1f;
                    if (red_channel[i] == 0 && green_channel[i] == 0 && blue_channel[i] == 0) alphaValue = 0f;
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

        byte[] ReadChannel(EndianBinaryReader r, byte repeatByte, uint pixels) {
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
    }
}