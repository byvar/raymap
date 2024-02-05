using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GenericExport
{
    public static class PersoStatesHelper
    {
        public static bool HasStatesLeft(int prevState, PersoBehaviour persoBehaviour)
        {
            return prevState < persoBehaviour.perso.p3dData.family.states.Count;
        }

        public static void GoToNextState(PersoBehaviour persoBehaviour)
        {
            persoBehaviour.SetState(persoBehaviour.currentState + 1);
        }
    }
}
