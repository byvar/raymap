using UnityEngine;

namespace OpenSpace.Collide {
    /// <summary>
    /// Subblocks of a geometric object / R3Mesh
    /// </summary>
    public interface IGeometricObjectElementCollide {
        GameObject Gao { get; }
        IGeometricObjectElementCollide Clone(GeometricObjectCollide mesh);
    }
}
