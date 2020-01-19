using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model.MathDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model.AnimatedExportObjectModelDescription.Armature
{
    public class ArmatureModelNode
    {
        public string boneName;
        public Vector3d position = new Vector3d(0, 0, 0);
        public Quaternion rotation = new Quaternion(1.0f, 0.0f, 0.0f, 0.0f);
        public Vector3d scale = new Vector3d(1.0f, 1.0f, 1.0f);
    }
}
