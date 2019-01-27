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

        public int privilegedActivationsZDD; // consists of 16 bit pairs that describe the state of a zone, 00 = neutral, 01 = force active, 10 = force inactive
        public int privilegedActivationsZDE; // ..
        public int privilegedActivationsZDM; // ..
        public int privilegedActivationsZDR; // access these using GetPrivilegedActionZoneStatus

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

        public enum PrivilegedActivationStatus {
            Neutral = 0,
            ForceActive = 1,
            ForceInactive = 2
        }

        public PrivilegedActivationStatus GetPrivilegedActionZoneStatus(CollideMeshObject.Type type, int index)
        {
            int activations = 0;
            switch(type) {
                case CollideMeshObject.Type.ZDD: activations = privilegedActivationsZDD; break;
                case CollideMeshObject.Type.ZDE: activations = privilegedActivationsZDE; break;
                case CollideMeshObject.Type.ZDM: activations = privilegedActivationsZDM; break;
                case CollideMeshObject.Type.ZDR: activations = privilegedActivationsZDR; break;
            }
            int offset = index * 2;
            int value = ((1 << 2) - 1) & (activations >> (offset)); // extract 2 bits from offset
            return (PrivilegedActivationStatus)value;
        }

        private static LinkedList<CollideMeshObject> ParseZdxList(Reader reader, Pointer offset, CollSet collset, CollideMeshObject.Type type) {
            MapLoader l = MapLoader.Loader;
            LinkedList<CollideMeshObject> zdxList = null;

            int index = 0;

            Pointer.DoAt(ref reader, offset, () => {
                //zdxList = LinkedList<CollideMeshObject>.ReadHeader(r1, o1);
                zdxList = LinkedList<CollideMeshObject>.Read(ref reader, offset,
                    (off_element) => {
                        return CollideMeshObject.Read(reader, off_element, collset, index++, type: type);
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

        private static LinkedList<CollideElement> ParseZdxZoneList(Reader reader, Pointer offset, CollSet collset, CollideMeshObject.Type type) {
            MapLoader l = MapLoader.Loader;
            LinkedList<CollideElement> zdxZoneList = null;
            Pointer.DoAt(ref reader, offset, () => {
                //zdxList = LinkedList<CollideMeshObject>.ReadHeader(r1, o1);
                zdxZoneList = LinkedList<CollideElement>.Read(ref reader, offset,
                    (off_element) => {
                        return CollideElement.Read(reader, off_element, collset, type);
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

            c.privilegedActivationsZDD = reader.ReadInt32();
            c.privilegedActivationsZDE = reader.ReadInt32();
            c.privilegedActivationsZDM = reader.ReadInt32();
            c.privilegedActivationsZDR = reader.ReadInt32();

            c.zdd = ParseZdxList(reader, c.off_zdd, c, CollideMeshObject.Type.ZDD);
            c.zde = ParseZdxList(reader, c.off_zde, c, CollideMeshObject.Type.ZDE);
            c.zdm = ParseZdxList(reader, c.off_zdm, c, CollideMeshObject.Type.ZDM);
            c.zdr = ParseZdxList(reader, c.off_zdr, c, CollideMeshObject.Type.ZDR);
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
            c.zddZones = ParseZdxZoneList(reader, c.off_zones_zdd, c, CollideMeshObject.Type.ZDD);
            c.zdeZones = ParseZdxZoneList(reader, c.off_zones_zde, c, CollideMeshObject.Type.ZDE);
            c.zdmZones = ParseZdxZoneList(reader, c.off_zones_zdm, c, CollideMeshObject.Type.ZDM);
            c.zdrZones = ParseZdxZoneList(reader, c.off_zones_zdr, c, CollideMeshObject.Type.ZDR);

            return c;
        }
    }
}