using ModelDataExport.R3.SkinnedAnimatedMeshesExporting.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDataExport.R3.SkinnedAnimatedMeshesExporting
{
    public class ObjectsExportLibraryInterface
    {
        static string ObjectsExportLibraryPath = "D:/exported_rayman_meshes.json";

        public static void AddR3AnimatedMeshToLibrary(ExportableModel r3AnimatedMesh)
        {
            ObjectsExportLibraryModel exportObjectsLibrary = LoadObjectsExportLibraryOrCreateNewIfNotExists(ObjectsExportLibraryPath);
            exportObjectsLibrary.AddR3AnimatedMesh(r3AnimatedMesh);
            SaveObjectsExportLibrary(exportObjectsLibrary, ObjectsExportLibraryPath);
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
            File.Delete(ObjectsExportLibraryPath);
        }
    }
}
