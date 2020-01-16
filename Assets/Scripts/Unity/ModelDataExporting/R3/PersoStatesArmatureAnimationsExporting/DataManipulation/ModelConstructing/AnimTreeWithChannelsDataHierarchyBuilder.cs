using Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.DataManipulation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.DataManipulation.ModelConstructing
{
    public class AnimTreeWithChannelsDataHierarchyBuilder
    {
        AnimTreeWithChannelsDataHierarchy result = new AnimTreeWithChannelsDataHierarchy();
        HashSet<AnimHierarchyWithChannelInfo> nodesToBuildResultFrom = new HashSet<AnimHierarchyWithChannelInfo>();

        public AnimTreeWithChannelsDataHierarchyBuilder()
        {
            result.AddNode(
                null,
                "ROOT_CHANNEL",
                new Vector3(0.0f, 0.0f, 0.0f),
                new Quaternion(0.0f, 0.0f, 0.0f, 1.0f),
                new Vector3(1.0f, 1.0f, 1.0f),
                new Vector3(0.0f, 0.0f, 0.0f),
                new Quaternion(0.0f, 0.0f, 0.0f, 1.0f),
                new Vector3(1.0f, 1.0f, 1.0f)
            );
        }

        public void AddAnimHierarchyWithChannelInfo(AnimHierarchyWithChannelInfo animHierarchy)
        {
            animHierarchy.ParentChannelName = animHierarchy.ParentChannelName != null ? animHierarchy.ParentChannelName : "ROOT_CHANNEL";
            nodesToBuildResultFrom.Add(animHierarchy);
        }

        private void BuildTreeWithProperNodesPuttingOrder()
        {
            while (nodesToBuildResultFrom.Count != 0)
            {
                foreach (var node in nodesToBuildResultFrom)
                {
                    if (result.Contains(node.ParentChannelName))
                    {
                        result.AddNode(
                            node.ParentChannelName,
                            node.ChannelName,
                            new Vector3(0.0f, 0.0f, 0.0f),
                            new Quaternion(0.0f, 0.0f, 0.0f, 1.0f),
                            new Vector3(1.0f, 1.0f, 1.0f),
                            node.LocalPosition,
                            node.LocalRotation,
                            node.LocalScale
                        );
                        nodesToBuildResultFrom.Remove(node);
                        break;
                    }
                }
            }
        }

        public AnimTreeWithChannelsDataHierarchy Build()
        {
            BuildTreeWithProperNodesPuttingOrder();
            var absoluteSpatialGameChannelsHierarchyContextSimulator = new AbsoluteSpatialGameChannelsHierarchyContextSimulator();
            absoluteSpatialGameChannelsHierarchyContextSimulator.SimulateInSceneAndFillWithAbsoluteOffsets(result);
            return result;
        }
    }
}