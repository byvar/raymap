using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace {
    public static class Util {
        public static byte[] GetBytesInEndian(byte[] array, int position, int length, bool isLittleEndian) {
            byte[] data = new byte[length];
            Array.Copy(array, position, data, 0, length);
            if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
            return data;
        }

        public static uint ToUInt32(byte[] array, int position, bool isLittleEndian) {
            return BitConverter.ToUInt32(GetBytesInEndian(array, position, 4, isLittleEndian), 0);
        }
        public static int ToInt32(byte[] array, int position, bool isLittleEndian) {
            return BitConverter.ToInt32(GetBytesInEndian(array, position, 4, isLittleEndian), 0);
        }
        public static ushort ToUInt16(byte[] array, int position, bool isLittleEndian) {
            return BitConverter.ToUInt16(GetBytesInEndian(array, position, 2, isLittleEndian), 0);
        }
        public static short ToInt16(byte[] array, int position, bool isLittleEndian) {
            return BitConverter.ToInt16(GetBytesInEndian(array, position, 2, isLittleEndian), 0);
        }
        public static int ExtractBits(int value, int count, int offset) {
            return (((1 << count) - 1) & (value >> (offset)));
        }

        public static int SetBits(int bits, int value, int count, int offset) {
            int mask = ((1 << count) - 1) << offset;
            bits = (bits & ~mask) | (value << offset);
            return bits;
        }

        public static void AppendArrayAndMergeReferences<T>(ref T[] array1, ref T[] array2, int originalLength = -1) {
            if (array1 == null || array2 == null) return;
            if(originalLength == -1) originalLength = array1.Length;
            Array.Resize<T>(ref array1, originalLength + array2.Length);
            Array.Copy(array2, 0, array1, originalLength, array2.Length);
            array2 = array1;
        }

        public static bool ByteArrayToFile(string fileName, byte[] byteArray) {
			if (byteArray == null) return false;
            if (FileSystem.mode == FileSystem.Mode.Web) return false;
            try {
                Directory.CreateDirectory(new FileInfo(fileName).Directory.FullName);
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write)) {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            } catch (Exception ex) {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }

        public static Mesh CreateBox(float sz) {
            Mesh mesh = new Mesh();

            float length = sz;
            float width = sz;
            float height = sz;
            
            Vector3 p0 = new Vector3(-length * .5f, -width * .5f, height * .5f);
            Vector3 p1 = new Vector3(length * .5f, -width * .5f, height * .5f);
            Vector3 p2 = new Vector3(length * .5f, -width * .5f, -height * .5f);
            Vector3 p3 = new Vector3(-length * .5f, -width * .5f, -height * .5f);
            Vector3 p4 = new Vector3(-length * .5f, width * .5f, height * .5f);
            Vector3 p5 = new Vector3(length * .5f, width * .5f, height * .5f);
            Vector3 p6 = new Vector3(length * .5f, width * .5f, -height * .5f);
            Vector3 p7 = new Vector3(-length * .5f, width * .5f, -height * .5f);

            Vector3[] vertices = new Vector3[] {
	            p0, p1, p2, p3,
	            p7, p4, p0, p3,
	            p4, p5, p1, p0,
	            p6, p7, p3, p2,
	            p5, p6, p2, p1,
	            p7, p6, p5, p4
            };

            Vector3 up = Vector3.up;
            Vector3 down = Vector3.down;
            Vector3 front = Vector3.forward;
            Vector3 back = Vector3.back;
            Vector3 left = Vector3.left;
            Vector3 right = Vector3.right;

            Vector3[] normales = new Vector3[] {
	            down, down, down, down,
	            left, left, left, left,
	            front, front, front, front,
	            back, back, back, back,
	            right, right, right, right,
	            up, up, up, up
            };
            
            Vector2 _00 = new Vector2(0f, 0f);
            Vector2 _10 = new Vector2(1f, 0f);
            Vector2 _01 = new Vector2(0f, 1f);
            Vector2 _11 = new Vector2(1f, 1f);

            Vector2[] uvs = new Vector2[]
            {
	            _11, _01, _00, _10,
	            _11, _01, _00, _10,
	            _11, _01, _00, _10,
	            _11, _01, _00, _10,
	            _11, _01, _00, _10,
	            _11, _01, _00, _10,
            };
            
            int[] triangles = new int[] {
                3, 1, 0,
                3, 2, 1,
                3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
                3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
                3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
                3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
                3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
                3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
                3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
                3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
                3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
                3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
            };

            mesh.vertices = vertices;
            mesh.normals = normales;
            mesh.uv = uvs;
            mesh.triangles = triangles;

            mesh.RecalculateBounds();
            return mesh;
        }


        public static Texture2D CreateDummyTexture() {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f));
            texture.Apply();
            return texture;
        }

        public static Texture2D CreateDummyCheckerTexture() {
            Texture2D texture = new Texture2D(2, 2);
            Color col1 = Color.white;
            Color col2 = new Color(0.9f, 0.9f, 0.9f, 1f); // very light grey
            texture.SetPixel(0, 0, col1);
            texture.SetPixel(1, 1, col1);
            texture.SetPixel(0, 1, col2);
            texture.SetPixel(1, 0, col2);
            texture.filterMode = FilterMode.Point;
            texture.Apply();
            return texture;
        }

        public static Texture2D CreateDummyLineTexture() {
            Texture2D texture = new Texture2D(2, 2);
            Color col1 = Color.white;
            Color col2 = new Color(0.9f, 0.9f, 0.9f, 1f); // very light grey
            texture.SetPixel(0, 0, col1);
            texture.SetPixel(1, 1, col2);
            texture.SetPixel(0, 1, col1);
            texture.SetPixel(1, 0, col2);
            texture.filterMode = FilterMode.Point;
            texture.Apply();
            return texture;
        }

		private static readonly string[] SizeSuffixes =
				  { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
		public static string SizeSuffix(Int64 value, int decimalPlaces = 1) {
			if (value < 0) { return "-" + SizeSuffix(-value); }

			int i = 0;
			decimal dValue = (decimal)value;
			while (Math.Round(dValue, decimalPlaces) >= 1000) {
				dValue /= 1024;
				i++;
			}

			return string.Format("{0:n" + decimalPlaces + "} {1}", dValue, SizeSuffixes[i]);
		}
    }
}
