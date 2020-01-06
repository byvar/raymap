using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Unity.AnimationExporting
{
    [Serializable]
    public class AnimationFrameModelNode
    {
        public string boneName;
        public double positionX, positionY, positionZ;
        public double localPositionX, localPositionY, localPositionZ;
        public double rotationW, rotationX, rotationY, rotationZ;
        public double localRotationW, localRotationX, localRotationY, localRotationZ;
        public double scaleX, scaleY, scaleZ;
        public double localScaleX, localScaleY, localScaleZ;
        public bool hasBone;
        
        public List<AnimationFrameModelNode> children = new List<AnimationFrameModelNode>();

        public AnimationFrameModelNode(string boneName,
            float positionX, float positionY, float positionZ,
            float localPositionX, float localPositionY, float localPositionZ,
            float rotationW, float rotationX, float rotationY, float rotationZ,
            float localRotationW, float localRotationX, float localRotationY, float localRotationZ,
            float scaleX, float scaleY, float scaleZ,
            float localScaleX, float localScaleY, float localScaleZ,
            bool hasBone)
        {
            this.boneName = boneName;
            this.positionX = positionX;
            this.positionY = positionY;
            this.positionZ = positionZ;
            this.localPositionX = localPositionX;
            this.localPositionY = localPositionY;
            this.localPositionZ = localPositionZ;
            this.rotationW = rotationW;
            this.rotationX = rotationX;
            this.rotationY = rotationY;
            this.rotationZ = rotationZ;
            this.localRotationW = localRotationW;
            this.localRotationX = localRotationX;
            this.localRotationY = localRotationY;
            this.localRotationZ = localRotationZ;
            this.scaleX = scaleX;
            this.scaleY = scaleY;
            this.scaleZ = scaleZ;
            this.localScaleX = localScaleX;
            this.localScaleY = localScaleY;
            this.localScaleZ = localScaleZ;
            this.hasBone = hasBone;
        }

        public bool addChild(string parentBoneName, AnimationFrameModelNode node)
        {
            if (parentBoneName.Equals(boneName))
            {
                children.Add(node);
                return true;
            } else
            {
                foreach (AnimationFrameModelNode child in children)
                {
                    bool successfullyAdded = child.addChild(parentBoneName, node);
                    if (successfullyAdded)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
