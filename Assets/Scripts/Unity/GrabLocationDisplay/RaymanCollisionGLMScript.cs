using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaymanCollisionGLMScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0, Time.deltaTime*100.0f, 0);
    }
}
