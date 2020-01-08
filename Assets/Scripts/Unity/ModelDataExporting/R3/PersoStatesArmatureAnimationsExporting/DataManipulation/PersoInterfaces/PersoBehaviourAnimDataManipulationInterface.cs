using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Unity.ModelDataExporting.AnimationExporting.DataManipulation.Model;
using OpenSpace.Animation.Component;
using UnityEngine;

namespace Assets.Scripts.Unity.ModelDataExporting.AnimationExporting.DataManipulation.PersoInterfaces
{
    class PersoBehaviourAnimDataManipulationInterface
    {
        private PersoBehaviour persoBehaviour;

        public PersoBehaviourAnimDataManipulationInterface(PersoBehaviour persoBehaviour)
        {
            this.persoBehaviour = persoBehaviour;
        }

        public IEnumerable<AnimHierarchyWithChannelInfo> IterateAnimHierarchiesWithChannelInfosForGivenFrame(int animationFrameNumber)
        {
            AnimA3DGeneral animA3DGeneral = persoBehaviour.a3d;

            for (int i = 0; i < animA3DGeneral.num_channels; i++)
            {
                AnimChannel channel = animA3DGeneral.channels[animA3DGeneral.start_channels + i];
                AnimFramesKFIndex openspaceKeyframeIndex = animA3DGeneral.framesKFIndex[animationFrameNumber + channel.framesKF];
                AnimKeyframe openspaceKeyframe = animA3DGeneral.keyframes[openspaceKeyframeIndex.kf];
                AnimVector openspaceKeyframePositionVector = animA3DGeneral.vectors[openspaceKeyframe.positionVector];
                AnimQuaternion openspaceKeyframeRotationQuaternion = animA3DGeneral.quaternions[openspaceKeyframe.quaternion];
                AnimVector openspaceKeyframeScaleVector = animA3DGeneral.vectors[openspaceKeyframe.scaleVector];
                Vector3 localPosition = openspaceKeyframePositionVector.vector;
                Quaternion localRotation = openspaceKeyframeRotationQuaternion.quaternion;
                Vector3 localScale = openspaceKeyframeScaleVector.vector;

                string channelName = "Channel " + channel.id;
                var channelInHierarchyInfo = GetParentChannelName(channel.id, animationFrameNumber, animA3DGeneral);

                if (channelInHierarchyInfo.found)
                {
                    yield return new AnimHierarchyWithChannelInfo(
                    channelInHierarchyInfo.parentName,
                    channelName,
                    localPosition,
                    localRotation,
                    localScale);
                }                
            }
        }

        class ChannelInHierarchyInfo
        {
            public bool found;
            public string parentName;

            public ChannelInHierarchyInfo(bool found, string parentName)
            {
                this.found = found;
                this.parentName = parentName;
            }
        }

        private ChannelInHierarchyInfo GetParentChannelName(short channelId, int animationFrameNumber, AnimA3DGeneral animA3DGeneral)
        {
            bool existsAtAll = false;
            AnimOnlyFrame animOnlyFrame = animA3DGeneral.onlyFrames[animA3DGeneral.start_onlyFrames + animationFrameNumber];
            for (int i = animOnlyFrame.start_hierarchies_for_frame;
                i < animOnlyFrame.start_hierarchies_for_frame + animOnlyFrame.num_hierarchies_for_frame; i++)
            {
                AnimHierarchy hierarchy = animA3DGeneral.hierarchies[i];
                if (hierarchy.childChannelID == channelId)
                {
                    return new ChannelInHierarchyInfo(true, "Channel " + hierarchy.parentChannelID);
                }

                if (hierarchy.childChannelID == channelId || hierarchy.parentChannelID == channelId)
                {
                    existsAtAll = true;
                }
            }
            return new ChannelInHierarchyInfo(existsAtAll, null);
        }
    }
}
