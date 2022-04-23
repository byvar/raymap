using BinarySerializer.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static BinarySerializer.Ubisoft.CPA.PS1.GLI_VisualMaterial;

namespace BinarySerializer.Ubisoft.CPA.PS1 {
	public static class GLI_TextureBoundsExtensions {
		public static bool IsTransparent(this GLI_TextureBounds tb) => false;//tb.texture.IsTransparent();
	}
}
