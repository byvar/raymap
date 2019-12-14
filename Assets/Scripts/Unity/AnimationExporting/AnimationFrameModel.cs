using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Unity.AnimationExporting
{
    [Serializable]
    public class AnimationFrameModel
    {
        public List<AnimationFrameModelNode> nodes = new List<AnimationFrameModelNode>();

        private AnimationFrameModelNode constructNode(string boneName,
            float positionX, float positionY, float positionZ,
            float localPositionX, float localPositionY, float localPositionZ,
            float rotationX, float rotationY, float rotationZ,
            float localRotationX, float localRotationY, float localRotationZ,
            float scaleX, float scaleY, float scaleZ,
            float localScaleX, float localScaleY, float localScaleZ,
            bool hasBone)
        {
            return new AnimationFrameModelNode(boneName,
                positionX, positionY, positionZ,
                localPositionX, localPositionY, localPositionZ,
                rotationX, rotationY, rotationZ,
                localRotationX, localRotationY, localRotationZ,
                scaleX, scaleY, scaleZ,
                localScaleX, localScaleY, localScaleZ,
                hasBone);
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
            float positionX, float positionY, float positionZ,
            float localPositionX, float localPositionY, float localPositionZ,
            float rotationX, float rotationY, float rotationZ,
            float localRotationX, float localRotationY, float localRotationZ,
            float scaleX, float scaleY, float scaleZ,
            float localScaleX, float localScaleY, float localScaleZ,
            bool hasBone)
        {
            AnimationFrameModelNode node = constructNode(
                boneName,
                positionX, positionY, positionZ,
                localPositionX, localPositionY, localPositionZ,
                rotationX, rotationY, rotationZ,
                localRotationX, localRotationY, localRotationZ,
                scaleX, scaleY, scaleZ,
                localScaleX, localScaleY, localScaleZ,
                hasBone);

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
