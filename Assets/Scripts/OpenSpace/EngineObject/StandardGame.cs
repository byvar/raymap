using UnityEngine;
using UnityEditor;

namespace OpenSpace.EngineObject
{
    public class StandardGame
    {
        public Pointer offset;
        public uint index0;
        public uint index1;
        public uint index2;

        public int customBits;
        public int aiCustomBits;

        public StandardGame(Pointer offset)
        {
            this.offset = offset;
        }

        public static StandardGame Read(Reader reader, Pointer offset)
        {
            StandardGame stdGame = new StandardGame(offset);
            stdGame.index0 = reader.ReadUInt32();
            stdGame.index1 = reader.ReadUInt32();
            stdGame.index2 = reader.ReadUInt32();

            Pointer.Read(reader); // 0xC SuperObject from Perso probably
            if (Settings.s.engineMode == Settings.EngineMode.R2) {
                reader.ReadBytes(0x14); // 0x10 - 0x23
                stdGame.customBits = reader.ReadInt32(); // 0x24 custom bits
                stdGame.aiCustomBits = reader.ReadInt32(); // 0x24 AI custom bits
            } else {
                reader.ReadBytes(0x10); // 0x10 - 0x1F
                stdGame.customBits = reader.ReadInt32(); // 0x20 custom bits
                stdGame.aiCustomBits = reader.ReadInt32(); // 0x24 AI custom bits
            }

            return stdGame;
        }
        
        public string GetFamilyName()
        {
            MapLoader l = MapLoader.Loader;
            if (index0 >= 0 && index0 < l.objectTypes[0].Length) {
                return l.objectTypes[0][index0].name;
            } else {
                return "";
            }
        }

        public string GetModelName()
        {
            MapLoader l = MapLoader.Loader;
            if (index1 >= 1 && index1 < l.objectTypes[1].Length) {
                return l.objectTypes[1][index1].name;
            } else {
                return "";
            }
        }

        public string GetPersoName()
        {
            MapLoader l = MapLoader.Loader;
            if (index2 >= 2 && index2 < l.objectTypes[2].Length) {
                return l.objectTypes[2][index2].name;
            } else {
                return "";
            }
        }

        public void Write(Writer writer, CustomBitsComponent[] components)
        {
            foreach (CustomBitsComponent comp in components) {
                Pointer.Goto(ref writer, comp.offset);
                writer.Write(comp.rawFlags);
            }
        }
    }
}