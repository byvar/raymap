﻿using UnityEngine;
using UnityEditor;

namespace OpenSpace.Object.Properties {
    public class StandardGame {
        public LegacyPointer offset;
        public uint[] objectTypes = new uint[3];
        public LegacyPointer off_superobject;

        public uint customBits;
        public uint aiCustomBits;
        public byte platformType;
        public byte miscFlags;
        public byte transparencyZoneMin;
        public byte transparencyZoneMax;

        public int hitPoints;
        public int hitPointsMax;

        public uint customBitsInitial;
        public uint aiCustomBitsInitial;
        public float tooFarLimit;

        public bool IsAlwaysActive
        {
            get
            {
                return ((miscFlags >> 6) & 1) != 0;
            }
        }

        public bool IsMainActor
        {
            get
            {
                return (customBits & 0x80000000) == 0x80000000;
            }
        }

        public StandardGame(LegacyPointer offset)
        {
            this.offset = offset;
        }

        public static StandardGame Read(Reader reader, LegacyPointer offset)
        {
            MapLoader l = MapLoader.Loader;
            //l.print("StdGame: " + offset);
            StandardGame stdGame = new StandardGame(offset);

			if (Legacy_Settings.s.game != Legacy_Settings.Game.R2Revolution && Legacy_Settings.s.game != Legacy_Settings.Game.LargoWinch) {
				stdGame.objectTypes[0] = reader.ReadUInt32();
				stdGame.objectTypes[1] = reader.ReadUInt32();
				stdGame.objectTypes[2] = reader.ReadUInt32();
				stdGame.off_superobject = LegacyPointer.Read(reader); // 0xC SuperObject from Perso probably

				if (Legacy_Settings.s.engineVersion < Legacy_Settings.EngineVersion.R3) {
					if (Legacy_Settings.s.platform == Legacy_Settings.Platform.DC) {
						reader.ReadInt32();
						reader.ReadInt32();
						reader.ReadInt32();
						reader.ReadByte();
						reader.ReadByte();
						reader.ReadByte();
						reader.ReadByte();
					} else {
						reader.ReadBytes(0x10); // 0x10 - 0x19
						stdGame.hitPoints = reader.ReadByte();
						reader.ReadByte();
						stdGame.hitPointsMax = reader.ReadByte();
						reader.ReadByte();
					}
					stdGame.customBits = reader.ReadUInt32(); // 0x24 custom bits
					stdGame.platformType = reader.ReadByte(); // 0x28
					stdGame.miscFlags = reader.ReadByte(); // 0x29
					stdGame.transparencyZoneMin = reader.ReadByte(); // 0x2A
					stdGame.transparencyZoneMax = reader.ReadByte(); // 0x2B
					stdGame.customBitsInitial = reader.ReadUInt32(); // 0x2C
					reader.ReadByte(); // 0x30
					reader.ReadByte(); // 0x31
					reader.ReadByte(); // 0x32
					stdGame.tooFarLimit = (float)reader.ReadByte(); // 0x33 - Objects are only activated within this radius
					reader.ReadInt32(); // 0x34
					reader.ReadInt32(); // 0x38
					reader.ReadInt32(); // 0x3C
					reader.ReadByte(); // 0x40
					byte activationBits = reader.ReadByte(); // 0x41, referenced in fn_vTreatDynamicHierarchy, used to determine if object is handled (treated)

				} else {
					reader.ReadBytes(0x10); // 0x10 - 0x1F
					stdGame.customBits = reader.ReadUInt32(); // 0x20 custom bits
					stdGame.aiCustomBits = reader.ReadUInt32(); // 0x24 AI custom bits
					stdGame.platformType = reader.ReadByte();
					stdGame.miscFlags = reader.ReadByte();
					stdGame.transparencyZoneMin = reader.ReadByte();
					stdGame.transparencyZoneMax = reader.ReadByte();
					stdGame.customBitsInitial = reader.ReadUInt32();
					stdGame.aiCustomBitsInitial = reader.ReadUInt32();
				}
			} else if (Legacy_Settings.s.game == Legacy_Settings.Game.R2Revolution) {
				// Revolution
				reader.ReadInt32();
				reader.ReadInt32();
				reader.ReadInt32();
				/*reader.ReadByte();
				reader.ReadByte();
				reader.ReadByte();
				reader.ReadByte();*/
				reader.ReadUInt16();
				stdGame.objectTypes[0] = reader.ReadUInt16();
				stdGame.objectTypes[1] = reader.ReadUInt16();
				stdGame.objectTypes[2] = reader.ReadUInt16();
				reader.ReadUInt32();
				stdGame.off_superobject = LegacyPointer.Read(reader);
				reader.ReadBytes(0xC);
				stdGame.platformType = reader.ReadByte();
				stdGame.miscFlags = reader.ReadByte();
				stdGame.transparencyZoneMin = reader.ReadByte();
				stdGame.transparencyZoneMax = reader.ReadByte();
				stdGame.customBits = reader.ReadUInt32();
				reader.ReadUInt32();
				reader.ReadUInt32(); // a pointer?
				stdGame.customBitsInitial = reader.ReadUInt32();
			} else if (Legacy_Settings.s.game == Legacy_Settings.Game.LargoWinch) {
				// Almost same as revo
				reader.ReadInt32();
				reader.ReadInt32();
				reader.ReadInt32();
				reader.ReadUInt16();
				reader.ReadUInt16(); // 0xCDCD, Largo uses this for padding
				stdGame.objectTypes[0] = reader.ReadUInt32();
				stdGame.objectTypes[1] = reader.ReadUInt32();
				stdGame.objectTypes[2] = reader.ReadUInt32();
				reader.ReadUInt32();
				stdGame.off_superobject = LegacyPointer.Read(reader);
				reader.ReadBytes(0x10);
				stdGame.platformType = reader.ReadByte();
				stdGame.miscFlags = reader.ReadByte();
				stdGame.transparencyZoneMin = reader.ReadByte();
				stdGame.transparencyZoneMax = reader.ReadByte();
				stdGame.customBits = reader.ReadUInt32();
				reader.ReadUInt32();
				reader.ReadUInt32(); // a pointer?
				stdGame.customBitsInitial = reader.ReadUInt32();
			}

            return stdGame;
        }

