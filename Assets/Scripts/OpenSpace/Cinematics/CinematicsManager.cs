using OpenSpace.Animation.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Cinematics {
    public class CinematicsManager {
        public LegacyPointer offset;
		public LinkedList<Cinematic> cinematics; 
		public Matrix matrix;
		public uint unk;

        public CinematicsManager(LegacyPointer offset) {
            this.offset = offset;
        }

        public static CinematicsManager Read(Reader reader, LegacyPointer offset) {
            MapLoader l = MapLoader.Loader;
			CinematicsManager cm = new CinematicsManager(offset);
			cm.cinematics = LinkedList<Cinematic>.Read(ref reader, LegacyPointer.Current(reader), (off_cine) => {
				return Cinematic.Read(reader, off_cine);
			}, type: LinkedList.Type.Double);
			cm.matrix = Matrix.Read(reader, LegacyPointer.Current(reader));
			cm.unk = reader.ReadUInt32();
			return cm;
        }
    }
}
