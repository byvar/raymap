using Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.DataManipulation.Model;
using Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.DataManipulation.OpenspaceInterfaces;
using OpenSpace.Animation.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.DataManipulation
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
            animationExportInterface.ResetAnimationState();
            while (animationExportInterface.AreAnimationClipsLeft())
            {
                RaymapAnimationClipModelFacadeAccessor raymapAnimationClipModelFacadeAccessor =
                    new RaymapAnimationClipModelFacadeAccessor(animationExportInterface.GetCurrentAnimationClipName());
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
