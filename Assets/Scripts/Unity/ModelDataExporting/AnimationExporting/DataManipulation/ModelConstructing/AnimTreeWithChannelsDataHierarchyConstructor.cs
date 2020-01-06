using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Unity.ModelDataExporting.AnimationExporting.DataManipulation.Model;
using Assets.Scripts.Unity.ModelDataExporting.AnimationExporting.DataManipulation.PersoInterfaces;

namespace Assets.Scripts.Unity.ModelDataExporting.AnimationExporting.DataManipulation.ModelConstructing
{
    class AnimTreeWithChannelsDataHierarchyConstructor
    {
        public AnimTreeWithChannelsDataHierarchy ConstructFromGiven(PersoBehaviourAnimDataManipulationInterface persoBehaviourInterface)
        {
            AnimTreeWithChannelsDataHierarchyBuilder builder = new AnimTreeWithChannelsDataHierarchyBuilder();
            foreach (AnimHierarchyWithChannelInfo animHierarchyWithChannelInfo in 
                persoBehaviourInterface.IterateAnimHierarchiesWithChannelInfosForCurrentFrame())
            {
                builder.AddAnimHierarchyWithChannelInfo(animHierarchyWithChannelInfo);
            }
            return builder.Build();
        }
    }
}
