using OpenSpace.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenSpace {
    // Only contains data read in the header of the level file
    public class Globals {
        public Pointer off_transitDynamicWorld = null;
        public Pointer off_actualWorld = null;
        public Pointer off_dynamicWorld = null;
        public Pointer off_inactiveDynamicWorld = null;
        public Pointer off_fatherSector = null;
        public Pointer off_firstSubMapPosition = null;

        /* The following 7 values are the "Always" structure. The spawnable perso data is dynamically copied to these superobjects.
        There can be at most (num_always) objects of this type active in a level, and they get reused by other objects when they despawn. */
        public uint num_always;
        public LinkedList<Perso> spawnablePersos;
        public Pointer off_always_reusableSO; // There are (num_always) empty SuperObjects starting with this one.
        public Pointer off_always_reusableUnknown1; // (num_always) * 0x2c blocks
        public Pointer off_always_reusableUnknown2; // (num_always) * 0x4 blocks

        public Pointer off_backgroundGameMaterial = null; // Donald Duck only
        public GameMaterial backgroundGameMaterial = null;

		public Pointer off_camera = null; // We use this to determine the camera object in Revolution, without names
    }
}