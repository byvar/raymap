using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class DsgVar {
        public Pointer offset;

        public Pointer off_dsgMemBuffer;
        public Pointer off_dsgVarInfo; // points to DsgVarInfo, which is an array of dsgVarInfoEntries

        public uint something3; // No idea what this is

        public uint amountOfInfos;
        public uint dsgMemBufferLength;

        public DsgVarInfoEntry[] dsgVarInfos;

        public DsgVar(Pointer offset) {
            this.offset = offset;
        }

        public static DsgVar Read(EndianBinaryReader reader, Pointer offset) {
            DsgVar d = new DsgVar(offset);

            d.off_dsgMemBuffer = Pointer.Read(reader);
            d.off_dsgVarInfo = Pointer.Read(reader);

            // Unknown stuff
            d.something3 = reader.ReadUInt32();

            d.amountOfInfos = reader.ReadUInt32();
            d.dsgMemBufferLength = reader.ReadUInt32() * 4;

            d.dsgVarInfos = new DsgVarInfoEntry[d.amountOfInfos];

            if (d.off_dsgVarInfo != null && d.amountOfInfos > 0) {

                Pointer off_current = Pointer.Goto(ref reader, d.off_dsgVarInfo);
                for (int i = 0; i < d.amountOfInfos; i++) {
                    DsgVarInfoEntry infoEntry = DsgVarInfoEntry.Read(reader, Pointer.Current(reader));

                    infoEntry.value = d.ReadValueFromDsgMemBuffer(reader, infoEntry);
                    d.dsgVarInfos[i] = infoEntry;
                }
                Pointer.Goto(ref reader, off_current); // Move the reader back to where it was
            }

            /*if (d.off_AI_model != null) {
                Pointer.Goto(ref reader, d.off_AI_model);
                d.AI_model = AIModel.Read(reader, d.off_AI_model);
            }*/
            return d;
        }

        public object ReadValueFromDsgMemBuffer(EndianBinaryReader reader, DsgVarInfoEntry infoEntry)
        {
            
            Pointer original = Pointer.Goto(ref reader, off_dsgMemBuffer + infoEntry.offsetInBuffer);
            object returnValue = null;

            try {

                switch (infoEntry.type) {
                    case DsgVarType.Boolean1:
                    case DsgVarType.Boolean2:
                        returnValue = reader.ReadBoolean(); break;
                    case DsgVarType.Byte:
                        returnValue = reader.ReadByte(); break;
                    case DsgVarType.Float:
                        returnValue = reader.ReadSingle(); break;
                    case DsgVarType.Integer1:
                    case DsgVarType.Integer2:
                        returnValue = reader.ReadInt32(); break;
                    case DsgVarType.Word1:
                    case DsgVarType.Word2:
                        returnValue = reader.ReadInt16(); break;
                    default:
                        returnValue = reader.ReadInt32(); break;
                }

            } catch (Exception e) {
                returnValue = "Exception";
            }

            Pointer.Goto(ref reader, original);

            return returnValue;
        }
    }
}
