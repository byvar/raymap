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
			if (Settings.s.game == Settings.Game.LargoWinch) {
				d.offsetInBuffer = reader.ReadUInt32();
				d.typeNumber = reader.ReadUInt32();
				d.saveType = reader.ReadByte();
				d.initType = reader.ReadByte();
				reader.ReadByte();
				reader.ReadByte();
			} else if (Settings.s.game == Settings.Game.R2Revolution) {
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
                case DsgVarType.WayPointArray: return DsgVarType.WayPoint;
                case DsgVarType.GraphArray: return DsgVarType.Graph;
            }

            return DsgVarType.None;
        }

        public static string GetCSharpStringFromType(DsgVarType type) {
            string typeText = "";

            switch (type) {
                case DsgVarInfoEntry.DsgVarType.None:
                    break;
                case DsgVarInfoEntry.DsgVarType.Boolean:
                    typeText = "DsgVarBool"; break;
                case DsgVarInfoEntry.DsgVarType.Byte:
                    typeText = "DsgVarByte"; break;
                case DsgVarInfoEntry.DsgVarType.UByte:
                    typeText = "DsgVarUByte"; break;
                case DsgVarInfoEntry.DsgVarType.Short:
                    typeText = "DsgVarShort"; break;
                case DsgVarInfoEntry.DsgVarType.UShort:
                    typeText = "DsgVarUShort"; break;
                case DsgVarInfoEntry.DsgVarType.Int:
                    typeText = "DsgVarInt"; break;
                case DsgVarInfoEntry.DsgVarType.UInt:
                    typeText = "DsgVarUInt"; break;
                case DsgVarInfoEntry.DsgVarType.Float:
                    typeText = "DsgVarFloat"; break;
                case DsgVarInfoEntry.DsgVarType.Vector:
                    typeText = "Vector3"; break;
                case DsgVarInfoEntry.DsgVarType.List:
                    typeText = "DsgVarList"; break;
                case DsgVarInfoEntry.DsgVarType.Comport:
                    typeText = "Comport"; break;
                case DsgVarInfoEntry.DsgVarType.Action:
                    typeText = "Action"; break;
                case DsgVarInfoEntry.DsgVarType.Caps:
                    typeText = "Caps"; break;
                case DsgVarInfoEntry.DsgVarType.Input:
                    typeText = "Input"; break;
                case DsgVarInfoEntry.DsgVarType.SoundEvent:
                    typeText = "SoundEvent"; break;
                case DsgVarInfoEntry.DsgVarType.Light:
                    typeText = "Light"; break;
                case DsgVarInfoEntry.DsgVarType.GameMaterial:
                    typeText = "GameMaterial"; break;
                case DsgVarInfoEntry.DsgVarType.VisualMaterial:
                    typeText = "VisualMaterial"; break;
                case DsgVarInfoEntry.DsgVarType.Perso:
                    typeText = "Perso"; break;
                case DsgVarInfoEntry.DsgVarType.WayPoint:
                    typeText = "WayPoint"; break;
                case DsgVarInfoEntry.DsgVarType.Graph:
                    typeText = "Graph"; break;
                case DsgVarInfoEntry.DsgVarType.Text:
                    typeText = "DsgVarString"; break;
                case DsgVarInfoEntry.DsgVarType.SuperObject:
                    typeText = "SuperObject"; break;
                case DsgVarInfoEntry.DsgVarType.SOLinks:
                    typeText = "SOLinks"; break;
                case DsgVarInfoEntry.DsgVarType.PersoArray:
                    typeText = "List<Perso>"; break;
                case DsgVarInfoEntry.DsgVarType.VectorArray:
                    typeText = "List<Vector3>"; break;
                case DsgVarInfoEntry.DsgVarType.FloatArray:
                    typeText = "List<DsgVarFloat>"; break;
                case DsgVarInfoEntry.DsgVarType.IntegerArray:
                    typeText = "List<DsgVarInt>"; break;
                case DsgVarInfoEntry.DsgVarType.WayPointArray:
                    typeText = "List<WayPoint>"; break;
                case DsgVarInfoEntry.DsgVarType.TextArray: // These are text references
                    typeText = "List<DsgVarTextRef>"; break;
                case DsgVarInfoEntry.DsgVarType.TextRefArray: // Don't know what these are then?
                    typeText = "List<DsgVarTextRef2>"; break;
                case DsgVarInfoEntry.DsgVarType.GraphArray:
                    typeText = "List<Graph>"; break;
                case DsgVarInfoEntry.DsgVarType.Array9:
                    typeText = "List<Unknown_7>"; break;
                case DsgVarInfoEntry.DsgVarType.SoundEventArray:
                    typeText = "List<DsgVarSoundEvent>"; break;
                case DsgVarInfoEntry.DsgVarType.Array11:
                    typeText = "List<Unknown_11>"; break;
                case DsgVarInfoEntry.DsgVarType.Way:
                    typeText = "Way"; break;
                case DsgVarInfoEntry.DsgVarType.ActionArray:
                    typeText = "List<Action>"; break;
                case DsgVarInfoEntry.DsgVarType.SuperObjectArray:
                    typeText = "List<SuperObject>"; break;
            }
            return typeText;
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
            WayPoint,
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
            GraphArray,
            Array9,
            SoundEventArray,
            Array11,
            Way, // TT SE only
            ActionArray, // Hype
			SuperObjectArray,
			ObjectList, // Largo
		}
    }
}
