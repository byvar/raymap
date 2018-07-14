using OpenSpace;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightBehaviour : MonoBehaviour {
    public LightInfo r3l;
    public Light l = null;
    bool loaded = false;
    public Color color;
    public Color backgroundColor;
    float intensity;
    public float activeIntensity = 1f;
    public bool active = true;

    // Use this for initialization
    void Start() {
    }

	public void Init() {
        //color = new Color(Mathf.Clamp01(r3l.color.x), Mathf.Clamp01(r3l.color.y), Mathf.Clamp01(r3l.color.z), Mathf.Clamp01(r3l.color.w));
        intensity = Mathf.Max(r3l.color.x, r3l.color.y, r3l.color.z);
        if (intensity > 1) {
            Vector3 colorVector = new Vector3(r3l.color.x / intensity, r3l.color.y / intensity, r3l.color.z / intensity);
            color = new Color(Mathf.Clamp01(colorVector.x), Mathf.Clamp01(colorVector.y), Mathf.Clamp01(colorVector.z), Mathf.Clamp01(r3l.color.w));
        } else if (intensity > 0) {
            color = new Color(Mathf.Clamp01(r3l.color.x), Mathf.Clamp01(r3l.color.y), Mathf.Clamp01(r3l.color.z), Mathf.Clamp01(r3l.color.w));
        } else {
            // shadow, can't display it since colors are additive in Unity
        }
        backgroundColor = new Color(Mathf.Clamp01(r3l.background_color.x), Mathf.Clamp01(r3l.background_color.y), Mathf.Clamp01(r3l.background_color.z), Mathf.Clamp01(r3l.background_color.w));
        if (r3l.alphaLightFlag != 0) {
            color = new Color(color.r * r3l.color.w, color.g * r3l.color.w, color.b * r3l.color.w);
            backgroundColor = new Color(
                backgroundColor.r * r3l.background_color.w,
                backgroundColor.g * r3l.background_color.w,
                backgroundColor.b * r3l.background_color.w);
        }
        if (intensity > 0 && r3l.type != 6) {
            l = gameObject.AddComponent<Light>();
            l.color = color;
            intensity = Mathf.Clamp(intensity, 0f, 2f); // don't want too bright lights
            if (intensity < 1) intensity = 1;
            l.intensity = intensity;
            if (r3l.castShadows != 0 || r3l.createsShadowsOrNot != 0) {
                l.shadows = LightShadows.Hard;
                l.shadowNearPlane = 0.1f;
            }
            switch (r3l.type) {
                case 1:
                    // parallel light main
                    l.type = LightType.Directional;
                    break;
                case 2:
                    // spherical light main

                    /* Calculation of range/attenuation:
                    Unity uses attenuation equation: 1.0 / (1.0 + 25.0*r*r) where r=0..1 depending on range
                    R3 uses this for spherical lights:
                    if ( v8 == *(float *)&v17 ) v10 = 1.0;
                    else v10 = 1.0 / (*(float *)&v17 - v8);

                    which is 1 / (far^2 - near^2).
                    */
                    l.range = r3l.far * 2f;
                    //light.intensity = Mathf.Pow((r3l.near / r3l.far), 0.5f) * 2f;
                    //light.intensity *= Mathf.Pow(far / near, 0.5f) * 2f;
                    //light.intensity = Mathf.Pow((near/far),0.5f) * 2f;
                    /*float powAtt1 = Mathf.Pow(attFactor1, 2);
                    float powAtt2 = Mathf.Pow(attFactor2, 2);
                    if (powAtt1 == powAtt2) {
                        light.range = 10f;
                    } else {
                        light.range = 10f * 1.25f * (Mathf.Sqrt((powAtt1 - powAtt2 - 1f) / 25f));
                        if (light.range <= 0) light.range = 1f;
                        //light.range = 10f * 1/(attFactor1 - attFactor2);
                    }*/
                    break;
                case 3:
                    // hotspot light
                    // r2: cone
                    break;
                case 4:
                    // ambient light
                    //RenderSettings.ambientLight = color;
                    l.range = 0f;
                    //l.range = 400f;
                    l.intensity *= 0.4f;
                    break;
                case 5:
                    // parallel light other type?
                    // also seems to be the one with exterMinPos & exterMaxPos, so not spherical
                    l.range = r3l.attFactor3 * 1.5f;
                    break;
                case 6:
                    // r2: background color
                    // fog
                    // has the following properties:
                    // near
                    // far
                    // littleAlpha = blendNear
                    // intensityMin = blendFar

                    l.range = 0;

                    // game code mentions it's an orthogonal/parallel light, we'll approximate it with a spotlight
                    //light.range = far;
                    //light.intensity = Mathf.Pow((near / far), 0.5f) * 2f;
                    //light.type = LightType.Spot;
                    //light.spotAngle = 90f;
                    // nah, treat it as spherical. it has a range and all
                    break;
                case 7:
                    // parallel in a sphere light
                    // Since it's parallel this thing has a rotation, but it's also spherical so it makes more sense to use point lights.
                    l.range = r3l.far*2;
                    break;
                case 8:
                    // spherical light that ignores characters/persos?
                    break;
            }
        }
        loaded = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (loaded) {
            if (active == true && activeIntensity < 1f) {
                activeIntensity += Time.deltaTime / 1.5f;
            } else if (active == false && activeIntensity > 0f) {
                activeIntensity -= Time.deltaTime / 1.5f;
                if (activeIntensity < 0f) activeIntensity = 0f;
                if (activeIntensity == 0f) {
                    gameObject.SetActive(false);
                }
            }
            if (l != null) {
                l.intensity = intensity * activeIntensity;
            }
            /*if (r3l.type == 7) {
                if (r3l.containingSectors.FirstOrDefault(s => s.Active) != null) {
                    Vector3 camPos = Camera.main.transform.localPosition;
                    if (Vector3.Distance(camPos, transform.position) < r3l.far) {
                        Camera.main.backgroundColor = backgroundColor;
                    }
                }
            }*/
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
}
