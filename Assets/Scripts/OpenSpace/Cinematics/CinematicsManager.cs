using OpenSpace.Animation.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Cinematics {
    public class CinematicsManager {
        public Pointer offset;
		public LinkedList<Cinematic> cinematics; 
		public Matrix matrix;
		public uint unk;

        public CinematicsManager(Pointer offset) {
            this.offset = offset;
        }

        public static CinematicsManager Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
			CinematicsManager cm = new CinematicsManager(offset);
			cm.cinematics = LinkedList<Cinematic>.Read(ref reader, Pointer.Current(reader), (off_cine) => {
				return Cinematic.Read(reader, off_cine);
			}, type: LinkedList.Type.Double);
			cm.matrix = Matrix.Read(reader, Pointer.Current(reader));
			cm.unk = reader.ReadUInt32();
			return cm;
        }
    }
}
