using Newtonsoft.Json;
using OpenSpace.Animation;
using OpenSpace.Collide;
using OpenSpace.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace {
    public class MechanicsIDCard {
        public Pointer offset;
        public uint type;
        public uint flags;
        public float gravity;
        public float maxRebound;
        public float slopeLimit;
        public Vector3 inertia;
        public float tiltIntensity;
        public float tiltInertia;
        public float tiltOrigin;
        public Vector3 maxInertia;

        public MechanicsIDCard(Pointer offset) {
            this.offset = offset;
        }

        public static MechanicsIDCard Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            MechanicsIDCard c = new MechanicsIDCard(offset);

            c.type = reader.ReadUInt32(); // 0x0
            c.flags = reader.ReadUInt32(); // 0x4
            c.gravity = reader.ReadSingle();
            c.maxRebound = reader.ReadSingle();
            reader.ReadUInt32();
            c.slopeLimit = reader.ReadSingle();
            float x = reader.ReadSingle();
            float z = reader.ReadSingle();
            float y = reader.ReadSingle();
            c.inertia = new Vector3(x, y, z);
            c.tiltIntensity = reader.ReadSingle();
            c.tiltInertia = reader.ReadSingle();
            c.tiltOrigin = reader.ReadSingle();
            x = reader.ReadSingle();
            z = reader.ReadSingle();
            y = reader.ReadSingle();
            c.maxInertia = new Vector3(x, y, z);
            return c;
        }

        public void Write(Writer writer)
        {
            // Write flags
            (offset+0x4).DoAt(ref writer, () =>
            {
                writer.Write(this.flags);
            });
        }

        public static MechanicsIDCard FromOffsetOrRead(Pointer offset, Reader reader) {
            if (offset == null) return null;
            MechanicsIDCard c = FromOffset(offset);
            if (c == null) {
                Pointer off_current = Pointer.Goto(ref reader, offset);
                c = MechanicsIDCard.Read(reader, offset);
                Pointer.Goto(ref reader, off_current);
                MapLoader.Loader.mechanicsIDCards.Add(c);
            }
            return c;
        }

        public static MechanicsIDCard FromOffset(Pointer offset) {
            MapLoader l = MapLoader.Loader;
            for (int i = 0; i < l.mechanicsIDCards.Count; i++) {
                if (offset == l.mechanicsIDCards[i].offset) return l.mechanicsIDCards[i];
            }
            return null;
        }
    }
}
