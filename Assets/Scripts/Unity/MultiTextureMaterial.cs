using OpenSpace;
using OpenSpace.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MultiTextureMaterial : MonoBehaviour {
    public VisualMaterial visMat;
	public OpenSpace.ROM.VisualMaterial visMatROM;
    public Material mat;
    public string[] textureNames = { "Placeholder" };
    int currentTexture = 0;
    public int textureIndex = 0;
	public int CurrentTextureROM { get; set; }
	public float CurrentTextureROMTime { get; set; }
    public bool animate = true;
    //float currentTime = 0f;

    public void Start() {
		MultiTextureManager mtm = MapLoader.Loader.controller.GetComponent<MultiTextureManager>();
		if (mtm != null) {
			mtm.Register(this);
		}
		if (visMat != null) {
			textureNames = visMat.animTextures.Select(a => (a == null || a.texture == null) ? "Null" : a.texture.name).ToArray();
			SetTexture(visMat.currentAnimTexture);
		} else if (visMatROM != null) {
			textureNames = visMatROM.textures.Value.vmTex.Select(a => (a.texRef.Value == null || a.texRef.Value.texInfo.Value == null)
			? "Null"
			: (a.texRef.Value.texInfo.Value.name ?? "TexInfo " + a.texRef.Value.texInfo.index)).ToArray();
			SetTexture(0);
		}
    }

    public void SetTexture(int index) {
		if (visMat != null) {
			if (index < 0 || index > visMat.animTextures.Count) return;
			textureIndex = index;
			currentTexture = index;
			TextureInfo tex = visMat.animTextures[index].texture;
			if (tex != null) {
				mat.SetTexture("_Tex0", tex.Texture);
			}
		} else if (visMatROM != null) {
			if (index < 0 || index > visMatROM.num_textures) return;
			textureIndex = index;
			currentTexture = index;
			OpenSpace.ROM.TextureInfo tex = visMatROM.textures.Value.vmTex[index].texRef.Value.texInfo.Value;
			if (tex != null) {
				mat.SetTexture("_Tex0", tex.Texture);
			}
		}
    }

    public void LateUpdate() {
		if (animate) {
			if (visMat != null) {
				textureIndex = visMat.currentAnimTexture;
			} else if (visMatROM != null) {
				textureIndex = CurrentTextureROM;
				//textureIndex = visMatROM..currentAnimTexture;
			}
		}
        /*if (animate && !visMat.IsLockedAnimatedTexture) {
            if (textureIndex >= 0 && textureIndex < visMat.animTextures.Count) {
                currentTime += Time.deltaTime * Mathf.Abs(Settings.s.textureAnimationSpeedModifier);
                float time = visMat.animTextures[currentTexture].time;
                //float time = 1f / 30f;
                while (currentTime >= time) {
                    if (time <= 0) {
                        animate = false;
                        break;
                    }
                    currentTime -= time;
                    textureIndex = ++textureIndex % visMat.animTextures.Count;
                    time = visMat.animTextures[textureIndex].time;
                }
            } else {
                textureIndex = 0;
            }
        }*/
        if (textureIndex != currentTexture) {
            currentTexture = textureIndex;
            SetTexture(currentTexture);
        }
    }
}
