using Assets.Scripts.GenericExport.Capturing;
using Assets.Scripts.GenericExport.Model;
using System.Collections;
using System.Collections.Generic;

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

            var result = new Perso3DAnimatedData();

            while (PersoStatesHelper.HasStatesLeft(currentState, persoBehaviour))
            {
                result.states[currentState] = new Dictionary<int, Perso3DFrameExportData>();
                int currentFrame = 0;
                while (PersoStateFramesHelper.HasFramesLeftInCurrentState(persoBehaviour))
                {
                    Perso3DFrameExportData perso3DFrameExportData = 
                        Perso3DFrameExportDataCapturer.Capture3DFrameExportData(persoBehaviour);
                    result.states[currentState][currentFrame] = perso3DFrameExportData;
                    PersoStateFramesHelper.GoToNextFrame(persoBehaviour);
                    currentFrame++;
                    yield return null;
                }

                currentState = currentState + 1;
                PersoStatesHelper.GoToNextState(persoBehaviour);
                yield return null;
            }          
        }
    }
}
