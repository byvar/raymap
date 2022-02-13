using OpenSpace.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenSpace {
    // Only contains data read in the header of the level file
    public class Globals {
        public LegacyPointer off_transitDynamicWorld = null;
        public LegacyPointer off_actualWorld = null;
        public LegacyPointer off_dynamicWorld = null;
        public LegacyPointer off_inactiveDynamicWorld = null;
        public LegacyPointer off_fatherSector = null;
        public LegacyPointer off_firstSubMapPosition = null;

        /* The following 7 values are the "Always" structure. The spawnable perso data is dynamically copied to these superobjects.
        There can be at most (num_always) objects of this type active in a level, and they get reused by other objects when they despawn. */
        public uint num_always;
        public LinkedList<Perso> spawnablePersos;
        public LegacyPointer off_always_reusableSO; // There are (num_always) empty SuperObjects starting with this one.
        public LegacyPointer off_always_reusableUnknown1; // (num_always) * 0x2c blocks
        public LegacyPointer off_always_reusableUnknown2; // (num_always) * 0x4 blocks

        public LegacyPointer off_backgroundGameMaterial = null; // Donald Duck only
        public GameMaterial backgroundGameMaterial = null;

		public LegacyPointer off_camera = null; // We use this to determine the camera object in Revolution, without names
    }
}