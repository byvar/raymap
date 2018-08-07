using System.Collections.Generic;

namespace OpenSpace.Collide
{
    public class ZdxList
    {
        public Pointer offset;
        public CollSet collset;
        public Pointer off_element_first;
        public Pointer off_element_last;
        public uint num_elements;

        public List<CollideElement> elementList = new List<CollideElement>();

        public ZdxList(CollSet collset, Pointer offset) {
            this.collset = collset;
            this.offset = offset;
        }

        public static ZdxList Read(EndianBinaryReader reader, CollSet collset, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            ZdxList zdxList = new ZdxList(collset, offset);
            zdxList.off_element_first = Pointer.Read(reader);
            zdxList.off_element_last = Pointer.Read(reader);
            zdxList.num_elements = reader.ReadUInt32();

            //if (zdxList.off_element_first != null) l.print(offset);
            Pointer nextElement = zdxList.off_element_first;
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