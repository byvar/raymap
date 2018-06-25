using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenSpace.AI {
    public enum DsgVarType {
        None = -1,
        Boolean,
        Byte,
        UByte, // Unsigned
        Short,
        UShort, // Unsigned
        Int,
        UInt, // Unsigned
        Float,
        Waypoint,
        Perso,
        List,
        Vector,
        Comport,
        Action,
        Text,
        GameMaterial,
        Caps, // Capabilities
        Graph,
        Array1,
        Array2,
        Array3,
        Array4,
        Array5,
        Array6,
        Input
    }

    /*public abstract class DsgVarTypeUtil {

        public static string GetFormattedValue(DsgVarType type, int value, int processHandle)
        {
            switch (type) {
                case DsgVarType.Boolean1: return (value > 0 ? "True" : "False");
                case DsgVarType.Boolean2: return (value > 0 ? "True" : "False");
                case DsgVarType.Byte: return ((int)BitConverter.ToChar(BitConverter.GetBytes(value), 0)).ToString();
                case DsgVarType.Word1: return (BitConverter.ToInt16(BitConverter.GetBytes(value), 0)).ToString();
                case DsgVarType.Word2: return (BitConverter.ToInt16(BitConverter.GetBytes(value), 0)).ToString();
                case DsgVarType.Integer1: return value.ToString();
                case DsgVarType.Integer2: return value.ToString();
                case DsgVarType.Float: return (BitConverter.ToSingle(BitConverter.GetBytes(value), 0)).ToString();
                case DsgVarType.SuperObjectRef:
                    string name = MemoryRead.GetSuperObjectName(processHandle, value);
                    return name + "("+value.ToString("X8")+")";

                default: return value.ToString("X8") + "("+value.ToString()+")";
            }
        }
    }*/
}
