using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.Utils;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model.AnimatedExportObjectModelDescription;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model.AnimatedExportObjectModelDescription.Armature;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.ArmatureModelConstructing
{
    public class SkinnedMeshArmatureModelConstructor
    {
        public ArmatureModel ConstructFrom(Matrix4x4[] bindposes, Transform[] bones)
        {
            var result = new ArmatureModel();
            HashSet<TreeBuildingNodeInfo<ArmatureModelNode, string>> ArmatureTreeBuildingNodes = 
                new HashSet<TreeBuildingNodeInfo<ArmatureModelNode, string>>();

            ArmatureTreeBuildingNodes.Add(
                    new TreeBuildingNodeInfo<ArmatureModelNode, string>(
                        null,
                        "ROOT_CHANNEL",
                        new ArmatureModelNode())
                );

            for (int i = 0; i < bones.Length; i++)
            {
                var boneTransform = bones[i];
                var channelGameObject = ObjectsHierarchyHelper.GetProperChannelForTransform(boneTransform).gameObject;
                ArmatureModelNode boneBindPoseTransform = GetBindPoseBoneTransformForBindPoseMatrix(bones[i], bindposes[i]);
                boneBindPoseTransform.boneName = channelGameObject.name;
                var parentChannelName = ObjectsHierarchyHelper.GetParentChannelNameOrNullIfNotPresent(channelGameObject.transform);

                if (parentChannelName == null)
                {
                    parentChannelName = "ROOT_CHANNEL";
                }

                ArmatureTreeBuildingNodes.Add(
                    new TreeBuildingNodeInfo<ArmatureModelNode, string>(
                            parentChannelName,
                            channelGameObject.name,
                            boneBindPoseTransform
                        )
                    );
            }

            var resultTree = (Tree<ArmatureModelNode, string>) result;
            resultTree = Tree<ArmatureModelNode, string>.BuildTreeWithProperNodesPuttingOrder(resultTree, ArmatureTreeBuildingNodes);
            result = (ArmatureModel)resultTree;
            return result;
        }

        private ArmatureModelNode GetBindPoseBoneTransformForBindPoseMatrix(Transform bone, Matrix4x4 bindposeMatrix)
        {
            GameObject boneWorkingDuplicate = UnityEngine.Object.Instantiate(bone.gameObject);
            boneWorkingDuplicate.transform.SetParent(null);

            Matrix4x4 localMatrix = bindposeMatrix.inverse;
            boneWorkingDuplicate.transform.localPosition = localMatrix.MultiplyPoint(Vector3.zero);
            boneWorkingDuplicate.transform.localRotation = Quaternion.LookRotation(localMatrix.GetColumn(2), localMatrix.GetColumn(1));
            boneWorkingDuplicate.transform.localScale =
                new Vector3(localMatrix.GetColumn(0).magnitude, localMatrix.GetColumn(1).magnitude, localMatrix.GetColumn(2).magnitude);

            var result = new ArmatureModelNode();
            result.position = new Model.MathDescription.Vector3d(
                boneWorkingDuplicate.transform.position.x,
                boneWorkingDuplicate.transform.position.y,
                boneWorkingDuplicate.transform.position.z);
            result.rotation = new Model.MathDescription.Quaternion(
                boneWorkingDuplicate.transform.rotation.w,
                boneWorkingDuplicate.transform.rotation.x,
                boneWorkingDuplicate.transform.rotation.y,
                boneWorkingDuplicate.transform.rotation.z
                );
            result.scale = new Model.MathDescription.Vector3d(
                boneWorkingDuplicate.transform.lossyScale.x,
                boneWorkingDuplicate.transform.lossyScale.y,
                boneWorkingDuplicate.transform.lossyScale.z
                );

            UnityEngine.Object.Destroy(boneWorkingDuplicate);
            return result;
        }
    }
}
