using UnityEngine;
using UnityEditor;
using OpenSpace;

public class GraphNode {

    public Pointer offset;
    public GraphNode nextNode;
    public GraphNode previousNode;
    public GraphNode node;
    public WayPoint wayPoint;

    public Pointer off_nextNode;
    public Pointer off_prevNode;
    public Pointer off_node;
    public Pointer off_wayPoint;

    public GraphNode(Pointer offset)
    {
        this.offset = offset;
    }

    public static GraphNode Read(EndianBinaryReader reader, Pointer offset)
    {
        GraphNode node = new GraphNode(offset);

        node.off_nextNode = Pointer.Read(reader);
        node.off_prevNode = Pointer.Read(reader);

        //node.off_node = Pointer.Read(reader);
        reader.ReadUInt32();

        node.off_wayPoint = Pointer.Read(reader);

        /*Pointer start = Pointer.Goto(ref reader, node.off_node);
        node.node = GraphNode.Read(reader, node.off_node);
        Pointer.Goto(ref reader, start);*/

        Pointer start = Pointer.Goto(ref reader, node.off_wayPoint);
        node.wayPoint = WayPoint.Read(reader, node.off_wayPoint);
        Pointer.Goto(ref reader, start);

        return node;
    }
}