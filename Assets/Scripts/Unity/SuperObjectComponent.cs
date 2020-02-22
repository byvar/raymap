using OpenSpace;
using OpenSpace.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SuperObjectComponent : MonoBehaviour {
    public SuperObject so;
    public uint matrixType;
    public string flagPreview;
	public string drawFlagsPreview;
    public string spoOffset;

    public void Start() {
        if (so != null) {
            if(so.matrix != null) this.matrixType = so.matrix.type;
            this.flagPreview = so.flags.flags.ToString();
			this.drawFlagsPreview = so.drawFlags.flags.ToString();
            this.spoOffset = so.offset.ToString();

			if (MapLoader.Loader is OpenSpace.Loader.R3Loader) {
				// Set mirror flag
				bool isDontDrawInMirror = so.drawFlags.HasFlag(OpenSpace.Object.Properties.SuperObjectDrawFlags.Flags.DontDrawInMirror);
				bool isDrawOnlyInMirror = so.drawFlags.HasFlag(OpenSpace.Object.Properties.SuperObjectDrawFlags.Flags.DrawOnlyInMirror);
				if (isDontDrawInMirror || isDrawOnlyInMirror) {
					Renderer[] rs = GetComponents<Renderer>();
					foreach (Renderer r in rs) {
						SetMirrorLayer(r, isDrawOnlyInMirror);
					}
					rs = GetComponentsInChildren<Renderer>(true);
					foreach (Renderer r in rs) {
						SetMirrorLayer(r, isDontDrawInMirror);
					}
				}
			}
        }
    }
	public void SetMirrorLayer(Renderer r, bool drawInMirror) {
		if (r.gameObject.layer == LayerMask.NameToLayer("Visual")) {
			if (drawInMirror) {
				r.gameObject.layer = LayerMask.NameToLayer("VisualOnlyInMirror");
			} else {
				r.gameObject.layer = LayerMask.NameToLayer("VisualNotInMirror");
			}
		}
	}
}
 