using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Unity.ModelDataExporting.AnimationExporting.DataManipulation.Model
{
    public class AnimTreeWithChannelsDataHierarchy
    {
        AnimTreeChannelsHierarchyNode root;

        internal void AddNode(
            string parentChannelName, 
            string channelName,
            Vector3 absolutePosition,
            Quaternion absoluteRotation,
            Vector3 absoluteScale,
            Vector3 localPosition,
            Quaternion localRotation,
            Vector3 localScale)
        {
            throw new NotImplementedException();
        }
    }
}
