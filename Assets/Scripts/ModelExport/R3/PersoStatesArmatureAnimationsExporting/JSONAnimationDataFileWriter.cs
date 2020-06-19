using ModelExport.R3.PersoStatesArmatureAnimationsExporting.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModelExport.R3.PersoStatesArmatureAnimationsExporting
{
    public class JSONAnimationDataFileWriter
    {
        public void writeAnimationModel(AnimationsModel animationsModel)
        {
            var settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(animationsModel, settings);
            System.IO.File.WriteAllText(GetAnimationModelExportPath(), jsonString);
        }

        private string GetAnimationModelExportPath()
        {
            return Path.Combine(UnitySettings.ExportPath, "ModelExport", "AnimationModel.json");
        }
    }
}
