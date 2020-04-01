using UnityEngine;

namespace OpenSpace.ROM {
    /// <summary>
    /// Elements of a geometric object
    /// </summary>
    public interface IGeometricObjectElementCollide {
        GameMaterial GetMaterial(int? index);
    }
}
