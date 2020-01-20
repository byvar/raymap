using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Unity.ModelDataExporting.MathDescription;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.ArmatureModelConstructing;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.MeshGeometryConstructing;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model.AnimatedExportObjectModelDescription;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model.AnimatedExportObjectModelDescription.Armature;
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
            ArmatureModel armatureModel = ConstructArmatureModel(
                skinnedMeshRendererComponent.sharedMesh.bindposes,
                skinnedMeshRendererComponent.bones);
            MeshGeometry meshGeometry = DeriveMeshGeometryData(skinnedMeshRendererComponent.sharedMesh, skinnedMeshRendererComponent.bones);
            TransformModel transformModel = GetTransformModel(r3AnimatedMesh.transform);
            result.armatureModel = armatureModel;
            result.meshGeometry = meshGeometry;
            result.transform = transformModel;
            return result;
        }

        private TransformModel GetTransformModel(Transform transform)
        {
            var result = new TransformModel();
            result.position = new Vector3d(transform.position.x, transform.position.y, transform.position.z);
            result.rotation = new MathDescription.Quaternion(transform.rotation.w, transform.rotation.x, transform.rotation.y, transform.rotation.z);
            result.scale = new Vector3d(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
            result.localPosition = new Vector3d(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
            result.localRotation = new MathDescription.Quaternion(transform.localRotation.w, transform.localRotation.x, transform.localRotation.y, transform.localRotation.z);
            result.localScale = new Vector3d(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            return result;
        }

        private MeshGeometry DeriveMeshGeometryData(Mesh sharedMesh, Transform[] bones)
        {
            return new SkinnedMeshGeometryDataConstructor().ConstructFrom(sharedMesh, bones);
        }

        private ArmatureModel ConstructArmatureModel(Matrix4x4[] bindposes, Transform[] bones)
        {
            return new SkinnedMeshArmatureModelConstructor().ConstructFrom(bindposes, bones);
        }
    }
}
