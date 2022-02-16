using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenSpace.Input {
    public class InputStructure {
        public LegacyPointer offset;
        public uint num_entryActions;
        public LegacyPointer off_entryActions;
        public List<EntryAction> entryActions = new List<EntryAction>();

        public InputStructure(LegacyPointer offset) {
            this.offset = offset;
        }

        public static InputStructure Read(Reader reader, LegacyPointer offset) {
            InputStructure input = new InputStructure(offset);
			if (Legacy_Settings.s.game == Legacy_Settings.Game.LargoWinch) {
				input.num_entryActions = reader.ReadUInt32();
				input.off_entryActions = LegacyPointer.Read(reader);
			} else {
				switch (Legacy_Settings.s.platform) {
					case Legacy_Settings.Platform.GC:
						//reader.ReadBytes(0x1714);
						if (Legacy_Settings.s.game == Legacy_Settings.Game.R3) {
							reader.ReadBytes(0x12E0);
						} else if (Legacy_Settings.s.game == Legacy_Settings.Game.RA
							|| Legacy_Settings.s.game == Legacy_Settings.Game.DDPK) {
							//reader.ReadBytes(0x16e8);
							reader.ReadBytes(0x12C8);
						}
						input.num_entryActions = reader.ReadUInt32();
						input.off_entryActions = LegacyPointer.Read(reader);
						reader.ReadBytes(0x418);
						break;
					case Legacy_Settings.Platform.PC:
					case Legacy_Settings.Platform.MacOS:
						if (Legacy_Settings.s.game == Legacy_Settings.Game.RedPlanet) {
							reader.ReadBytes(0x32CC);
						} else if (Legacy_Settings.s.engineVersion == Legacy_Settings.EngineVersion.R2) {
							reader.ReadBytes(0x700);
						} else if (Legacy_Settings.s.game == Legacy_Settings.Game.Dinosaur) {
							reader.ReadBytes(0xC58);
						} else {
							reader.ReadBytes(0x16BC);
						}
						input.num_entryActions = reader.ReadUInt32();
						input.off_entryActions = LegacyPointer.Read(reader);
						if (Legacy_Settings.s.game == Legacy_Settings.Game.RedPlanet) {
							reader.ReadBytes(0x14);
						} else {
							reader.ReadBytes(0x418);
						}
						break;
					case Legacy_Settings.Platform.Xbox:
					case Legacy_Settings.Platform.Xbox360:
					case Legacy_Settings.Platform.PS3:
						reader.ReadBytes(0x16BC);
						input.num_entryActions = reader.ReadUInt32();
						input.off_entryActions = LegacyPointer.Read(reader);
						reader.ReadBytes(0x418);
						break;
					case Legacy_Settings.Platform.iOS:
						reader.ReadBytes(0x2A0);
						input.num_entryActions = reader.ReadUInt32();
						input.off_entryActions = LegacyPointer.Read(reader);
						reader.ReadBytes(0x14);
						break;
					case Legacy_Settings.Platform.DC:
						reader.ReadBytes(0x278);
						input.num_entryActions = reader.ReadUInt32();
						input.off_entryActions = LegacyPointer.Read(reader);
						reader.ReadUInt32();
						LegacyPointer.Read(reader);
						break;
					case Legacy_Settings.Platform.PS2:
						if (Legacy_Settings.s.game == Legacy_Settings.Game.R2Revolution) {
							reader.ReadBytes(0x130);
							input.num_entryActions = reader.ReadUInt32();
							input.off_entryActions = LegacyPointer.Read(reader);
							LegacyPointer.Read(reader);
							reader.ReadUInt16();
							reader.ReadUInt16();
							reader.ReadUInt32(); // 0F00020000040100
							reader.ReadBytes(0x300);
						} else {
							if (Legacy_Settings.s.mode == Legacy_Settings.Mode.Rayman3PS2Demo_2002_05_17) {
								reader.ReadBytes(0x1368);
							} else {
								reader.ReadBytes(0x1250);
								if (Legacy_Settings.s.game == Legacy_Settings.Game.RA || Legacy_Settings.s.game == Legacy_Settings.Game.RM
									|| Legacy_Settings.s.mode == Legacy_Settings.Mode.Rayman3PS2Demo_2002_12_18) {
									reader.ReadBytes(0x10);
								}
							}
							input.num_entryActions = reader.ReadUInt32();
							input.off_entryActions = LegacyPointer.Read(reader);
							reader.ReadBytes(0x418);
						}
						break;
				}
			}

            if (input.off_entryActions != null && input.num_entryActions > 0) {
				//input.entryActions = new EntryAction[input.num_entryActions];
				LegacyPointer.DoAt(ref reader, input.off_entryActions, () => {
					for (int i = 0; i < input.num_entryActions; i++) {
						input.entryActions.Add(EntryAction.Read(reader, LegacyPointer.Current(reader)));
					}
				});
            }

            return input;
        }
    }
}
