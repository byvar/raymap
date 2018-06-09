using OpenSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Moddable : MonoBehaviour {
    Vector3 startPos;
    Quaternion startRot;
    Vector3 startScale;
    public Matrix mat;

    public void Start() {
        startPos = transform.localPosition;
        startRot = transform.localRotation;
        startScale = transform.localScale;
    }

    public void SaveChanges(EndianBinaryWriter writer) {
        if (startPos != transform.localPosition || startRot != transform.localRotation || startScale != transform.localScale) {
            mat.type = 7;
            mat.SetTRS(transform.localPosition, transform.localRotation, transform.localScale, convertAxes: true, setVec: true);
            mat.Write(writer);
        }
    }
}
