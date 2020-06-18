using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model.ObjectsExportLibraryModelDescription;
using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model
{
    public class ObjectsExportLibraryModel
    {
        public ArmatureHierarchyModel armatureHierarchy = new ArmatureHierarchyModel();
        public Dictionary<string, AnimatedExportObjectModel> animatedExportObjects = new Dictionary<string, AnimatedExportObjectModel>();

        public void AddR3AnimatedMesh(R3AnimatedMesh r3AnimatedMesh)
        {
            var currentContextArmatureHierarchy = r3AnimatedMesh.GetCurrentOverallArmatureHierarchy();
            ConsolidateArmatureHierarchyModels(currentContextArmatureHierarchy);

            if (animatedExportObjects.ContainsKey(r3AnimatedMesh.GetName()))
            {
                throw new InvalidOperationException("Objects Export Library already contains mesh of name " + r3AnimatedMesh.GetName());
            }

            AnimatedExportObjectModel animatedExportObjectModel = r3AnimatedMesh.ToAnimatedExportObjectModel();
            animatedExportObjects.Add(animatedExportObjectModel.Name, animatedExportObjectModel);
        }

        private void ConsolidateArmatureHierarchyModels(ArmatureHierarchyModel currentContextArmatureHierarchy)
        {
            var currentChannels = armatureHierarchy.GetTreeBuildingNodes();
            var newChannels = currentContextArmatureHierarchy.GetTreeBuildingNodes();
            var actualNewChannels = newChannels.Difference(currentChannels);
            var armatureHierarchyTree = (Tree<ArmatureHierarchyModelNode, string>)armatureHierarchy;
            armatureHierarchyTree = Tree<ArmatureHierarchyModelNode, string>.BuildTreeWithProperNodesPuttingOrder(
                armatureHierarchyTree, actualNewChannels.ToBuildingTreeNodesQueue());
            armatureHierarchy = (ArmatureHierarchyModel)armatureHierarchyTree;
        }
    }
}

