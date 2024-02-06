using System.Collections.Generic;

namespace Assets.Scripts.GenericExport.Utils
{
    public class TreeNodeContainer<KeyType, ValueType>
    {
        public List<TreeNodeContainer<KeyType, ValueType>> children = 
            new List<TreeNodeContainer<KeyType, ValueType>>();
    }

    public class Tree<KeyType, ValueType>
    {
        public List<TreeNodeContainer<KeyType, ValueType>> roots = 
            new List<TreeNodeContainer<KeyType, ValueType>>();
    }
}
