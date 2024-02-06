using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GenericExport.Capturing
{
    public static class ObjectKindDeterminer
    {
        public static bool IsChannel(Transform transform)
        {
            return transform.gameObject.name.StartsWith("Channel");
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
    }
}
