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

        public DsgVarType type;
        public object value;

        public DsgVarInfoEntry(Pointer offset) {
            this.offset = offset;
        }

        public static DsgVarInfoEntry Read(EndianBinaryReader reader, Pointer offset) {
            DsgVarInfoEntry d = new DsgVarInfoEntry(offset);
            d.offsetInBuffer = reader.ReadUInt32();
            d.typeNumber = reader.ReadUInt32();
            d.type = (DsgVarType)d.typeNumber;
            d.saveType = reader.ReadUInt32();
            d.initType = reader.ReadUInt32();
            
            return d;
        }
    }
}
