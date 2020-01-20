using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model.AnimatedExportObjectModelDescription;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model.AnimatedExportObjectModelDescription.Armature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model
{
    public class AnimatedExportObjectModel
    {
        public string Name;
        public TransformModel transform;
        public MeshGeometry meshGeometry;
        public ArmatureModel armatureModel;
    }
}
