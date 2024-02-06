using System.Collections.Generic;
using UnityEditor;

namespace Assets.Scripts.GenericExport.Utils
{
    public class TreeNodeContainer<KeyType, ValueType>
    {
        public Dictionary<KeyType, TreeNodeContainer<KeyType, ValueType>> children = 
            new Dictionary<KeyType, TreeNodeContainer<KeyType, ValueType>>();
        public ValueType value;

        public void Add(KeyType parentKey, KeyType key, ValueType value)
        {
            if (parentKey == null)
            {
                children[key] = new TreeNodeContainer<KeyType, ValueType>();
                children[key].value = value;
                return;
            }

            if (children.ContainsKey(parentKey))
            {
                children[parentKey].Add(default, key, value);
            } else
            {
                foreach (var child in children.Values)
                {
                    child.Add(parentKey, key, value);
                }
            }
        }
    }

    public class Tree<KeyType, ValueType>
    {
        public Dictionary<KeyType, TreeNodeContainer<KeyType, ValueType>> roots = 
            new Dictionary<KeyType, TreeNodeContainer<KeyType, ValueType>>();

        public void Add(KeyType parentKey, KeyType key, ValueType value)
        {
            if (parentKey == null)
            {
                roots[key] = new TreeNodeContainer<KeyType, ValueType>();
                roots[key].value = value;
                return;
            }

            if (roots.ContainsKey(parentKey))
            {
                roots[parentKey].Add(default, key, value);
            } else
            {
                foreach (var root in roots.Values)
                {
                    root.Add(parentKey, key, value);
                }
            }
        }
    }
}
