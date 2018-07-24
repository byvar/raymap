using OpenSpace.Animation;
using OpenSpace.Visual;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Collide {
    public class CollideMaterial {
        public Pointer offset;

        public ushort type;
        public ushort identifier;
        public Vector3 direction;
        public float coef;
        public uint typeForAI;

        public bool Slide           { get { return GetFlag(00); } set { SetFlag(00, value); } }
        public bool Trampoline      { get { return GetFlag(01); } set { SetFlag(01, value); } }
        public bool GrabbableLedge  { get { return GetFlag(02); } set { SetFlag(02, value); } }
        public bool Wall            { get { return GetFlag(03); } set { SetFlag(03, value); } }
        public bool FlagUnknown     { get { return GetFlag(04); } set { SetFlag(04, value); } }
        public bool HangableCeiling { get { return GetFlag(05); } set { SetFlag(05, value); } }
        public bool ClimbableWall   { get { return GetFlag(06); } set { SetFlag(06, value); } }
        public bool Electric        { get { return GetFlag(07); } set { SetFlag(07, value); } }
        public bool LavaDeathWarp   { get { return GetFlag(08); } set { SetFlag(08, value); } }
        public bool FallTrigger     { get { return GetFlag(09); } set { SetFlag(09, value); } }
        public bool HurtTrigger     { get { return GetFlag(10); } set { SetFlag(10, value); } }
        public bool DeathWarp       { get { return GetFlag(11); } set { SetFlag(11, value); } }
        public bool FlagUnk2        { get { return GetFlag(12); } set { SetFlag(12, value); } }
        public bool FlagUnk3        { get { return GetFlag(13); } set { SetFlag(13, value); } }
        public bool Water           { get { return GetFlag(14); } set { SetFlag(14, value); } }
        public bool NoCollision     { get { return GetFlag(15); } set { SetFlag(15, value); } }

        public void SetFlag(int index, bool value) {
            BitArray bitArray = new BitArray(BitConverter.GetBytes(identifier));
            bitArray.Set(index, value);
            ushort[] array = new ushort[1];
            bitArray.CopyTo(array, 0);
            this.identifier = array[0];
        }

        public bool GetFlag(int index) {
            BitArray bitArray = new BitArray(BitConverter.GetBytes(identifier));
            return bitArray.Get(index);
        }

        public CollideMaterial(Pointer offset) {
            this.offset = offset;
        }

        public static CollideMaterial Read(EndianBinaryReader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            CollideMaterial cm = new CollideMaterial(offset);
            //l.print(offset);

            cm.type = reader.ReadUInt16();
            cm.identifier = reader.ReadUInt16();
            if (Settings.s.engineMode == Settings.EngineMode.R3) {
                cm.direction = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                cm.coef = reader.ReadSingle();
            }
            cm.typeForAI = reader.ReadUInt32();
            return cm;
        }

        public static CollideMaterial FromOffsetOrRead(Pointer offset, EndianBinaryReader reader) {
            CollideMaterial cm = FromOffset(offset);
            if (cm == null) {
                Pointer off_current = Pointer.Goto(ref reader, offset);
                cm = CollideMaterial.Read(reader, offset);
                Pointer.Goto(ref reader, off_current);
                MapLoader.Loader.collideMaterials.Add(cm);
            }
            return cm;
        }

        public static CollideMaterial FromOffset(Pointer offset) {
            MapLoader l = MapLoader.Loader;
            for (int i = 0; i < l.collideMaterials.Count; i++) {
                if (offset == l.collideMaterials[i].offset) return l.collideMaterials[i];
            }
            return null;
        }
    }
}
