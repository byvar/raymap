using OpenSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenSpace.Waypoints {
    public class Arc {

        public LegacyPointer offset;
        public LegacyPointer off_nextArc;
        public uint field_0x4;
        public uint field_0x8;
        public LegacyPointer off_node;
        public uint capabilities;
        public uint field_0x14;
        public int weight;
        public int field_0x1C;

        public GraphNode graphNode;

        public Arc(LegacyPointer offset)
        {
            this.offset = offset;
        }

        public static Arc Read(Reader reader, LegacyPointer offset)
        {
            Arc arc = new Arc(offset);
            
            // First three pointers are part of linkedlist (next, previous, header)
            arc.off_node = LegacyPointer.Read(reader);
            arc.capabilities = reader.ReadUInt32();
            arc.field_0x14 = reader.ReadUInt32();
            arc.weight = reader.ReadInt32();
            arc.field_0x1C = reader.ReadInt32();

            if (arc.off_node != null) {
                LegacyPointer.DoAt(ref reader, arc.off_node, () => {
                    arc.graphNode = GraphNode.FromOffsetOrRead(arc.off_node, reader);
                });
            }

            return arc;
        }
    }
}
