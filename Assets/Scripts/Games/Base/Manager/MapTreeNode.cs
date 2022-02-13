using System.Collections.Generic;

namespace Raymap {
    public class MapTreeNode {
        public MapTreeNode(string id, string displayName) {
            Id = id;
            DisplayName = displayName;
        }

        public string Id { get; set; }

        public string DisplayName { get; set; }

        public MapTreeNode[] Children { get; set; }

        public IEnumerable<MapTreeNode> GetMaps() {
            if(Id != null) yield return this;
            if (Children != null) {
                foreach (var child in Children) {
                    foreach (var x in child.GetMaps()) yield return x;
                }
            }
        }
    }
}