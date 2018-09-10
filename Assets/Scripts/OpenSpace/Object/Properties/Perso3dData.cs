using UnityEngine;
using UnityEditor;

namespace OpenSpace.Object.Properties {
    public class Perso3dData {
        public Pointer offset;

        public Pointer off_stateInitial;
        public Pointer off_stateCurrent;
        public Pointer off_state2;

        public Pointer off_objectList;
        public Pointer off_objectListInitial;
        public Pointer off_family;

        public Family family = null;
        public ObjectList objectList = null;
        public State stateCurrent = null;

        public Perso3dData(Pointer offset) {
            this.offset = offset;
        }

        public static Perso3dData Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            Perso3dData d = new Perso3dData(offset);

            d.off_stateInitial = Pointer.Read(reader);
            d.off_stateCurrent = Pointer.Read(reader);
            d.off_state2 = Pointer.Read(reader);

            d.off_objectList = Pointer.Read(reader);
            d.off_objectListInitial = Pointer.Read(reader);
            d.off_family = Pointer.Read(reader);
            d.family = Family.FromOffset(d.off_family);
            d.stateCurrent = State.FromOffset(d.family, d.off_stateCurrent);

            return d;
        }

        public void Write(Writer writer) {
            Pointer.Write(writer, off_stateInitial);
            Pointer.Write(writer, off_stateCurrent);
            Pointer.Write(writer, off_stateCurrent);
            Pointer.Write(writer, off_objectList);
        }
    }
}