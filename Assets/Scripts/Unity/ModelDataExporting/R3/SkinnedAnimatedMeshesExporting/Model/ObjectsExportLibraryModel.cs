using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model
{
    public class ObjectsExportLibraryModel
    {
        public Dictionary<string, AnimatedExportObjectModel> animatedExportObjects = new Dictionary<string, AnimatedExportObjectModel>();

        public void AddR3AnimatedMesh(R3AnimatedMesh r3AnimatedMesh)
        {
            if (animatedExportObjects.ContainsKey(r3AnimatedMesh.GetName()))
            {
                throw new InvalidOperationException("Objects Export Library already contains mesh of name " + r3AnimatedMesh.GetName());
            }

            AnimatedExportObjectModel animatedExportObjectModel = r3AnimatedMesh.ToAnimatedExportObjectModel();
            animatedExportObjects.Add(animatedExportObjectModel.Name, animatedExportObjectModel);
        }
    }
}

