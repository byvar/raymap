using OpenSpace.EngineObject;
using System.Collections.Generic;

namespace OpenSpace.Collide {
    public class CollSet {
        public Pointer offset;
        public Perso perso;

        // Struct
        public Pointer off_zdd;
        public Pointer off_zde;
        public Pointer off_zdm;
        public Pointer off_zdr;
        public Pointer off_activation_zdd;
        public Pointer off_activation_zde;
        public Pointer off_activation_zdm;
        public Pointer off_activation_zdr;
        public Pointer off_zones_zdd;
        public Pointer off_zones_zde;
        public Pointer off_zones_zdm;
        public Pointer off_zones_zdr;

        public uint privilegedActivationZDD;
        public uint privilegedActivationZDE;
        public uint privilegedActivationZDM;
        public uint privilegedActivationZDR;

        // Generated
        public LinkedList<CollideMeshObject> zdd;
        public LinkedList<CollideMeshObject> zde;
        public LinkedList<CollideMeshObject> zdm;
        public LinkedList<CollideMeshObject> zdr;

        public ZdxList zddZones;
        public ZdxList zdeZones;
        public ZdxList zdmZones;
        public ZdxList zdrZones;

        public CollSet(Perso perso, Pointer offset) {
            this.perso = perso;
            this.offset = offset;
        }

        private static LinkedList<CollideMeshObject> ParseZdxList(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            LinkedList<CollideMeshObject> zdxList = null;
            Pointer.DoAt(ref reader, offset, (Reader r1, Pointer o1) => {
                //zdxList = LinkedList<CollideMeshObject>.ReadHeader(r1, o1);
                zdxList = LinkedList<CollideMeshObject>.Read(r1, o1,
                    (Reader r, Pointer o) => {
                        return CollideMeshObject.Read(r, o);
                    },
                    flags: LinkedList.Flags.ReadAtPointer
                        | (l.mode == MapLoader.Mode.Rayman3GC ?
                            LinkedList.Flags.HasHeaderPointers :
                            LinkedList.Flags.NoPreviousPointersForDouble),
                    type: l.mode == MapLoader.Mode.RaymanArenaGC ?
                        LinkedList.Type.SingleNoElementPointers :
                        LinkedList.Type.Default
                );
            });
            return zdxList;
        }

        public static CollSet Read(Reader reader, Perso perso, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            CollSet c = new CollSet(perso, offset);

            c.off_zdd = Pointer.Read(reader);
            c.off_zde = Pointer.Read(reader);
            c.off_zdm = Pointer.Read(reader);
            c.off_zdr = Pointer.Read(reader);

            c.off_activation_zdd = Pointer.Read(reader);
            c.off_activation_zde = Pointer.Read(reader);
            c.off_activation_zdm = Pointer.Read(reader);
            c.off_activation_zdr = Pointer.Read(reader);

            c.off_zones_zdd = Pointer.Read(reader);
            c.off_zones_zde = Pointer.Read(reader);
            c.off_zones_zdr = Pointer.Read(reader);
            c.off_zones_zdm = Pointer.Read(reader);

            c.privilegedActivationZDD = reader.ReadUInt32();
            c.privilegedActivationZDE = reader.ReadUInt32();
            c.privilegedActivationZDM = reader.ReadUInt32();
            c.privilegedActivationZDR = reader.ReadUInt32();

            c.zdd = ParseZdxList(reader, c.off_zdd);
            c.zde = ParseZdxList(reader, c.off_zde);
            c.zdm = ParseZdxList(reader, c.off_zdm);
            c.zdr = ParseZdxList(reader, c.off_zdr);

            if (l.mode != MapLoader.Mode.RaymanArenaGC) {
                if (c.off_zones_zdd != null) {
                    Pointer.Goto(ref reader, c.off_zones_zdd);
                    c.zddZones = ZdxList.Read(reader, c, c.off_zones_zdd);
                }
                if (c.off_zones_zde != null) {
                    Pointer.Goto(ref reader, c.off_zones_zde);
                    c.zdeZones = ZdxList.Read(reader, c, c.off_zones_zde);
                }
                if (c.off_zones_zdm != null) {
                    Pointer.Goto(ref reader, c.off_zones_zdm);
                    c.zdmZones = ZdxList.Read(reader, c, c.off_zones_zdm);
                }
                if (c.off_zones_zdr != null) {
                    Pointer.Goto(ref reader, c.off_zones_zdr);
                    c.zdrZones = ZdxList.Read(reader, c, c.off_zones_zdr);
                }
            }

            return c;
        }
    }
}