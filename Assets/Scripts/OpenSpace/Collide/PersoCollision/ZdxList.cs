using System.Collections.Generic;

namespace OpenSpace.Collide
{
    public class ZdxList
    {
        Pointer offset;
        CollSet collset;

        public List<CollideElement> elementList = new List<CollideElement>();

        public ZdxList(CollSet collset, Pointer offset)
        {
            this.collset = collset;
            this.offset = offset;
        }

        public static ZdxList Read(EndianBinaryReader reader, CollSet collset, Pointer offset)
        {
            ZdxList zdxList = new ZdxList(collset, offset);

            Pointer nextElement = Pointer.Read(reader);
            while (nextElement != null) {
                Pointer.Goto(ref reader, nextElement);
                CollideElement element = CollideElement.Read(reader, zdxList, nextElement);
                zdxList.elementList.Add(element);
                nextElement = element.nextElement;
            }

            return zdxList;
        }
    }
}