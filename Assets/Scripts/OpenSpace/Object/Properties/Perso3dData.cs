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

        public Pointer off_morphList;

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
            d.stateCurrent = State.FromOffset(d.family, d.off_stateCurrent); // 0x1C

            //reader.ReadBytes(0x98); //0x20 - 0xB7
            //d.off_morphList = Pointer.Read(reader); // 0xB8, first morph list element

            return d;
        }

        public void UpdateCurrentState(Reader reader)
        {
            MapLoader l = MapLoader.Loader;

            off_stateInitial = Pointer.Read(reader);
            off_stateCurrent = Pointer.Read(reader);
            off_state2 = Pointer.Read(reader);
            stateCurrent = State.FromOffset(family, off_stateCurrent);
        }

        public void Write(Writer writer) {
            Pointer.Write(writer, off_stateInitial);
            Pointer.Write(writer, off_stateCurrent);
            Pointer.Write(writer, off_state2);
            Pointer.Write(writer, off_objectList);
        }
    }
}