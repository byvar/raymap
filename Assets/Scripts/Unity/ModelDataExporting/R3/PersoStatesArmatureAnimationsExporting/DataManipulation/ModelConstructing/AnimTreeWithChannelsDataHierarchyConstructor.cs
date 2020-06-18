using Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.DataManipulation.Model;
using Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.DataManipulation.OpenspaceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.DataManipulation.ModelConstructing
{
    public class AnimTreeWithChannelsDataHierarchyConstructor
    {
        public AnimTreeWithChannelsDataHierarchy ConstructFromGiven(
            AnimA3DGeneralDataManipulationInterface animaA3DGeneralDataManipulator,
            int animationFrameNumber)
        {
            AnimTreeWithChannelsDataHierarchyBuilder builder = new AnimTreeWithChannelsDataHierarchyBuilder();
            foreach (AnimHierarchyWithChannelInfo animHierarchyWithChannelInfo in
                animaA3DGeneralDataManipulator.IterateAnimHierarchiesWithChannelInfosForGivenFrame(animationFrameNumber))
            {
                builder.AddAnimHierarchyWithChannelInfo(animHierarchyWithChannelInfo);
            }
            return builder.Build();
        }
    }
}
