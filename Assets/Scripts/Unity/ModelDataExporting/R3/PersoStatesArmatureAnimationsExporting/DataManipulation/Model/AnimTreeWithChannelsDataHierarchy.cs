using Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.DataManipulation.ModelConstructing;
using Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.Model;
using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.DataManipulation.Model
{
    public class AnimTreeWithChannelsDataHierarchy : Tree<AnimTreeChannelsHierarchyNode, string>
    {
        public void AddNode(
            string parentChannelName,
            string channelName,
            Vector3 absolutePosition,
            Quaternion absoluteRotation,
            Vector3 absoluteScale,
            Vector3 localPosition,
            Quaternion localRotation,
            Vector3 localScale)
        {
            AnimTreeChannelsHierarchyNode node = new AnimTreeChannelsHierarchyNode(
                channelName,
                localPosition,
                localRotation,
                localScale,
                absolutePosition,
                absoluteRotation,
                absoluteScale
                );
            AddNode(parentChannelName, channelName, node);
        }

        public AnimationFrameModel ToAnimationFrameModel()
        {
            return AnimTreeWithChannelsDataHierarchyToAnimationFrameModelConverter.Convert(this);
        }

        public IEnumerable<AnimTreeChannelsHierarchyNode> IterateChannels()
        {
            foreach (var Channel in IterateNodes())
            {
                yield return Channel.Node;
            }
        }
    }
}