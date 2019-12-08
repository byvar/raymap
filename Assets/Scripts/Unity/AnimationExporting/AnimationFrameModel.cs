using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Unity.AnimationExporting
{
    public class AnimationFrameModel
    {
        List<AnimationFrameModelNode> nodes = new List<AnimationFrameModelNode>();

        private AnimationFrameModelNode constructNode(string boneName,
            float positionX, float positionY, float positionZ, float rotationX,
            float rotationY, float rotationZ,
            float scaleX, float scaleY, float scaleZ)
        {
            return new AnimationFrameModelNode(boneName, positionX, positionY, positionZ, rotationX, rotationY, rotationZ,
                scaleX, scaleY, scaleZ);
        }

        private void addNode(string parentBoneName, AnimationFrameModelNode node)
        {
            foreach (AnimationFrameModelNode child in nodes)
            {
                bool successfullyAdded = child.addChild(parentBoneName, node);
                if (successfullyAdded)
                {
                    return;
                }
            }
            throw new KeyNotFoundException("Could not find given parent bone to insert under it as a child bone!");
        }

        public void addNode(string parentBoneName, string boneName,
            float positionX, float positionY, float positionZ, float rotationX,
            float rotationY, float rotationZ,
            float scaleX, float scaleY, float scaleZ)
        {
            AnimationFrameModelNode node = constructNode(
                boneName, positionX, positionY, positionZ, rotationX, rotationY, rotationZ,
                scaleX, scaleY, scaleZ);

            if (parentBoneName == null)
            {
                nodes.Add(node);
            } else
            {
                addNode(parentBoneName, node);
            }
        }
    }
}
