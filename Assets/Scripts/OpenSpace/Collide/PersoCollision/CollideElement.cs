namespace OpenSpace.Collide
{
    public class CollideElement
    {
        public Pointer offset;
        public ZdxList parentList;
        public Pointer nextElement;
        public int index;

        CollideElement(Pointer offset, ZdxList parentList)
        {
            this.offset = offset;
            this.parentList = parentList;
        }

        public static CollideElement Read(Reader reader, ZdxList parentList, Pointer offset)
        {
            CollideElement element = new CollideElement(offset, parentList);

            element.nextElement = Pointer.Read(reader);
            element.index = reader.ReadInt32();

            return element;
        }
    }
}