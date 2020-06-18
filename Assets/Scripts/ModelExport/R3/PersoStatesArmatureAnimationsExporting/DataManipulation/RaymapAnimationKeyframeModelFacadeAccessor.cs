using ModelExport.R3.PersoStatesArmatureAnimationsExporting.DataManipulation.Model;
using ModelExport.R3.PersoStatesArmatureAnimationsExporting.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelExport.R3.PersoStatesArmatureAnimationsExporting.DataManipulation
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
