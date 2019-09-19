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
        CameraPosXYZ = 4
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
            Quaternion inverseParent = transform.parent != null ? Quaternion.Inverse(transform.parent.rotation) : Quaternion.identity;
            Quaternion parentRot = transform.parent != null ? transform.parent.rotation * Quaternion.Euler(0, -90, 0) : Quaternion.Euler(0, -90, 0);
            Quaternion lookRot;
            switch (mode) {
                case LookAtMode.ViewRotation: // Rotates based on camera rotation. Objects on the side of the view will appear to face camera.
                    lookRot = (inverseParent * Quaternion.LookRotation(
                        cam.transform.rotation * Vector3.back,
                        cam.transform.rotation * Vector3.up)) * Quaternion.Euler(0, -90, 0);
                    transform.localRotation = lookRot;
                    //transform.LookAt(transform.position + cam.transform.rotation * Vector3.right, cam.transform.rotation * Vector3.up);
                    //transform.Rotate(new Vector3(-addRotation, 0, 0), Space.Self);
                    break;
                case LookAtMode.CameraPos: // Rotates to camera pos. Objects on the side of the view will appear rotated
                    lookRot = (inverseParent * Quaternion.LookRotation(
                        (cam.transform.position - transform.position),
                        parentRot * Vector3.up)) * Quaternion.Euler(0, -90, 0);
                    transform.localRotation = Quaternion.Euler(0,lookRot.eulerAngles.y, 0);
                    //transform.rotation = Quaternion.Euler(newRot.eulerAngles.x, lookRot.eulerAngles.y, newRot.eulerAngles.z);
                    /*transform.LookAt(transform.position + new Vector3(rotatedRight.x, 0f, rotatedRight.z), cam.transform.rotation * Vector3.up);
                    transform.Rotate(new Vector3(-addRotation, 0, 0), Space.Self);*/
                    /*transform.LookAt(cam.transform.position);
                    transform.Rotate(new Vector3(-addRotation, -90, 0), Space.Self);*/
                    break;
                case LookAtMode.Unknown:
                    float addRotation = 0f;
                    if (transform.parent != null) addRotation = transform.parent.rotation.eulerAngles.x;
                    transform.LookAt(transform.position + cam.transform.rotation * Vector3.right, cam.transform.rotation * Vector3.up);
                    transform.Rotate(new Vector3(-addRotation, 0, 0), Space.Self);
                    break;
                case LookAtMode.CameraPosXYZ: // Rotates to camera pos on all axes

                    if (cam.transform.position == transform.position) {
                        break;
                    }

                    lookRot = (inverseParent * Quaternion.LookRotation(
                        (cam.transform.position - transform.position),
                        parentRot * Vector3.up)) * Quaternion.Euler(0, -90, 0);
                    transform.localRotation = Quaternion.Euler(0, lookRot.eulerAngles.y, lookRot.eulerAngles.z);
                    //transform.rotation = Quaternion.Euler(newRot.eulerAngles.x, lookRot.eulerAngles.y, newRot.eulerAngles.z);
                    /*transform.LookAt(transform.position + new Vector3(rotatedRight.x, 0f, rotatedRight.z), cam.transform.rotation * Vector3.up);
                    transform.Rotate(new Vector3(-addRotation, 0, 0), Space.Self);*/
                    /*transform.LookAt(cam.transform.position);
                    transform.Rotate(new Vector3(-addRotation, -90, 0), Space.Self);*/
                    break;
            }
        }
    }
}
 