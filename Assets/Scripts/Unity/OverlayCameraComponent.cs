using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class OverlayCameraComponent : MonoBehaviour
{

    public Camera CameraBase;
    public Camera Camera;

    public void Start()
    {
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        if (Camera != null && CameraBase != null) {
            Camera.orthographic = CameraBase.orthographic;
            Camera.orthographicSize = CameraBase.orthographicSize;
            Camera.fieldOfView = CameraBase.fieldOfView;
        }
    }

    public void Update()
    {
        UpdateCamera();
    }
}
