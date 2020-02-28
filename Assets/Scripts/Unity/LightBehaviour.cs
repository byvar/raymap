using OpenSpace;
using OpenSpace.Visual;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightBehaviour : MonoBehaviour {
    public LightInfo li;
	public OpenSpace.ROM.LightInfo liROM;
    public Light l = null;
    bool loaded = false;
    public Color color;
    public Color backgroundColor;
    float intensity;
    public float activeIntensity = 1f;
    public bool active = true;
    public LightManager lightManager;
    private Vector3 pos;
    private Quaternion rot;
    private Vector3 scl;
    private Color col;
    private Color bckCol;
    private bool modified = false;
	public bool IsActive {
		get {
			if (li != null) {
				return li.turnedOn != 0;
			} else if (liROM != null) {
				return liROM.IsActive;
			} else return false;
		}
	}
	public bool IsObjectLighted(LightInfo.ObjectLightedFlag type) {

		if (li != null) {
			return li.IsObjectLighted(type);
		} else if (liROM != null) {
			return true;
		} else return true;
	}
	public int Type {
		get {
			if (li != null) {
				return li.type;
			} else if (liROM != null) {
				return liROM.Type;
			} else return 0;
		}
	}
	public float Near {
		get {
			if (li != null) {
				return li.near;
			} else if (liROM != null) {
				return liROM.near;
			} else return 0f;
		}
	}
	public float Far {
		get {
			if (li != null) {
				return li.far;
			} else if (liROM != null) {
				return liROM.far;
			} else return 0f;
		}
	}
	public float FogBlendNear {
		get {
			if (li != null) {
				return li.bigAlpha_fogBlendNear;
			} else if (liROM != null) {
				return liROM.fogBlendNear;
			} else return 0f;
		}
	}
	public float FogBlendFar {
		get {
			if (li != null) {
				return li.intensityMin_fogBlendFar;
			} else if (liROM != null) {
				return liROM.fogBlendFar;
			} else return 0f;
		}
	}
	public Vector4 Color {
		get {
			if (li != null) {
				return li.color;
			} else if (liROM != null) {
				return liROM.color;
			} else return Vector4.zero;
		}
	}
	public Vector4 BackgroundColor {
		get {
			if (li != null) {
				return li.background_color;
			} else if (liROM != null) {
				return liROM.backgroundColor;
			} else return Vector4.zero;
		}
	}
	public int AlphaLightFlag {
		get {
			if (li != null) {
				return li.alphaLightFlag;
			} else if (liROM != null) {
				return liROM.AlphaLightFlag;
			} else return 0;
		}
	}
	public int PaintingLightFlag {
		get {
			if (li != null) {
				return li.paintingLightFlag;
			} else if (liROM != null) {
				return liROM.PaintingLightFlag;
			} else return 0;
		}
	}

	public bool IsModified {
        get { return modified; }
    }

    // Use this for initialization
    void Start() {
    }

	public void Init(LightManager lm, LightInfo li) {
		this.li = li;
		li.light = this; // very dirty
		this.lightManager = lm;

		name = (li.name == null ? "Light" : li.name) + " @ " + li.Offset + " | " +
						"Type: " + Type + " - Far: " + Far + " - Near: " + Near +
						//" - FogBlendNear: " + FogBlendNear + " - FogBlendFar: " + FogBlendFar +
						" - AlphaLF: " + AlphaLightFlag +
						" - PaintingLF: " + PaintingLightFlag +
						" - ObjectLF: " + li.objectLightedFlag;
		Vector3 pos = li.transMatrix.GetPosition(convertAxes: true);
		Quaternion rot = li.transMatrix.GetRotation(convertAxes: true) * Quaternion.Euler(-90, 0, 0);
		Vector3 scale = li.transMatrix.GetScale(convertAxes: true);
		transform.SetParent(lm.transform);
		transform.localPosition = pos;
		transform.localRotation = rot;
		transform.localScale = scale;
		this.pos = pos;
		this.rot = rot;
		this.scl = scale;
        //color = new Color(Mathf.Clamp01(r3l.color.x), Mathf.Clamp01(r3l.color.y), Mathf.Clamp01(r3l.color.z), Mathf.Clamp01(r3l.color.w));
        intensity = Mathf.Max(li.color.x, li.color.y, li.color.z);
        if (intensity > 1) {
            Vector3 colorVector = new Vector3(li.color.x / intensity, li.color.y / intensity, li.color.z / intensity);
            color = new Color(Mathf.Clamp01(colorVector.x), Mathf.Clamp01(colorVector.y), Mathf.Clamp01(colorVector.z), Mathf.Clamp01(li.color.w));
        } else if (intensity > 0) {
            color = new Color(Mathf.Clamp01(li.color.x), Mathf.Clamp01(li.color.y), Mathf.Clamp01(li.color.z), Mathf.Clamp01(li.color.w));
        } else {
            // shadow, can't display it since colors are additive in Unity
        }
        backgroundColor = new Color(Mathf.Clamp01(li.background_color.x), Mathf.Clamp01(li.background_color.y), Mathf.Clamp01(li.background_color.z), Mathf.Clamp01(li.background_color.w));
        /*if (li.alphaLightFlag != 0) {
            color = new Color(color.r * li.color.w, color.g * li.color.w, color.b * li.color.w);
            backgroundColor = new Color(
                backgroundColor.r * li.background_color.w,
                backgroundColor.g * li.background_color.w,
                backgroundColor.b * li.background_color.w);
        }*/
        col = color;
        bckCol = backgroundColor;
        loaded = true;
    }


	public void Init(LightManager lm, OpenSpace.ROM.LightInfo li) {
		this.liROM = li;
		this.lightManager = lm;

		name = "Light @ " + li.Offset + " | " +
						"Type: " + Type + " - Far: " + Far + " - Near: " + Near +
						//" - FogBlendNear: " + FogBlendNear + " - FogBlendFar: " + FogBlendFar +
						" - AlphaLF: " + AlphaLightFlag +
						" - PaintingLF: " + PaintingLightFlag +
						" - ObjectLF: " + li.objectLightedFlag;
		transform.SetParent(lm.transform);
		liROM.transform.Apply(gameObject);
		transform.localRotation = transform.localRotation * Quaternion.Euler(-90, 0, 0);
		pos = transform.localPosition;
		rot = transform.localRotation;
		scl = transform.localScale;
		//color = new Color(Mathf.Clamp01(r3l.color.x), Mathf.Clamp01(r3l.color.y), Mathf.Clamp01(r3l.color.z), Mathf.Clamp01(r3l.color.w));
		intensity = Mathf.Max(li.color.x, li.color.y, li.color.z);
		if (intensity > 1) {
			Vector3 colorVector = new Vector3(li.color.x / intensity, li.color.y / intensity, li.color.z / intensity);
			color = new Color(Mathf.Clamp01(colorVector.x), Mathf.Clamp01(colorVector.y), Mathf.Clamp01(colorVector.z), Mathf.Clamp01(li.color.w));
		} else if (intensity > 0) {
			color = new Color(Mathf.Clamp01(li.color.x), Mathf.Clamp01(li.color.y), Mathf.Clamp01(li.color.z), Mathf.Clamp01(li.color.w));
		} else {
			// shadow, can't display it since colors are additive in Unity
		}
		backgroundColor = new Color(Mathf.Clamp01(li.backgroundColor.x), Mathf.Clamp01(li.backgroundColor.y), Mathf.Clamp01(li.backgroundColor.z), Mathf.Clamp01(li.backgroundColor.w));
		/*if (li.alphaLightFlag != 0) {
            color = new Color(color.r * li.color.w, color.g * li.color.w, color.b * li.color.w);
            backgroundColor = new Color(
                backgroundColor.r * li.background_color.w,
                backgroundColor.g * li.background_color.w,
                backgroundColor.b * li.background_color.w);
        }*/
		col = color;
		bckCol = backgroundColor;
		loaded = true;
	}

	// Update is called once per frame
	void Update () {
        if (loaded && lightManager != null) {
            if (pos != transform.position || rot != transform.rotation || scl != transform.localScale || col != color || bckCol != backgroundColor) {
                modified = true;
                pos = transform.position;
                rot = transform.rotation;
                scl = transform.localScale;
				if (li != null) {
					if (Settings.s.engineVersion == Settings.EngineVersion.R3) {
						li.transMatrix.type = 7;
						li.transMatrix.SetTRS(transform.position, transform.rotation, transform.localScale, convertAxes: true, setVec: true);
					} else {
						li.transMatrix.SetTRS(transform.position, transform.rotation, transform.localScale, convertAxes: true, setVec: false);
					}
					li.color = new Vector4(color.r, color.g, color.b, color.a);
					li.background_color = backgroundColor;
					intensity = Mathf.Max(li.color.x, li.color.y, li.color.z);
					if (intensity > 1) {
						Vector3 colorVector = new Vector3(li.color.x / intensity, li.color.y / intensity, li.color.z / intensity);
						color = new Color(Mathf.Clamp01(colorVector.x), Mathf.Clamp01(colorVector.y), Mathf.Clamp01(colorVector.z), Mathf.Clamp01(li.color.w));
					} else if (intensity > 0) {
						color = new Color(Mathf.Clamp01(li.color.x), Mathf.Clamp01(li.color.y), Mathf.Clamp01(li.color.z), Mathf.Clamp01(li.color.w));
					} else {
						// shadow, can't display it since colors are additive in Unity
					}
					backgroundColor = new Color(Mathf.Clamp01(li.background_color.x), Mathf.Clamp01(li.background_color.y), Mathf.Clamp01(li.background_color.z), Mathf.Clamp01(li.background_color.w));
				}
                bckCol = backgroundColor;
                col = color;
				lightManager.sectorManager.RecalculateSectorLighting();
			}
        }
	}

    public void Activate() {
        active = true;
        gameObject.SetActive(true);
    }

    public void Deactivate() {
        active = false;
        //gameObject.SetActive(false);
    }

    public void OnDrawGizmos() {
        Gizmos.color = new Color(color.r, color.g, color.b, 1f);
        switch (Type) {
            case 2:
            case 7:
            case 8:
                Gizmos.DrawIcon(transform.position, "PointLight Gizmo", true);
                break;
            case 1:
                Gizmos.DrawIcon(transform.position, "DirectionalLight Gizmo", true);
                break;
            case 4:
                Gizmos.DrawIcon(transform.position, "AreaLight Gizmo", true);
                break;
        }
    }
    public void OnDrawGizmosSelected() {
        Gizmos.color = new Color(color.r, color.g, color.b, 1f);
        Gizmos.matrix = Matrix4x4.identity;
        switch (Type) {
            case 1:
				Gizmos.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 20); break;
            case 2:
            case 7:
            case 8:
                Gizmos.DrawWireSphere(transform.position, Far); break;
        }
    }
}
