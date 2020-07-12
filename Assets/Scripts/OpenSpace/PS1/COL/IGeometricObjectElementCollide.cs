using UnityEngine;

namespace OpenSpace.PS1 {
    /// <summary>
    /// Elements of a geometric object
    /// </summary>
    public interface IGeometricObjectElementCollide {
        GameMaterial GetMaterial(int? index);
    }
}
