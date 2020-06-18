using Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.DataManipulation.Model;
using Assets.Scripts.Utils;
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
        Queue<TreeBuildingNodeInfo<AnimTreeChannelsHierarchyNode, string>> nodesToBuildResultFrom = 
            new Queue<TreeBuildingNodeInfo<AnimTreeChannelsHierarchyNode, string>>();

        public AnimTreeWithChannelsDataHierarchyBuilder()
        {
            result.AddNode(
                null,
                "ROOT_CHANNEL",
                false,
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
            nodesToBuildResultFrom.Enqueue(
                new TreeBuildingNodeInfo<AnimTreeChannelsHierarchyNode, string>(
                        animHierarchy.ParentChannelName,
                        animHierarchy.ChannelName,
                        new AnimTreeChannelsHierarchyNode(
                                animHierarchy.ChannelName,
                                animHierarchy.IsKeyframe,
                                animHierarchy.LocalPosition,
                                animHierarchy.LocalRotation,
                                animHierarchy.LocalScale,
                                new Vector3(0.0f, 0.0f, 0.0f),
                                new Quaternion(0.0f, 0.0f, 0.0f, 1.0f),
                                new Vector3(1.0f, 1.0f, 1.0f)
                            )
                    )
                );
        }

        public AnimTreeWithChannelsDataHierarchy Build()
        {
            var resultTree = (Assets.Scripts.Utils.Tree<AnimTreeChannelsHierarchyNode, string>)result;
            resultTree = Assets.Scripts.Utils.Tree<AnimTreeChannelsHierarchyNode, string>.BuildTreeWithProperNodesPuttingOrder(
                resultTree,
                nodesToBuildResultFrom);
            result = (AnimTreeWithChannelsDataHierarchy)resultTree;
            var absoluteSpatialGameChannelsHierarchyContextSimulator = new AbsoluteSpatialGameChannelsHierarchyContextSimulator();
            absoluteSpatialGameChannelsHierarchyContextSimulator.SimulateInSceneAndFillWithAbsoluteOffsets(result);
            return result;
        }
    }
}