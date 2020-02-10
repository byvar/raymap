using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.DataManipulation.Model;
using OpenSpace.Animation.Component;
using UnityEngine;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.DataManipulation.OpenspaceInterfaces
{
    public class AnimA3DGeneralDataManipulationInterface
    {
        private AnimA3DGeneral animA3DGeneral;

        public AnimA3DGeneralDataManipulationInterface(AnimA3DGeneral animA3DGeneral)
        {
            this.animA3DGeneral = animA3DGeneral;
        }

        public IEnumerable<AnimHierarchyWithChannelInfo> IterateAnimHierarchiesWithChannelInfosForGivenFrame(int animationFrameNumber)
        {
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

                int framesSinceKF = (int)animationFrameNumber - (int)openspaceKeyframe.frame;

                AnimKeyframe nextKF = null;
                int framesDifference;
                float interpolation;
                if (openspaceKeyframe.IsEndKeyframe)
                {
                    AnimFramesKFIndex next_kfi = animA3DGeneral.framesKFIndex[0 + channel.framesKF];
                    nextKF = animA3DGeneral.keyframes[next_kfi.kf];
                    framesDifference = animA3DGeneral.num_onlyFrames - 1 + (int)nextKF.frame - (int)openspaceKeyframe.frame;
                    if (framesDifference == 0)
                    {
                        interpolation = 0;
                    }
                    else
                    {
                        interpolation = framesSinceKF / (float)framesDifference;
                    }
                }
                else
                {
                    nextKF = animA3DGeneral.keyframes[openspaceKeyframeIndex.kf + 1];
                    framesDifference = (int)nextKF.frame - (int)openspaceKeyframe.frame;
                    interpolation = framesSinceKF / (float)framesDifference;
                }
                AnimVector pos2 = animA3DGeneral.vectors[nextKF.positionVector];
                AnimQuaternion qua2 = animA3DGeneral.quaternions[nextKF.quaternion];
                AnimVector scl2 = animA3DGeneral.vectors[nextKF.scaleVector];
                localPosition = Vector3.Lerp(openspaceKeyframePositionVector.vector, pos2.vector, interpolation);
                localRotation = Quaternion.Lerp(openspaceKeyframeRotationQuaternion.quaternion, qua2.quaternion, interpolation);
                localScale = Vector3.Lerp(openspaceKeyframeScaleVector.vector, scl2.vector, interpolation);

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