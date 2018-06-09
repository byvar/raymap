using UnityEngine;

namespace OpenSpace.Collide {
    /// <summary>
    /// Subblocks of a geometric object / R3Mesh
    /// </summary>
    public interface ICollideGeometricElement {
        ICollideGeometricElement Clone(CollideMeshObject mesh);
    }
}
