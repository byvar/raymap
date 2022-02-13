using OpenSpace;
using OpenSpace.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SuperObjectComponent : MonoBehaviour, IReferenceable {
    public SuperObject so;
	public OpenSpace.ROM.SuperObject soROM;
	public OpenSpace.ROM.SuperObjectDynamic soROMDynamic;
	public OpenSpace.PS1.SuperObject soPS1;
	public List<SuperObjectComponent> Children { get; set; } = new List<SuperObjectComponent>();

	public uint matrixType;
    public string flagPreview;
	public string drawFlagsPreview;
    public string spoOffset;

    public ReferenceFields References { get => ((IReferenceable)so).References; set => ((IReferenceable)so).References = value; }

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

	public SuperObject.Type Type {
		get {
			if (so != null) return so.type;
			if (soROMDynamic != null) return SuperObject.Type.Perso;
			if (soPS1 != null) return soPS1.type;
			if (soROM != null) {
				if (soROM.data?.Value is OpenSpace.ROM.Sector) {
					return SuperObject.Type.Sector;
				} else {
					return SuperObject.Type.IPO;
				}
			}
			return SuperObject.Type.Unknown;
		}
	}
	public LegacyPointer Offset {
		get {
			if (so != null) return so.offset;
			if (soROMDynamic != null) return soROMDynamic.Offset;
			if (soPS1 != null) return soPS1.Offset;
			if (soROM != null) return soROM.Offset;
			return null;
		}
	}
}
 