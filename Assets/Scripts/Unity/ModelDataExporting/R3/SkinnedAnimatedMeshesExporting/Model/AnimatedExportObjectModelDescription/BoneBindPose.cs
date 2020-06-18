using Assets.Scripts.Unity.ModelDataExporting.MathDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model.AnimatedExportObjectModelDescription
{
    public class BoneBindPose
    {
        public Vector3d position;
        public Quaternion rotation;
        public Vector3d scale;
        public string boneName;

        public BoneBindPose(string boneName, Vector3d position, Quaternion rotation, Vector3d scale)
        {
            this.boneName = boneName;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }
    }
}
