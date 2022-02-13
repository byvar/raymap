using System;
using System.Collections.Generic;

namespace OpenSpace.Waypoints {
    public class ArcList {

        public LegacyPointer offset;
        public LinkedList<Arc> list;

        public ArcList(LegacyPointer offset)
        {
            this.offset = offset;
        }

        public static ArcList Read(Reader reader, LegacyPointer offset)
        {
            ArcList arcList = new ArcList(offset);

            LegacyPointer.DoAt(ref reader, offset, () => {
                //zdxList = LinkedList<CollideMeshObject>.ReadHeader(r1, o1);
                arcList.list = LinkedList<Arc>.Read(ref reader, offset,
                    (off_element) => {
                        return Arc.Read(reader, off_element);
                    },
                    flags: LinkedList.Flags.HasHeaderPointers,
                    type: LinkedList.Type.Double
                );
            });

            return arcList;
        }
    }
}