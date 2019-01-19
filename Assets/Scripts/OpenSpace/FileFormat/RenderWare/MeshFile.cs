using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VrSharp.PvrTexture;

namespace OpenSpace.FileFormat.RenderWare {
	// R2 PS2 mesh file
	// https://gtamods.com/wiki/RenderWare_binary_stream_file
	public class MeshFile {
		public Section root;

		public uint Count { get; } = 0;

		public MeshFile(string path) {
			Stream fs = FileSystem.GetFileReadStream(path);
			using (Reader reader = new Reader(fs, Settings.s.IsLittleEndian)) {
				root = Section.Read(reader);
			}
			if (root != null && root.type == Section.Type.Clump) {
			}
		}
	}
}