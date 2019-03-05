using UnityEngine;
using UnityEditor;

namespace OpenSpace.Object.Properties {
    public class StandardGame {
        public Pointer offset;
        public uint[] objectTypes = new uint[3];
        public Pointer off_superobject;

        public uint customBits;
        public uint aiCustomBits;
        public byte isAPlatform;
        public byte updateCheckByte;
        public byte transparencyZoneMin;
        public byte transparencyZoneMax;
        public uint customBitsInitial;
        public uint aiCustomBitsInitial;
        public float tooFarLimit;

        public bool IsAlwaysActive
        {
            get
            {
                return ((updateCheckByte >> 6) & 1) != 0;
            }
        }

        public bool IsMainActor
        {
            get
            {
                return (customBits & 0x80000000) == 0x80000000;
            }
        }

        public StandardGame(Pointer offset)
        {
            this.offset = offset;
        }

        public static StandardGame Read(Reader reader, Pointer offset)
        {
            MapLoader l = MapLoader.Loader;
            //l.print(offset);
            StandardGame stdGame = new StandardGame(offset);

            if (Settings.s.game != Settings.Game.R2Revolution) {
                stdGame.objectTypes[0] = reader.ReadUInt32();
                stdGame.objectTypes[1] = reader.ReadUInt32();
                stdGame.objectTypes[2] = reader.ReadUInt32();
                stdGame.off_superobject = Pointer.Read(reader); // 0xC SuperObject from Perso probably

                if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                    if (Settings.s.platform == Settings.Platform.DC) {
                        reader.ReadInt32();
                        reader.ReadInt32();
                        reader.ReadInt32();
                        reader.ReadByte();
                        reader.ReadByte();
                        reader.ReadByte();
                        reader.ReadByte();
                    } else {
                        reader.ReadBytes(0x14); // 0x10 - 0x23
                    }
                    stdGame.customBits = reader.ReadUInt32(); // 0x24 custom bits
                    stdGame.isAPlatform = reader.ReadByte(); // 0x28
                    stdGame.updateCheckByte = reader.ReadByte(); // 0x29
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
                    stdGame.isAPlatform = reader.ReadByte();
                    stdGame.updateCheckByte = reader.ReadByte();
                    stdGame.transparencyZoneMin = reader.ReadByte();
                    stdGame.transparencyZoneMax = reader.ReadByte();
                    stdGame.customBitsInitial = reader.ReadUInt32();
                    stdGame.aiCustomBitsInitial = reader.ReadUInt32();
                }
            } else {
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
                stdGame.off_superobject = Pointer.Read(reader);
                reader.ReadBytes(0xC);
                stdGame.isAPlatform = reader.ReadByte();
                stdGame.updateCheckByte = reader.ReadByte();
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

        public bool ConsideredOnScreen()
        {
            return (updateCheckByte & (1 << 5)) != 0;
        }

        public bool ConsideredTooFarAway()
        {
            return (updateCheckByte & (1 << 7)) != 0;
        }

        public void Write(Writer writer)
        {
            if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                Pointer.Goto(ref writer, Pointer.Current(writer) + 0x24);
                writer.Write(customBits);
                writer.Write(isAPlatform);
                writer.Write(updateCheckByte);
                writer.Write(transparencyZoneMin);
                writer.Write(transparencyZoneMax);
                writer.Write(customBitsInitial);
            } else {
                Pointer.Goto(ref writer, Pointer.Current(writer) + 0x20);
                writer.Write(customBits);
                writer.Write(aiCustomBits);
                writer.Write(isAPlatform);
                writer.Write(updateCheckByte);
                writer.Write(transparencyZoneMin);
                writer.Write(transparencyZoneMax);
                writer.Write(customBitsInitial);
                writer.Write(aiCustomBitsInitial);
            }
        }
    }
}