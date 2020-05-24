using OpenSpace;
using OpenSpace.AI;
using OpenSpace.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GrabLocationDisplayScript_PCSX2 : MonoBehaviour {
    
    public string executableName = "PCSX2.exe";
    public string offset = "00000000";
    private ProcessMemoryStream stream;

    // Start is called before the first frame update
    void Start()
    {
        stream = new ProcessMemoryStream(executableName, ProcessMemoryStream.Mode.Read); 
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 vector40 = new Vector3(0,0,0);

        int offset_int = int.Parse(offset, System.Globalization.NumberStyles.HexNumber);
        if (offset_int == 0) {
            return;
        }
        stream.Seek(offset_int, System.IO.SeekOrigin.Begin);
        Reader reader = new Reader(stream);

        vector40.x = reader.ReadSingle();
        vector40.z = reader.ReadSingle();
        vector40.y = reader.ReadSingle();
        
        for (float i = -0.05f; i <= 0.05f; i += 0.02f) {
            for (float j = -0.05f; j <= 0.05f; j += 0.02f) {

                Vector3 o = new Vector3(i, 0, j);

                Debug.DrawRay(vector40 + o, new Vector3(0, -0.9f, 0), Color.blue);
                Debug.DrawRay(vector40 + o, new Vector3(-1f, 0, 0), Color.blue);
                Debug.DrawRay(vector40 + o, new Vector3(1f, 0, 0), Color.blue);
                Debug.DrawRay(vector40 + o, new Vector3(0f, 0, -1f), Color.blue);
                Debug.DrawRay(vector40 + o, new Vector3(0f, 0, 1f), Color.blue);
                
            }
        }
    }
}
