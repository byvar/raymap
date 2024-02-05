using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.GenericExport
{
    public class Perso3DDataExporter
    {
        protected PersoBehaviour persoBehaviour;

        public Perso3DDataExporter(PersoBehaviour persoBehaviour)
        {
            this.persoBehaviour = persoBehaviour;          
        }

        public void ExportPersoAnimated3DData()
        {
            persoBehaviour.playAnimation = false;
            persoBehaviour.StartCoroutine(ExportPersoAnimated3DDataCoroutine());
        }

        public IEnumerator ExportPersoAnimated3DDataCoroutine()
        {
            int currentState = 0;
            while (PersoStatesHelper.HasStatesLeft(currentState, persoBehaviour))
            {
                while (PersoStateFramesHelper.HasFramesLeftInCurrentState(persoBehaviour))
                {

                    PersoStateFramesHelper.GoToNextFrame(persoBehaviour);
                    yield return null;
                }

                currentState = currentState + 1;
                PersoStatesHelper.GoToNextState(persoBehaviour);
                yield return null;
            }          
        }
    }
}
