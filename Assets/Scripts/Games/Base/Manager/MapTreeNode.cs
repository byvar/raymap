using System.Collections.Generic;

namespace Raymap {
    public class MapTreeNode {
        public MapTreeNode(string name) {
            Name = name;
        }

        public string Name { get; set; }

        public List<MapTreeNode> Children { get; set; } = new List<MapTreeNode>();
    }
}