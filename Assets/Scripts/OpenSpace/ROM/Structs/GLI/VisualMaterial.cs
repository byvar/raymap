using OpenSpace.Loader;
using UnityEngine;

namespace OpenSpace.ROM {
	public class VisualMaterial : ROMStruct {
		public Reference<VisualMaterialTextures> textures;
		public ushort num_textures;
		public ushort flags;
		public ushort unk0;
		public ushort unk1;
		public float scrollSpeedX;
		public float scrollSpeedY;
		public ushort num_animTextures;

        protected override void ReadInternal(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			unk0 = reader.ReadUInt16();
			unk1 = reader.ReadUInt16();
			scrollSpeedX = reader.ReadSingle();
			scrollSpeedY = reader.ReadSingle();
			textures = new Reference<VisualMaterialTextures>(reader);
			num_textures = reader.ReadUInt16();
			textures.Resolve(reader, (vmt) => { vmt.length = num_textures; });
			num_animTextures = reader.ReadUInt16();
			flags = reader.ReadUInt16();
        }

		public Material GetMaterial(Hint hints, GameObject gao = null) {
			Material mat;
			bool billboard = (hints & Hint.Billboard) == Hint.Billboard;
			if (textures.Value != null && num_textures > 0) {
				TextureInfo texInfo = textures.Value.vmTex[0].texRef.Value.texInfo;
				if (texInfo.RenderTransparent || texInfo.RenderWater1 || texInfo.RenderWater2) {
					if (texInfo.AlphaIsTransparency || texInfo.RenderWater1 || texInfo.RenderWater2) {
						mat = new Material(MapLoader.Loader.baseTransparentMaterial);
					} else {
						mat = new Material(MapLoader.Loader.baseLightMaterial);
					}
				} else {
					mat = new Material(MapLoader.Loader.baseMaterial);
				}
				mat.SetInt("_NumTextures", 1);
				string textureName = "_Tex0";
				mat.SetTexture(textureName, textures.Value.vmTex[0].texRef.Value.texInfo.Value.Texture);
				mat.SetVector(textureName + "Params", new Vector4(0,
					(scrollSpeedX != 0 || scrollSpeedY != 0) ? 1f : 0f,
					0f, 0f));
				mat.SetVector(textureName + "Params2", new Vector4(
					0f, 0f, ScrollX, ScrollY));
			} else {
				mat = new Material(MapLoader.Loader.baseMaterial);
			}
			mat.SetVector("_AmbientCoef", Vector4.one);
			mat.SetVector("_DiffuseCoef", Vector4.one);
			if (billboard) mat.SetFloat("_Billboard", 1f);
			if (gao != null && num_textures > 1) {
				MultiTextureMaterial mtmat = gao.AddComponent<MultiTextureMaterial>();
				mtmat.visMatROM = this;
				mtmat.mat = mat;
			}
			return mat;
		}

		public float ScrollX {
			get {
				return scrollSpeedX * Mathf.Abs(Settings.s.textureAnimationSpeedModifier);
			}
		}
		public float ScrollY {
			get {
				return scrollSpeedY * Settings.s.textureAnimationSpeedModifier;
			}
		}

		public static ushort flags_renderBackFaces = 0x0100;

		public bool RenderBackFaces {
			get { return (flags & flags_renderBackFaces) != 0; }
		}

		public enum Hint {
			None = 0,
			Transparent = 1,
			Billboard = 2
		}
	}
}
