using OpenSpace;
using OpenSpace.AI;
using OpenSpace.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GrabLocationDisplayScript : MonoBehaviour
{
    GameObject raymanGAO = null;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (raymanGAO == null) {
            GameObject[] gameObjects = FindObjectsOfType(typeof(GameObject)) as GameObject[];

            foreach (GameObject gao in gameObjects) {
                if (gao.name.Contains("YLT_RaymanModel")) {
                    raymanGAO = gao;
                    break;
                }
            }
        } else {

            DsgVarComponent dsgVarComponent = raymanGAO.GetComponent<DsgVarComponent>();
            if (dsgVarComponent != null) {

                DsgVarComponent.DsgVarEditableEntry vector38Entry = null;
                DsgVarComponent.DsgVarEditableEntry vector40Entry = null;

                if (Settings.s.game == Settings.Game.R2) {
                    vector38Entry = dsgVarComponent.editableEntries[38];
                    vector40Entry = dsgVarComponent.editableEntries[40];
                } else if (Settings.s.game == Settings.Game.R3) {
                    return;
                    /*vector38Entry = dsgVarComponent.dsgVarEntries[38];
                    vector40Entry = dsgVarComponent.dsgVarEntries[40];*/
                }

                Vector3 vector38_xzy = (Vector3)vector38Entry.valueCurrent.AsVector;
                Vector3 vector40_xzy = (Vector3)vector40Entry.valueCurrent.AsVector;

                Vector3 vector38 = new Vector3(vector38_xzy.x, vector38_xzy.z, vector38_xzy.y);
                Vector3 vector40 = new Vector3(vector40_xzy.x, vector40_xzy.z, vector40_xzy.y);

                Vector3 raymanVec = raymanGAO.transform.position;
                Vector3 dir = (vector40) - (raymanVec);
                Vector3 dirFlat = new Vector3(dir.x, 0, dir.z);

                for (float i = -0.05f; i <= 0.05f; i+=0.02f) {
                    for (float j = -0.05f; j <= 0.05f; j += 0.02f) {

                        Vector3 o = new Vector3(i, 0, j);

                        Debug.DrawRay(vector38 + o , new Vector3(0, -0.9f, 0), Color.red);
                        Debug.DrawRay(vector38 + o, new Vector3(-1f, 0, 0), Color.red);
                        Debug.DrawRay(vector38 + o, new Vector3(1f, 0, 0), Color.red);
                        Debug.DrawRay(vector38 + o, new Vector3(0f, 0, -1f), Color.red);
                        Debug.DrawRay(vector38 + o, new Vector3(0f, 0, 1f), Color.red);

                        Debug.DrawRay(vector40 + o , new Vector3(0, -0.9f, 0), Color.blue);
                        Debug.DrawRay(vector40 + o, new Vector3(-1f, 0, 0), Color.blue);
                        Debug.DrawRay(vector40 + o, new Vector3(1f, 0, 0), Color.blue);
                        Debug.DrawRay(vector40 + o, new Vector3(0f, 0, -1f), Color.blue);
                        Debug.DrawRay(vector40 + o, new Vector3(0f, 0, 1f), Color.blue);

                        
                        Debug.DrawRay(raymanVec + o, dir, Color.blue);
                        Debug.DrawRay(raymanVec + o, dirFlat, Color.blue);
                    }
                }
            }
        }

        //Debug.draw
    }
}
