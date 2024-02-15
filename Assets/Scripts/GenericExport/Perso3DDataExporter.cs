using Assets.Scripts.GenericExport.Model;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Assets.Scripts.GenericExport
{
    public static class ToFileSaver
    {
        private static string RemoveInvalidChars(string filename)
        {
            return string.Concat(filename.Split(Path.GetInvalidFileNameChars()));
        }

        public static void Save(string fileName, string content)
        {
            System.IO.File.WriteAllText(GetAnimationModelExportPath(fileName), content);
        }

        private static string GetAnimationModelExportPath(string fileName)
        {
            string dirPath = Path.Combine(UnitySettings.ExportPath, "ModelExport");
            Directory.CreateDirectory(dirPath);
            return Path.Combine(UnitySettings.ExportPath, "ModelExport", RemoveInvalidChars(fileName).Replace(" ", "_"));
        }
    }

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
                result.states[currentState] = new Dictionary<int, FrameDataBlock>();
                int currentFrame = 0;
                while (PersoStateFramesHelper.HasFramesLeftInCurrentState(persoBehaviour))
                {
                    result.states[currentState][currentFrame] =
                        FrameDataBlock.GetConsolidated(result.states[currentState], currentFrame, persoBehaviour);

                    PersoStateFramesHelper.GoToNextFrame(persoBehaviour);
                    currentFrame++;
                    yield return null;
                }

                currentState = currentState + 1;
                PersoStatesHelper.GoToNextState(persoBehaviour);
                yield return null;
            }

            var settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(result, settings);

            ToFileSaver.Save($"{persoBehaviour.name}.perso3d", jsonString);
        }
    }
}
