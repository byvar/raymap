using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.DataManipulation.Model
{
    public class AnimHierarchyWithChannelInfo
    {
        public string ParentChannelName;
        public string ChannelName;
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
        public Vector3 LocalScale;
        public bool IsKeyframe;

        public AnimHierarchyWithChannelInfo(string ParentChannelName, string ChannelName,
            Vector3 LocalPosition, Quaternion LocalRotation, Vector3 LocalScale, bool IsKeyframe)
        {
            this.ParentChannelName = ParentChannelName;
            this.ChannelName = ChannelName;
            this.LocalPosition = LocalPosition;
            this.LocalRotation = LocalRotation;
            this.LocalScale = LocalScale;
            this.IsKeyframe = IsKeyframe;
        }
    }
}