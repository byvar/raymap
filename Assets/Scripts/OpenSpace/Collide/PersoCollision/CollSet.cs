using OpenSpace.EngineObject;

namespace OpenSpace.Collide
{
    public class CollSet
    {
        public Pointer offset;
        public Perso perso;

        // Struct

        public Pointer off_zddList;
        public Pointer off_zdeList;
        public Pointer off_zdmList;
        public Pointer off_zdrList;

        public uint privilegedActivationZDD;
        public uint privilegedActivationZDE;
        public uint privilegedActivationZDM;
        public uint privilegedActivationZDR;

        // Generated

        public ZdxList zddList;
        public ZdxList zdeList;
        public ZdxList zdmList;
        public ZdxList zdrList;

        public CollSet(Perso perso, Pointer offset)
        {
            this.perso = perso;
            this.offset = offset;
        }

        public static CollSet Read(EndianBinaryReader reader, Perso perso, Pointer offset)
        {
            MapLoader loader = MapLoader.Loader;
            CollSet collset = new CollSet(perso, offset);

            reader.ReadBytes(32); // 32 byte gap
            collset.off_zddList = Pointer.Read(reader);
            collset.off_zdeList = Pointer.Read(reader);
            collset.off_zdmList = Pointer.Read(reader);
            collset.off_zdrList = Pointer.Read(reader);

            collset.privilegedActivationZDD = reader.ReadUInt32();
            collset.privilegedActivationZDE = reader.ReadUInt32();
            collset.privilegedActivationZDM = reader.ReadUInt32();
            collset.privilegedActivationZDR = reader.ReadUInt32();

            if (collset.off_zddList != null) {
                Pointer.Goto(ref reader, collset.off_zddList);
                collset.zddList = ZdxList.Read(reader, collset, collset.off_zddList);
            }
            if (collset.off_zdeList != null) {
                Pointer.Goto(ref reader, collset.off_zdeList);
                collset.zdeList = ZdxList.Read(reader, collset, collset.off_zdeList);
            }
            if (collset.off_zdmList != null) {
                Pointer.Goto(ref reader, collset.off_zdmList);
                collset.zdmList = ZdxList.Read(reader, collset, collset.off_zdmList);
            }
            if (collset.off_zdrList != null) {
                Pointer.Goto(ref reader, collset.off_zdrList);
                collset.zdrList = ZdxList.Read(reader, collset, collset.off_zdrList);
            }

            return collset;
        }
    }
}