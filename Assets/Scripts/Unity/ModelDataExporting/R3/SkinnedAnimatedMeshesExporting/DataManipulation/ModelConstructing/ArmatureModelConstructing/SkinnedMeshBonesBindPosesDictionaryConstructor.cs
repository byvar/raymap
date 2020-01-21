using System.Collections.Generic;
using Assets.Scripts.Unity.ModelDataExporting.MathDescription;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.Utils;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model.AnimatedExportObjectModelDescription;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.ArmatureModelConstructing
{
    public class SkinnedMeshBonesBindPosesDictionaryConstructor
    {
        public Dictionary<string, BoneBindPose> ConstructFrom(Matrix4x4[] bindposes, Transform[] bones)
        {
            var result = new Dictionary<string, BoneBindPose>();
            for (int i = 0; i < bones.Length; i++)
            {
                var boneTransform = bones[i];
                var channelGameObject = ObjectsHierarchyHelper.GetProperChannelForTransform(boneTransform).gameObject;
                BoneBindPose boneBindPoseTransform = GetBindPoseBoneTransformForBindPoseMatrix(bones[i], bindposes[i]);
                boneBindPoseTransform.boneName = channelGameObject.name;
                result.Add(channelGameObject.name, boneBindPoseTransform);
            }
            return result;
        }

        private BoneBindPose GetBindPoseBoneTransformForBindPoseMatrix(Transform bone, Matrix4x4 bindposeMatrix)
        {
            GameObject boneWorkingDuplicate = UnityEngine.Object.Instantiate(bone.gameObject);
            boneWorkingDuplicate.transform.SetParent(null);

            Matrix4x4 localMatrix = bindposeMatrix.inverse;
            boneWorkingDuplicate.transform.localPosition = localMatrix.MultiplyPoint(Vector3.zero);
            boneWorkingDuplicate.transform.localRotation = UnityEngine.Quaternion.LookRotation(localMatrix.GetColumn(2), localMatrix.GetColumn(1));
            boneWorkingDuplicate.transform.localScale =
                new Vector3(localMatrix.GetColumn(0).magnitude, localMatrix.GetColumn(1).magnitude, localMatrix.GetColumn(2).magnitude);

            var result = 
                new BoneBindPose(
                    "", 
                    new Vector3d(0.0f, 0.0f, 0.0f),
                    new MathDescription.Quaternion(1.0f, 0.0f, 0.0f, 0.0f),
                    new Vector3d(1.0f, 1.0f, 1.0f));
            result.position = new Vector3d(
                boneWorkingDuplicate.transform.position.x,
                boneWorkingDuplicate.transform.position.y,
                boneWorkingDuplicate.transform.position.z);
            result.rotation = new MathDescription.Quaternion(
                boneWorkingDuplicate.transform.rotation.w,
                boneWorkingDuplicate.transform.rotation.x,
                boneWorkingDuplicate.transform.rotation.y,
                boneWorkingDuplicate.transform.rotation.z
                );
            result.scale = new Vector3d(
                boneWorkingDuplicate.transform.lossyScale.x,
                boneWorkingDuplicate.transform.lossyScale.y,
                boneWorkingDuplicate.transform.lossyScale.z
                );

            UnityEngine.Object.Destroy(boneWorkingDuplicate);
            return result;
        }
    }
}
