using OpenSpace;
using OpenSpace.Visual;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightBehaviour : MonoBehaviour {
    public LightInfo li;
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

    public bool IsModified {
        get { return modified; }
    }

    // Use this for initialization
    void Start() {
    }

	public void Init() {
        pos = transform.position;
        rot = transform.rotation;
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
	
	// Update is called once per frame
	void Update () {
        if (loaded && lightManager != null) {
            if (pos != transform.position || rot != transform.rotation || scl != transform.localScale || col != color || bckCol != backgroundColor) {
                modified = true;
                lightManager.sectorManager.RecalculateSectorLighting();
                pos = transform.position;
                rot = transform.rotation;
                scl = transform.localScale;
                if (Settings.s.engineVersion == Settings.EngineVersion.R3) {
                    li.transMatrix.type = 7;
                    li.transMatrix.SetTRS(transform.position, transform.rotation, transform.localScale, convertAxes: true, setVec: true);
                } else {
                    li.transMatrix.SetTRS(transform.position, transform.rotation, transform.localScale, convertAxes: true, setVec: false);
                }
                intensity = Mathf.Max(li.color.x, li.color.y, li.color.z);
                li.color = color;
                li.background_color = backgroundColor;
                if (intensity > 1) {
                    Vector3 colorVector = new Vector3(li.color.x / intensity, li.color.y / intensity, li.color.z / intensity);
                    color = new Color(Mathf.Clamp01(colorVector.x), Mathf.Clamp01(colorVector.y), Mathf.Clamp01(colorVector.z), Mathf.Clamp01(li.color.w));
                } else if (intensity > 0) {
                    color = new Color(Mathf.Clamp01(li.color.x), Mathf.Clamp01(li.color.y), Mathf.Clamp01(li.color.z), Mathf.Clamp01(li.color.w));
                } else {
                    // shadow, can't display it since colors are additive in Unity
                }
                backgroundColor = new Color(Mathf.Clamp01(li.background_color.x), Mathf.Clamp01(li.background_color.y), Mathf.Clamp01(li.background_color.z), Mathf.Clamp01(li.background_color.w));
                bckCol = backgroundColor;
                col = color;
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
        switch (li.type) {
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
        switch (li.type) {
            case 1:
                Gizmos.DrawRay(transform.position, transform.rotation.eulerAngles); break;
            case 2:
            case 7:
            case 8:
                Gizmos.DrawWireSphere(transform.position, li.far); break;
        }
    }
}
