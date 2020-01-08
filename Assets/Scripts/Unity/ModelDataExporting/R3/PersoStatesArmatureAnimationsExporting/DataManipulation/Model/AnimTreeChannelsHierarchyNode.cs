using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Unity.ModelDataExporting.AnimationExporting.DataManipulation.Model
{
    public class AnimTreeChannelsHierarchyNode
    {
        public string Name;
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
        public Vector3 LocalScale;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
        List<AnimTreeChannelsHierarchyNode> children;

        public void TraverseAndCollectAll(List<AnimTreeChannelsHierarchyNode> nodes)
        {
            nodes.Add(this);

            foreach (var child in children)
            {
                child.TraverseAndCollectAll(nodes);
            }
        }

        public void TraverseChildParentPairsAndCollectAll(List<AnimTreeWithChannelsDataHierarchy.ParentChildPair> pairs)
        {
            foreach (var child in children)
            {
                pairs.Add(new AnimTreeWithChannelsDataHierarchy.ParentChildPair(this, child));
            }
            foreach (var child in children)
            {
                child.TraverseChildParentPairsAndCollectAll(pairs);
            }
        }
    }
}
