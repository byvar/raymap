using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenSpace.Input {
    public class InputStructure {
        public Pointer offset;
        public uint num_entryActions;
        public Pointer off_entryActions;
        public EntryAction[] entryActions;

        public InputStructure(Pointer offset) {
            this.offset = offset;
        }

        public static InputStructure Read(Reader reader, Pointer offset) {
            InputStructure input = new InputStructure(offset);
            if (Settings.s.platform == Settings.Platform.GC) {
                //reader.ReadBytes(0x1714);
                if (MapLoader.Loader.mode == MapLoader.Mode.Rayman3GC) {
                    reader.ReadBytes(0x12E0);
                } else if (MapLoader.Loader.mode == MapLoader.Mode.RaymanArenaGC) {
                    //reader.ReadBytes(0x16e8);
                    reader.ReadBytes(0x12C8);
                }
                input.num_entryActions = reader.ReadUInt32();
                input.off_entryActions = Pointer.Read(reader);
                reader.ReadBytes(0x418);
            } else if (Settings.s.platform == Settings.Platform.PC) {
                if (Settings.s.engineMode == Settings.EngineMode.R2) {
                    reader.ReadBytes(0x700);
                } else {
                    reader.ReadBytes(0x16BC);
                }
                input.num_entryActions = reader.ReadUInt32();
                input.off_entryActions = Pointer.Read(reader);
                reader.ReadBytes(0x418);
            } else if (Settings.s.platform == Settings.Platform.iOS) {
                reader.ReadBytes(0x2A0);
                input.num_entryActions = reader.ReadUInt32();
                input.off_entryActions = Pointer.Read(reader);
                reader.ReadBytes(0x14);
            }

            if (input.off_entryActions != null && input.num_entryActions > 0) {
                input.entryActions = new EntryAction[input.num_entryActions];
                Pointer off_current = Pointer.Goto(ref reader, input.off_entryActions);
                for (int i = 0; i < input.num_entryActions; i++) {
                    input.entryActions[i] = EntryAction.Read(reader, Pointer.Current(reader));
                }
                Pointer.Goto(ref reader, off_current);
            }

            return input;
        }
    }
}
