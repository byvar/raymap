using Assets.Scripts.Unity.ModelDataExporting.MathDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.Model
{
    public class AnimationFrameModelNode
    {
        public string boneName;
        public bool isKeyframe;
        public Vector3d position;
        public Vector3d localPosition;
        public Quaternion rotation;
        public Quaternion localRotation;
        public Vector3d scale;
        public Vector3d localScale;
        public bool hasBone;

        public AnimationFrameModelNode(
            string boneName,
            bool isKeyframe,
            Vector3d position,
            Vector3d localPosition,
            Quaternion rotation,
            Quaternion localRotation,
            Vector3d scale,
            Vector3d localScale,
            bool hasBone)
        {
            this.boneName = boneName;
            this.isKeyframe = isKeyframe;
            this.position = position;
            this.localPosition = localPosition;
            this.rotation = rotation;
            this.localRotation = localRotation;
            this.scale = scale;
            this.localScale = localScale;
            this.hasBone = hasBone;
        }            
    }
}
