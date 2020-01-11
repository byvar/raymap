using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Unity.AnimationExporting;
using Assets.Scripts.Unity.ModelDataExporting.AnimationExporting.DataManipulation.Model;

namespace Assets.Scripts.Unity.ModelDataExporting.AnimationExporting.DataManipulation
{
    public class RaymapAnimationKeyframeModelFacadeAccessor
    {
        private AnimTreeWithChannelsDataHierarchy animTreeWithChannelsDataHierarchy;

        public RaymapAnimationKeyframeModelFacadeAccessor(AnimTreeWithChannelsDataHierarchy animTreeWithChannelsDataHierarchy,
            int FrameNumber)
        {
            this.animTreeWithChannelsDataHierarchy = animTreeWithChannelsDataHierarchy;
            this.FrameNumber = FrameNumber;
        }

        public int FrameNumber;

        public AnimationFrameModel GetAnimationFrameModel()
        {
            return animTreeWithChannelsDataHierarchy.ToAnimationFrameModel();
        }
    }
}
