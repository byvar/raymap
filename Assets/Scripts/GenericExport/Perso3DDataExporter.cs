using Assets.Scripts.GenericExport.Capturing;
using Assets.Scripts.GenericExport.Model;
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
                    Perso3DFrameExportData perso3DFrameExportData = 
                        Perso3DFrameExportDataCapturer.Capture3DFrameExportData(persoBehaviour);
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
