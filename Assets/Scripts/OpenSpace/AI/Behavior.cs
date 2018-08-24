using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class Behavior : BehaviorOrMacro {

        public enum BehaviorType
        {
            Normal, Reflex
        }

        public Pointer offset;

        public string name = null;
        public Pointer off_scripts;
        public uint unknown;
        public byte num_scripts;
        public Script[] scripts;

        public BehaviorType type;
        public int number;

        public Behavior(Pointer offset) {
            this.offset = offset;
        }

        public static Behavior FromOffset(Pointer offset)
        {
            if (offset == null) return null;
            MapLoader l = MapLoader.Loader;
            return l.behaviors.FirstOrDefault(f => f.offset == offset);
        }

        public static Behavior Read(Reader reader, Pointer offset, AIModel aiModel, BehaviorType type, int number) {
            MapLoader l = MapLoader.Loader;
            Behavior behavior = new Behavior(offset);

            behavior.aiModel = aiModel;
            behavior.type = type;
            behavior.number = number;

            if (Settings.s.hasNames)
            {
                behavior.name = new string(reader.ReadChars(0x100)).TrimEnd('\0');
            } else
            {
                behavior.name = behavior.type.ToString() + "Behaviour #" + number + " @"+offset;
            }
            behavior.off_scripts = Pointer.Read(reader);
            behavior.unknown = reader.ReadUInt32();
            if (Settings.s.game == Settings.Game.R2Demo) {
                reader.ReadUInt32();
            }
            behavior.num_scripts = reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            //if (entry.name != null) l.print(entry.name);

            behavior.scripts = new Script[behavior.num_scripts];
            if (behavior.off_scripts != null && behavior.num_scripts > 0) {
                Pointer off_current = Pointer.Goto(ref reader, behavior.off_scripts);
                for (int i = 0; i < behavior.num_scripts; i++) {
                    behavior.scripts[i] = Script.Read(reader, Pointer.Current(reader), behavior);
                }
                Pointer.Goto(ref reader, off_current);
            }

            l.behaviors.Add(behavior);

            return behavior;
        }

        public override string ToString()
        {
            return "(" + this.aiModel.name + ") " + this.name;
        }
    }
}
