using OpenSpace.Object;
using System;
using System.Collections;
using UnityEngine;

public class SuperObjectComponent : MonoBehaviour {
    public SuperObject so;
    public uint matrixType;
    public string flagPreview;

    public void Start() {
        if (so != null) {
            this.matrixType = so.matrix.type;
            this.flagPreview = so.flags.flags.ToString();
        }
    }
}
 