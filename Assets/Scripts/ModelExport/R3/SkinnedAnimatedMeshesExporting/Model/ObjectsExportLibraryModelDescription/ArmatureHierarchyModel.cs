using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelExport.R3.SkinnedAnimatedMeshesExporting.Model.ObjectsExportLibraryModelDescription
{
    public class ArmatureHierarchyModelNode
    {
        public string boneName;

        public ArmatureHierarchyModelNode(string boneName)
        {
            this.boneName = boneName;
        }
    }

    public class ArmatureHierarchyModel : Tree<ArmatureHierarchyModelNode, string>
    {
    }
}