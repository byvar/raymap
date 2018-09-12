using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class DsgVarInfoEntry {
        public Pointer offset;

        public uint offsetInBuffer; // offset in DsgMemBuffer
        public uint typeNumber;
        public uint saveType;
        public uint initType;
        public uint number;

        // Derived values
        public DsgVarType type;
        public object value;
        public object initialValue;

        public string NiceVariableName {
            get {
                return type + "_" + number;
            }
        }

        public DsgVarInfoEntry(Pointer offset) {
            this.offset = offset;
        }

        public static DsgVarInfoEntry Read(Reader reader, Pointer offset, uint number) {
            MapLoader l = MapLoader.Loader;
            DsgVarInfoEntry d = new DsgVarInfoEntry(offset);
            d.offsetInBuffer = reader.ReadUInt32();
            d.typeNumber = reader.ReadUInt32();
            d.saveType = reader.ReadUInt32();
            d.initType = reader.ReadUInt32();

            d.number = number;

            d.type = Settings.s.aiTypes.GetDsgVarType(d.typeNumber);

            return d;
        }

        public enum DsgVarType {
            None,
            Boolean,
            Byte,
            UByte, // Unsigned
            Short,
            UShort, // Unsigned
            Int,
            UInt, // Unsigned
            Float,
            Vector,
            List,
            Comport,
            Action,
            Caps, // Capabilities
            Input,
            SoundEvent,
            Light,
            GameMaterial,
            VisualMaterial,
            Perso,
            Waypoint,
            Graph,
            Text,
            SuperObject,
            SOLinks,
            PersoArray,
            VectorArray,
            FloatArray,
            IntegerArray,
            WayPointArray,
            TextArray,
            Array7,
            Array8,
            Array9,
            Array10,
            Array11,
            Way // TT SE only
        }
    }
}