        public string GetName(int index)
        {
            MapLoader l = MapLoader.Loader;
            if (objectTypes[index] >= 0 && objectTypes[index] < l.objectTypes[index].Length) {
                return l.objectTypes[index][objectTypes[index]].name;
            } else {
                return "";
            }
        }

        public bool IsActive()
        {
			return (miscFlags & (1 << 2)) != 0;
        }

		public bool ConsideredOnScreen()
        {
            return (miscFlags & (1 << 5)) != 0;
        }

        public bool ConsideredTooFarAway()
        {
            return (miscFlags & (1 << 7)) != 0;
        }

        public void Write(Writer writer)
        {
			LegacyPointer.Goto(ref writer, offset);
            if (Legacy_Settings.s.engineVersion < Legacy_Settings.EngineVersion.R3) {
                LegacyPointer.Goto(ref writer, offset + 0x24);
                writer.Write(customBits);
                writer.Write(platformType);
                writer.Write(miscFlags);
                writer.Write(transparencyZoneMin);
                writer.Write(transparencyZoneMax);
                writer.Write(customBitsInitial);
            } else {
                LegacyPointer.Goto(ref writer, offset + 0x20);
                writer.Write(customBits);
                writer.Write(aiCustomBits);
                writer.Write(platformType);
                writer.Write(miscFlags);
                writer.Write(transparencyZoneMin);
                writer.Write(transparencyZoneMax);
                writer.Write(customBitsInitial);
                writer.Write(aiCustomBitsInitial);
            }
        }
    }
}