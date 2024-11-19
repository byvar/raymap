using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GenericExport
{
    public static class PersoStateFramesHelper
    {
        public static void GoToNextFrame(PersoBehaviour persoBehaviour)
        {
            persoBehaviour.currentFrame = persoBehaviour.currentFrame + 1;
            persoBehaviour.UpdateAnimation();
        }

        public static bool HasFramesLeftInCurrentState(PersoBehaviour persoBehaviour)
        {
            return persoBehaviour.currentFrame < persoBehaviour.GetCurrentStateFramesCount() - 1;
        }
    }
}
