using OpenSpace;
using OpenSpace.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.AI {

    public class DsgVarUtil {

        public static string DsgVarEntryToCSharpAssignment(DsgVarInfoEntry dsgVarEntry)
        {
            DsgVarComponent.DsgVarEditableEntry editableEntry = new DsgVarComponent.DsgVarEditableEntry((int)dsgVarEntry.number, dsgVarEntry);

            string text = "";
            string typeText = "";

            switch (dsgVarEntry.type) {
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
                case DsgVarInfoEntry.DsgVarType.Array6:
                    typeText = "List<Unknown_6>"; break;
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

            text += typeText + " " + "dsgVar_" + dsgVarEntry.number;

            if (dsgVarEntry.initialValue != null) {
                text += " = ";

                string stringVal = editableEntry.valueInitial.ToString();

                text += stringVal;
            }

            text += ";";

            return text;
        }
    }
}
