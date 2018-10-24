using OpenSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenSpace.Waypoints {
    public class Arc {

        public Pointer offset;
        public Pointer off_nextArc;
        public uint field_0x4;
        public uint field_0x8;
        public Pointer off_node;
        public uint capabilities;
        public uint field_0x14;
        public int weight;
        public int field_0x1C;

        public GraphNode graphNode;

        public Arc(Pointer offset)
        {
            this.offset = offset;
        }

        public static Arc Read(Reader reader, Pointer offset)
        {
            Arc arc = new Arc(offset);
            
            // First three pointers are part of linkedlist (next, previous, header)
            arc.off_node = Pointer.Read(reader);
            arc.capabilities = reader.ReadUInt32();
            arc.field_0x14 = reader.ReadUInt32();
            arc.weight = reader.ReadInt32();
            arc.field_0x1C = reader.ReadInt32();

            if (arc.off_node != null) {
                Pointer.DoAt(ref reader, arc.off_node, () => {
                    arc.graphNode = GraphNode.FromOffsetOrRead(arc.off_node, reader);
                });
            }

            return arc;
        }
    }
}
