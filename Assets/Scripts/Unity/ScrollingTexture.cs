using OpenSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScrollingTexture : MonoBehaviour
{
    public VisualMaterial r3mat;
    public Material mat;

    public void Start() {
    }

    public void Update()
    {
        if (r3mat.scrollingEnabled)
        {
            float offsetU = Time.timeSinceLevelLoad * 60 * r3mat.scrollX;
            float offsetV = Time.timeSinceLevelLoad * 60 * r3mat.scrollY;
            mat.SetTextureOffset("_MainTex", new Vector2(offsetU, offsetV));
        }
    }
}
