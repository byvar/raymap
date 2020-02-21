using Newtonsoft.Json;
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

        [Flags]
        public enum CollisionFlags_R2 : ushort {
            None            = 0,
            Slide           = 1 << 0,
            Trampoline      = 1 << 1,
            GrabbaleLedge   = 1 << 2,
            Wall            = 1 << 3,
            FlagUnknown     = 1 << 4,
            HangableCeiling = 1 << 5,
            ClimbableWall   = 1 << 6,
            Electric        = 1 << 7,
            LavaDeathWarp   = 1 << 8,
            FallTrigger     = 1 << 9,
            HurtTrigger     = 1 << 10,
            DeathWarp       = 1 << 11,
            FlagUnk2        = 1 << 12,
            FlagUnk3        = 1 << 13,
            Water           = 1 << 14,
            NoCollision     = 1 << 15,
            All             = 0xFFFF
        }

        public ushort type;
        public ushort identifier;
        public CollisionFlags_R2 identifier_R2;
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
            ushort flag = (ushort)(1 << index);
            if (value) {
                identifier = (ushort)(identifier | flag);
            } else {
                identifier = (ushort)(identifier & (ushort)(~flag));
            }
        }

        public bool GetFlag(int index) {
            ushort flag = (ushort)(1 << index);
            return (identifier & flag) != 0;
        }
        public bool GetFlag(CollisionFlags_R2 flags) {
            return (identifier_R2 & flags) != CollisionFlags_R2.None;
        }
        public void SetFlag(CollisionFlags_R2 flags, bool on) {
            if (on) {
                identifier_R2 = identifier_R2 | flags;
            } else {
                identifier_R2 = identifier_R2 & (~flags);
            }
            identifier = (ushort)(identifier_R2);
        }

        public CollideMaterial(Pointer offset) {
            this.offset = offset;
        }

        public static CollideMaterial Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            CollideMaterial cm = new CollideMaterial(offset);
			//l.print(offset);

			if (Settings.s.game == Settings.Game.R2Revolution) {
				cm.type = reader.ReadUInt16();
				cm.identifier = reader.ReadUInt16();
			} else {
				cm.type = reader.ReadUInt16();
				cm.identifier = reader.ReadUInt16();
				if (Settings.s.engineVersion == Settings.EngineVersion.R3) {
					cm.direction = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
					cm.coef = reader.ReadSingle();
				}
				cm.typeForAI = reader.ReadUInt32();
			}

            // For specific games
            cm.identifier_R2 = (CollisionFlags_R2)cm.identifier;
            return cm;
        }

        public static CollideMaterial FromOffsetOrRead(Pointer offset, Reader reader) {
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

        public void SetMaterial(MeshRenderer mr) {
            mr.material = MapLoader.Loader.collideMaterial;
            if (NoCollision) {
                mr.material = MapLoader.Loader.collideTransparentMaterial;
                //mr.material.SetTexture("_MainTex", Util.CreateDummyCheckerTexture());
                mr.material.color = new Color(1, 1, 1, 0.3f); // transparent cyan
            }
            if (Slide) mr.material.color = Color.blue;
            if (Water) {
                mr.material = MapLoader.Loader.collideTransparentMaterial;
                //mr.material.SetTexture("_MainTex", Util.CreateDummyCheckerTexture());
                mr.material.color = new Color(0, 1, 1, 0.5f); // transparent cyan
            }
            if (ClimbableWall || HangableCeiling) {
                mr.material.color = new Color(244f / 255f, 131f / 255f, 66f / 255f); // ORANGE
            }
            if (LavaDeathWarp || DeathWarp) {
                mr.material.color = Color.red;
            }
            if (HurtTrigger) mr.material.color = Colors.HurtTrigger;
            if (FallTrigger) mr.material.color = Colors.FallTrigger;
            if (Trampoline) mr.material.color = Colors.Trampoline;
            if (Electric) mr.material.color = Colors.Electric;
            if (Wall) mr.material.color = Colors.Wall;
            if (GrabbableLedge) mr.material.color = Colors.GrabbableLedge;
            if (FlagUnknown || FlagUnk2 || FlagUnk3) mr.material.color = Colors.FlagUnknown;
        }

        public static class Colors {
            public static Color
                HurtTrigger = new Color(126 / 255f, 2 / 255f, 204 / 255f), // purple
                FallTrigger = Color.black,
                Trampoline = Color.yellow,
                Electric = new Color(219f / 255f, 140 / 255f, 212 / 255f), // Pink
                Wall = new Color(126 / 255f, 2 / 255f, 204 / 255f),
                GrabbableLedge = Color.green,
                FlagUnknown = new Color(124 / 255f, 68 / 255f, 33 / 255f); // brown
        }
    }
}
