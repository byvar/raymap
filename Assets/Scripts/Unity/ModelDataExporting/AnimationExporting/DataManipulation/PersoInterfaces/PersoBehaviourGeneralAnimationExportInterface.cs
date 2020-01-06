using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Unity.ModelDataExporting.AnimationExporting.DataManipulation.Model;
using Assets.Scripts.Unity.ModelDataExporting.AnimationExporting.DataManipulation.ModelConstructing;
using Assets.Scripts.Unity.ModelDataExporting.AnimationExporting.DataManipulation.PersoInterfaces;
using OpenSpace.Animation.Component;

namespace Assets.Scripts.Unity.ModelDataExporting.AnimationExporting.DataManipulation
{
    public class PersoBehaviourGeneralAnimationExportInterface
    {
        private PersoBehaviour persoBehaviour;

        public PersoBehaviourGeneralAnimationExportInterface(PersoBehaviour persoBehaviour)
        {
            this.persoBehaviour = persoBehaviour;
        }

        public void ResetAnimationState()
        {
            throw new NotImplementedException();
        }

        public bool IsValidAnimationClip()
        {
            throw new NotImplementedException();
        }

        public bool IsValidAnimationFrame()
        {
            throw new NotImplementedException();
        }

        public AnimTreeWithChannelsDataHierarchy DeriveAnimTreeWithChannelsDataHierarchyForCurrentFrame()
        {
            AnimTreeWithChannelsDataHierarchyConstructor animTreeWithChannelsDataHierarchyConstructor =
                new AnimTreeWithChannelsDataHierarchyConstructor();
            return animTreeWithChannelsDataHierarchyConstructor.ConstructFromGiven(new PersoBehaviourAnimDataManipulationInterface(persoBehaviour));
        }

        public void NextKeyframe()
        {
            throw new NotImplementedException();
        }

        public void NextAnimationClip()
        {
            throw new NotImplementedException();
        }
    }
}
