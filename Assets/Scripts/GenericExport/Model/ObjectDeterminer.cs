using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GenericExport.Model
{
    public static class ObjectDeterminer
    {
        public static bool IsSubmesh(Transform transform)
        {
            return (transform.GetComponent<SkinnedMeshRenderer>() != null ||
                transform.GetComponent<MeshFilter>() != null) &&
                transform.GetComponent<Renderer>()?.enabled == true;
        }

        public static bool IsChannel(Transform transform)
        {
            return transform.name.Contains("Channel");
        }

        public static string GetChainedChannelsKey(Transform submesh)
        {
            var baseT = submesh;
            string result = submesh.name;
            while (baseT.GetComponent<PersoBehaviour>() == null)
            {
                if (IsChannel(baseT))
                {
                    result = baseT.name + ">" + result;
                }
                baseT = baseT.parent;
            }
            return result;
        }
    }
}
