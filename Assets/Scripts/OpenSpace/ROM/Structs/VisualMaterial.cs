using OpenSpace.Loader;
using UnityEngine;

namespace OpenSpace.ROM {
	public class VisualMaterial : ROMStruct {
		public Reference<VisualMaterialTextures> textures;
		public ushort num_textures;
		public ushort flags;

        protected override void ReadInternal(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			reader.ReadUInt16();
			reader.ReadUInt16();
			reader.ReadUInt32();
			reader.ReadUInt32();
			textures = new Reference<VisualMaterialTextures>(reader);
			num_textures = reader.ReadUInt16();
			textures.Resolve(reader, (vmt) => { vmt.length = num_textures; });
			reader.ReadUInt16();
			flags = reader.ReadUInt16();
        }

		public Material Mat {
			get {
				Material mat = new Material(MapLoader.Loader.baseMaterial);
				if (textures.Value != null && num_textures > 0) {
					mat.SetInt("_NumTextures", 1);
					mat.SetTexture("_Tex0", textures.Value.vmTex[0].texRef.Value.texInfo.Value.Texture);
				}
				mat.SetVector("_AmbientCoef", Vector4.one);
				mat.SetVector("_DiffuseCoef", Vector4.one);
				return mat;
			}
		}
    }
}
