using UnityEngine;
using OpenSpace.Collide;

[ExecuteInEditMode]
public class CollideComponent : MonoBehaviour {
    public IGeometricObjectElementCollide collide;
    public CollideMaterial col => collide?.GetMaterial(index)?.collideMaterial;

    public OpenSpace.ROM.IGeometricObjectElementCollide collideROM;
    public OpenSpace.ROM.CollideMaterial colROM => collideROM?.GetMaterial(index)?.collideMaterial;

    public int? index;

    public CollideType type;
}