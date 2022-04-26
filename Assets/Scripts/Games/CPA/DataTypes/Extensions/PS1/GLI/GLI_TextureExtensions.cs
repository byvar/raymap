using BinarySerializer.Unity;
using Raymap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static BinarySerializer.Ubisoft.CPA.PS1.GLI_VisualMaterial;

namespace BinarySerializer.Ubisoft.CPA.PS1 {
	public static class GLI_TextureExtensions {
		public static bool IsTransparent(this GLI_Texture tb) {
			return tb.Context.GetUnityLevel().GetUnityData<GLI_UnityTexture, GLI_Texture>(tb).IsTransparent;
		}

		public static Texture2D GetTexture(this GLI_Texture tb) {
			return tb.Context.GetUnityLevel().GetUnityData<GLI_UnityTexture, GLI_Texture>(tb).Texture;
		}
	}
}
