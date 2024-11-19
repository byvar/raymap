using Assets.Scripts.GenericExport.Model.DataBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GenericExport.Manipulation.Meshes
{
    public class MeshUVMapFetcher
    {
        public static ExportUVMap GetUVMap(Mesh mesh)
        {
            List<Vector2> uvMap = mesh.uv.ToList();
            var result = new ExportUVMap();
            result.uv = uvMap.Select(x => new ExportVector2(x.x, x.y)).ToList();
            return result;
        }
    }
}
