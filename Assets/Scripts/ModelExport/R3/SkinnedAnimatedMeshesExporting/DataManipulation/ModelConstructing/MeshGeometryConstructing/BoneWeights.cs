using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelExport.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.MeshGeometryConstructing
{
    public class BoneWeights
    {
        public string BoneName;
        public Dictionary<int, float> Weights = new Dictionary<int, float>();
    }
}
