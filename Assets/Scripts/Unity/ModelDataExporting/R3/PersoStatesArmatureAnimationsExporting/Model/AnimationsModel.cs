using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.Model
{
    [Serializable]
    public class AnimationsModel
    {
        public Dictionary<string, Dictionary<int, AnimationFrameModel>> animationClips = new Dictionary<string, Dictionary<int, AnimationFrameModel>>();

        public void addAnimationFrameModelToAnimationClip(string animationClipName, AnimationFrameModel animationFrameModel, int frameNumber)
        {
            if (!animationClips.ContainsKey(animationClipName))
            {
                animationClips[animationClipName] = new Dictionary<int, AnimationFrameModel>();
            }

            animationClips[animationClipName][frameNumber] = animationFrameModel;
        }

        internal void addAnimationFrameModelToAnimationClip(object name, AnimationFrameModel animationFrameModel, object frameNumber)
        {
            throw new NotImplementedException();
        }
    }
}
