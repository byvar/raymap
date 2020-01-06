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
        private PersoBehaviourGeneralAnimationExportInterface persoBehaviourInterface;

        public PersoAnimationStatesDataManipulator(PersoBehaviour persoBehaviour)
        {
            this.persoBehaviourInterface = new PersoBehaviourGeneralAnimationExportInterface(persoBehaviour);
        }

        public IEnumerable<RaymapAnimationClipModelFacadeAccessor> IterateAnimationClips()
        {
            persoBehaviourInterface.ResetAnimationState();
            while (persoBehaviourInterface.IsValidAnimationClip())
            {
                RaymapAnimationClipModelFacadeAccessor raymapAnimationClipModelFacadeAccessor = new RaymapAnimationClipModelFacadeAccessor();
                while (persoBehaviourInterface.IsValidAnimationFrame())
                {
                    AnimTreeWithChannelsDataHierarchy animTreeWithChannelsDataHierarchy = 
                        persoBehaviourInterface.DeriveAnimTreeWithChannelsDataHierarchyForGivenFrame(
                            persoBehaviourInterface.GetCurrentFrameNumberForExport());
                    RaymapAnimationKeyframeModelFacadeAccessor raymapAnimationKeyframeModelFacadeAccessor = 
                        new RaymapAnimationKeyframeModelFacadeAccessor(animTreeWithChannelsDataHierarchy);
                    raymapAnimationClipModelFacadeAccessor.AddKeyframe(raymapAnimationKeyframeModelFacadeAccessor,
                        raymapAnimationKeyframeModelFacadeAccessor.FrameNumber);
                    persoBehaviourInterface.NextKeyframe();
                }
                yield return raymapAnimationClipModelFacadeAccessor;
                persoBehaviourInterface.NextAnimationClip();
            }
        }
    }
}
