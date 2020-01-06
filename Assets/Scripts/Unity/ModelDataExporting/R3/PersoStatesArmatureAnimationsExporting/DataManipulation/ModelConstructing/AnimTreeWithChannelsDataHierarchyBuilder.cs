using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Unity.ModelDataExporting.AnimationExporting.DataManipulation.Model;
using Assets.Scripts.Unity.ModelDataExporting.R3.AnimationExporting.DataManipulation.ModelConstructing;
using UnityEngine;

namespace Assets.Scripts.Unity.ModelDataExporting.AnimationExporting.DataManipulation.ModelConstructing
{
    class AnimTreeWithChannelsDataHierarchyBuilder
    {
        AnimTreeWithChannelsDataHierarchy result = new AnimTreeWithChannelsDataHierarchy();

        public AnimTreeWithChannelsDataHierarchyBuilder()
        {
            result.AddNode(
                null,
                "ROOT_CHANNEL",
                new Vector3(0.0f, 0.0f, 0.0f),
                new Quaternion(1.0f, 0.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 1.0f, 1.0f),
                new Vector3(0.0f, 0.0f, 0.0f),
                new Quaternion(1.0f, 0.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 1.0f, 1.0f)
            );
        }

        public void AddAnimHierarchyWithChannelInfo(AnimHierarchyWithChannelInfo animHierarchy)
        {
            result.AddNode(
                animHierarchy.ParentChannelName,
                animHierarchy.ChannelName,
                new Vector3(0.0f, 0.0f, 0.0f),
                new Quaternion(1.0f, 0.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 1.0f, 1.0f),
                animHierarchy.LocalPosition,
                animHierarchy.LocalRotation,
                animHierarchy.LocalScale
            );
        }

        public AnimTreeWithChannelsDataHierarchy Build()
        {
            var absoluteSpatialGameChannelsHierarchyContextSimulator = new AbsoluteSpatialGameChannelsHierarchyContextSimulator();
            result = absoluteSpatialGameChannelsHierarchyContextSimulator.SimulateInSceneAndFillWithAbsoluteOffsets(result);
            return result;
        }
    }
}
