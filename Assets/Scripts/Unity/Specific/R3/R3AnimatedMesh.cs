using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class R3AnimatedMesh : MonoBehaviour
{
    ObjectsExportLibraryInterface objectsExportLibraryInterface;

    bool IsSkinnedMesh()
    {
        return GetComponent<SkinnedMeshRenderer>() != null;
    }

    bool IsChannelParentedMesh()
    {
        throw new NotImplementedException();
    }

    public void AddToExportObjectsLibrary()
    {
        if (IsSkinnedMesh())
        {
            objectsExportLibraryInterface.AddSkinnedMeshToLibrary(this);
        } else if (IsChannelParentedMesh())
        {
            objectsExportLibraryInterface.AddChannelParentedMesh(this);
        } else
        {
            throw new InvalidOperationException("The GameObject to export animated mesh from does not component Skinned Mesh Renderer nor is child of the channel object!");
        }
    }

    public void ClearExportObjectsLibrary()
    {
        objectsExportLibraryInterface.ClearExportObjectsLibrary();
    }
}
