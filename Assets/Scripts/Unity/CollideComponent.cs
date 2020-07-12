using UnityEngine;
using OpenSpace.Collide;

[ExecuteInEditMode]
public class CollideComponent : MonoBehaviour {
    public IGeometricObjectElementCollide collide;
    public CollideMaterial col => collide?.GetMaterial(index)?.collideMaterial;

    public OpenSpace.ROM.IGeometricObjectElementCollide collideROM;
    public OpenSpace.ROM.CollideMaterial colROM => collideROM?.GetMaterial(index)?.collideMaterial;

    public OpenSpace.PS1.IGeometricObjectElementCollide collidePS1;
    public OpenSpace.PS1.CollideMaterial colPS1 => collidePS1?.GetMaterial(index)?.collideMaterial;

    public int? index;

    public CollideType type = CollideType.None;

    public CollideMaterial.CollisionFlags_R2 CollisionFlagsR2 {
        get {
            var flags = CollideMaterial.CollisionFlags_R2.None;

            if (col != null) flags = col.identifier_R2;
            else if (colROM != null) flags = (CollideMaterial.CollisionFlags_R2)colROM.identifier;
            else if (colPS1 != null) flags = (CollideMaterial.CollisionFlags_R2)colPS1.identifier;
            return flags;
        }
    }
}