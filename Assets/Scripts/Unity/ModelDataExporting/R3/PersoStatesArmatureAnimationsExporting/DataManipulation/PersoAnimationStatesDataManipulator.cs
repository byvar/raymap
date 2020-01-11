using Assets.Scripts.Unity.ModelDataExporting.AnimationExporting.DataManipulation;
using Assets.Scripts.Unity.ModelDataExporting.AnimationExporting.DataManipulation.Model;
using OpenSpace.Animation.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Unity.AnimationExporting.DataManipulation
{
    public class PersoAnimationStatesDataManipulator
    {
        private PersoGeneralAnimationExportInterface animationExportInterface;

        public PersoAnimationStatesDataManipulator(PersoBehaviour persoBehaviour)
        {
            this.animationExportInterface = new PersoGeneralAnimationExportInterface(persoBehaviour.perso.p3dData.family);
        }

        public IEnumerable<RaymapAnimationClipModelFacadeAccessor> IterateAnimationClips()
        {
            while (animationExportInterface.AreAnimationClipsLeft())
            {
                RaymapAnimationClipModelFacadeAccessor raymapAnimationClipModelFacadeAccessor = new RaymapAnimationClipModelFacadeAccessor();
                animationExportInterface.ResetAnimationState();
                while (animationExportInterface.AreAnimationFramesLeft())
                {
                    AnimTreeWithChannelsDataHierarchy animTreeWithChannelsDataHierarchy = 
                        animationExportInterface.DeriveAnimTreeWithChannelsDataHierarchyForGivenFrame(
                            animationExportInterface.GetCurrentFrameNumberForExport());
                    RaymapAnimationKeyframeModelFacadeAccessor raymapAnimationKeyframeModelFacadeAccessor = 
                        new RaymapAnimationKeyframeModelFacadeAccessor(animTreeWithChannelsDataHierarchy,
                        animationExportInterface.GetCurrentFrameNumberForExport());
                    raymapAnimationClipModelFacadeAccessor.AddKeyframe(raymapAnimationKeyframeModelFacadeAccessor,
                        raymapAnimationKeyframeModelFacadeAccessor.FrameNumber);
                    animationExportInterface.NextKeyframe();
                }
                yield return raymapAnimationClipModelFacadeAccessor;
                animationExportInterface.NextAnimationClip();
            }
        }
    }
}
