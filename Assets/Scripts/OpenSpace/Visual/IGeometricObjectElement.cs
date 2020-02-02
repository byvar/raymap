using UnityEngine;

namespace OpenSpace.Visual {
    /// <summary>
    /// Subblocks of a geometric object
    /// </summary>
    public interface IGeometricObjectElement {
        IGeometricObjectElement Clone(GeometricObject mesh);
        GameObject Gao {
            get;
        }
    }
}
