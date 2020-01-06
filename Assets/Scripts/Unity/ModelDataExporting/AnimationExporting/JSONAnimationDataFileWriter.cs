using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Unity.AnimationExporting
{
    public class JSONAnimationDataFileWriter
    {
        private string filePath = "D:\\exported_rayman_animations.json";

        public void writeAnimationModel(AnimationsModel animationsModel)
        {
            var settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(animationsModel, settings);
            System.IO.File.WriteAllText(filePath, jsonString);
        }
    }
}
