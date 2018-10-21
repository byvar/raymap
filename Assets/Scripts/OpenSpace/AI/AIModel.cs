using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class AIModel {
        public Pointer offset;

        public Pointer off_behaviors_normal;
        public Pointer off_behaviors_reflex;
        public Pointer off_dsgVar;
        public Pointer off_macros;
        public uint flags;

        public Behavior[] behaviors_normal = null;
        public Behavior[] behaviors_reflex = null;
        public DsgVar dsgVar;
        public Macro[] macros = null;

        public Mind mind;
        public string name;

        public AIModel(Pointer offset)
        {
            this.offset = offset;
        }

        public static AIModel Read(Reader reader, Pointer offset)
        {
            MapLoader l = MapLoader.Loader;
            AIModel ai = new AIModel(offset);

            ai.off_behaviors_normal = Pointer.Read(reader);
            ai.off_behaviors_reflex = Pointer.Read(reader);
            ai.off_dsgVar = Pointer.Read(reader);
            if (Settings.s.engineVersion >= Settings.EngineVersion.R2) {
                ai.off_macros = Pointer.Read(reader);
                ai.flags = reader.ReadUInt32();
            }

            if (ai.off_behaviors_normal != null) {
                Pointer.Goto(ref reader, ai.off_behaviors_normal);
                Pointer off_entries = Pointer.Read(reader);
                uint num_entries = reader.ReadUInt32();
                ai.behaviors_normal = new Behavior[num_entries];
                if (num_entries > 0 && off_entries != null) {
                    Pointer.Goto(ref reader, off_entries);
                    for (int i = 0; i < num_entries; i++) {
                        ai.behaviors_normal[i] = Behavior.Read(reader, Pointer.Current(reader), ai, Behavior.BehaviorType.Normal, i);
                    }
                }
            }
            if (ai.off_behaviors_reflex != null) {
                Pointer.Goto(ref reader, ai.off_behaviors_reflex);
                Pointer off_entries = Pointer.Read(reader);
                uint num_entries = reader.ReadUInt32();
                ai.behaviors_reflex = new Behavior[num_entries];
                if (num_entries > 0 && off_entries != null) {
                    Pointer.Goto(ref reader, off_entries);
                    for (int i = 0; i < num_entries; i++) {
                        ai.behaviors_reflex[i] = Behavior.Read(reader, Pointer.Current(reader), ai, Behavior.BehaviorType.Reflex, i);
                    }
                }
            }

            if (ai.off_dsgVar != null) {
                Pointer.Goto(ref reader, ai.off_dsgVar);
                ai.dsgVar = DsgVar.Read(reader, ai.off_dsgVar);
            }

            if (ai.off_macros != null) {
                Pointer.Goto(ref reader, ai.off_macros);
                Pointer off_entries = Pointer.Read(reader);
                byte num_entries = reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                ai.macros = new Macro[num_entries];
                if (num_entries > 0 && off_entries != null) {
                    Pointer.Goto(ref reader, off_entries);
                    for (int i = 0; i < num_entries; i++) {
                        ai.macros[i] = Macro.Read(reader, Pointer.Current(reader), ai, i);
                    }
                }
            }
            //l.aiModels.Add(ai);
            return ai;
        }


        public static AIModel FromOffset(Pointer offset)
        {
            if (offset == null) return null;
            MapLoader l = MapLoader.Loader;
            return l.aiModels.FirstOrDefault(f => f.offset == offset);
        }

        public static AIModel FromOffsetOrRead(Pointer offset, Reader reader) {
            if (offset == null) return null;
            AIModel ai = FromOffset(offset);
            if (ai == null) {
                Pointer.DoAt(ref reader, offset, () => {
                    ai = AIModel.Read(reader, offset);
                    MapLoader.Loader.aiModels.Add(ai);
                });
            }
            return ai;
        }

        public Behavior GetBehaviorByOffset(Pointer offset)
        {
            if (offset == null) {
                return null;
            }
            // Look in both behavior lists
            if (behaviors_normal != null) {
                foreach (Behavior behavior in behaviors_normal) {

                    if (behavior.offset == offset) {
                        return behavior;
                    }
                }
            }
            if (behaviors_reflex != null) {
                foreach (Behavior behavior in behaviors_reflex) {

                    if (behavior.offset == offset) {
                        return behavior;
                    }

                }
            }
            return null;
        }

        public Macro GetMacroByOffset(Pointer offset)
        {
            if (offset == null) {
                return null;
            }
            // Look in both behavior lists
            if (macros != null) {
                foreach (Macro macro in macros) {

                    if (macro.offset == offset) {
                        return macro;
                    }
                }
            }
            
            return null;
        }
    }
}