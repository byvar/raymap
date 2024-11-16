using Assets.Scripts.GenericExport.Model.DataBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GenericExport.Manipulation.Meshes
{
    public class MeshTextureFetcher
    {
        public static ExportTexture GetTexture(Transform meshObject)
        {
            Renderer renderer = meshObject.GetComponent<Renderer>();
            Material material = renderer.material;
            Texture texture = material.mainTexture;
            throw new NotImplementedException();
        }
    }
}
