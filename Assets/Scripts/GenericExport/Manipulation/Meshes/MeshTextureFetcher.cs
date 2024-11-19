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
            Texture2D texture = material.GetTexture("_Tex0") as Texture2D;
            Color[] pixels = texture.GetPixels();
            int width = texture.width;
            int height = texture.height;

            var result = new ExportTexture();
            result.width = width;
            result.height = height;
            result.pixels = pixels.Select(x => ExportColor.FromColor(x)).ToList();

            return result;
        }
    }
}
