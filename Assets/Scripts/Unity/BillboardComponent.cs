using OpenSpace.Object;
using OpenSpace.Visual;
using System;
using System.Collections;
using UnityEngine;

public class BillboardBehaviour : MonoBehaviour {
    private Camera cam = null;
    public bool isLookAt = false;

    private Camera Cam {
        get {
            if (cam == null) cam = Camera.main;
            return cam;
        }
    }

    void LateUpdate() {
        if (Cam != null) {
            if (isLookAt) {
                //transform.LookAt(cam.transform.position);
                //transform.Rotate(new Vector3(0, -90, 0), Space.Self);
                float addRotation = 0f;
                if (transform.parent != null) addRotation = transform.parent.rotation.eulerAngles.x;
                transform.LookAt(transform.position + cam.transform.rotation * Vector3.right, cam.transform.rotation * Vector3.up);
                transform.Rotate(new Vector3(-addRotation, 0, 0), Space.Self);
            }
        }
    }
}
 