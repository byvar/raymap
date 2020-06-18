using ModelDataExport.R3.SkinnedAnimatedMeshesExporting;
using ModelDataExport.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.ArmatureModelConstructing;
using ModelDataExport.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.ModelConversion;
using ModelDataExport.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.Utils;
using ModelDataExport.R3.SkinnedAnimatedMeshesExporting.Model;
using ModelDataExport.R3.SkinnedAnimatedMeshesExporting.Model.ObjectsExportLibraryModelDescription;
using System;
using UnityEngine;

public static class ExportableModelExtensions {
    public static void AddToExportObjectsLibrary(this ExportableModel m)
    {
        ObjectsExportLibraryInterface.AddR3AnimatedMeshToLibrary(m);
    }

    public static ArmatureHierarchyModel GetCurrentOverallArmatureHierarchy(this ExportableModel m)
    {
        return (new ArmatureHierarchyModelConstructor()).DeriveArmatureHierarchyModel(m);
    }

    public static void ClearExportObjectsLibrary(this ExportableModel m)
    {
        ObjectsExportLibraryInterface.ClearExportObjectsLibrary();
    }

    public static string GetName(this ExportableModel m)
    {
        return m.gameObject.name;
    }

    public static AnimatedExportObjectModel ToAnimatedExportObjectModel(this ExportableModel m)
    {
        if (m.IsSkinnedMesh())
        {
            return (new SkinnedR3MeshToAnimatedExportObjectModelConverter()).convert(m);
        } else if (m.IsChannelParentedMesh())
        {
            return (new ChannelParentedR3MeshToAnimatedExportObjectModelConverter()).convert(m);
        } else
        {
            throw new InvalidOperationException("This mesh is not Skinned Mesh nor it is a channel parented mesh!");
        }
    }

    private static bool IsChannelParentedMesh(this ExportableModel m)
    {
        return m.GetComponent<SkinnedMeshRenderer>() == null && m.GetComponent<MeshRenderer>() != null && m.HasParentChannel();
    }

    private static bool HasParentChannel(this ExportableModel m)
    {
        return ObjectsHierarchyHelper.GetParentChannelNameOrNullIfNotPresent(m.transform) != null;
    }

    public static GameObject GetParentChannel(this ExportableModel m)
    {
        return ObjectsHierarchyHelper.GetProperChannelForTransform(m.transform).gameObject;
    }

    private static bool IsSkinnedMesh(this ExportableModel m)
    {
        return m.GetComponent<SkinnedMeshRenderer>() != null;
    }
}
