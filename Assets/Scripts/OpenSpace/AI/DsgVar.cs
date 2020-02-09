using OpenSpace.Object;
using OpenSpace.Waypoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class DsgVar : OpenSpaceStruct {
        public Pointer off_dsgMemBuffer;
        public Pointer off_dsgVarInfo; // points to DsgVarInfo, which is an array of dsgVarInfoEntries
        public uint amountOfInfos;
        public uint dsgMemBufferLength;

        public DsgVarInfoEntry[] dsgVarInfos;
        public DsgVarValue[] defaultValues;

        protected override void ReadInternal(Reader reader) {
            off_dsgMemBuffer = Pointer.Read(reader);
            off_dsgVarInfo = Pointer.Read(reader);
            if (Settings.s.game == Settings.Game.R2Revolution) {
                dsgMemBufferLength = reader.ReadUInt16();
                amountOfInfos = reader.ReadUInt16();
            } else {
                dsgMemBufferLength = reader.ReadUInt32();
                amountOfInfos = reader.ReadByte();
                reader.ReadBytes(3);
            }
            dsgVarInfos = new DsgVarInfoEntry[amountOfInfos];
            defaultValues = new DsgVarValue[amountOfInfos];
            if (amountOfInfos > 0) {
                Pointer.DoAt(ref reader, off_dsgVarInfo, () => {
                    //l.print(dsgVar.amountOfInfos);
                    for (uint i = 0; i < amountOfInfos; i++) {
                        dsgVarInfos[i] = DsgVarInfoEntry.Read(reader, Pointer.Current(reader), i);
                        defaultValues[i] = new DsgVarValue(dsgVarInfos[i].type);
                        defaultValues[i].ReadFromDsgVarBuffer(reader, dsgVarInfos[i], this);

                        //l.print(infoEntry.offset + " - " + infoEntry.typeNumber + " - " + infoEntry.type + " - " + infoEntry.offsetInBuffer);

                        /*if (dsgMem != null) {
                            if (Settings.s.platform != Settings.Platform.DC) {
                                infoEntry.value = dsgVar.ReadValueFromDsgMemBuffer(reader, infoEntry, dsgMem);
                            }
                            if (dsgMem.memBufferInitial != null) {
                                infoEntry.initialValue = dsgVar.ReadInitialValueFromDsgMemBuffer(reader, infoEntry, dsgMem);
                                if (Settings.s.platform == Settings.Platform.DC) {
                                    infoEntry.value = infoEntry.initialValue;
                                }
                            }
                        } else {
                            infoEntry.value = dsgVar.ReadValueFromDsgVarBuffer(reader, infoEntry, dsgVar);
                        }
                        dsgVar.dsgVarInfos[i] = infoEntry;*/
                    }
                });
            }
        }
    }
}
