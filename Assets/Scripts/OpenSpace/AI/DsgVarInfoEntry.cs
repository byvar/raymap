using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public partial class DsgVarInfoEntry {
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
			if (CPA_Settings.s.game == CPA_Settings.Game.LargoWinch) {
				d.offsetInBuffer = reader.ReadUInt32();
				d.typeNumber = reader.ReadUInt32();
				d.saveType = reader.ReadByte();
				d.initType = reader.ReadByte();
				reader.ReadByte();
				reader.ReadByte();
			} else if (CPA_Settings.s.game == CPA_Settings.Game.R2Revolution) {
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

            d.type = CPA_Settings.s.aiTypes.GetDsgVarType(d.typeNumber);

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
                case DsgVarType.None:
                    break;
                case DsgVarType.Boolean:
                    typeText = "DsgVarBool"; break;
                case DsgVarType.Byte:
                    typeText = "DsgVarByte"; break;
                case DsgVarType.UByte:
                    typeText = "DsgVarUByte"; break;
                case DsgVarType.Short:
                    typeText = "DsgVarShort"; break;
                case DsgVarType.UShort:
                    typeText = "DsgVarUShort"; break;
                case DsgVarType.Int:
                    typeText = "DsgVarInt"; break;
                case DsgVarType.UInt:
                    typeText = "DsgVarUInt"; break;
                case DsgVarType.Float:
                    typeText = "DsgVarFloat"; break;
                case DsgVarType.Vector:
                    typeText = "Vector3"; break;
                case DsgVarType.List:
                    typeText = "DsgVarList"; break;
                case DsgVarType.Comport:
                    typeText = "Comport"; break;
                case DsgVarType.Action:
                    typeText = "Action"; break;
                case DsgVarType.Caps:
                    typeText = "Caps"; break;
                case DsgVarType.Input:
                    typeText = "Input"; break;
                case DsgVarType.SoundEvent:
                    typeText = "SoundEvent"; break;
                case DsgVarType.Light:
                    typeText = "Light"; break;
                case DsgVarType.GameMaterial:
                    typeText = "GameMaterial"; break;
                case DsgVarType.VisualMaterial:
                    typeText = "VisualMaterial"; break;
                case DsgVarType.Perso:
                    typeText = "Perso"; break;
                case DsgVarType.WayPoint:
                    typeText = "WayPoint"; break;
                case DsgVarType.Graph:
                    typeText = "Graph"; break;
                case DsgVarType.Text:
                    typeText = "DsgVarString"; break;
                case DsgVarType.SuperObject:
                    typeText = "SuperObject"; break;
                case DsgVarType.SOLinks:
                    typeText = "SOLinks"; break;
                case DsgVarType.PersoArray:
                    typeText = "List<Perso>"; break;
                case DsgVarType.VectorArray:
                    typeText = "List<Vector3>"; break;
                case DsgVarType.FloatArray:
                    typeText = "List<DsgVarFloat>"; break;
                case DsgVarType.IntegerArray:
                    typeText = "List<DsgVarInt>"; break;
                case DsgVarType.WayPointArray:
                    typeText = "List<WayPoint>"; break;
                case DsgVarType.TextArray: // These are text references
                    typeText = "List<DsgVarTextRef>"; break;
                case DsgVarType.TextRefArray: // Don't know what these are then?
                    typeText = "List<DsgVarTextRef2>"; break;
                case DsgVarType.GraphArray:
                    typeText = "List<Graph>"; break;
                case DsgVarType.SOLinksArray:
                    typeText = "List<SOLinks>"; break;
                case DsgVarType.SoundEventArray:
                    typeText = "List<DsgVarSoundEvent>"; break;
                case DsgVarType.VisualMatArray:
                    typeText = "List<VisualMaterial>"; break;
                case DsgVarType.Way:
                    typeText = "Way"; break;
                case DsgVarType.ActionArray:
                    typeText = "List<Action>"; break;
                case DsgVarType.SuperObjectArray:
                    typeText = "List<SuperObject>"; break;
            }
            return typeText;
        }
    }
}
