using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.DataManipulation.Model;
using Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.DataManipulation.ModelConstructing;
using Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.DataManipulation.OpenspaceInterfaces.Helpers;
using OpenSpace.Animation.Component;
using OpenSpace.Object.Properties;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.DataManipulation.OpenspaceInterfaces
{
    public class PersoGeneralAnimationExportInterface
    {
        private FamilyAnimationStatesHelper familyAnimationStatesHelper;

        int currentFrameNumber = 0;

        public PersoGeneralAnimationExportInterface(Family family)
        {
            this.familyAnimationStatesHelper = new FamilyAnimationStatesHelper(family);
        }

        public bool AreAnimationClipsLeft()
        {
            return familyAnimationStatesHelper.AreValidPersoAnimationStatesLeftIncludingCurrentOne();
        }

        public string GetCurrentAnimationClipName()
        {
            return "Animation " + familyAnimationStatesHelper.GetCurrentPersoStateIndex();
        }

        public void ResetAnimationState()
        {
            familyAnimationStatesHelper.SwitchToFirstAnimationState();
        }

        public bool AreAnimationFramesLeft()
        {
            return familyAnimationStatesHelper.AreKeyframesLeftForCurrentAnimationStateStartingWithFrameNumber(currentFrameNumber);
        }

        public AnimTreeWithChannelsDataHierarchy DeriveAnimTreeWithChannelsDataHierarchyForGivenFrame(
            int animationFrameNumber)
        {
            AnimTreeWithChannelsDataHierarchyConstructor animTreeWithChannelsDataHierarchyConstructor =
                new AnimTreeWithChannelsDataHierarchyConstructor();
            return animTreeWithChannelsDataHierarchyConstructor.ConstructFromGiven(
                new AnimA3DGeneralDataManipulationInterface(familyAnimationStatesHelper.GetAnimA3DGeneralForCurrentPersoAnimationState()),
                animationFrameNumber);
        }

        public int GetCurrentFrameNumberForExport()
        {
            return currentFrameNumber;
        }

        public void NextKeyframe()
        {
            currentFrameNumber = familyAnimationStatesHelper.GetStateAnimationNextKeyframeFrameNumberAfter(currentFrameNumber);
        }

        public void NextAnimationClip()
        {
            familyAnimationStatesHelper.AcquireNextValidPersoAnimationState();
            currentFrameNumber = familyAnimationStatesHelper.GetFirstValidStateAnimationKeyframeFrameNumber();
        }
    }
}