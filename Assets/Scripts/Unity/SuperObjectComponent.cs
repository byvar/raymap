using OpenSpace.Object;
using System;
using System.Collections;
using UnityEngine;

public class SuperObjectComponent : MonoBehaviour {
    public SuperObject so;
    public uint matrixType;
    public string flagPreview;
	public string drawFlagsPreview;
    public string spoOffset;

    public void Start() {
        if (so != null) {
            this.matrixType = so.matrix.type;
            this.flagPreview = so.flags.flags.ToString();
			this.drawFlagsPreview = so.drawFlags.flags.ToString();
            this.spoOffset = so.offset.ToString();
        }
    }
}
 