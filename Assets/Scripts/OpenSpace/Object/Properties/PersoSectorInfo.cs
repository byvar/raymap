using UnityEngine;
using UnityEditor;

namespace OpenSpace.Object.Properties {
    public class PersoSectorInfo {
        public Pointer offset;

        public Pointer off_sector;
        public Sector sector;

        public PersoSectorInfo(Pointer offset) {
            this.offset = offset;
        }

        public static PersoSectorInfo Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            PersoSectorInfo si = new PersoSectorInfo(offset);

            si.off_sector = Pointer.Read(reader);
            // Size in r2: 14 dwords total
            // Size in r3 & ra: 6 dwords total
            return si;
        }
    }
}