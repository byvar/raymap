using OpenSpace;
using OpenSpace.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScrollingTexture : MonoBehaviour {
    public VisualMaterial visMat;
    public Material mat;
    public float animationSpeed = 60f; // Force 60fps w/ frameskip
    private float updateCounter = 0f;
    public uint currentFrame = 0;

    public void Start() {
    }

    public void Update() {
        if (visMat != null && visMat.ScrollingEnabled) {
            updateCounter += Time.deltaTime * animationSpeed;
            while (updateCounter >= 1f) {
                updateCounter -= 1f;
                currentFrame++;
                if(updateCounter < 1f) UpdateFrame();
            }
        }
    }

    public void ResetMaterial(VisualMaterial visMat, Material mat) {
        this.visMat = visMat;
        this.mat = mat;
        currentFrame = 0;
        updateCounter = 0;
    }

    void UpdateFrame() {
        if (visMat != null) {
            for (int i = 0; i < visMat.textures.Count; i++) {
                VisualMaterialTexture t = visMat.textures[i];
                if (t.ScrollingEnabled) {
                    float offsetU = t.currentScrollX + (t.IsScrollX ? currentFrame * t.ScrollX : 0);
                    float offsetV = t.currentScrollY + (t.IsScrollY ? currentFrame * t.ScrollY : 0);
                    if (i == 0) {
                        mat.SetTextureOffset("_MainTex", new Vector2(offsetU, offsetV));
                    } else if (i > 0 && mat.HasProperty("_MainTex" + i)) {
                        mat.SetTextureOffset("_MainTex" + i, new Vector2(offsetU, offsetV));
                    }
                }
            }
        }
    }
}
