using OpenSpace.Loader;
using UnityEngine;

namespace OpenSpace.ROM {
	public class VisualMaterial : ROMStruct {
		public Reference<VisualMaterialTextures> textures;
		public ushort num_textures;
		public ushort flags;
		public ushort unk0;
		public ushort unk1;
		public uint unk2;
		public uint unk3;
		public ushort unk4;

        protected override void ReadInternal(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			unk0 = reader.ReadUInt16();
			unk1 = reader.ReadUInt16();
			unk2 = reader.ReadUInt32();
			unk3 = reader.ReadUInt32();
			textures = new Reference<VisualMaterialTextures>(reader);
			num_textures = reader.ReadUInt16();
			textures.Resolve(reader, (vmt) => { vmt.length = num_textures; });
			unk4 = reader.ReadUInt16();
			flags = reader.ReadUInt16();
        }

		public Material Mat {
			get {
				Material mat;
				if (textures.Value != null && num_textures > 0) {
					if (textures.Value.vmTex[0].texRef.Value.texInfo.Value.IsTransparent) {
						mat = new Material(MapLoader.Loader.baseTransparentMaterial);
					} else {
						mat = new Material(MapLoader.Loader.baseMaterial);
					}
					mat.SetInt("_NumTextures", 1);
					mat.SetTexture("_Tex0", textures.Value.vmTex[0].texRef.Value.texInfo.Value.Texture);
				} else {
					mat = new Material(MapLoader.Loader.baseMaterial);
				}
				mat.SetVector("_AmbientCoef", Vector4.one);
				mat.SetVector("_DiffuseCoef", Vector4.one);
				return mat;
			}
		}
    }
}
