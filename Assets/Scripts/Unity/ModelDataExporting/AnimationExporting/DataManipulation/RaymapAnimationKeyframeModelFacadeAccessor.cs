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

        public RaymapAnimationKeyframeModelFacadeAccessor(AnimTreeWithChannelsDataHierarchy animTreeWithChannelsDataHierarchy)
        {
            this.animTreeWithChannelsDataHierarchy = animTreeWithChannelsDataHierarchy;
        }

        public object FrameNumber { get; internal set; }

        internal AnimationFrameModel GetAnimationFrameModel()
        {
            throw new NotImplementedException();
        }
    }
}
