using OpenSpace;
using OpenSpace.AI;
using OpenSpace.Object;
using System.Collections;
using System.Collections.Generic;
using OpenSpace.FileFormat;
using UnityEditor;
using UnityEngine;

public class GrabLocationDisplayScript : MonoBehaviour
{
    //GameObject raymanGAO = null;
    public GameObject GaoDsgVar38;
    public GameObject GaoDsgVar40;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var reader = MapLoader.Loader.livePreviewReader;

        if (reader != null) {
            MemoryFile mem = (MemoryFile)MapLoader.Loader.files_array[0];
            new Pointer(0x500298, mem).Goto(ref reader);
            (Pointer.Read(reader)+ 0x234).Goto(ref reader);
            (Pointer.Read(reader)+ 0x10).Goto(ref reader);
            (Pointer.Read(reader) + 0xC).Goto(ref reader);
            (Pointer.Read(reader) + 0xB0).Goto(ref reader);
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            
            GaoDsgVar38.transform.position = new Vector3(x,z,y);
        }

        //GaoDsgVar40.transform.position = vector40;

        /* oldMethod
        if (raymanGAO == null) {
            GameObject[] gameObjects = FindObjectsOfType(typeof(GameObject)) as GameObject[];

            foreach (GameObject gao in gameObjects) {
                if (gao.name.Contains("[rayman] YLT_RaymanModel")) {
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
                    //vector38Entry = dsgVarComponent.dsgVarEntries[38];
                    //vector40Entry = dsgVarComponent.dsgVarEntries[40];
                }

                Vector3 vector38_xzy = (Vector3)vector38Entry.valueCurrent.AsVector;
                Vector3 vector40_xzy = (Vector3)vector40Entry.valueCurrent.AsVector;

                Vector3 vector38 = new Vector3(vector38_xzy.x, vector38_xzy.z, vector38_xzy.y);
                Vector3 vector40 = new Vector3(vector40_xzy.x, vector40_xzy.z, vector40_xzy.y);

                GaoDsgVar38.transform.position = vector38;
                GaoDsgVar40.transform.position = vector40;
            }
        }*/

        //Debug.draw
    }
}
