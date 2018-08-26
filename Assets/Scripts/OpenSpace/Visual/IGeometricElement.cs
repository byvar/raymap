using UnityEngine;

namespace OpenSpace.Visual {
    /// <summary>
    /// Subblocks of a geometric object / R3Mesh
    /// </summary>
    public interface IGeometricElement {
        IGeometricElement Clone(MeshObject mesh);
        GameObject Gao {
            get;
        }
    }
}
