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

                DsgVarInfoEntry vector38Entry = dsgVarComponent.dsgVarEntries[38];
                DsgVarInfoEntry vector40Entry = dsgVarComponent.dsgVarEntries[40];
                Vector3 vector38_xzy = (Vector3)vector38Entry.value;
                Vector3 vector40_xzy = (Vector3)vector40Entry.value;

                Vector3 vector38 = new Vector3(vector38_xzy.x, vector38_xzy.z, vector38_xzy.y);
                Vector3 vector40 = new Vector3(vector40_xzy.x, vector40_xzy.z, vector40_xzy.y);

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
                    }
                }
            }
        }

        //Debug.draw
    }
}
