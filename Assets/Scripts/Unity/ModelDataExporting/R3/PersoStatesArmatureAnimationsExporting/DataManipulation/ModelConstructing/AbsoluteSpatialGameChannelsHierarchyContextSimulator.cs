using Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.DataManipulation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.DataManipulation.ModelConstructing
{
    public class AbsoluteSpatialGameChannelsHierarchyContextSimulator
    {
        public void SimulateInSceneAndFillWithAbsoluteOffsets(
            AnimTreeWithChannelsDataHierarchy animTreeWithChannelsDataHierarchy)
        {
            Dictionary<string, GameObject> gameObjectsHierarchyRepresentation = new Dictionary<string, GameObject>();
            foreach (var channel in animTreeWithChannelsDataHierarchy.IterateChannels())
            {
                gameObjectsHierarchyRepresentation.Add(channel.Name, CreateChannelInSceneRepresentation(channel));
            }

            foreach (var parentChildPair in animTreeWithChannelsDataHierarchy.IterateParentChildPairs())
            {
                if (parentChildPair.Parent != null)
                {
                    ParentChildTo(gameObjectsHierarchyRepresentation[parentChildPair.Child.Name],
                    gameObjectsHierarchyRepresentation[parentChildPair.Parent.Name]);
                }
            }

            foreach (var channel in animTreeWithChannelsDataHierarchy.IterateChannels())
            {
                SetChannelTransform(gameObjectsHierarchyRepresentation[channel.Name], channel);
            }

            foreach (var channel in animTreeWithChannelsDataHierarchy.IterateChannels())
            {
                FillChannelModelWithAbsoluteOffsets(channel, gameObjectsHierarchyRepresentation[channel.Name]);
            }

            foreach (var channel in animTreeWithChannelsDataHierarchy.IterateChannels())
            {
                DestroyChannelRepresentation(gameObjectsHierarchyRepresentation[channel.Name]);
            }

            gameObjectsHierarchyRepresentation.Clear();
        }

        private GameObject CreateChannelInSceneRepresentation(AnimTreeChannelsHierarchyNode channel)
        {
            return new GameObject(channel.Name);
        }

        private void ParentChildTo(GameObject child, GameObject parent)
        {
            child.transform.SetParent(parent.transform);
        }

        private void SetChannelTransform(GameObject channel, AnimTreeChannelsHierarchyNode channelModel)
        {
            channel.transform.localPosition = channelModel.LocalPosition;
            channel.transform.localRotation = channelModel.LocalRotation;
            channel.transform.localScale = channelModel.LocalScale;
        }

        private void FillChannelModelWithAbsoluteOffsets(AnimTreeChannelsHierarchyNode channelModel, GameObject channel)
        {
            channelModel.Position = channel.transform.position;
            channelModel.Rotation = channel.transform.rotation;
            channelModel.Scale = channel.transform.lossyScale;
        }

        private void DestroyChannelRepresentation(GameObject channel)
        {
            UnityEngine.Object.Destroy(channel);
        }
    }
}