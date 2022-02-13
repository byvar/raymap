using UnityEngine;
using UnityEditor;

namespace OpenSpace.Object.Properties {
    public class Perso3dData {
        public LegacyPointer offset;

        public LegacyPointer off_stateInitial;
        public LegacyPointer off_stateCurrent;
        public LegacyPointer off_state2;

        public LegacyPointer off_objectList;
        public LegacyPointer off_objectListInitial;
        public LegacyPointer off_family;

        public LegacyPointer off_morphList;

        public Family family = null;
        public ObjectList objectList = null;
        public State stateCurrent = null;

        public Perso3dData(LegacyPointer offset) {
            this.offset = offset;
        }

        public static Perso3dData Read(Reader reader, LegacyPointer offset) {
            MapLoader l = MapLoader.Loader;
            Perso3dData d = new Perso3dData(offset);

            d.off_stateInitial = LegacyPointer.Read(reader);
            d.off_stateCurrent = LegacyPointer.Read(reader);
            d.off_state2 = LegacyPointer.Read(reader);

            d.off_objectList = LegacyPointer.Read(reader);
            d.off_objectListInitial = LegacyPointer.Read(reader);
            d.off_family = LegacyPointer.Read(reader);
            d.family = Family.FromOffset(d.off_family);
            d.stateCurrent = State.FromOffset(d.family, d.off_stateCurrent); // 0x1C

            //reader.ReadBytes(0x98); //0x20 - 0xB7
            //d.off_morphList = Pointer.Read(reader); // 0xB8, first morph list element

            return d;
        }

        public void UpdateCurrentState(Reader reader)
        {
            MapLoader l = MapLoader.Loader;

            off_stateInitial = LegacyPointer.Read(reader);
            off_stateCurrent = LegacyPointer.Read(reader);
            off_state2 = LegacyPointer.Read(reader);
            stateCurrent = State.FromOffset(family, off_stateCurrent);
        }

        public void Write(Writer writer) {
            LegacyPointer.Goto(ref writer, offset);
            LegacyPointer.Write(writer, off_stateInitial);
            LegacyPointer.Write(writer, off_stateCurrent);
            LegacyPointer.Write(writer, off_state2);
            LegacyPointer.Write(writer, off_objectList);
        }
    }
}