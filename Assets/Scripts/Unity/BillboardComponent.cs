using OpenSpace.Object;
using OpenSpace.Visual;
using System;
using System.Collections;
using UnityEngine;

public class BillboardBehaviour : MonoBehaviour {
    private Camera cam = null;
    public enum LookAtMode {
        None = 0,
        ViewRotation = 1,
        CameraPos = 2,
        Unknown = 3,
    }
    public LookAtMode mode = LookAtMode.None;

    private Camera Cam {
        get {
            if (cam == null) cam = Camera.main;
            return cam;
        }
    }

    void LateUpdate() {
        if (Cam != null && mode != LookAtMode.None) {
            float addRotation = 0f;
            if (transform.parent != null) addRotation = transform.parent.rotation.eulerAngles.x;
            switch (mode) {
                case LookAtMode.ViewRotation: // Rotates based on camera rotation. Objects on the side of the view will appear to face camera.
                    transform.LookAt(transform.position + cam.transform.rotation * Vector3.right, cam.transform.rotation * Vector3.up);
                    transform.Rotate(new Vector3(-addRotation, 0, 0), Space.Self);
                    break;
                case LookAtMode.CameraPos: // Rotates to camera pos. Objects on the side of the view will appear rotated
                    transform.LookAt(cam.transform.position);
                    transform.Rotate(new Vector3(-addRotation, -90, 0), Space.Self);
                    break;
                case LookAtMode.Unknown:
                    transform.LookAt(transform.position + cam.transform.rotation * Vector3.right, cam.transform.rotation * Vector3.up);
                    transform.Rotate(new Vector3(-addRotation, 0, 0), Space.Self);
                    break;
            }
        }
    }
}
 