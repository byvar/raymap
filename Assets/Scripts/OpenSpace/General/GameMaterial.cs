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

			if (Settings.s.game == Settings.Game.R2Revolution) {
				gm.soundMaterial = reader.ReadUInt32();
				gm.collideMaterial = CollideMaterial.Read(reader, Pointer.Current(reader));
				// Maybe the first uint16 of collidematerial in Revolution is actually sound material, but eh
			} else {
				if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
					gm.off_visualMaterial = Pointer.Read(reader);
					gm.off_mechanicsMaterial = Pointer.Read(reader);
				}
				gm.soundMaterial = reader.ReadUInt32();
				gm.off_collideMaterial = Pointer.Read(reader, allowMinusOne: true);

				if (gm.off_visualMaterial != null) {
					gm.visualMaterial = VisualMaterial.FromOffsetOrRead(gm.off_visualMaterial, reader);
				}
				if (gm.off_collideMaterial != null) {
					gm.collideMaterial = CollideMaterial.FromOffsetOrRead(gm.off_collideMaterial, reader);
				}
			}
            return gm;
        }

        public static GameMaterial FromOffsetOrRead(Pointer offset, Reader reader) {
            if (offset == null) return null;
            GameMaterial gm = FromOffset(offset);
            if (gm == null) {
                Pointer.DoAt(ref reader, offset, () => {
                    gm = GameMaterial.Read(reader, offset);
                    MapLoader.Loader.gameMaterials.Add(gm);
                });
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
