using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenSpace.Animation.Component;
using OpenSpace.Object.Properties;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.PersoStatesArmatureAnimationsExporting.DataManipulation.PersoInterfaces.Helpers
{
    class FamilyAnimationStatesHelper
    {
        private Family family;
        private AnimA3DGeneral animA3DGeneralForCurrentPersoAnimationState;

        public FamilyAnimationStatesHelper(Family family)
        {
            this.family = family;
        }

        public bool AreValidPersoAnimationStatesLeftIncludingCurrentOne()
        {
            throw new NotImplementedException();
        }

        public bool AreKeyframesLeftForCurrentAnimationStateStartingWithFrameNumber(int currentFrameNumber)
        {
            throw new NotImplementedException();
        }

        public AnimA3DGeneral GetAnimA3DGeneralForCurrentPersoAnimationState()
        {
            throw new NotImplementedException();
        }

        public int GetStateAnimationNextKeyframeFrameNumberAfter(int currentFrameNumber)
        {
            throw new NotImplementedException();
        }

        public void AcquireNextValidPersoAnimationState()
        {
            throw new NotImplementedException();
        }

        public int GetFirstValidStateAnimationKeyframeFrameNumber()
        {
            throw new NotImplementedException();
        }
    }
}
