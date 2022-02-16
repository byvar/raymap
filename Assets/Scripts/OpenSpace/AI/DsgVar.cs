using OpenSpace.Object;
using OpenSpace.Waypoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class DsgVar : OpenSpaceStruct {
        public LegacyPointer off_dsgMemBuffer;
        public LegacyPointer off_dsgVarInfo; // points to DsgVarInfo, which is an array of dsgVarInfoEntries
        public uint amountOfInfos;
        public uint dsgMemBufferLength;

        public DsgVarInfoEntry[] dsgVarInfos;
        public DsgVarValue[] defaultValues;

        protected override void ReadInternal(Reader reader) {
            off_dsgMemBuffer = LegacyPointer.Read(reader);
            off_dsgVarInfo = LegacyPointer.Read(reader);
            if (Legacy_Settings.s.game == Legacy_Settings.Game.R2Revolution) {
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
                LegacyPointer.DoAt(ref reader, off_dsgVarInfo, () => {
                    //l.print(dsgVar.amountOfInfos);
                    for (uint i = 0; i < amountOfInfos; i++) {
                        dsgVarInfos[i] = DsgVarInfoEntry.Read(reader, LegacyPointer.Current(reader), i);
                        defaultValues[i] = new DsgVarValue(dsgVarInfos[i].type, null);
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
