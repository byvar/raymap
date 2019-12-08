using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Unity.AnimationExporting
{
    public class AnimationsModel
    {
        Dictionary<string, List<AnimationFrameModel>> animationClips = new Dictionary<string, List<AnimationFrameModel>>();

        public void addAnimationFrameModelToAnimationClip(string animationClipName, AnimationFrameModel animationFrameModel)
        {
            if (!animationClips.ContainsKey(animationClipName))
            {
                animationClips[animationClipName] = new List<AnimationFrameModel>();
            }

            animationClips[animationClipName].Add(animationFrameModel);
        }
    }
}
