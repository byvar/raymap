using Assets.Scripts.Unity.ModelDataExporting.MathDescription;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.Utils;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model.AnimatedExportObjectModelDescription;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model.AnimatedExportObjectModelDescription.MaterialsDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.ModelConversion
{
    public abstract class R3MeshToAnimatedExportObjectModelConverterBase
    {
        public AnimatedExportObjectModel convert(R3AnimatedMesh r3AnimatedMesh)
        {
            AnimatedExportObjectModel result = new AnimatedExportObjectModel();
            result.Name = r3AnimatedMesh.gameObject.name;
            Dictionary<string, BoneBindPose> bonesBindPoses = 
                GetBonesBindPoseTransforms(r3AnimatedMesh).
                ToDictionary(
                    x => x.Key,
                    x => new BoneBindPose(
                        x.Key,
                        x.Value.position,
                        x.Value.rotation,
                        x.Value.scale
                        ));
            MeshGeometry meshGeometry = DeriveMeshGeometryData(GetMesh(r3AnimatedMesh), GetBonesTransforms(r3AnimatedMesh));
            TransformModel transformModel = GetTransformModel(r3AnimatedMesh.transform);
            result.bindBonePoses = bonesBindPoses;
            result.meshGeometry = meshGeometry;
            result.transform = transformModel;
            result.materials = GetMaterials(r3AnimatedMesh);
            return result;
        }

        protected List<Model.AnimatedExportObjectModelDescription.MaterialsDescription.Material> GetMaterials(R3AnimatedMesh r3AnimatedMesh)
        {
            var result = new List<Model.AnimatedExportObjectModelDescription.MaterialsDescription.Material>();
            var unityMaterialConverter = new UnityMaterialToMaterialDescriptionConverter();
            foreach (var unityMaterial in r3AnimatedMesh.GetComponent<Renderer>().materials)
            {
                result.Add(unityMaterialConverter.MaterialDescriptionFromUnityMaterial(unityMaterial));
            }
            return result;
        }

        protected TransformModel GetTransformModel(Transform transform)
        {
            var result = new TransformModel(
                new Vector3d(transform.position.x, transform.position.y, transform.position.z),
                new MathDescription.Quaternion(transform.rotation.w, transform.rotation.x, transform.rotation.y, transform.rotation.z),
                new Vector3d(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z),
                new Vector3d(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z),
                new MathDescription.Quaternion(transform.localRotation.w, transform.localRotation.x, transform.localRotation.y, transform.localRotation.z),
                new Vector3d(transform.localScale.x, transform.localScale.y, transform.localScale.z)
                );
            return result;
        }

        protected abstract Dictionary<string, TransformModel> GetBonesBindPoseTransforms(R3AnimatedMesh r3AnimatedMesh);
        protected abstract MeshGeometry DeriveMeshGeometryData(Mesh mesh, Transform[] bones);

        protected abstract Mesh GetMesh(R3AnimatedMesh r3AnimatedMesh);
        protected abstract Transform[] GetBonesTransforms(R3AnimatedMesh r3AnimatedMesh);
    }
}
