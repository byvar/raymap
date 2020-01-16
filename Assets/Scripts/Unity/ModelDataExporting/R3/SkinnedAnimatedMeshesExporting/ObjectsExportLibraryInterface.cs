using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting
{
    public class ObjectsExportLibraryInterface
    {
        string ObjectsExportLibraryPath = "D:/exported_rayman_meshes.json";

        public void AddSkinnedMeshToLibrary(R3AnimatedMesh r3AnimatedMesh)
        {
            ObjectsExportLibraryModel exportObjectsLibrary = LoadObjectsExportLibraryOrCreateNewIfNotExists(ObjectsExportLibraryPath);
            exportObjectsLibrary.AddSkinnedMeshObject(r3AnimatedMesh);
            SaveObjectsExportLibrary(exportObjectsLibrary, ObjectsExportLibraryPath);
        }

        private ObjectsExportLibraryModel LoadObjectsExportLibraryOrCreateNewIfNotExists(string objectsExportLibraryPath)
        {
            throw new NotImplementedException();
        }

        private void SaveObjectsExportLibrary(ObjectsExportLibraryModel exportObjectsLibrary, string objectsExportLibraryPath)
        {
            throw new NotImplementedException();
        }

        public void AddChannelParentedMesh(R3AnimatedMesh r3AnimatedMesh)
        {
            ObjectsExportLibraryModel exportObjectsLibrary = LoadObjectsExportLibraryOrCreateNewIfNotExists(ObjectsExportLibraryPath);
            exportObjectsLibrary.AddChannelParentedObject(r3AnimatedMesh);
            SaveObjectsExportLibrary(exportObjectsLibrary, ObjectsExportLibraryPath);
        }

        public void ClearExportObjectsLibrary()
        {
            throw new NotImplementedException();
        }
    }
}
