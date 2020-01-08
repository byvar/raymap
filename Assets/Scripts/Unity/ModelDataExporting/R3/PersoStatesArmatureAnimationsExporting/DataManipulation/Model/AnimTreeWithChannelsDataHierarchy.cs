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
        public class ParentChildPair
        {
            public AnimTreeChannelsHierarchyNode Parent;
            public AnimTreeChannelsHierarchyNode Child;

            public ParentChildPair(AnimTreeChannelsHierarchyNode Parent, AnimTreeChannelsHierarchyNode Child)
            {
                this.Parent = Parent;
                this.Child = Child;
            }
        }

        AnimTreeChannelsHierarchyNode root;

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
            throw new NotImplementedException();
        }

        public IEnumerable<AnimTreeChannelsHierarchyNode> IterateChannels()
        {
            List<AnimTreeChannelsHierarchyNode> nodes = new List<AnimTreeChannelsHierarchyNode>();
            if (root != null)
            {
                root.TraverseAndCollectAll(nodes);
            }
            foreach (var node in nodes)
            {
                yield return node;
            }
        }

        public IEnumerable<ParentChildPair> IterateParentChildPairs()
        {
            List<ParentChildPair> pairs = new List<ParentChildPair>();
            if (root != null)
            {
                root.TraverseChildParentPairsAndCollectAll(pairs);
            }
            foreach (var pair in pairs)
            {
                yield return pair;
            }
        }
    }
}
