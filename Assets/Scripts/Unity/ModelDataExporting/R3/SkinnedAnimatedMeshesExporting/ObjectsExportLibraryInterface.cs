using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting
{
    public class ObjectsExportLibraryInterface
    {
        string ObjectsExportLibraryPath = "D:/exported_rayman_meshes.json";

        public void AddR3AnimatedMeshToLibrary(R3AnimatedMesh r3AnimatedMesh)
        {
            ObjectsExportLibraryModel exportObjectsLibrary = LoadObjectsExportLibraryOrCreateNewIfNotExists(ObjectsExportLibraryPath);
            exportObjectsLibrary.AddR3AnimatedMesh(r3AnimatedMesh);
            SaveObjectsExportLibrary(exportObjectsLibrary, ObjectsExportLibraryPath);
        }

        private ObjectsExportLibraryModel LoadObjectsExportLibraryOrCreateNewIfNotExists(string objectsExportLibraryPath)
        {
            if (File.Exists(objectsExportLibraryPath))
            {
                return JsonConvert.DeserializeObject<ObjectsExportLibraryModel>(File.ReadAllText(objectsExportLibraryPath));
            } else
            {
                return new ObjectsExportLibraryModel();
            }            
        }

        private void SaveObjectsExportLibrary(ObjectsExportLibraryModel exportObjectsLibrary, string objectsExportLibraryPath)
        {
            var settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(exportObjectsLibrary, settings);
            System.IO.File.WriteAllText(objectsExportLibraryPath, jsonString);
        }

        public void ClearExportObjectsLibrary()
        {
            File.Delete(ObjectsExportLibraryPath);
        }
    }
}
