using UnityEngine;
using OpenSpace.Collide;

[ExecuteInEditMode]
public class CollideComponent : MonoBehaviour {
    public GeometricObjectElementCollideTriangles collide;
    public CollideMaterial col => collide?.gameMaterial?.collideMaterial;
}