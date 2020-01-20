using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model.ObjectsExportLibraryModelDescription;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.ArmatureModelConstructing
{
    public class PersoChannelsHierarchyHelper
    {
        public static IEnumerable<Tuple<string, string>> IterateParentChildChannelPairs(Transform persoBeheaviourComponentOwner)
        {
            var enumerable = new List<Tuple<string, string>>();
            enumerable.Add(new Tuple<string, string>(null, "ROOT_CHANNEL"));
            TraverseAndCollectAll("ROOT_CHANNEL", persoBeheaviourComponentOwner, enumerable);
            foreach (var iterator in enumerable)
            {
                yield return iterator;
            }
        }

        private static void TraverseAndCollectAll(string CurrentChannelName,
            Transform CurrentTransform, List<Tuple<string, string>> enumerable)
        {
            foreach (Transform Child in CurrentTransform)
            {
                if (Child.gameObject.name.Contains("Channel"))
                {
                    enumerable.Add(new Tuple<string, string>(CurrentChannelName, Child.gameObject.name));
                    TraverseAndCollectAll(Child.gameObject.name, Child, enumerable);
                } else
                {
                    TraverseAndCollectAll(CurrentChannelName, Child, enumerable);
                }                
            }
        }
    }

    public class ArmatureHierarchyModelConstructor
    {
        public ArmatureHierarchyModel DeriveArmatureHierarchyModel(R3AnimatedMesh r3AnimatedMesh)
        {
            var persoBehaviourComponentOwner = GetPersoParent(r3AnimatedMesh.transform);
            return BuildArmatureHierarchyModel(persoBehaviourComponentOwner);
        }

        private ArmatureHierarchyModel BuildArmatureHierarchyModel(Transform persoBehaviourComponentOwner)
        {
            var armatureHierarchyModelBuildingNodes = new Queue<TreeBuildingNodeInfo<ArmatureHierarchyModelNode, string>>();
            foreach (var parentChildChannelPair in PersoChannelsHierarchyHelper.IterateParentChildChannelPairs(persoBehaviourComponentOwner))
            {
                armatureHierarchyModelBuildingNodes.Enqueue(
                    new TreeBuildingNodeInfo<ArmatureHierarchyModelNode, string>(
                            parentChildChannelPair.Item1,
                            parentChildChannelPair.Item2,
                            new ArmatureHierarchyModelNode(parentChildChannelPair.Item2)
                        )
                    );
            }
            var resultTree = Tree<ArmatureHierarchyModelNode, string>.BuildTreeWithProperNodesPuttingOrder(null, armatureHierarchyModelBuildingNodes);
            var result = (ArmatureHierarchyModel)resultTree;
            return result;
        }

        private Transform GetPersoParent(Transform transform)
        {
            var current = transform;
            while (current.gameObject.GetComponent<PersoBehaviour>() == null)
            {
                current = current.parent;
                if (current == null)
                {
                    throw new InvalidOperationException("Could not find Perso parent for " + transform.gameObject.name);
                }
            }
            return current;
        }
    }
}
