using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GenericExport.Capturing
{
    public static class ObjectKindDeterminer
    {
        public static bool IsChannel(Transform transform)
        {
            return transform.gameObject.name.StartsWith("Channel");
        }

        public static bool IsMesh(Transform transform)
        {
            return (transform.GetComponent<Renderer>() != null && transform.GetComponent<Renderer>().enabled)
                && (transform.GetComponent<SkinnedMeshRenderer>() != null || transform.GetComponent<MeshFilter>() != null);
        }
    }

    public static class ChannelForMeshObtainer
    {
        public static GameObject GetClosestParentChannel(Transform baseTransform)
        {
            Transform baseT = baseTransform;
            while (!ObjectKindDeterminer.IsChannel(baseT))
            {
                baseT = baseT.parent;
            }
            return baseT.gameObject;
        }
    }

    public static class PersoTraverser
    {
        public static List<Tuple<GameObject, GameObject>> GetChannelsHierarchy(PersoBehaviour persoBehaviour)
        {
            var result = new List<Tuple<GameObject, GameObject>>();

            foreach (Transform transform in persoBehaviour.GetComponentsInChildren<Transform>())
            {
                if (ObjectKindDeterminer.IsChannel(transform) && ObjectKindDeterminer.IsChannel(transform.parent))
                {
                    result.Add(Tuple.Create(transform.parent?.gameObject, transform.gameObject));
                } else if (ObjectKindDeterminer.IsChannel(transform))
                {
                    result.Add(Tuple.Create<GameObject, GameObject>(null, transform.gameObject));
                }
            }

            return result;
        }

        public static List<Tuple<GameObject, GameObject>> GetChannelsMeshesHierarchy(PersoBehaviour persoBehaviour)
        {
            var result = new List<Tuple<GameObject, GameObject>>();

            foreach (Transform transform in persoBehaviour.GetComponentsInChildren<Transform>())
            {
                if (ObjectKindDeterminer.IsMesh(transform))
                {
                    result.Add(Tuple.Create(ChannelForMeshObtainer.GetClosestParentChannel(transform), transform.gameObject));
                }
            }

            return result;
        }

        public static string GetChainedChannelsKeyForSubmesh(GameObject submesh)
        {
            string key = submesh.name;
            Transform baseT = submesh.transform;
            while (baseT.GetComponent<PersoBehaviour>() == null)
            {
                while (!ObjectKindDeterminer.IsChannel(baseT))
                {
                    baseT = baseT.parent;
                }
                key = baseT.name + ">" + key;
                baseT = baseT.parent;
            }
            
            return key;
        }
    }
}
