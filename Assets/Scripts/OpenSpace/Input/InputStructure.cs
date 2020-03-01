using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenSpace.Input {
    public class InputStructure {
        public Pointer offset;
        public uint num_entryActions;
        public Pointer off_entryActions;
        public List<EntryAction> entryActions = new List<EntryAction>();

        public InputStructure(Pointer offset) {
            this.offset = offset;
        }

        public static InputStructure Read(Reader reader, Pointer offset) {
            InputStructure input = new InputStructure(offset);
			if (Settings.s.game == Settings.Game.LargoWinch) {
				input.num_entryActions = reader.ReadUInt32();
				input.off_entryActions = Pointer.Read(reader);
			} else {
				switch (Settings.s.platform) {
					case Settings.Platform.GC:
						//reader.ReadBytes(0x1714);
						if (Settings.s.game == Settings.Game.R3) {
							reader.ReadBytes(0x12E0);
						} else if (Settings.s.game == Settings.Game.RA
							|| Settings.s.game == Settings.Game.DDPK) {
							//reader.ReadBytes(0x16e8);
							reader.ReadBytes(0x12C8);
						}
						input.num_entryActions = reader.ReadUInt32();
						input.off_entryActions = Pointer.Read(reader);
						reader.ReadBytes(0x418);
						break;
					case Settings.Platform.PC:
						if (Settings.s.engineVersion == Settings.EngineVersion.R2) {
							reader.ReadBytes(0x700);
						} else if (Settings.s.game == Settings.Game.Dinosaur) {
							reader.ReadBytes(0xC58);
						} else {
							reader.ReadBytes(0x16BC);
						}
						input.num_entryActions = reader.ReadUInt32();
						input.off_entryActions = Pointer.Read(reader);
						reader.ReadBytes(0x418);
						break;
					case Settings.Platform.Xbox:
					case Settings.Platform.Xbox360:
					case Settings.Platform.PS3:
						reader.ReadBytes(0x16BC);
						input.num_entryActions = reader.ReadUInt32();
						input.off_entryActions = Pointer.Read(reader);
						reader.ReadBytes(0x418);
						break;
					case Settings.Platform.iOS:
						reader.ReadBytes(0x2A0);
						input.num_entryActions = reader.ReadUInt32();
						input.off_entryActions = Pointer.Read(reader);
						reader.ReadBytes(0x14);
						break;
					case Settings.Platform.DC:
						reader.ReadBytes(0x278);
						input.num_entryActions = reader.ReadUInt32();
						input.off_entryActions = Pointer.Read(reader);
						reader.ReadUInt32();
						Pointer.Read(reader);
						break;
					case Settings.Platform.PS2:
						if (Settings.s.game == Settings.Game.R2Revolution) {
							reader.ReadBytes(0x130);
							input.num_entryActions = reader.ReadUInt32();
							input.off_entryActions = Pointer.Read(reader);
							Pointer.Read(reader);
							reader.ReadUInt16();
							reader.ReadUInt16();
							reader.ReadUInt32(); // 0F00020000040100
							reader.ReadBytes(0x300);
						} else {
							reader.ReadBytes(0x1250);
							if (Settings.s.mode == Settings.Mode.Rayman3PS2Demo_2002_12_18) {
								reader.ReadBytes(0x10);
							}
							input.num_entryActions = reader.ReadUInt32();
							input.off_entryActions = Pointer.Read(reader);
							reader.ReadBytes(0x418);
						}
						break;
				}
			}

            if (input.off_entryActions != null && input.num_entryActions > 0) {
				//input.entryActions = new EntryAction[input.num_entryActions];
				Pointer.DoAt(ref reader, input.off_entryActions, () => {
					for (int i = 0; i < input.num_entryActions; i++) {
						input.entryActions.Add(EntryAction.Read(reader, Pointer.Current(reader)));
					}
				});
            }

            return input;
        }
    }
}
