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
        if (r3mat.scrollingEnabled) {
            updateCounter += Time.deltaTime * animationSpeed;
            while (updateCounter >= 1f) {
                updateCounter -= 1f;
                currentFrame++;
                if(updateCounter < 1f) UpdateFrame();
            }
        }
    }

    void UpdateFrame() {
        float offsetU = currentFrame * r3mat.scrollX;
        float offsetV = currentFrame * r3mat.scrollY;
        mat.SetTextureOffset("_MainTex", new Vector2(offsetU, offsetV));
    }
}
