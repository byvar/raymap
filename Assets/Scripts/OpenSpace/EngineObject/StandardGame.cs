using UnityEngine;
using UnityEditor;

namespace OpenSpace.EngineObject
{
    public class StandardGame
    {
        public Pointer offset;
        public uint[] objectTypes = new uint[3];

        public int customBits;
        public int aiCustomBits;
        public byte isAPlatform;
        public byte unk;
        public byte transparencyZoneMin;
        public byte transparencyZoneMax;
        public int customBitsInitial;
        public int aiCustomBitsInitial;

        public StandardGame(Pointer offset)
        {
            this.offset = offset;
        }

        public static StandardGame Read(Reader reader, Pointer offset)
        {
            MapLoader l = MapLoader.Loader;
            l.print(offset);
            StandardGame stdGame = new StandardGame(offset);
            stdGame.objectTypes[0] = reader.ReadUInt32();
            stdGame.objectTypes[1] = reader.ReadUInt32();
            stdGame.objectTypes[2] = reader.ReadUInt32();
            Pointer.Read(reader); // 0xC SuperObject from Perso probably

            if (Settings.s.engineMode == Settings.EngineMode.R2) {
                reader.ReadBytes(0x14); // 0x10 - 0x23
                stdGame.customBits = reader.ReadInt32(); // 0x24 custom bits
                stdGame.isAPlatform = reader.ReadByte();
                stdGame.unk = reader.ReadByte();
                stdGame.transparencyZoneMin = reader.ReadByte();
                stdGame.transparencyZoneMax = reader.ReadByte();
                stdGame.customBitsInitial = reader.ReadInt32();

            } else {
                reader.ReadBytes(0x10); // 0x10 - 0x1F
                stdGame.customBits = reader.ReadInt32(); // 0x20 custom bits
                stdGame.aiCustomBits = reader.ReadInt32(); // 0x24 AI custom bits
                stdGame.isAPlatform = reader.ReadByte();
                stdGame.unk = reader.ReadByte();
                stdGame.transparencyZoneMin = reader.ReadByte();
                stdGame.transparencyZoneMax = reader.ReadByte();
                stdGame.customBitsInitial = reader.ReadInt32();
                stdGame.aiCustomBitsInitial = reader.ReadInt32();
            }

            return stdGame;
        }
        
        public string GetName(int index) {
            MapLoader l = MapLoader.Loader;
            if (objectTypes[index] >= 0 && objectTypes[index] < l.objectTypes[index].Length) {
                return l.objectTypes[index][objectTypes[index]].name;
            } else {
                return "";
            }
        }

        public void Write(Writer writer) {
            if (Settings.s.engineMode == Settings.EngineMode.R2) {
                Pointer.Goto(ref writer, Pointer.Current(writer) + 0x24);
                writer.Write(customBits);
                writer.Write(isAPlatform);
                writer.Write(unk);
                writer.Write(transparencyZoneMin);
                writer.Write(transparencyZoneMax);
                writer.Write(customBitsInitial);
            } else {
                Pointer.Goto(ref writer, Pointer.Current(writer) + 0x20);
                writer.Write(customBits);
                writer.Write(aiCustomBits);
                writer.Write(isAPlatform);
                writer.Write(unk);
                writer.Write(transparencyZoneMin);
                writer.Write(transparencyZoneMax);
                writer.Write(customBitsInitial);
                writer.Write(aiCustomBitsInitial);
            }
        }
    }
}