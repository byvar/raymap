using Assets.Scripts.Unity.ModelDataExporting.MathDescription;
using Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.DataManipulation.Model;
using Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.DataManipulation.ModelConstructing
{
    public class AnimTreeWithChannelsDataHierarchyToAnimationFrameModelConverter
    {
        public static AnimationFrameModel Convert(AnimTreeWithChannelsDataHierarchy hierarchy)
        {
            AnimationFrameModel result = new AnimationFrameModel();

            result.AddNode(
                null,
                hierarchy.GetRoot().Name,
                new AnimationFrameModelNode(
                    hierarchy.GetRoot().Name,
                    hierarchy.GetRoot().IsKeyframe,
                    new Vector3d(
                        hierarchy.GetRoot().Position.x,
                        hierarchy.GetRoot().Position.y,
                        hierarchy.GetRoot().Position.z),
                    new Vector3d(
                        hierarchy.GetRoot().LocalPosition.x,
                        hierarchy.GetRoot().LocalPosition.y,
                        hierarchy.GetRoot().LocalPosition.z),
                    new Quaternion(
                        hierarchy.GetRoot().Rotation.w,
                        hierarchy.GetRoot().Rotation.x,
                        hierarchy.GetRoot().Rotation.y,
                        hierarchy.GetRoot().Rotation.z),
                    new Quaternion(
                        hierarchy.GetRoot().LocalRotation.w,
                        hierarchy.GetRoot().LocalRotation.x,
                        hierarchy.GetRoot().LocalRotation.y,
                        hierarchy.GetRoot().LocalRotation.z),
                    new Vector3d(
                        hierarchy.GetRoot().Scale.x,
                        hierarchy.GetRoot().Scale.y,
                        hierarchy.GetRoot().Scale.z),
                    new Vector3d(
                        hierarchy.GetRoot().LocalScale.x,
                        hierarchy.GetRoot().LocalScale.y,
                        hierarchy.GetRoot().LocalScale.z),
                    true)
               );

            foreach (var childParentPair in hierarchy.IterateParentChildPairs())
            {
                result.AddNode(
                    childParentPair.Parent.Name,
                    childParentPair.Child.Name,
                    new AnimationFrameModelNode(
                        childParentPair.Child.Name,
                        childParentPair.Child.IsKeyframe,
                        new Vector3d(
                            childParentPair.Child.Position.x,
                            childParentPair.Child.Position.y,
                            childParentPair.Child.Position.z),
                        new Vector3d(
                            childParentPair.Child.LocalPosition.x,
                            childParentPair.Child.LocalPosition.y,
                            childParentPair.Child.LocalPosition.z),
                        new Quaternion(
                            childParentPair.Child.Rotation.w,
                            childParentPair.Child.Rotation.x,
                            childParentPair.Child.Rotation.y,
                            childParentPair.Child.Rotation.z),
                        new Quaternion(
                            childParentPair.Child.LocalRotation.w,
                            childParentPair.Child.LocalRotation.x,
                            childParentPair.Child.LocalRotation.y,
                            childParentPair.Child.LocalRotation.z),
                        new Vector3d(
                            childParentPair.Child.Scale.x,
                            childParentPair.Child.Scale.y,
                            childParentPair.Child.Scale.z),
                        new Vector3d(
                            childParentPair.Child.LocalScale.x,
                            childParentPair.Child.LocalScale.y,
                            childParentPair.Child.LocalScale.z),
                            true));
            }

            return result;
        }
    }
}