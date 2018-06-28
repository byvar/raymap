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

        public static DsgVar Read(EndianBinaryReader reader, Pointer offset, DsgMem dsgMem=null) {
            DsgVar dsgVar = new DsgVar(offset);

            dsgVar.off_dsgMemBuffer = Pointer.Read(reader);
            dsgVar.off_dsgVarInfo = Pointer.Read(reader);

            // Unknown stuff
            if (dsgMem==null) {
                dsgVar.something3 = reader.ReadUInt32();
            }

            if (dsgMem == null) {
                dsgVar.amountOfInfos = reader.ReadUInt32();
                dsgVar.dsgMemBufferLength = reader.ReadUInt32() * 4;
            } else {
                dsgVar.dsgMemBufferLength = reader.ReadUInt32();
                dsgVar.amountOfInfos = reader.ReadUInt32();
            }

            dsgVar.dsgVarInfos = new DsgVarInfoEntry[dsgVar.amountOfInfos];

            if (dsgVar.off_dsgVarInfo != null && dsgVar.amountOfInfos > 0) {

                Pointer off_current = Pointer.Goto(ref reader, dsgVar.off_dsgVarInfo);
                for (int i = 0; i < dsgVar.amountOfInfos; i++) {
                    DsgVarInfoEntry infoEntry = DsgVarInfoEntry.Read(reader, Pointer.Current(reader));

                    if (dsgMem != null) {
                        infoEntry.value = dsgVar.ReadValueFromDsgMemBuffer(reader, infoEntry, dsgMem);
                    } else {
                        infoEntry.value = dsgVar.ReadValueFromDsgVarBuffer(reader, infoEntry, dsgVar);
                    }
                    dsgVar.dsgVarInfos[i] = infoEntry;
                }
                Pointer.Goto(ref reader, off_current); // Move the reader back to where it was
            }

            /*if (d.off_AI_model != null) {
                Pointer.Goto(ref reader, d.off_AI_model);
                d.AI_model = AIModel.Read(reader, d.off_AI_model);
            }*/
            return dsgVar;
        }

        public object ReadValueFromBuffer(EndianBinaryReader reader, DsgVarInfoEntry infoEntry, Pointer buffer)
        {

            Pointer original = Pointer.Goto(ref reader, buffer + infoEntry.offsetInBuffer);
            object returnValue = null;

            try {

                switch (infoEntry.type) {
                    case DsgVarType.Boolean:
                        returnValue = reader.ReadBoolean(); break;
                    case DsgVarType.Byte:
                        returnValue = reader.ReadSByte(); break;
                    case DsgVarType.UByte:
                        returnValue = reader.ReadByte(); break;
                    case DsgVarType.Float:
                        returnValue = reader.ReadSingle(); break;
                    case DsgVarType.Int:
                        returnValue = reader.ReadInt32(); break;
                    case DsgVarType.UInt:
                        returnValue = reader.ReadUInt32(); break;
                    case DsgVarType.Short:
                        returnValue = reader.ReadInt16(); break;
                    case DsgVarType.UShort:
                        returnValue = reader.ReadUInt16(); break;
                    case DsgVarType.Vector:
                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        float z = reader.ReadSingle();
                        returnValue = new Vector3(x, y, z);
                        /*Pointer off_vec = Pointer.Read(reader);
                        if (off_vec != null) {
                            Pointer.Goto(ref reader, off_vec);
                            float x = reader.ReadSingle();
                            float y = reader.ReadSingle();
                            float z = reader.ReadSingle();
                            returnValue = new Vector3(x, y, z);
                        } else {
                            returnValue = "null";
                        }*/
                        break;
                    default:
                        returnValue = reader.ReadInt32(); break;
                }

            } catch (Exception e) {
                returnValue = "Exception: " + e.Message;
            }

            Pointer.Goto(ref reader, original);

            return returnValue;
        }

        public object ReadValueFromDsgMemBuffer(EndianBinaryReader reader, DsgVarInfoEntry infoEntry, DsgMem dsgMem)
        {
            return ReadValueFromBuffer(reader, infoEntry, dsgMem.memBuffer);
        }

        public object ReadValueFromDsgVarBuffer(EndianBinaryReader reader, DsgVarInfoEntry infoEntry, DsgVar dsgVar)
        {
            return ReadValueFromBuffer(reader, infoEntry, dsgVar.off_dsgMemBuffer);
        }
    }
}
