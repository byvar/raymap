using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Unity.ModelDataExporting.MathDescription;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.ArmatureModelConstructing;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.MeshGeometryConstructing;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.ModelConversion;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.Utils;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model.AnimatedExportObjectModelDescription;
using UnityEngine;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.ModelConversion
{
    public class SkinnedR3MeshToAnimatedExportObjectModelConverter : R3MeshToAnimatedExportObjectModelConverterBase
    {
        protected override MeshGeometry DeriveMeshGeometryData(Mesh mesh, Transform[] bones)
        {
            return new SkinnedMeshGeometryDataConstructor().ConstructFrom(mesh, bones);
        }

        protected override Dictionary<string, TransformModel> GetBonesBindPoseTransforms(R3AnimatedMesh r3AnimatedMesh)
        {
            var result = new Dictionary<string, TransformModel>();
            Transform[] bones = r3AnimatedMesh.GetComponent<SkinnedMeshRenderer>().bones;
            Matrix4x4[] bindposes = r3AnimatedMesh.GetComponent<SkinnedMeshRenderer>().sharedMesh.bindposes;
            for (int i = 0; i < bones.Length; i++)
            {
                var boneTransform = bones[i];
                var channelGameObject = ObjectsHierarchyHelper.GetProperChannelForTransform(boneTransform).gameObject;
                BoneBindPose boneBindPoseTransform = BoneBindPoseHelper.GetBindPoseBoneTransformForBindPoseMatrix(bones[i], bindposes[i]);
                boneBindPoseTransform.boneName = channelGameObject.name;
                result.Add(
                    boneBindPoseTransform.boneName,
                    new TransformModel(
                        boneBindPoseTransform.position,
                        boneBindPoseTransform.rotation,
                        boneBindPoseTransform.scale,
                        new Vector3d(0.0f, 0.0f, 0.0f),
                        new MathDescription.Quaternion(1.0f, 0.0f, 0.0f, 0.0f),
                        new Vector3d(1.0f, 1.0f, 1.0f)
                        )
                    );

            }
            return result;
        }

        protected override Mesh GetMesh(R3AnimatedMesh r3AnimatedMesh)
        {
            return r3AnimatedMesh.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        }

        protected override Transform[] GetBonesTransforms(R3AnimatedMesh r3AnimatedMesh)
        {
            return r3AnimatedMesh.GetComponent<SkinnedMeshRenderer>().bones;
        }
    }
}
