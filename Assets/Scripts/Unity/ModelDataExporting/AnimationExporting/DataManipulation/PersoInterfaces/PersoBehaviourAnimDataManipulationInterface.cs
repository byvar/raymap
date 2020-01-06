using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Unity.ModelDataExporting.AnimationExporting.DataManipulation.Model;

namespace Assets.Scripts.Unity.ModelDataExporting.AnimationExporting.DataManipulation.PersoInterfaces
{
    class PersoBehaviourAnimDataManipulationInterface
    {
        private PersoBehaviour persoBehaviour;

        public PersoBehaviourAnimDataManipulationInterface(PersoBehaviour persoBehaviour)
        {
            this.persoBehaviour = persoBehaviour;
        }

        internal IEnumerable<AnimHierarchyWithChannelInfo> IterateAnimHierarchiesWithChannelInfosForCurrentFrame()
        {
            throw new NotImplementedException();
        }
    }
}
