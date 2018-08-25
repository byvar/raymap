using OpenSpace.Animation;
using OpenSpace.Collide;
using OpenSpace.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace {
    public class GameMaterial {
        public Pointer offset;

        public Pointer off_visualMaterial;
        public Pointer off_mechanicsMaterial;
        public uint soundMaterial;
        public Pointer off_collideMaterial;

        public VisualMaterial visualMaterial;
        public CollideMaterial collideMaterial;

        public GameMaterial(Pointer offset) {
            this.offset = offset;
        }

        public static GameMaterial Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            GameMaterial gm = new GameMaterial(offset);

            if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                gm.off_visualMaterial = Pointer.Read(reader);
                gm.off_mechanicsMaterial = Pointer.Read(reader);
            }
            // Very ugly code incoming
            gm.soundMaterial = reader.ReadUInt32();
            gm.off_collideMaterial = Pointer.Read(reader, allowMinusOne: true);

            if (gm.off_visualMaterial != null) {
                gm.visualMaterial = VisualMaterial.FromOffsetOrRead(gm.off_visualMaterial, reader);
            }
            if (gm.off_collideMaterial != null) {
                gm.collideMaterial = CollideMaterial.FromOffsetOrRead(gm.off_collideMaterial, reader);
            }
            return gm;
        }

        public static GameMaterial FromOffsetOrRead(Pointer offset, Reader reader) {
            GameMaterial gm = FromOffset(offset);
            if (gm == null) {
                Pointer off_current = Pointer.Goto(ref reader, offset);
                gm = GameMaterial.Read(reader, offset);
                Pointer.Goto(ref reader, off_current);
                MapLoader.Loader.gameMaterials.Add(gm);
            }
            return gm;
        }

        public static GameMaterial FromOffset(Pointer offset) {
            MapLoader l = MapLoader.Loader;
            for (int i = 0; i < l.gameMaterials.Count; i++) {
                if (offset == l.gameMaterials[i].offset) return l.gameMaterials[i];
            }
            return null;
        }
    }
}
