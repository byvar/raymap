using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VrSharp.PvrTexture;

namespace OpenSpace.FileFormat.RenderWare {
	// R2 PS2 frame format
	// https://gtamods.com/wiki/Frame_List_(RW_Section)
	public class Frame {
		public Matrix matrix;
		public int index;
		public uint flags;
	}
}