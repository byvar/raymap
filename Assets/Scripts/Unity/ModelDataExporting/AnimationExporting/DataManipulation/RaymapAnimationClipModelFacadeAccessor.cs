using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Unity.ModelDataExporting.AnimationExporting.DataManipulation
{
    public class RaymapAnimationClipModelFacadeAccessor
    {
        Dictionary<int, RaymapAnimationKeyframeModelFacadeAccessor> keyframes;

        public IEnumerable<RaymapAnimationKeyframeModelFacadeAccessor> IterateKeyframes()
        {
            foreach (var keyframeAccessor in keyframes)
            {
                yield return keyframeAccessor.Value;
            }
        }

        internal void AddKeyframe(RaymapAnimationKeyframeModelFacadeAccessor raymapAnimationKeyframeModelFacadeAccessor, object frameNumber)
        {
            throw new NotImplementedException();
        }
    }
}
