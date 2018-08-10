using OpenSpace.EngineObject;
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
            DsgVar dsgVar = new DsgVar(offset);
            dsgVar.off_dsgMemBuffer = Pointer.Read(reader);
            dsgVar.off_dsgVarInfo = Pointer.Read(reader);

            // Unknown stuff
            if (dsgMem==null && l.mode != MapLoader.Mode.RaymanArenaGC && l.mode != MapLoader.Mode.Rayman3GC) {
                dsgVar.something3 = reader.ReadUInt32();
            }

            if (l.mode == MapLoader.Mode.RaymanArenaGC || l.mode == MapLoader.Mode.Rayman3GC) {
                dsgVar.dsgMemBufferLength = reader.ReadUInt32();
                dsgVar.amountOfInfos = reader.ReadByte();
            } else if (dsgMem == null) {
                dsgVar.amountOfInfos = reader.ReadUInt32();
                dsgVar.dsgMemBufferLength = reader.ReadUInt32() * 4;
            } else {
                dsgVar.dsgMemBufferLength = reader.ReadUInt32();
                dsgVar.amountOfInfos = reader.ReadUInt32();
            }

            dsgVar.dsgVarInfos = new DsgVarInfoEntry[dsgVar.amountOfInfos];

            if (dsgVar.off_dsgVarInfo != null && dsgVar.amountOfInfos > 0) {

                Pointer off_current = Pointer.Goto(ref reader, dsgVar.off_dsgVarInfo);
                for (uint i = 0; i < dsgVar.amountOfInfos; i++) {
                    DsgVarInfoEntry infoEntry = DsgVarInfoEntry.Read(reader, Pointer.Current(reader), i);

                    if (dsgMem != null) {
                        infoEntry.value = dsgVar.ReadValueFromDsgMemBuffer(reader, infoEntry, dsgMem);
                        if (dsgMem.memBufferInitial != null) {
                            infoEntry.initialValue = dsgVar.ReadInitialValueFromDsgMemBuffer(reader, infoEntry, dsgMem);
                        }
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

        public object ReadValueFromBuffer(Reader reader, DsgVarInfoEntry infoEntry, Pointer buffer)
        {

            Pointer original = Pointer.Goto(ref reader, buffer + infoEntry.offsetInBuffer);
            object returnValue = null;

            try {

                switch (infoEntry.type) {
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
                    case DsgVarInfoEntry.DsgVarType.Graph:
                            Pointer off_graph = Pointer.Read(reader);
                            if (off_graph != null) {
                                Pointer originalBeforeGraph = Pointer.Goto(ref reader, off_graph);
                                Graph graph = Graph.Read(reader, off_graph);
                                Pointer.Goto(ref reader, originalBeforeGraph);

                                MapLoader.Loader.AddGraph(graph);

                            returnValue = "Graph " + off_graph;
                        }

                        break;
                    case DsgVarInfoEntry.DsgVarType.Waypoint:
                        Pointer off_waypoint = Pointer.Read(reader);
                        if (off_waypoint != null) {
                            Pointer originalBeforeWaypoint = Pointer.Goto(ref reader, off_waypoint);
                            WayPoint wayPoint = WayPoint.Read(reader, off_waypoint);
                            Pointer.Goto(ref reader, originalBeforeWaypoint);

                            MapLoader.Loader.AddIsolateWaypoint(wayPoint);

                            returnValue = "WayPoint " + off_waypoint;
                        }

                        break;

                    case DsgVarInfoEntry.DsgVarType.Perso:

                        returnValue = Pointer.Read(reader);

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
    }
}
