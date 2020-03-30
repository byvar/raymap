using UnityEngine;
using OpenSpace.Collide;

[ExecuteInEditMode]
public class CollideComponent : MonoBehaviour {
    public GeometricObjectElementCollideTriangles collide;
    public CollideMaterial col => collide?.gameMaterial?.collideMaterial;

    public OpenSpace.ROM.GeometricObjectElementCollideTriangles collideROM;
    public OpenSpace.ROM.CollideMaterial colROM {
        get {
            OpenSpace.ROM.ROMStruct val = collideROM?.material?.Value;
            if (val == null) return null;
            if (!(val is OpenSpace.ROM.GameMaterial)) return null;
            OpenSpace.ROM.GameMaterial gmt = val as OpenSpace.ROM.GameMaterial;
            return gmt.collideMaterial.Value;
        }
    }
}