using OpenSpace.Loader;
using UnityEngine;

namespace OpenSpace.ROM {
	public class MeshElementTriangles : ROMStruct {
		public ushort length;
		public ushort num_uvs;
		public ushort[] triangles;
		public Vector2[] uvs;
		public Vector3[] colors;
		
        protected override void ReadInternal(Reader reader) {
			MapLoader.Loader.print("Triangles: " + Pointer.Current(reader) + " - " + string.Format("{0:X4}", length) + " - " + string.Format("{0:X4}", num_uvs));
			// For DS: https://github.com/scurest/apicula

			// For N64: http://www.shootersforever.com/forums_message_boards/viewtopic.php?t=6920
			// Or RSP commands sheet
			// maybe this can help https://github.com/ricrpi/mupen64plus-video-gles2rice/blob/master/src/RSP_Parser.cpp
			// 8 bytes per command, 1st byte is RSP byte

			if (Settings.s.platform == Settings.Platform._3DS) {
				triangles = new ushort[3 * length];
				for (int i = 0; i < triangles.Length; i++) {
					triangles[i] = reader.ReadUInt16();
				}
				uvs = new Vector2[num_uvs];
				colors = new Vector3[num_uvs];
				for (int i = 0; i < num_uvs; i++) {
					uvs[i] = new Vector2(reader.ReadSingle(), reader.ReadSingle());
				}
				for (int i = 0; i < num_uvs; i++) {
					// What is this???
					reader.ReadUInt32();
					reader.ReadUInt32();
					reader.ReadUInt32();
					//colors[i] = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
				}
			}
		}
	}
}
