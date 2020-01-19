using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utils
{
    public class ParentChildPair<T>
    {
        public T Parent;
        public T Child;

        public ParentChildPair(T Parent, T Child)
        {
            this.Parent = Parent;
            this.Child = Child;
        }
    }

    public class TreeBuildingNodeInfo<T, KeyType>
    {
        public KeyType ParentId;
        public KeyType NodeId;
        public T Node;

        public TreeBuildingNodeInfo(KeyType ParentId, KeyType NodeId, T Node)
        {
            this.ParentId = ParentId;
            this.NodeId = NodeId;
            this.Node = Node;
        }
    }

    public class TreeNodeContainer<T, KeyType>
    {
        public T Node;
        public KeyType Id;

        public List<TreeNodeContainer<T, KeyType>> Children;

        public TreeNodeContainer(KeyType Id, T Node)
        {
            this.Node = Node;
            this.Children = new List<TreeNodeContainer<T, KeyType>>();
        }

        public void TraverseAndCollectAll(List<TreeNodeIterator<T, KeyType>> Nodes)
        {
            Nodes.Add(new TreeNodeIterator<T, KeyType>(Id, Node));

            foreach (var Child in Children)
            {
                Child.TraverseAndCollectAll(Nodes);
            }
        }

        public void TraverseChildParentPairsAndCollectAll(List<ParentChildPair<T>> pairs)
        {
            foreach (var Child in Children)
            {
                pairs.Add(new ParentChildPair<T>(this.Node, Child.Node));
            }
            foreach (var Child in Children)
            {
                Child.TraverseChildParentPairsAndCollectAll(pairs);
            }
        }

        public bool TraverseAndAddNode(KeyType ParentIdentifier, KeyType NodeIdentifier, T Node)
        {
            if (Id.Equals(ParentIdentifier))
            {
                Children.Add(new TreeNodeContainer<T, KeyType>(NodeIdentifier, Node));
                return true;
            }
            else
            {
                foreach (var Child in Children)
                {
                    if (Child.TraverseAndAddNode(ParentIdentifier, NodeIdentifier, Node))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }

    public class TreeNodeIterator<T, KeyType>
    {
        public KeyType Id;
        public T Node;

        public TreeNodeIterator(KeyType Id, T Node)
        {
            this.Id = Id;
            this.Node = Node;
        }
    }

    public class Tree<T, KeyType>
    {
        private TreeNodeContainer<T, KeyType> Root;

        public T GetRoot()
        {
            return Root.Node;
        }

        public void AddNode(
            KeyType parentIdentifier,
            KeyType nodeIdentifier,
            T node)
        {
            if (parentIdentifier == null)
            {
                Root = new TreeNodeContainer<T, KeyType>(nodeIdentifier, node);
            }
            else
            {
                if (Root.TraverseAndAddNode(parentIdentifier, nodeIdentifier, node))
                {
                    return;
                }
                else
                {
                    throw new InvalidOperationException("Did not find parent node of that id! " + nodeIdentifier);
                }
            }
        }

        public IEnumerable<TreeNodeIterator<T, KeyType>> IterateNodes()
        {
            List<TreeNodeIterator<T, KeyType>> nodes = new List<TreeNodeIterator<T, KeyType>>();
            if (Root != null)
            {
                Root.TraverseAndCollectAll(nodes);
            }
            foreach (var node in nodes)
            {
                yield return node;
            }
        }

        public IEnumerable<ParentChildPair<T>> IterateParentChildPairs()
        {
            List<ParentChildPair<T>> pairs = new List<ParentChildPair<T>>();
            if (Root != null)
            {
                Root.TraverseChildParentPairsAndCollectAll(pairs);
            }
            foreach (var pair in pairs)
            {
                yield return pair;
            }
        }

        public bool Contains(KeyType Id)
        {
            foreach (var node in IterateNodes())
            {
                if (node.Id.Equals(Id))
                {
                    return true;
                }
            }
            return false;
        }

        public static Tree<T, KeyType> BuildTreeWithProperNodesPuttingOrder(
            Tree<T, KeyType> ExistingTree, 
            HashSet<TreeBuildingNodeInfo<T, KeyType>> TreeBuldingNodes)
        {
            Tree<T, KeyType> result;
            if (ExistingTree == null)
            {
                result = new Tree<T, KeyType>();
            }
            else
            {
                result = ExistingTree;
            }
            
            while (TreeBuldingNodes.Count != 0)
            {
                foreach (var Node in TreeBuldingNodes)
                {
                    if (result.Contains(Node.ParentId))
                    {
                        result.AddNode(
                            Node.ParentId,
                            Node.NodeId,
                            Node.Node
                        );
                        TreeBuldingNodes.Remove(Node);
                        break;
                    } else if (Node.ParentId == null)
                    {
                        result.AddNode(
                            default(KeyType),
                            Node.NodeId,
                            Node.Node
                            );
                    }
                }
            }
            return result;
        }
    }
}
