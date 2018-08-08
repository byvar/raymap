using OpenSpace;
using OpenSpace.EngineObject;
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
    public PersoBehaviour persoBehaviour;
    public DsgVarComponent dsgVarComponent;
    public StandardGame stdGame;

    public void Start() {
        startPos = transform.localPosition;
        startRot = transform.localRotation;
        startScale = transform.localScale;
    }

    public void SaveChanges(Writer writer) {
        if (startPos != transform.localPosition || startRot != transform.localRotation || startScale != transform.localScale) {
            if (Settings.s.engineMode == Settings.EngineMode.R3) {
                mat.type = 7;
                mat.SetTRS(transform.localPosition, transform.localRotation, transform.localScale, convertAxes: true, setVec: true);
            } else {
                mat.SetTRS(transform.localPosition, transform.localRotation, transform.localScale, convertAxes: true, setVec: false);
            }
            mat.Write(writer);
        }

        if (persoBehaviour!=null && persoBehaviour.perso!=null)
        {
            Perso.Write(persoBehaviour.perso, writer);

            if (stdGame != null) {
                stdGame.Write(writer, persoBehaviour.perso.Gao.GetComponents<CustomBitsComponent>());
            }
        }

        if (dsgVarComponent!=null) {
            dsgVarComponent.Write(writer);
        }
    }
}
