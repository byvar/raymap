using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Visual {
    public struct VisualSetLOD {
        public float LODdistance;
        public LegacyPointer off_data;
        public IGeometricObject obj;
    }
}
