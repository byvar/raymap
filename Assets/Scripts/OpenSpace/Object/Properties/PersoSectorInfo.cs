using UnityEngine;
using UnityEditor;

namespace OpenSpace.Object.Properties {
    public class PersoSectorInfo {
        public LegacyPointer offset;

        public LegacyPointer off_sector;
        public Sector sector;

        public PersoSectorInfo(LegacyPointer offset) {
            this.offset = offset;
        }

        public static PersoSectorInfo Read(Reader reader, LegacyPointer offset) {
            MapLoader l = MapLoader.Loader;
            PersoSectorInfo si = new PersoSectorInfo(offset);

            si.off_sector = LegacyPointer.Read(reader);
            // Size in r2: 14 dwords total
            // Size in r3 & ra: 6 dwords total
            return si;
        }
    }
}