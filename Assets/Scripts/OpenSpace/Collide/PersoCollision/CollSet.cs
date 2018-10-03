using OpenSpace.Object;
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

        public LinkedList<CollideElement> zddZones;
        public LinkedList<CollideElement> zdeZones;
        public LinkedList<CollideElement> zdmZones;
        public LinkedList<CollideElement> zdrZones;

        public CollSet(Perso perso, Pointer offset) {
            this.perso = perso;
            this.offset = offset;
        }

        private static LinkedList<CollideMeshObject> ParseZdxList(Reader reader, Pointer offset, CollideMeshObject.Type type) {
            MapLoader l = MapLoader.Loader;
            LinkedList<CollideMeshObject> zdxList = null;
            Pointer.DoAt(ref reader, offset, () => {
                //zdxList = LinkedList<CollideMeshObject>.ReadHeader(r1, o1);
                zdxList = LinkedList<CollideMeshObject>.Read(ref reader, offset,
                    (off_element) => {
                        return CollideMeshObject.Read(reader, off_element, type: type);
                    },
                    flags: LinkedList.Flags.ReadAtPointer
                        | (Settings.s.hasLinkedListHeaderPointers ?
                            LinkedList.Flags.HasHeaderPointers :
                            LinkedList.Flags.NoPreviousPointersForDouble),
                    type: LinkedList.Type.Minimize
                );
            });
            return zdxList;
        }

        private static LinkedList<CollideElement> ParseZdxZoneList(Reader reader, Pointer offset, CollideMeshObject.Type type) {
            MapLoader l = MapLoader.Loader;
            LinkedList<CollideElement> zdxZoneList = null;
            Pointer.DoAt(ref reader, offset, () => {
                //zdxList = LinkedList<CollideMeshObject>.ReadHeader(r1, o1);
                zdxZoneList = LinkedList<CollideElement>.Read(ref reader, offset,
                    (off_element) => {
                        return CollideElement.Read(reader, off_element);
                    },
                    flags: LinkedList.Flags.NoPreviousPointersForDouble,
                    type: LinkedList.Type.Minimize
                );
            });
            return zdxZoneList;
        }

        public static CollSet Read(Reader reader, Perso perso, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            //if (Settings.s.platform == Settings.Platform.DC) return null;
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

            c.zdd = ParseZdxList(reader, c.off_zdd, CollideMeshObject.Type.ZDD);
            c.zde = ParseZdxList(reader, c.off_zde, CollideMeshObject.Type.ZDE);
            c.zdm = ParseZdxList(reader, c.off_zdm, CollideMeshObject.Type.ZDM);
            c.zdr = ParseZdxList(reader, c.off_zdr, CollideMeshObject.Type.ZDR);
            if (c.zdd != null) foreach (CollideMeshObject col in c.zdd) {
                    if (col == null) continue;
                    col.gao.transform.SetParent(perso.Gao.transform);
                }
            if (c.zde != null) foreach (CollideMeshObject col in c.zde) {
                    if (col == null) continue;
                    col.gao.transform.SetParent(perso.Gao.transform);
                }
            if (c.zdm != null) foreach (CollideMeshObject col in c.zdm) {
                    if (col == null) continue;
                    col.gao.transform.SetParent(perso.Gao.transform);
                }
            if (c.zdr != null) foreach (CollideMeshObject col in c.zdr) {
                    if (col == null) continue;
                    col.gao.transform.SetParent(perso.Gao.transform);
                }
            c.zddZones = ParseZdxZoneList(reader, c.off_zones_zdd, CollideMeshObject.Type.ZDD);
            c.zdeZones = ParseZdxZoneList(reader, c.off_zones_zde, CollideMeshObject.Type.ZDE);
            c.zdmZones = ParseZdxZoneList(reader, c.off_zones_zdm, CollideMeshObject.Type.ZDM);
            c.zdrZones = ParseZdxZoneList(reader, c.off_zones_zdr, CollideMeshObject.Type.ZDR);

            return c;
        }
    }
}