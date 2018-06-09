using UnityEngine;

namespace OpenSpace.Visual {
    /// <summary>
    /// Any visual set element of a physical object
    /// </summary>
    public interface IGeometricObject {
        IGeometricObject Clone();
    }
}
