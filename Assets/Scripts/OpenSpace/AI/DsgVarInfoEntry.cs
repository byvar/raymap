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
			//l.print(offset);
			if (Settings.s.game == Settings.Game.R2Revolution) {
				d.offsetInBuffer = reader.ReadUInt16();
				reader.ReadUInt16();
				d.typeNumber = reader.ReadByte();
				d.saveType = reader.ReadByte();
				d.initType = d.saveType;
			} else {
				d.offsetInBuffer = reader.ReadUInt32();
				d.typeNumber = reader.ReadUInt32();
				d.saveType = reader.ReadUInt32();
				d.initType = reader.ReadUInt32();
			}

			d.number = number;

            d.type = Settings.s.aiTypes.GetDsgVarType(d.typeNumber);

            return d;
        }

        public static DsgVarType GetDsgVarTypeFromArrayType(DsgVarType arrayType)
        {
            switch(arrayType) {
                
                case DsgVarType.ActionArray: return DsgVarType.Action;
                case DsgVarType.FloatArray: return DsgVarType.Float;
                case DsgVarType.IntegerArray: return DsgVarType.Int;
                case DsgVarType.PersoArray: return DsgVarType.Perso;
                case DsgVarType.SoundEventArray: return DsgVarType.SoundEvent;
                case DsgVarType.SuperObjectArray: return DsgVarType.SuperObject;
                case DsgVarType.TextArray: return DsgVarType.Text;
                case DsgVarType.TextRefArray: return DsgVarType.None;
                case DsgVarType.VectorArray: return DsgVarType.Vector;
                case DsgVarType.WayPointArray: return DsgVarType.Waypoint;
            }

            return DsgVarType.None;
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
            VisualMaterial, // Also an array?
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
            TextRefArray,
            Array6,
            Array9,
            SoundEventArray,
            Array11,
            Way, // TT SE only
            ActionArray, // Hype
			SuperObjectArray
		}
    }
}
