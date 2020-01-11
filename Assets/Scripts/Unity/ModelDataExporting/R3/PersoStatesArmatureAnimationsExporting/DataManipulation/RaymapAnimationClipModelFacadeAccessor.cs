using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Unity.ModelDataExporting.AnimationExporting.DataManipulation
{
    public class RaymapAnimationClipModelFacadeAccessor
    {
        Dictionary<int, RaymapAnimationKeyframeModelFacadeAccessor> keyframes = new Dictionary<int, RaymapAnimationKeyframeModelFacadeAccessor>();

        public string Name;

        public RaymapAnimationClipModelFacadeAccessor(string Name)
        {
            this.Name = Name;
        }

        public IEnumerable<RaymapAnimationKeyframeModelFacadeAccessor> IterateKeyframes()
        {
            foreach (var keyframeAccessor in keyframes)
            {
                yield return keyframeAccessor.Value;
            }
        }

        internal void AddKeyframe(RaymapAnimationKeyframeModelFacadeAccessor raymapAnimationKeyframeModelFacadeAccessor, int frameNumber)
        {
            keyframes.Add(frameNumber, raymapAnimationKeyframeModelFacadeAccessor);
        }
    }
}
