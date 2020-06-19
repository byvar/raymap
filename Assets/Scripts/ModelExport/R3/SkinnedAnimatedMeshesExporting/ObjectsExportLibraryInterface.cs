using ModelExport.R3.SkinnedAnimatedMeshesExporting.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelExport.R3.SkinnedAnimatedMeshesExporting
{
    public class ObjectsExportLibraryInterface
    {
        public static void AddR3AnimatedMeshToLibrary(ExportableModel r3AnimatedMesh)
        {
            string exportObjectsLibraryPath = GetExportObjectsLibraryPath();

            ObjectsExportLibraryModel exportObjectsLibrary = LoadObjectsExportLibraryOrCreateNewIfNotExists(exportObjectsLibraryPath);
            exportObjectsLibrary.AddR3AnimatedMesh(r3AnimatedMesh);
            SaveObjectsExportLibrary(exportObjectsLibrary, exportObjectsLibraryPath);
        }

        private static string GetExportObjectsLibraryPath()
        {
            return Path.Combine(UnitySettings.ExportPath, "ModelExport", "ExportObjectsLibrary.json");
        }

        private static ObjectsExportLibraryModel LoadObjectsExportLibraryOrCreateNewIfNotExists(string objectsExportLibraryPath)
        {
            if (File.Exists(objectsExportLibraryPath))
            {
                return JsonConvert.DeserializeObject<ObjectsExportLibraryModel>(File.ReadAllText(objectsExportLibraryPath));
            } else
            {
                return new ObjectsExportLibraryModel();
            }            
        }

        private static void SaveObjectsExportLibrary(ObjectsExportLibraryModel exportObjectsLibrary, string objectsExportLibraryPath)
        {
            var settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(exportObjectsLibrary, settings);
            System.IO.File.WriteAllText(objectsExportLibraryPath, jsonString);
        }

        public static void ClearExportObjectsLibrary()
        {
            string exportObjectsLibraryPath = GetExportObjectsLibraryPath();
            File.Delete(exportObjectsLibraryPath);
        }
    }
}
