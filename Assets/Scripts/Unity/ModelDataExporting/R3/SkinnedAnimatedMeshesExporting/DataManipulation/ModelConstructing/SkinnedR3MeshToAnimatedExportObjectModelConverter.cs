using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model.AnimatedExportObjectModelDescription;
using UnityEngine;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing
{
    public class SkinnedR3MeshToAnimatedExportObjectModelConverter
    {
        public AnimatedExportObjectModel convert(R3AnimatedMesh r3AnimatedMesh)
        {
            AnimatedExportObjectModel result = new AnimatedExportObjectModel();
            result.Name = r3AnimatedMesh.gameObject.name;
            SkinnedMeshRenderer skinnedMeshRendererComponent = r3AnimatedMesh.GetComponent<SkinnedMeshRenderer>();
            ArmatureModel armatureModel = ConstructArmatureModel(skinnedMeshRendererComponent.bones);
            MeshGeometry meshGeometry = DeriveMeshGeometryData(skinnedMeshRendererComponent.sharedMesh);
            TransformModel transformModel = GetTransformModel(r3AnimatedMesh.transform);
            result.armatureModel = armatureModel;
            result.meshGeometry = meshGeometry;
            result.transform = transformModel;
            return result;
        }

        private TransformModel GetTransformModel(Transform transform)
        {
            throw new NotImplementedException();
        }

        private MeshGeometry DeriveMeshGeometryData(Mesh sharedMesh)
        {
            return new MeshGeometryDataConstructor().ConstructFrom(sharedMesh);
        }

        private ArmatureModel ConstructArmatureModel(Transform[] bones)
        {
            return new ArmatureModelConstructor().ConstructFrom(bones);
        }
    }
}
