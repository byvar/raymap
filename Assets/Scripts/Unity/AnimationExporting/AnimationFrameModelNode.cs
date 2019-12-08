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
        public double rotationX, rotationY, rotationZ;
        public double scaleX, scaleY, scaleZ;
        
        public List<AnimationFrameModelNode> children = new List<AnimationFrameModelNode>();

        public AnimationFrameModelNode(string boneName, float positionX, float positionY, float positionZ, float rotationX, float rotationY, float rotationZ, float scaleX, float scaleY, float scaleZ)
        {
            this.boneName = boneName;
            this.positionX = positionX;
            this.positionY = positionY;
            this.positionZ = positionZ;
            this.rotationX = rotationX;
            this.rotationY = rotationY;
            this.rotationZ = rotationZ;
            this.scaleX = scaleX;
            this.scaleY = scaleY;
            this.scaleZ = scaleZ;
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
