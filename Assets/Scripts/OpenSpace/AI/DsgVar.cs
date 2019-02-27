using OpenSpace.Object;
using OpenSpace.Waypoints;
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

        public static DsgVar Read(Reader reader, Pointer offset, DsgMem dsgMem=null) {
            MapLoader l = MapLoader.Loader;
			//l.print("DsgVar " + offset);
            DsgVar dsgVar = new DsgVar(offset);
            dsgVar.off_dsgMemBuffer = Pointer.Read(reader);
            dsgVar.off_dsgVarInfo = Pointer.Read(reader);

            /*if (dsgMem != null) {
                l.print(offset + " - " + dsgVar.off_dsgMemBuffer + " - " + dsgVar.off_dsgVarInfo);
                l.print("DsgMem initial: " + dsgMem.memBufferInitial + " - cur: " + dsgMem.memBuffer);
            }*/

            // Unknown stuff
            if (dsgMem==null
               && Settings.s.platform != Settings.Platform.GC
               && Settings.s.platform != Settings.Platform.DC
			   && Settings.s.game != Settings.Game.R2Revolution
			   && Settings.s.engineVersion >= Settings.EngineVersion.R2) {
                dsgVar.something3 = reader.ReadUInt32();
            }

			if (Settings.s.game == Settings.Game.R2Revolution) {
				dsgVar.dsgMemBufferLength = reader.ReadUInt16();
				dsgVar.amountOfInfos = reader.ReadUInt16();
			} else if (Settings.s.platform == Settings.Platform.GC
                || Settings.s.platform == Settings.Platform.DC
                || Settings.s.engineVersion < Settings.EngineVersion.R2) {
                dsgVar.dsgMemBufferLength = reader.ReadUInt32();
                dsgVar.amountOfInfos = reader.ReadByte();
            } else if (dsgMem == null) {
                dsgVar.amountOfInfos = reader.ReadUInt32();
                dsgVar.dsgMemBufferLength = reader.ReadUInt32() * 4;
            } else {
                dsgVar.dsgMemBufferLength = reader.ReadUInt32();
                dsgVar.amountOfInfos = reader.ReadUInt32();
            }

            if (dsgMem != null && dsgMem.memBufferInitial == null) dsgMem.memBufferInitial = dsgVar.off_dsgMemBuffer;

            dsgVar.dsgVarInfos = new DsgVarInfoEntry[dsgVar.amountOfInfos];

            if (dsgVar.amountOfInfos > 0) {
                Pointer.DoAt(ref reader, dsgVar.off_dsgVarInfo, () => {
                    //l.print(dsgVar.amountOfInfos);
                    for (uint i = 0; i < dsgVar.amountOfInfos; i++) {
                        DsgVarInfoEntry infoEntry = DsgVarInfoEntry.Read(reader, Pointer.Current(reader), i);

                        if (dsgMem != null) {
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
                        dsgVar.dsgVarInfos[i] = infoEntry;
                    }
                });
            }

            /*if (d.off_AI_model != null) {
                Pointer.Goto(ref reader, d.off_AI_model);
                d.AI_model = AIModel.Read(reader, d.off_AI_model);
            }*/
            return dsgVar;
        }

        public object ReadValueFromBuffer(Reader reader, DsgVarInfoEntry infoEntry, Pointer buffer)
        {
            return ReadValueFromBuffer(reader, infoEntry.type, infoEntry.offsetInBuffer, infoEntry.number, buffer, infoEntry);
        }

        public object ReadValueFromBuffer(Reader reader, DsgVarInfoEntry.DsgVarType type, uint offsetInBuffer, uint number, Pointer buffer, DsgVarInfoEntry entry = null)
        {
            Pointer original = Pointer.Goto(ref reader, buffer + offsetInBuffer);
            object returnValue = null;

			try {

                switch (type) {
                    case DsgVarInfoEntry.DsgVarType.Boolean:
                        returnValue = reader.ReadBoolean(); break;
                    case DsgVarInfoEntry.DsgVarType.Byte:
                        returnValue = reader.ReadSByte(); break;
                    case DsgVarInfoEntry.DsgVarType.UByte:
                        returnValue = reader.ReadByte(); break;
                    case DsgVarInfoEntry.DsgVarType.Float:
                        returnValue = reader.ReadSingle(); break;
                    case DsgVarInfoEntry.DsgVarType.Int:
                        returnValue = reader.ReadInt32(); break;
                    case DsgVarInfoEntry.DsgVarType.UInt:
                        returnValue = reader.ReadUInt32(); break;
                    case DsgVarInfoEntry.DsgVarType.Short:
                        returnValue = reader.ReadInt16(); break;
                    case DsgVarInfoEntry.DsgVarType.UShort:
                        returnValue = reader.ReadUInt16(); break;
                    case DsgVarInfoEntry.DsgVarType.Vector:
                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        float z = reader.ReadSingle();
                        returnValue = new Vector3(x, y, z);
                        
                        break;
                    case DsgVarInfoEntry.DsgVarType.Text:
                        uint textInd = reader.ReadUInt32();
                        returnValue = MapLoader.Loader.fontStruct.GetTextForHandleAndLanguageID((int)textInd, 0);
                        break;
                    case DsgVarInfoEntry.DsgVarType.Graph:
                        Pointer off_graph = Pointer.Read(reader);
                        Graph graph = Graph.FromOffsetOrRead(off_graph, reader);
                        returnValue = "Graph " + off_graph;

                        break;
                    case DsgVarInfoEntry.DsgVarType.Waypoint:
                        Pointer off_waypoint = Pointer.Read(reader);
                        if (off_waypoint != null) {
                            WayPoint wayPoint = WayPoint.FromOffsetOrRead(off_waypoint, reader);
                            returnValue = wayPoint;
                        }

                        break;

                    case DsgVarInfoEntry.DsgVarType.Perso:
                    case DsgVarInfoEntry.DsgVarType.GameMaterial:
                    case DsgVarInfoEntry.DsgVarType.VisualMaterial:

                        returnValue = Pointer.Read(reader);

                        break;

                    case DsgVarInfoEntry.DsgVarType.SuperObject:

                        returnValue = Pointer.Read(reader);

                        break;
					case DsgVarInfoEntry.DsgVarType.Array11:
					case DsgVarInfoEntry.DsgVarType.Array9:
					case DsgVarInfoEntry.DsgVarType.Array6:
						MapLoader.Loader.print(type);
						returnValue = reader.ReadInt32(); break;

                    case DsgVarInfoEntry.DsgVarType.ActionArray:
                    case DsgVarInfoEntry.DsgVarType.FloatArray:
                    case DsgVarInfoEntry.DsgVarType.IntegerArray:
                    case DsgVarInfoEntry.DsgVarType.PersoArray:
                    case DsgVarInfoEntry.DsgVarType.SoundEventArray:
                    case DsgVarInfoEntry.DsgVarType.SuperObjectArray:
                    case DsgVarInfoEntry.DsgVarType.TextArray:
                    case DsgVarInfoEntry.DsgVarType.TextRefArray:
                    case DsgVarInfoEntry.DsgVarType.VectorArray:
                    case DsgVarInfoEntry.DsgVarType.WayPointArray:

                        returnValue = ReadArray(reader, entry, buffer);

                        break;

                    default:
                        returnValue = reader.ReadInt32(); break;
                }

            } catch (Exception e) {
				returnValue = "Exception: " + e.Message;
				//returnValue = "Exception: " + e.Message + "\nBuffer: " + buffer + " - InfoEntry.Offset: " + infoEntry.offsetInBuffer + " - typeNumber: " + infoEntry.typeNumber + "\n" + e.StackTrace;
			}

            Pointer.Goto(ref reader, original);

            return returnValue;
        }

        public object ReadValueFromDsgMemBuffer(Reader reader, DsgVarInfoEntry infoEntry, DsgMem dsgMem)
        {
            return ReadValueFromBuffer(reader, infoEntry, dsgMem.memBuffer);
        }

        public object ReadInitialValueFromDsgMemBuffer(Reader reader, DsgVarInfoEntry infoEntry, DsgMem dsgMem)
        {
            return ReadValueFromBuffer(reader, infoEntry, dsgMem.memBufferInitial);
        }

        public object ReadValueFromDsgVarBuffer(Reader reader, DsgVarInfoEntry infoEntry, DsgVar dsgVar)
        {
            return ReadValueFromBuffer(reader, infoEntry, dsgVar.off_dsgMemBuffer);
        }

        public object[] ReadArray(Reader reader, DsgVarInfoEntry entry, Pointer buffer)
        {
            uint o = entry.number * 16;

            Pointer.Goto(ref reader, off_dsgVarInfo + o);
            int a = reader.ReadInt32();

            Pointer byteAddress = off_dsgMemBuffer + 4 + a;

            Pointer.Goto(ref reader, byteAddress);
            byte arraySize = reader.ReadByte();

            //this.

            /*Pointer.Goto(ref reader, this.off_dsgMemBuffer + 4);
            Pointer bufferStartAddress = Pointer.Read(reader);

            Pointer.Goto(ref reader, bufferStartAddress + infoEntry.offsetInBuffer);
            uint offset = reader.ReadUInt32();
            Pointer.Goto(ref reader, this.off_dsgMemBuffer + 4 + offset);
            uint arraySize = reader.ReadByte();
            */
            object[] resultList = new object[arraySize];

            DsgVarInfoEntry.DsgVarType itemType = DsgVarInfoEntry.GetDsgVarTypeFromArrayType(entry.type);

            for (uint i = 0; i < arraySize; i++) {
                if (itemType==DsgVarInfoEntry.DsgVarType.Vector)
                    resultList[i] = ReadValueFromBuffer(reader, itemType, 8 + i * 12, i, buffer);
                else if (itemType == DsgVarInfoEntry.DsgVarType.Text)
                    resultList[i] = ReadValueFromBuffer(reader, itemType, 40 + 8 + i * 4, i, buffer);
                else
                    resultList[i] = ReadValueFromBuffer(reader, itemType, 8 + i * 4, i, buffer);
            }

            return resultList;
        }
    }
}
