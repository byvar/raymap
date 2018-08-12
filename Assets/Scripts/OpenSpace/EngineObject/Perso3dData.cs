using UnityEngine;
using UnityEditor;

namespace OpenSpace.EngineObject {
    public class Perso3dData {
        public Pointer offset;

        public Pointer off_state0;
        public Pointer off_stateCurrent;
        public Pointer off_state2;

        public Pointer off_physicalObjects;
        public Pointer off_physicalObjectsInitial;
        public Pointer off_family;

        public Family family = null;
        public PhysicalObject[] physical_objects = null;
        public State stateCurrent = null;

        public Perso3dData(Pointer offset) {
            this.offset = offset;
        }

        public static Perso3dData Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            Perso3dData d = new Perso3dData(offset);

            d.off_state0 = Pointer.Read(reader);
            d.off_stateCurrent = Pointer.Read(reader);
            d.off_state2 = Pointer.Read(reader);

            d.off_physicalObjects = Pointer.Read(reader);
            d.off_physicalObjectsInitial = Pointer.Read(reader);
            d.off_family = Pointer.Read(reader);
            d.family = Family.FromOffset(d.off_family);
            d.stateCurrent = State.FromOffset(d.family, d.off_stateCurrent);

            return d;
        }

        public void Write(Writer writer) {
            Pointer.Goto(ref writer, offset + 4);
            Pointer.Write(writer, off_stateCurrent);
        }
    }
}