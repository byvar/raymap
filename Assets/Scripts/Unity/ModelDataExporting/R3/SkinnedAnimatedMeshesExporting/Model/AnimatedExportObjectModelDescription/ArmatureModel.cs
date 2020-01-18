using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model.AnimatedExportObjectModelDescription
{
    public class ArmatureModelNode
    {
        public string boneName;
        public Vector3d position;
        public Quaternion rotation;
        public Vector3d scale;
        public Vector3d localPosition;
        public Quaternion localRotation;
        public Vector3d localScale;
    }

    public class ArmatureModel : Tree<ArmatureModelNode, string>
    {
        
    }
}
