using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.ArmatureModelConstructing;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.Utils;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model.ObjectsExportLibraryModelDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class R3AnimatedMesh : MonoBehaviour
{
    ObjectsExportLibraryInterface objectsExportLibraryInterface;

    void Start()
    {
        objectsExportLibraryInterface = new ObjectsExportLibraryInterface();
    }

    public void AddToExportObjectsLibrary()
    {
        objectsExportLibraryInterface.AddR3AnimatedMeshToLibrary(this);
    }

    public ArmatureHierarchyModel GetCurrentOverallArmatureHierarchy()
    {
        return (new ArmatureHierarchyModelConstructor()).DeriveArmatureHierarchyModel(this);
    }

    public void ClearExportObjectsLibrary()
    {
        objectsExportLibraryInterface.ClearExportObjectsLibrary();
    }

    public string GetName()
    {
        return gameObject.name;
    }

    public AnimatedExportObjectModel ToAnimatedExportObjectModel()
    {
        if (IsSkinnedMesh())
        {
            return (new SkinnedR3MeshToAnimatedExportObjectModelConverter()).convert(this);
        } else if (IsChannelParentedMesh())
        {
            return (new ChannelParentedR3MeshToAnimatedExportObjectModelConverter()).convert(this);
        } else
        {
            throw new InvalidOperationException("This mesh is not Skinned Mesh nor it is a channel parented mesh!");
        }
    }

    private bool IsChannelParentedMesh()
    {
        return GetComponent<SkinnedMeshRenderer>() == null && GetComponent<MeshRenderer>() != null && HasParentChannel();
    }

    private bool HasParentChannel()
    {
        return ObjectsHierarchyHelper.GetParentChannelNameOrNullIfNotPresent(transform) != null;
    }

    public GameObject GetParentChannel()
    {
        return ObjectsHierarchyHelper.GetProperChannelForTransform(transform).gameObject;
    }

    private bool IsSkinnedMesh()
    {
        return GetComponent<SkinnedMeshRenderer>() != null;
    }
}
