using OpenSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MultiTextureMaterial : MonoBehaviour {
    public VisualMaterial r3mat;
    public Material mat;
    public string[] textureNames = { "Placeholder" };
    int currentTexture = 0;
    public int textureIndex = 0;

    public void Start() {
        IEnumerable<TextureInfo> textures = r3mat.off_animTextures.Select(p => TextureInfo.FromOffset(p));
        textureNames = textures.Select(t => (t == null ? "Null" : t.name)).ToArray();
    }

    public void SetTexture(int index) {
        if (index < 0 || index > r3mat.off_animTextures.Count) return;
        TextureInfo tex = TextureInfo.FromOffset(r3mat.off_animTextures[index]);
        if (tex != null) {
            mat.SetTexture("_MainTex", tex.Texture);
        }
    }

    public void Update() {
        if (textureIndex != currentTexture) {
            currentTexture = textureIndex;
            SetTexture(currentTexture);
        }
    }
}
