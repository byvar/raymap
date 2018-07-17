using OpenSpace;
using OpenSpace.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScrollingTexture : MonoBehaviour {
    public VisualMaterial r3mat;
    public Material mat;
    public float animationSpeed = 60f; // Force 60fps w/ frameskip
    private float updateCounter = 0f;
    public uint currentFrame = 0;

    public void Start() {
    }

    public void Update() {
        if (r3mat.ScrollingEnabled) {
            updateCounter += Time.deltaTime * animationSpeed;
            while (updateCounter >= 1f) {
                updateCounter -= 1f;
                currentFrame++;
                if(updateCounter < 1f) UpdateFrame();
            }
        }
    }

    void UpdateFrame() {
        for (int i = 0; i < r3mat.textures.Count; i++) {
            VisualMaterialTexture t = r3mat.textures[i];
            if (t.ScrollingEnabled) {
                float offsetU = t.currentScrollX + (t.IsScrollX ? currentFrame * t.scrollX : 0);
                float offsetV = t.currentScrollY + (t.IsScrollY ? currentFrame * t.scrollY : 0);
                if (i == 0) {
                    mat.SetTextureOffset("_MainTex", new Vector2(offsetU, offsetV));
                } else  if (i == 1 && mat.HasProperty("_MainTex2")) mat.SetTextureOffset("_MainTex2", new Vector2(offsetU, offsetV));
            }
        }
    }
}
