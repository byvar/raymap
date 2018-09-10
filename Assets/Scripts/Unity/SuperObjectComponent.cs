using OpenSpace.Object;
using System;
using System.Collections;
using UnityEngine;

public class SuperObjectComponent : MonoBehaviour {
    public SuperObject so;
    public string flagPreview;

    public void Start() {
        if (so != null) {
            this.flagPreview = so.flags.flags.ToString();
        }
    }
}
 