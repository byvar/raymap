using UnityEngine;
using Raymap;
using static BinarySerializer.Ubisoft.CPA.PS1.GLI_VisualMaterial;

namespace BinarySerializer.Ubisoft.CPA.PS1 {
	public static class GLI_VisualMaterialExtensions {
		public static Material CreateMaterial(this GLI_VisualMaterial vm) {
			GLI_Texture b = vm.Texture;
			Material baseMaterial;
			SemiTransparentMode stMode = vm.BlendMode;
			var env = (Unity_Environment_CPA)vm.Context.GetUnityEnvironment();
			if (vm.IsLight || stMode == SemiTransparentMode.MinusOne) {
				baseMaterial = env.MaterialVisualLight;
			} else if ((vm?.Texture?.IsTransparent() ?? false) || stMode != SemiTransparentMode.One) {
				baseMaterial = env.MaterialVisualTransparent;
			} else {
				baseMaterial = env.MaterialVisualOpaque;
			}
			// TODO: Figure out transparency for these games
			//if (Settings.s.game == Settings.Game.JungleBook || Settings.s.game == Settings.Game.DD) {
            //	baseMaterial = MapLoader.Loader.baseMaterial;
            //}
			Material mat = new Material(baseMaterial);
			mat.SetInt("_NumTextures", 1);
			string textureName = "_Tex0";
			Texture2D tex = b.GetTexture();
			if (vm.ScrollingEnabled) tex.wrapMode = TextureWrapMode.Repeat;
			mat.SetTexture(textureName, tex);

			mat.SetVector(textureName + "Params", new Vector4(0,
				vm.ScrollingEnabled ? 1f : 0f,
				0f, 0f));
			mat.SetVector(textureName + "Params2", new Vector4(
				0f, 0f,
				vm.ScrollX, vm.ScrollY));
			mat.SetVector("_AmbientCoef", Vector4.one);
			mat.SetFloat("_Prelit", 1f);
			switch (stMode) {
				case SemiTransparentMode.MinusOne:
					mat.SetInt("_BlendOp", (int)UnityEngine.Rendering.BlendOp.ReverseSubtract);
					mat.SetInt("_SrcBlendMode", (int)UnityEngine.Rendering.BlendMode.One);
					mat.SetInt("_DstBlendMode", (int)UnityEngine.Rendering.BlendMode.One);
					break;
			}
			return mat;
		}
	}
}
