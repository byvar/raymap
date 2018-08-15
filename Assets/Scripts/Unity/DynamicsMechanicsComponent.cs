using UnityEngine;
using System.Collections;
using OpenSpace;
using OpenSpace.EngineObject;
using System.Collections.Generic;

public class DynamicsMechanicsComponent : MonoBehaviour
{
    public Dynamics dynamics;

    public Vector3 posA;
    public Vector3 posB;
    public Vector3 speed;

    public string dynamicsOffset;

    public Dynamics.DynamicsType type;

    public void SetDynamics(Dynamics dynamics)
    {
        this.dynamics = dynamics;
        this.dynamicsOffset = this.dynamics.offset.ToString();

        this.type = dynamics.type;
        if (dynamics.matrixA != null) {
            this.posA = dynamics.matrixA.GetPosition(convertAxes: true);
        }
        if (dynamics.matrixB != null) {
            this.posB = dynamics.matrixB.GetPosition(convertAxes: true);
        }
        this.speed = dynamics.speedVector;
    }
}
